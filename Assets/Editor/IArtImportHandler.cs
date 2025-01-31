namespace Editor.ArtAssetsPipeline
{
    public interface IArtImportHandler
    {
        /// <summary>
        /// Checks if this handler is responsible for handling the given asset.
        /// Return true if it should handle the asset.
        /// </summary>
        bool CanHandle(string assetPath);

        /// <summary>
        /// Executes the logic for importing/moving the asset.
        /// </summary>
        void Handle(string assetPath);
    }
}