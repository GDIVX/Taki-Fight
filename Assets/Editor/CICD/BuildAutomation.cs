using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CICD;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Editor.CICD
{
    public static class BuildAutomation
    {
        private static BuildAutomationSettings LoadSettings()
        {
            var guids = AssetDatabase.FindAssets("t:BuildAutomationSettings");
            if (guids.Length == 0)
            {
                Debug.LogError("❌ No BuildAutomationSettings asset found.");
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<BuildAutomationSettings>(path);
        }

        [MenuItem("Tools/Build/Build, Upload to Itch, and Notify")]
        public static void BuildAndDeploy()
        {
            var settings = LoadSettings();
            if (settings == null) return;

            string version = ExtractVersion(settings.FullChangelogPath);
            string shortChangelog = LoadChangelog(settings.ShortChangelogPath, version);

            string buildFolder =
                EditorUtility.SaveFolderPanel("Choose Build Output Folder", settings.DefaultBuildFolder, "");
            if (string.IsNullOrEmpty(buildFolder)) return;

            string exePath = Path.Combine(buildFolder, settings.BuildExeName + ".exe");

            // Build
            string[] scenes = System.Array.ConvertAll(EditorBuildSettings.scenes, s => s.path);
            var report = BuildPipeline.BuildPlayer(scenes, exePath, BuildTarget.StandaloneWindows64, BuildOptions.None);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError("❌ Build failed.");
                return;
            }

            // Upload to Itch.io
            if (!PushToItch(settings.ButlerExeFolder, buildFolder, settings.ItchChannel))
            {
                Debug.LogError("❌ Failed to upload build to Itch.io.");
                return;
            }

            string itchLink =
                $"https://{settings.ItchChannel.Split('/')[0]}.itch.io/{settings.ItchChannel.Split('/')[1].Split(':')[0]}";

            // Discord notify — fire and forget
            NotifyDiscord(settings.DiscordWebhookUrl, version, itchLink, shortChangelog).ContinueWith(task =>
            {
                if (task.IsFaulted)
                    Debug.LogError("❌ Discord webhook failed: " + task.Exception?.GetBaseException().Message);
                else
                    Debug.Log("✅ Discord webhook sent.");
            });

            Debug.Log("✅ Build and upload complete.");
        }

        private static string ExtractVersion(string path)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "[0.0.1]\n- Initial version.");
                AssetDatabase.Refresh();
                return "0.0.1";
            }

            foreach (var line in File.ReadLines(path))
            {
                var match = Regex.Match(line.Trim(), @"\[(\d+\.\d+\.\d+)\]");
                if (match.Success)
                    return match.Groups[1].Value;
            }

            File.WriteAllText(path, "[0.0.1]\n- Initial version.");
            AssetDatabase.Refresh();
            return "0.0.1";
        }

        private static string LoadChangelog(string path, string version)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, $"[{version}]\n");
                AssetDatabase.Refresh();
            }

            return File.ReadAllText(path).Trim();
        }

        private static bool PushToItch(string butlerFolder, string buildFolder, string channel)
        {
            string butlerExe = Path.Combine(butlerFolder, "butler.exe");

            if (!File.Exists(butlerExe))
            {
                Debug.LogError($"❌ Butler not found at: {butlerExe}");
                return false;
            }

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = butlerExe,
                    Arguments = $"push \"{buildFolder}\" {channel}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            Debug.Log(output);
            return proc.ExitCode == 0;
        }

        private static async Task NotifyDiscord(string webhookUrl, string version, string link, string changelog)
        {
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                Debug.LogWarning("⚠️ No Discord webhook URL provided.");
                return;
            }

            string msg = $"🚨 **New Build {version}**\n📥 [Download here]({link})\n```md\n{changelog}\n```";

            Debug.Log("Discord Payload:\n" + msg);

            try
            {
                using var client = new HttpClient();
                var payload = new { content = msg };
                var json = JsonConvert.SerializeObject(payload); 
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(webhookUrl, content);

                if (!response.IsSuccessStatusCode)
                    Debug.LogError($"❌ Discord notification failed: {response.StatusCode}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Discord webhook threw exception: {ex.Message}");
            }
        }
    }
}