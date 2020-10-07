﻿using Microsoft.Xna.Framework.Graphics;
using SharpFont;

namespace BragiFont
{
    /// <summary>
    /// Various constants defined for the font system.
    /// </summary>
    internal sealed class Constants
    {
        /// <summary>
        /// The default hidef texture size
        /// </summary>
        public const int DEFAULT_HIDEF_TEXTURE_SIZE = 2048;

        /// <summary>
        /// The default reach texture size
        /// </summary>
        public const int DEFAULT_REACH_TEXTURE_SIZE = 1024;

        /// <summary>
        /// The default cache surface format
        /// </summary>
        public const SurfaceFormat DEFAULT_CACHE_SURFACE_FORMAT = SurfaceFormat.Bgra4444;

        /// <summary>
        /// The glyph bitmap origin
        /// </summary>
        public static FTVector26Dot6 GlyphBitmapOrigin = new FTVector26Dot6(0, 0);

        /// <summary>
        /// The default character list
        /// </summary>
        private char[] _defaultCharacterList;

        /// <summary>
        /// The default characters
        /// </summary>
        public string DefaultCharacters = " AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789~`!@#$%^&*()_+-=[]\\{}|;':\",./<>?。？　【】｛｝、｜《》（）…￥";

        /// <summary>
        /// The default load flags
        /// </summary>
        public LoadFlags DefaultLoadFlags = LoadFlags.Default;

        /// <summary>
        /// The default load target
        /// </summary>
        public LoadTarget DefaultLoadTarget = LoadTarget.Normal;

        /// <summary>
        /// The default render mode
        /// </summary>
        public RenderMode DefaultRenderMode = RenderMode.Normal;

        /// <summary>
        /// The default spaces in a tab
        /// </summary>
        public int DefaultSpacesInTab = 4;

        /// <summary>
        /// The kerning sanity multiplier
        /// </summary>
        public int KerningSanityMultiplier = 5;

        /// <summary>
        /// Initializes the <see cref="Constants"/> class.
        /// </summary>
        static Constants() { }

        /// <summary>
        /// Prevents a default instance of the <see cref="Constants"/> class from being created.
        /// </summary>
        private Constants() { }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public static Constants Settings { get; } = new Constants();

        /// <summary>
        /// Gets or sets the default character list.
        /// </summary>
        /// <value>
        /// The default character list.
        /// </value>
        public char[] DefaultCharacterList
        {
            get => _defaultCharacterList ?? (_defaultCharacterList = DefaultCharacters.ToCharArray());
            set
            {
                _defaultCharacterList = value;
                DefaultCharacters = new string(_defaultCharacterList);
            }
        }
    }
}
