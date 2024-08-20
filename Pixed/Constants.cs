using System.Drawing;

namespace Pixed
{
    internal static class Constants
    {
        public const int DEFAULT_WIDTH = 16;
        public const int DEFAULT_HEIGHT = 16;
        public const int DEFAULT_FPS = 24;
        public const double LAYER_OPACITY = 0.2;

        public const int MODEL_VERSION = 2;

        public const int MAX_WIDTH = 1024;
        public const int MAX_HEIGHT = 1024;

        public const int MAX_PALETTE_COLORS = 256;
        public const int MAX_WORKER_COLORS = 256;

        public const int PREVIEW_FILM_SIZE = 96;
        public const int ANIMATED_PREVIEW_WIDTH = 200;

        public const int RIGHT_COLUMN_PADDING_LEFT = 10;

        public static readonly Color DEFAULT_PEN_COLOR = Color.Black;
        public static readonly Color TRANSPARENT_COLOR = Color.Transparent;
        public static readonly Color SEAMLESS_MODE_OVERLAY_COLOR = Color.FromArgb(0, 255, 255, 255);

        public const string CURRENT_COLORS_PALETTE_ID = "current-colors";
        public static readonly Color SELECTION_TRANSPARENT_COLOR = Color.FromArgb(153, 160, 215, 240);

        public static readonly Color TOOL_HIGHLIGHT_COLOR_LIGHT = Color.FromArgb(51, 255, 255, 255);
        public static readonly Color TOOL_HIGHLIGHT_COLOR_DARK = Color.FromArgb(51, 0, 0, 0);

        public static readonly Color ZOOMED_OUT_BACKGROUND_COLOR = Color.FromArgb(160, 160, 160);




    }
}
