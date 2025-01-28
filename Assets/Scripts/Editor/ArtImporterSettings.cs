using Sirenix.OdinInspector;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(fileName = "ArtImporterSettings", menuName = "Settings/ArtImporterSettings", order = 0)]
    public class ArtImporterSettings : ScriptableObject
    {
        [FolderPath, BoxGroup("Folders")]
        public string OutputFolderPath = "Assets/Art/Cards";

        [BoxGroup("Google Drive")]
        public string GoogleDriveFolderId;

        [BoxGroup("Google Drive")]
        public string GoogleDriveCredentialsJson;
    }
}