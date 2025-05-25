using System.IO;
using System.Linq;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn;
using UnityEditor;
using UnityEngine;
// For PawnData

// For CardData

namespace Editor.ArtPipeline
{
    public static class SpriteSorter
    {
        private const string CharactersFolder = "Assets/Sprites/Characters";
        private const string CardArtFolder = "Assets/Sprites/Card Art";

        /// <summary>
        ///     Sorts and assigns sprites, allowing for overrides based on the defined versioning rules.
        /// </summary>
        public static void SortAndAssignSprites(string assetPath)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (!sprite)
            {
                Debug.LogError($"[SpriteSorter] Failed to load sprite at path: {assetPath}");
                return;
            }

            string destinationFolder;

            // Determine the destination folder
            if (ShouldBeCharacterSprite(sprite))
            {
                destinationFolder = CharactersFolder;
            }
            else if (ShouldBeCardArtSprite(sprite))
            {
                destinationFolder = CardArtFolder;
            }
            else
            {
                Debug.LogWarning(
                    $"[SpriteSorter] Sprite '{sprite.name}' does not match known categories. Skipping processing.");
                return;
            }

            // Handle sprite movement
            var spriteFileName = Path.GetFileName(assetPath);
            var newAssetPath = $"{destinationFolder}/{spriteFileName}";
            AssetDatabase.MoveAsset(assetPath, newAssetPath);
            Debug.Log($"[SpriteSorter] Moved sprite: {sprite.name} to '{destinationFolder}'.");

            // Assign or override sprite on relevant ScriptableObjects
            AssignOrOverrideSprite(sprite, destinationFolder);
        }

        /// <summary>
        ///     Determines if the sprite fits the Character category.
        /// </summary>
        private static bool ShouldBeCharacterSprite(Sprite sprite)
        {
            return sprite.name.ToLower().Contains("char_");
        }

        /// <summary>
        ///     Determines if the sprite fits the Card Art category.
        /// </summary>
        private static bool ShouldBeCardArtSprite(Sprite sprite)
        {
            return sprite.name.ToLower().Contains("card_");
        }

        /// <summary>
        ///     Assigns or overrides a sprite in ScriptableObjects based on versioning rules.
        ///     Old assets are deleted if they are replaced.
        /// </summary>
        private static void AssignOrOverrideSprite(Sprite sprite, string destinationFolder)
        {
            // Process for characters (PawnData)
            if (ShouldBeCharacterSprite(sprite))
            {
                var pawnDataList = LoadAllAssets<PawnData>();
                foreach (var pawnData in pawnDataList)
                {
                    if (!ShouldOverride(pawnData.Sprite, sprite, destinationFolder)) continue;
                    DeleteOldSprite(pawnData.Sprite); // Delete existing sprite before replacement
                    pawnData.Sprite = sprite;
                    EditorUtility.SetDirty(pawnData);
                    Debug.Log(
                        $"[SpriteSorter] Assigned/overridden sprite '{sprite.name}' in PawnData '{pawnData.name}'.");
                }
            }

            // Process for card art (CardData)
            if (!ShouldBeCardArtSprite(sprite)) return;
            var cardDataList = LoadAllAssets<CardData>();
            foreach (var cardData in cardDataList)
                if (ShouldOverride(cardData.Image, sprite, destinationFolder))
                {
                    DeleteOldSprite(cardData.Image); // Delete existing sprite before replacement
                    cardData.Image = sprite;
                    EditorUtility.SetDirty(cardData);
                    Debug.Log(
                        $"[SpriteSorter] Assigned/overridden sprite '{sprite.name}' in CardData '{cardData.name}'.");
                }
        }

        /// <summary>
        ///     Determines whether the sprite should override the existing asset based on versioning rules.
        /// </summary>
        private static bool ShouldOverride(Sprite existingSprite, Sprite newSprite, string destinationFolder)
        {
            if (existingSprite == null) return true; // No current assignment, so always assign

            // Always override if the new sprite has "temp" in its name
            if (newSprite.name.ToLower().Contains("temp")) return true;

            // Handle versioning (e.g., v1, v2, v3)
            var existingVersion = ExtractVersion(existingSprite.name);
            var newVersion = ExtractVersion(newSprite.name);

            if (existingVersion >= 0 && newVersion > existingVersion) return true; // New version is higher

            // Do not override if the existing version is equal or higher
            Debug.Log(
                $"[SpriteSorter] Skipping override. Existing sprite '{existingSprite.name}' is newer or equal in version.");
            return false;
        }

        /// <summary>
        ///     Deletes an older sprite from the project.
        /// </summary>
        private static void DeleteOldSprite(Sprite oldSprite)
        {
            if (oldSprite == null) return;

            var oldSpritePath = AssetDatabase.GetAssetPath(oldSprite);
            if (!string.IsNullOrEmpty(oldSpritePath) && AssetDatabase.DeleteAsset(oldSpritePath))
                Debug.Log($"[SpriteSorter] Deleted old sprite at '{oldSpritePath}'.");
        }

        /// <summary>
        ///     Extracts a version number (e.g., v1, v2) from the sprite name.
        ///     Returns -1 if no version information is found.
        /// </summary>
        private static int ExtractVersion(string name)
        {
            var versionTag = name.ToLower()
                .Split('_')
                .FirstOrDefault(s => s.StartsWith("v") && int.TryParse(s.Substring(1), out _));

            if (versionTag != null && int.TryParse(versionTag.Substring(1), out var version)) return version;

            return -1; // No version found
        }

        /// <summary>
        ///     Loads all assets of type <typeparamref name="T" /> in the project.
        /// </summary>
        private static T[] LoadAllAssets<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            return guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
    }
}