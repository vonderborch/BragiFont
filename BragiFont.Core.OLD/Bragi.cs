using System;
using System.Collections.Generic;
using BragiFont.Internal;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;

namespace BragiFont
{
    /// <summary>
    /// The core font system.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public sealed class Bragi : IDisposable
    {
        /// <summary>
        /// The SharpFont library.
        /// </summary>
        private static readonly Library Library;

        /// <summary>
        /// The fonts we haved cached.
        /// </summary>
        private static readonly Dictionary<Tuple<string, int>, Font> Fonts;

        /// <summary>
        /// Initializes the <see cref="Bragi"/> class.
        /// </summary>
        static Bragi()
        {
            Library = new Library();
            Fonts = new Dictionary<Tuple<string, int>, Font>();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Bragi"/> class from being created.
        /// </summary>
        private Bragi() { }

        /// <summary>
        /// Gets the FontSystem.
        /// </summary>
        /// <value>
        /// The FontSystem.
        /// </value>
        public static Bragi Core { get; } = new Bragi();

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>
        /// The graphics device.
        /// </value>
        internal GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var font in Fonts.Values)
            {
                font.DisposeFinal();
            }

            Library.Dispose();
        }

        /// <summary>
        /// Initializes the specified graphics device.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Gets or loads the specified font with the specified size.
        /// </summary>
        /// <param name="path">The path to load the font from.</param>
        /// <param name="size">The size of the font.</param>
        /// <param name="graphicsDevice">The graphics device. Optional, but required if Bragi.Core.Initialize() hasn't been called yet.</param>
        /// <param name="preGenerateCharacters">if set to <c>true</c> [pre generate characters].</param>
        /// <param name="charactersToPregenerate">The characters to pregenerate.</param>
        /// <returns>The Font that matches the specified parameters.</returns>
        /// <exception cref="Exception">GraphicsDevice is not initialized! Please either initialize Bragi.Core or provide the GraphicsDevice when getting a new font.</exception>
        public Font GetFont(string path, int size, GraphicsDevice graphicsDevice = null, bool preGenerateCharacters = false, char[] charactersToPregenerate = null)
        {
            if (graphicsDevice == null && GraphicsDevice == null)
            {
                throw new Exception("GraphicsDevice is not initialized! Please either initialize Bragi.Core or provide the GraphicsDevice when getting a new font.");
            }

            if (graphicsDevice != null)
            {
                GraphicsDevice = graphicsDevice;
            }

            var key = new Tuple<string, int>(path, size);
            if (!Fonts.TryGetValue(key, out var font))
            {
                var face = Library.NewFace(path, 0);
                var fontSize = Fixed26Dot6.FromInt32(size);
                face.SetCharSize(fontSize, fontSize, 0, 0);
                face.SetTransform();
                font = new FontImplementation(key, face, size);
                if (preGenerateCharacters)
                {
                    font.PregenerateCharacters(charactersToPregenerate);
                }

                Fonts.Add(key, font);
            }

            return font;
        }

        /// <summary>
        /// Removes a font from the system.
        /// </summary>
        /// <param name="key">The key for the font we want to remove.</param>
        internal void RemoveFont(Tuple<string, int> key)
        {
            Fonts.Remove(key);
        }
    }
}
