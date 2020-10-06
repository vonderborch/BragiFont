using BragiFont.Internal;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;
using System;
using System.Collections.Generic;

namespace BragiFont
{
    public sealed class Bragi : IDisposable
    {
        private static Library _library = new Library();

        private static readonly Dictionary<Tuple<string, int>, Font> _fonts = new Dictionary<Tuple<string, int>, Font>();

        static Bragi() { }

        private Bragi() { }

        public static Bragi Core { get; } = new Bragi();

        internal GraphicsDevice GraphicsDevice { get; private set; }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public Font GetFont(string path, int size, GraphicsDevice graphicsDevice = null)
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
            if (!_fonts.TryGetValue(key, out var font))
            {
                var face = _library.NewFace(path, 0);
                var fontSize = Fixed26Dot6.FromInt32(size);
                face.SetCharSize(fontSize, fontSize, 0, 0);
                face.SetTransform();
                font = new FontImplementation(key, face, size);
                _fonts.Add(key, font);
            }

            return font;
        }

        internal void RemoveFont(Tuple<string, int> key)
        {
            _fonts.Remove(key);
        }

        public void Dispose()
        {
            foreach (var font in _fonts.Values)
            {
                font.DisposeFinal();
            }

            _library.Dispose();
        }
    }
}
