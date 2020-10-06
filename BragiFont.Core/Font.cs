using BragiFont.Internal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BragiFont
{
    public abstract class Font : IEquatable<Font>, IDisposable
    {
        protected readonly Tuple<string, int> _key;

        protected readonly string _fontFamily;
        protected readonly Face _face;
        protected readonly int _fontSize;

        internal Dictionary<char, Character> _characters;

        private List<GlyphCache> _glyphCaches;

        public Font(Tuple<string, int> key, Face face, int fontSize)
        {
            _key = key;
            _fontFamily = face.FamilyName;
            _fontSize = fontSize;
            _face = face;
            _characters = new Dictionary<char, Character>();
            _glyphCaches = new List<GlyphCache>();

            GlyphHeight = (int)face.Size.Metrics.Height.Ceiling();
        }

        public Tuple<string, int> Key => _key;

        public string FontFamily => _fontFamily;

        public Face Face => _face;

        public int FontSize => _fontSize;

        public LoadFlags LoadFlags { get; set; } = Constants.Settings.DEFAULT_LOAD_FLAGS;

        public LoadTarget LoadTarget { get; set; } = Constants.Settings.DEFAULT_LOAD_TARGET;

        public RenderMode RenderMode { get; set; } = Constants.Settings.DEFAULT_RENDER_MODE;

        public int SpacesInTab { get; set; } = Constants.Settings.DEFAULT_SPACES_IN_TAB;

        public int GlyphHeight { get; private set; }

        public void Dispose()
        {
            _face.Dispose();
            Bragi.Core.RemoveFont(Key);
        }

        public void DisposeFinal()
        {
            _face.Dispose();
        }

        public override bool Equals(object obj)
        {
            return obj == null
                ? false
                : (obj is Font || obj is FontImplementation) && Equals((Font)obj);
        }

        public bool Equals(Font other)
        {
            return Equals(this.Key, other.Key);
        }

        public override int GetHashCode()
        {
            return (_fontFamily.GetHashCode() * 397) ^ _fontSize.GetHashCode();
        }

        public override string ToString()
        {
            return $"font: [{_fontFamily}], size: [{_fontSize}]";
        }

        public static bool operator ==(Font left, Font right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Font left, Font right)
        {
            return !left.Equals(right);
        }

        public void Draw(SpriteBatch spriteBatch, string text, Color color, Rectangle boundaries)
        {
            var warpLine = boundaries.Width > 0;
            var offsetX = 0;
            var offsetY = 0;

            var width = warpLine ? boundaries.Width : spriteBatch.GraphicsDevice.Viewport.Width;
            var height = (boundaries.Height > 0 ? boundaries.Height : spriteBatch.GraphicsDevice.Viewport.Height) - boundaries.Y;

            var countX = 0;
            var underrun = 0;
            var finalCharacterIndex = text.Length - 1;

            for (var i = 0; i < text.Length; i++)
            {
                TryGetCharacter(text[i], out var cachedCharacter);

                if (warpLine && offsetX + cachedCharacter.Boundary.Width + countX > width || text[i] == '\n')
                {
                    offsetX = 0;
                    underrun = 0;
                    offsetY += cachedCharacter.Boundary.Height;
                }
                if (text[i] == '\r' || text[i] == '\n')
                {
                    continue;
                }
                if (offsetY > height || (!warpLine && offsetX > width))
                {
                    return;
                }

                // calculate underrun
                underrun += -(cachedCharacter.BearingX);
                if (offsetX == 0)
                {
                    offsetX += underrun;
                }
                if (underrun <= 0)
                {
                    underrun = 0;
                }

                spriteBatch.Draw(cachedCharacter.Texture, new Vector2(boundaries.X + offsetX, boundaries.Y + offsetY), cachedCharacter.Boundary, color);
                offsetX += cachedCharacter.Boundary.Width;

                // calculate kerning
                if (i != finalCharacterIndex)
                {
                    var nextCharacter = text[i + 1];
                    if (TryGetCharacter(nextCharacter, out var nextCachedCharacter))
                    {
                        var kerning = GetKerning(cachedCharacter, nextCachedCharacter);
                        var maxBounds = cachedCharacter.AdvanceX * Constants.Settings.KERNING_SANITY_MULTIPLIER;
                        if (kerning <= maxBounds && kerning >= -maxBounds)
                        {
                            offsetX += kerning;
                        }
                    }
                }
            }
        }

        public Text MakeText(string text)
        {
            var finalSize = new Vector2(0, 0);

            var textResult = new TextImplementation(text);

            var offsetX = 0;
            var offsetY = 0;

            var underrun = 0;
            var finalCharacterIndex = text.Length - 1;

            for (var i = 0; i < text.Length; i++)
            {
                TryGetCharacter(text[i], out var cachedCharacter);
                if (i == 0)
                {
                    finalSize.Y += cachedCharacter.Boundary.Height;
                }

                if (text[i] == '\n')
                {
                    finalSize.X = Math.Max(offsetX, finalSize.X);
                    offsetX = 0;
                    underrun = 0;
                    offsetY += cachedCharacter.Boundary.Height;
                    if (i != finalCharacterIndex)
                    {
                        finalSize.Y += cachedCharacter.Boundary.Height;
                    }
                }
                if (text[i] == '\r' || text[i] == '\n')
                {
                    continue;
                }

                // calculate underrun
                underrun += -(cachedCharacter.BearingX);
                if (offsetX == 0)
                {
                    offsetX += underrun;
                }
                if (underrun <= 0)
                {
                    underrun = 0;
                }

                textResult.AddCharacter(new TextCharacter(cachedCharacter, new Vector2(offsetX, offsetY)));
                offsetX += cachedCharacter.Boundary.Width;

                // calculate kerning
                if (i != finalCharacterIndex)
                {
                    var nextCharacter = text[i + 1];
                    if (TryGetCharacter(nextCharacter, out var nextCachedCharacter))
                    {
                        var kerning = GetKerning(cachedCharacter, nextCachedCharacter);
                        var maxBounds = cachedCharacter.AdvanceX * Constants.Settings.KERNING_SANITY_MULTIPLIER;
                        if (kerning <= maxBounds && kerning >= -maxBounds)
                        {
                            offsetX += kerning;
                        }
                    }
                }
            }

            return textResult;
        }

        public Vector2 MeasureText(string text)
        {
            var finalSize = new Vector2(0, 0);

            var offsetX = 0;
            var offsetY = 0;

            var underrun = 0;
            var finalCharacterIndex = text.Length - 1;

            for (var i = 0; i < text.Length; i++)
            {
                TryGetCharacter(text[i], out var cachedCharacter);
                if (i == 0)
                {
                    finalSize.Y += cachedCharacter.Boundary.Height;
                }

                if (text[i] == '\n')
                {
                    finalSize.X = Math.Max(offsetX, finalSize.X);
                    offsetX = 0;
                    underrun = 0;
                    offsetY += cachedCharacter.Boundary.Height;
                    if (i != finalCharacterIndex)
                    {
                        finalSize.Y += cachedCharacter.Boundary.Height;
                    }
                }
                if (text[i] == '\r' || text[i] == '\n')
                {
                    continue;
                }

                // calculate underrun
                underrun += -(cachedCharacter.BearingX);
                if (offsetX == 0)
                {
                    offsetX += underrun;
                }
                if (underrun <= 0)
                {
                    underrun = 0;
                }

                offsetX += cachedCharacter.Boundary.Width;

                // calculate kerning
                if (i != finalCharacterIndex)
                {
                    var nextCharacter = text[i + 1];
                    if (TryGetCharacter(nextCharacter, out var nextCachedCharacter))
                    {
                        var kerning = GetKerning(cachedCharacter, nextCachedCharacter);
                        var maxBounds = cachedCharacter.AdvanceX * Constants.Settings.KERNING_SANITY_MULTIPLIER;
                        if (kerning <= maxBounds && kerning >= -maxBounds)
                        {
                            offsetX += kerning;
                        }
                    }
                }
            }

            return finalSize;
        }

        private int GetKerning(Character left, Character right)
        {
            if (left.Kerning.TryGetValue(right.Char, out var kerning))
            {
                left.Kerning[right.Char] = kerning = (int)_face.GetKerning((uint)left.Index, (uint)right.Index, KerningMode.Default).X;
            }

            return kerning;
        }

        private bool TryGetCharacter(char character, out Character bragiCharacter)
        {
            if (!_characters.TryGetValue(character, out bragiCharacter))
            {
                bragiCharacter = GenerateCharacter(character);
                _characters.Add(character, bragiCharacter);
            }
            return true;
        }

        private Character GenerateCharacter(char character)
        {
            var glyphCache = _glyphCaches.FirstOrDefault(c => !c.Full);
            if (glyphCache == null)
            {
                glyphCache = new GlyphCache(this);
                _glyphCaches.Add(glyphCache);
            }

            if (!glyphCache.AddCharacterToCache(character, out var cachedCharacter))
            {
                glyphCache = new GlyphCache(this);
                var finalAttempt = glyphCache.AddCharacterToCache(character, out cachedCharacter);
                if (!finalAttempt)
                {
                    throw new Exception($"Could not generate character [{character}]!");
                }
                _glyphCaches.Add(glyphCache);
            }

            return cachedCharacter;
        }
    }
}
