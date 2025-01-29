using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using UnityEditor;
using UnityEngine;

namespace Editor.ArtAssetsPipeline
{
    public class ArtImporter : AssetPostprocessor
    {
        private static ArtImporterSettings _settings;
        private static readonly Dictionary<string, string> LastSyncedFiles = new Dictionary<string, string>();
        private static readonly List<IArtImportHandler> ImportHandlers = new List<IArtImportHandler>();

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Debug.Log("Initializing ArtImporter...");
            _settings = AssetDatabase.LoadAssetAtPath<ArtImporterSettings>("Assets/Settings/ArtImporterSettings.asset");
            if (_settings == null)
            {
                Debug.LogError("ArtImporterSettings not found. Please create one.");
                return;
            }

            LoadLastSyncedInfo();
            ImportHandlers.Clear();
            ImportHandlers.Add(new CardDataImportHandler(_settings));
            Debug.Log("ArtImporter initialized successfully.");
        }

        [MenuItem("Tools/Art Importer/Sync Google Drive Files")]
        private static void ManualSyncGoogleDriveFiles()
        {
            Debug.Log("Manual sync initiated.");
            if (_settings == null)
            {
                Debug.LogError("ArtImporterSettings not found. Please create one first.");
                return;
            }

            SyncGoogleDriveFiles();
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            Debug.Log("Processing imported assets...");
            if (_settings == null) return;

            foreach (string assetPath in importedAssets)
            {
                Debug.Log($"Checking asset: {assetPath}");
                if (!assetPath.StartsWith(_settings.ImportFolderPath)) continue;
                bool handled = false;

                foreach (var handler in ImportHandlers)
                {
                    Debug.Log($"Checking handler: {handler.GetType().Name} for asset {assetPath}");
                    if (handler.CanHandle(assetPath))
                    {
                        handler.Handle(assetPath);
                        handled = true;
                        Debug.Log($"Handled file {assetPath} with {handler.GetType().Name}");
                    }
                }

                if (!handled)
                {
                    Debug.LogWarning($"No import handler found for asset: {assetPath}");
                }
            }
        }

        private static string GetDestinationPathFromRules(string fileName)
        {
            Debug.Log($"Determining destination for file: {fileName}");
            foreach (var rule in _settings.ImportRules)
            {
                if (fileName.ToLower().Contains(rule.FileNameSuffixOrPattern.ToLower()))
                {
                    Debug.Log($"Match found in rules. Assigning to: {rule.ExportFolder}");
                    return Path.Combine(rule.ExportFolder, fileName);
                }
            }
            Debug.Log("No rule match found. Using default import folder.");
            return Path.Combine(_settings.ImportFolderPath, fileName);
        }

        private static void LoadLastSyncedInfo()
        {
            Debug.Log("Loading last synced file information...");
            string syncInfoPath = Path.Combine(Application.dataPath, "../Library/ArtImporterSyncInfo.txt");
            if (File.Exists(syncInfoPath))
            {
                string[] lines = File.ReadAllLines(syncInfoPath);
                LastSyncedFiles.Clear();
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        LastSyncedFiles[parts[0]] = parts[1];
                    }
                }
            }
        }

        private static void SaveLastSyncedInfo()
        {
            Debug.Log("Saving last synced file information...");
            string syncInfoPath = Path.Combine(Application.dataPath, "../Library/ArtImporterSyncInfo.txt");
            List<string> lines = new List<string>();
            foreach (var kvp in LastSyncedFiles)
            {
                lines.Add($"{kvp.Key}|{kvp.Value}");
            }
            File.WriteAllLines(syncInfoPath, lines);
        }

        private static async void SyncGoogleDriveFiles()
        {
            Debug.Log("Starting Google Drive sync...");
            if (string.IsNullOrEmpty(_settings.GoogleDriveFolderId) ||
                string.IsNullOrEmpty(_settings.GoogleDriveCredentialsJson))
            {
                Debug.LogError("Google Drive settings are not configured.");
                return;
            }

            try
            {
                var credential = GoogleCredential
                    .FromJson(_settings.GoogleDriveCredentialsJson)
                    .CreateScoped(DriveService.Scope.DriveReadonly);

                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ArtImporter"
                });

                var listRequest = service.Files.List();
                listRequest.Q = $"'{_settings.GoogleDriveFolderId}' in parents";
                listRequest.Fields = "files(id, name, modifiedTime)";

                FileList files = await listRequest.ExecuteAsync();
                int newFiles = 0, updatedFiles = 0;

                foreach (var file in files.Files)
                {
                    Debug.Log($"Processing file: {file.Name}");
                    string outputPath = GetDestinationPathFromRules(file.Name);
                    string directory = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    string lastModified = file.ModifiedTimeDateTimeOffset?.ToString("o");
                    if (LastSyncedFiles.TryGetValue(file.Id, out string storedModifiedTime) &&
                        storedModifiedTime == lastModified) continue;

                    await DownloadFileFromGoogleDrive(service, file.Id, outputPath);
                    LastSyncedFiles[file.Id] = lastModified;
                    if (storedModifiedTime == null) newFiles++; else updatedFiles++;
                }

                SaveLastSyncedInfo();
                AssetDatabase.Refresh();
                Debug.Log($"Sync completed. New files: {newFiles}, Updated files: {updatedFiles}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to sync Google Drive files: {ex.Message}");
            }
        }
    }
}