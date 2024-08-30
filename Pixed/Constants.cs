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

        public static readonly UniColor DEFAULT_PEN_COLOR = UniColor.Black;
        public static readonly UniColor SEAMLESS_MODE_OVERLAY_COLOR = new UniColor(0, 255, 255, 255);

        public const string CURRENT_COLORS_PALETTE_ID = "current-colors";
        public static readonly UniColor SELECTION_TRANSPARENT_COLOR = new UniColor(153, 160, 215, 240);

        public static readonly UniColor TOOL_HIGHLIGHT_COLOR_LIGHT = new UniColor(51, 255, 255, 255);
        public static readonly UniColor TOOL_HIGHLIGHT_COLOR_DARK = new UniColor(51, 0, 0, 0);

        public static readonly UniColor ZOOMED_OUT_BACKGROUND_COLOR = new UniColor(160, 160, 160);




    }
}
