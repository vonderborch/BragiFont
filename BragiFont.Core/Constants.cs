using Microsoft.Xna.Framework.Graphics;
using SharpFont;

namespace BragiFont
{
    internal sealed class Constants
    {
        private static readonly Constants _instance = new Constants();

        static Constants() { }

        private Constants() { }

        public static Constants Settings => _instance;

        public string DEFAULT_CHARACTERS = " AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789~`!@#$%^&*()_+-=[]\\{}|;':\",./<>?。？　【】｛｝、｜《》（）…￥";

        private char[] _defaultCharacterList = null;

        public char[] DEFAULT_CHARACTER_LIST
        {
            get
            {
                if (_defaultCharacterList == null)
                {
                    _defaultCharacterList = DEFAULT_CHARACTERS.ToCharArray();
                }

                return _defaultCharacterList;
            }
            set
            {
                _defaultCharacterList = value;
                DEFAULT_CHARACTERS = new string(_defaultCharacterList);
            }
        }

        public const char DEFAULT_PRE_CHARACTER = '\0';

        public const int DEFAULT_HIDEF_TEXTURE_SIZE = 2048;
        public const int DEFAULT_REACH_TEXTURE_SIZE = 1024;

        public const SurfaceFormat DEFAULT_CACHE_SURFACE_FORMAT = SurfaceFormat.Bgra4444;

        public LoadFlags DEFAULT_LOAD_FLAGS = LoadFlags.Default;

        public LoadTarget DEFAULT_LOAD_TARGET = LoadTarget.Normal;

        public RenderMode DEFAULT_RENDER_MODE = RenderMode.Normal;

        public static FTVector26Dot6 GLYPH_BITMAP_ORIGIN = new FTVector26Dot6(0, 0);

        public int DEFAULT_SPACES_IN_TAB = 4;

        public int KERNING_SANITY_MULTIPLIER = 5;
    }
}
