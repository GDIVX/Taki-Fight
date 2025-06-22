using Sirenix.OdinInspector;
using UnityEngine;

namespace CICD
{
    [CreateAssetMenu(fileName = "BuildAutomationSettings", menuName = "Automation/Build Settings")]
    public class BuildAutomationSettings : ScriptableObject
    {
        [FilePath(Extensions = "md", RequireExistingPath = false)]
        public string FullChangelogPath;

        [FilePath(Extensions = "txt", RequireExistingPath = false)]
        public string ShortChangelogPath;

        [FolderPath(RequireExistingPath = false)]
        public string DefaultBuildFolder;

        [InfoBox("Name of the final executable file, without extension.")]
        public string BuildExeName = "SummonersReach";
        
        [FolderPath(RequireExistingPath = true)]
        public string ButlerExeFolder;

        public string ItchChannel = "yourname/summoners-reach:win-playtest";

        public string DiscordWebhookUrl;
    }
}