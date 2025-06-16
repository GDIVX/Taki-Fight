using UnityEngine;

namespace Runtime.UI.Tooltip
{
    public interface ITooltipSource
    {
    }

    public interface IContentTooltipSource : ITooltipSource
    {
        string Header { get; }
        string Subtitle { get; }
        string Content { get; }
        Color BackgroundColor { get; }
        Sprite Icon { get; }
    }

    public struct ContentTooltipSource : IContentTooltipSource
    {
        public ContentTooltipSource(string header = "", string subtitle = "", string content = "",
            Color backgroundColor = default,
            Sprite icon = null)
        {
            Header = header;
            Subtitle = subtitle;
            Content = content;
            BackgroundColor = backgroundColor;
            Icon = icon;
        }

        public string Header { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public Color BackgroundColor { get; set; }
        public Sprite Icon { get; set; }
    }
}