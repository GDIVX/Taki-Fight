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
using File = System.IO.File;

namespace Editor.ArtAssetsPipeline
{
    /// <summary>
    /// A simplified importer that does not rely on OnPostprocessAllAssets.
    /// We manually download files, import them, and apply handlers.
    /// </summary>
    public static class ArtImporter
    {
        private static ArtImporterSettings _settings;
        private static readonly Dictionary<string, string> LastSyncedFiles = new Dictionary<string, string>();
        private static readonly List<IArtImportHandler> ImportHandlers = new List<IArtImportHandler>();

        [MenuItem("Tools/Art Importer (No Postprocessor)/Sync Google Drive Files")]
        public static void ManualSync()
        {
            Debug.Log("Manual sync initiated (No Postprocessor).");

            // Grab settings
            _settings = AssetDatabase.LoadAssetAtPath<ArtImporterSettings>(
                "Assets/Settings/ArtImporterSettings.asset"
            );
            if (_settings == null)
            {
                Debug.LogError("ArtImporterSettings not found. Please create one first.");
                return;
            }

            // Load last synced info
            LoadLastSyncedInfo();

            // Build up handlers
            ImportHandlers.Clear();
            ImportHandlers.Add(new CardDataImportHandler(_settings));

            // Kick off sync
            SyncGoogleDriveFiles();
        }

        private static void LoadLastSyncedInfo()
        {
            var syncInfoPath = Path.Combine(Application.dataPath, "../Library/ArtImporterSyncInfo.txt");
            if (!File.Exists(syncInfoPath)) return;

            var lines = File.ReadAllLines(syncInfoPath);
            LastSyncedFiles.Clear();

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 2) LastSyncedFiles[parts[0]] = parts[1];
            }
        }

        private static void SaveLastSyncedInfo()
        {
            var syncInfoPath = Path.Combine(Application.dataPath, "../Library/ArtImporterSyncInfo.txt");
            var lines = LastSyncedFiles.Select(kvp => $"{kvp.Key}|{kvp.Value}").ToArray();
            File.WriteAllLines(syncInfoPath, lines);
        }

        private static async void SyncGoogleDriveFiles()
        {
            Debug.Log("Starting Google Drive sync (No Postprocessor)...");

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

                using var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ArtImporterNoPostprocessor"
                });

                var listRequest = service.Files.List();
                listRequest.Q = $"'{_settings.GoogleDriveFolderId}' in parents";
                listRequest.Fields = "files(id, name, modifiedTime)";
                var files = await listRequest.ExecuteAsync();

                int newFiles = 0, updatedFiles = 0;
                var downloadedFilePaths = new List<string>();

                // 1) Download everything
                foreach (var driveFile in files.Files)
                {
                    var modified = driveFile.ModifiedTimeDateTimeOffset?.ToString("o");
                    if (LastSyncedFiles.TryGetValue(driveFile.Id, out var knownTime) &&
                        knownTime == modified)
                    {
                        // No change
                        continue;
                    }

                    // Build local path
                    var outputPath = GetDestinationPathFromRules(driveFile.Name);
                    outputPath = outputPath.Replace('\\', '/');
                    var dir = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                    // Download
                    await DownloadFileFromGoogleDrive(service, driveFile.Id, outputPath);

                    LastSyncedFiles[driveFile.Id] = modified;
                    if (knownTime == null) newFiles++; else updatedFiles++;
                    downloadedFilePaths.Add(outputPath);
                }

                // 2) Save info & do a single refresh
                SaveLastSyncedInfo();
                AssetDatabase.Refresh();

                // 3) For each downloaded file, run custom handlers
                foreach (var filePath in downloadedFilePaths)
                {
                    var assetPath = ConvertFullPathToAssetPath(filePath);

                    // (a) Check which handler can handle this file
                    bool handled = false;
                    foreach (var handler in ImportHandlers)
                    {
                        if (handler.CanHandle(assetPath))
                        {
                            handler.Handle(assetPath);
                            handled = true;
                            break;
                        }
                    }

                    if (!handled)
                    {
                        Debug.LogWarning($"No import handler found for: {assetPath}");
                    }
                }

                Debug.Log($"Sync completed. New: {newFiles}, Updated: {updatedFiles}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to sync Google Drive files (No Postprocessor): {ex.Message}");
            }
        }

        private static string GetDestinationPathFromRules(string fileName)
        {
            if (_settings.ImportRules == null || _settings.ImportRules.Count == 0)
            {
                Debug.LogWarning("No import rules configured. Using default import folder.");
                return Path.Combine(_settings.ImportFolderPath, fileName);
            }

            foreach (var rule in _settings.ImportRules)
            {
                if (fileName.ToLower().Contains(rule.FileNameSuffixOrPattern.ToLower()))
                {
                    return Path.Combine(rule.ExportFolder, fileName);
                }
            }

            return Path.Combine(_settings.ImportFolderPath, fileName);
        }

        private static async Task DownloadFileFromGoogleDrive(DriveService service, string fileId, string outputPath)
        {
            Debug.Log($"Downloading file {fileId} to {outputPath}");

            var request = service.Files.Get(fileId);
            await using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            var result = await request.DownloadAsync(stream);

            Debug.Log($"Downloaded file: {outputPath}, Status={result.Status}");
            long size = new FileInfo(outputPath).Length;
            Debug.Log($"File Size: {size} bytes");
        }

        private static string ConvertFullPathToAssetPath(string fullPath)
        {
            fullPath = fullPath.Replace('\\', '/');
            var projectPath = Application.dataPath.Replace("Assets", "");
            var relativePath = fullPath.Replace(projectPath, "");
            return relativePath;
        }
    }
}
