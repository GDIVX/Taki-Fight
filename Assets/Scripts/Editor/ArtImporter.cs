using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Runtime.CardGameplay.Card;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ArtImporter : AssetPostprocessor
    {
        private static ArtImporterSettings _settings;
        private static Dictionary<string, string> _lastSyncedFiles = new Dictionary<string, string>();

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // Load the settings file
            _settings = AssetDatabase.LoadAssetAtPath<ArtImporterSettings>("Assets/Settings/ArtImporterSettings.asset");
            if (_settings == null)
            {
                Debug.LogError("ArtImporterSettings not found. Please create one.");
                return;
            }

            // Load last synced files info
            LoadLastSyncedInfo();
        }

        // Add menu item for manual activation
        [MenuItem("Tools/Art Importer/Sync Google Drive Files")]
        private static void ManualSyncGoogleDriveFiles()
        {
            if (_settings == null)
            {
                Debug.LogError("ArtImporterSettings not found. Please create one first.");
                return;
            }

            SyncGoogleDriveFiles();
        }

        private static void LoadLastSyncedInfo()
        {
            string syncInfoPath = System.IO.Path.Combine(Application.dataPath, "../Library/ArtImporterSyncInfo.txt");
            if (System.IO.File.Exists(syncInfoPath))
            {
                string[] lines = System.IO.File.ReadAllLines(syncInfoPath);
                _lastSyncedFiles.Clear();
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        _lastSyncedFiles[parts[0]] = parts[1];
                    }
                }
            }
        }

        private static void SaveLastSyncedInfo()
        {
            string syncInfoPath = System.IO.Path.Combine(Application.dataPath, "../Library/ArtImporterSyncInfo.txt");
            List<string> lines = new List<string>();
            foreach (var kvp in _lastSyncedFiles)
            {
                lines.Add($"{kvp.Key}|{kvp.Value}");
            }
            System.IO.File.WriteAllLines(syncInfoPath, lines);
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (_settings == null) return;

            foreach (string assetPath in importedAssets)
            {
                // Check if the asset is in the input folder
                if (assetPath.StartsWith(_settings.OutputFolderPath))
                {
                    ProcessArtAsset(assetPath);
                }
            }
        }

        private static void ProcessArtAsset(string assetPath)
        {
            // Extract the file name without extension
            string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            // Parse the card name and type from the file name
            string[] parts = fileName.Split('_');
            if (parts.Length != 2)
            {
                Debug.LogError($"Invalid file name: {fileName}. Expected format: CardName_Type");
                return;
            }

            string cardName = parts[0];
            string artType = parts[1];

            // Only process _Artwork files for now
            if (artType.ToLower() != "artwork")
            {
                Debug.LogWarning(
                    $"Skipping file with unsupported type: {artType}. Only '_Artwork' is supported for now.");
                return;
            }

            // Find the corresponding CardData ScriptableObject
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(CardData)}");
            CardData cardData = null;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                CardData data = AssetDatabase.LoadAssetAtPath<CardData>(path);

                if (data.Title == cardName)
                {
                    cardData = data;
                    break;
                }
            }

            if (cardData == null)
            {
                Debug.LogError($"No CardData found for card: {cardName}");
                return;
            }

            // Move the asset to the output folder
            string outputPath = System.IO.Path.Combine(_settings.OutputFolderPath, $"{fileName}{System.IO.Path.GetExtension(assetPath)}");
            AssetDatabase.MoveAsset(assetPath, outputPath);

            // Inject the art asset into the CardData
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(outputPath);
            if (sprite != null)
            {
                SerializedObject serializedCardData = new SerializedObject(cardData);
                SerializedProperty imageProperty = serializedCardData.FindProperty("_image");
                imageProperty.objectReferenceValue = sprite;
                serializedCardData.ApplyModifiedProperties();

                Debug.Log($"Art asset (_Artwork) injected into CardData: {cardName}");
            }
            else
            {
                Debug.LogError($"Failed to load art asset: {outputPath}");
            }
        }

        private static async void SyncGoogleDriveFiles()
        {
            if (string.IsNullOrEmpty(_settings.GoogleDriveFolderId) ||
                string.IsNullOrEmpty(_settings.GoogleDriveCredentialsJson))
            {
                Debug.LogError("Google Drive settings are not configured.");
                return;
            }

            try
            {
                var credential = GoogleCredential.FromJson(_settings.GoogleDriveCredentialsJson)
                    .CreateScoped(DriveService.Scope.DriveReadonly);

                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ArtImporter"
                });

                // List files in the specified Google Drive folder
                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.Q = $"'{_settings.GoogleDriveFolderId}' in parents";
                listRequest.Fields = "files(id, name, modifiedTime)";

                FileList files = await listRequest.ExecuteAsync();
                int newFiles = 0;
                int updatedFiles = 0;

                foreach (var file in files.Files)
                {
                    string outputPath = System.IO.Path.Combine(_settings.OutputFolderPath, file.Name);
                    string lastModified = file.ModifiedTimeDateTimeOffset?.ToString("o");

                    // Check if file needs to be downloaded
                    if (!_lastSyncedFiles.TryGetValue(file.Id, out string storedModifiedTime) ||
                        storedModifiedTime != lastModified)
                    {
                        await DownloadFileFromGoogleDrive(service, file.Id, outputPath);
                        _lastSyncedFiles[file.Id] = lastModified;
                        
                        if (storedModifiedTime == null)
                            newFiles++;
                        else
                            updatedFiles++;
                    }
                }

                SaveLastSyncedInfo();
                Debug.Log($"Sync completed. New files: {newFiles}, Updated files: {updatedFiles}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to sync Google Drive files: {ex.Message}");
            }
        }

        private static async Task DownloadFileFromGoogleDrive(DriveService service, string fileId, string outputPath)
        {
            var request = service.Files.Get(fileId);
            using (var stream = new System.IO.FileStream(outputPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                await request.DownloadAsync(stream);
            }

            Debug.Log($"Downloaded file: {outputPath}");
            
            // Refresh the Asset Database to ensure Unity recognizes the new file
            AssetDatabase.Refresh();
        }
    }
}