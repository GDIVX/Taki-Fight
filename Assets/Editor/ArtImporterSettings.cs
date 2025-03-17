using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Editor
{
    [Serializable]
    public class ArtImportRule
    {
        [BoxGroup("Rule")] public ImportType Type;
        
        [BoxGroup("Rule")]
        [Tooltip("Use either plain text (e.g. '_Artwork') or a regex pattern if you want advanced matching.")]
        public string FileNameSuffixOrPattern;

        [BoxGroup("Rule")]
        [FolderPath(RequireExistingPath = false)]
        [Tooltip("Path (under 'Assets/...') where matching files should be moved.")]
        public string ExportFolder;
        
        public enum ImportType
        {
            CardArt,
            UI,
            Icon,
            Character,
            Extras
        }
    }

    [CreateAssetMenu(fileName = "ArtImporterSettings", menuName = "Settings/ArtImporterSettings", order = 0)]
    public class ArtImporterSettings : ScriptableObject
    {
        [BoxGroup("Folders")] [FolderPath(RequireExistingPath = false)]
        public string ImportFolderPath = "Assets/Art/Cards";

        [BoxGroup("Google Drive")] public string GoogleDriveFolderId;

        [BoxGroup("Google Drive")] [TextArea] public string GoogleDriveCredentialsJson;

        [BoxGroup("Rules")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = true)]
        [Tooltip("List of filename patterns and destinations.")]
        public List<ArtImportRule> ImportRules;
    }
}