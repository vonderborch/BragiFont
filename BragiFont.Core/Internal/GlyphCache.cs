using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;
using System;
using System.Collections.Generic;

namespace BragiFont.Internal
{
    internal class GlyphCache
    {
        private Font _font;

        public Texture2D Texture;

        public static int Width = Constants.DEFAULT_REACH_TEXTURE_SIZE;

        public static int Height = Constants.DEFAULT_HIDEF_TEXTURE_SIZE;

        private int CurrentX = 0;
        private int CurrentY = 0;

        public bool Full = false;

        private List<char> characters = new List<char>();

        private static ushort[] buffer;

        public GlyphCache(Font font)
        {
            _font = font;
            switch (Bragi.Core.GraphicsDevice.GraphicsProfile)
            {
                case GraphicsProfile.HiDef:
                    Height = Constants.DEFAULT_HIDEF_TEXTURE_SIZE;
                    Width = Constants.DEFAULT_HIDEF_TEXTURE_SIZE;
                    break;
                case GraphicsProfile.Reach:
                    Height = Constants.DEFAULT_REACH_TEXTURE_SIZE;
                    Width = Constants.DEFAULT_HIDEF_TEXTURE_SIZE;
                    break;
            }

            Texture = new Texture2D(Bragi.Core.GraphicsDevice, Width, Height, false, Constants.DEFAULT_CACHE_SURFACE_FORMAT);
        }

        public bool AddCharacterToCache(char character, out Character characterCache)
        {
            var index = _font.Face.GetCharIndex(character);
            _font.Face.LoadGlyph(index, _font.LoadFlags, _font.LoadTarget);
            using (var glyph = _font.Face.Glyph.GetGlyph())
            {
                glyph.ToBitmap(_font.RenderMode, Constants.GLYPH_BITMAP_ORIGIN, true);

                using (var bitmap = glyph.ToBitmapGlyph())
                {
                    if (CurrentX + glyph.Advance.X.Ceiling() >= Width)
                    {
                        CurrentY += _font.GlyphHeight + _font.Face.Size.Metrics.NominalHeight;
                        CurrentX = 0;
                    }
                    if (CurrentY >= Height - _font.GlyphHeight)
                    {
                        Full = true;
                        characterCache = null;
                        return false;
                    }

                    characterCache = AddCharacter(character, glyph, bitmap);
                }
            }

            return true;
        }

        private Character AddCharacter(char character, Glyph glyph, BitmapGlyph bitmapGlyph)
        {
            if (!(bitmapGlyph.Bitmap.Width == 0 || bitmapGlyph.Bitmap.Rows == 0))
            {
                var cBox = glyph.GetCBox(GlyphBBoxMode.Pixels);
                var bearingY = _font.Face.Glyph.Metrics.VerticalAdvance.Ceiling();
                var rectangle = new Rectangle(CurrentX + cBox.Left, CurrentY + (bearingY - cBox.Top), bitmapGlyph.Bitmap.Width, bitmapGlyph.Bitmap.Rows);
                var dataLength = bitmapGlyph.Bitmap.BufferData.Length;
                buffer = new ushort[dataLength];

                for (var i = 0; i < buffer.Length; i++)
                {
                    var c = bitmapGlyph.Bitmap.BufferData[i] >> 4;
                    buffer[i] = (ushort)((c << 4) | (c << 8) | (c << 12) | c);
                }

                if (character < 255 && character != '_')
                {
                    rectangle.Y += 1;
                }

                if (rectangle.X < 0)
                {
                    rectangle.Offset(-rectangle.X, 0);
                }

                if (rectangle.Y < 0)
                {
                    rectangle.Offset(0, -rectangle.Y);
                }

                if (glyph.Advance.X.Ceiling() != rectangle.Width)
                {
                    rectangle.Offset(Math.Abs(rectangle.Width - glyph.Advance.X.Ceiling()) / 2, 0);
                }

                Texture.SetData(0, rectangle, buffer, 0, dataLength);
            }

            characters.Add(character);

            var advanceX = glyph.Advance.X.Ceiling();
            if (character == '\t')
            {
                advanceX = Math.Abs(_font.Face.Size.Metrics.NominalWidth * _font.SpacesInTab);
            }

            var finalCharacter = new Character()
            {
                Boundary = new Rectangle(CurrentX, CurrentY, advanceX, _font.GlyphHeight + _font.Face.Size.Metrics.NominalHeight),
                Texture = this.Texture,
                Index = characters.Count - 1,
                PreCharacter = characters.Count > 1 ? characters[characters.Count - 2] : (char?)null,
                Char = character,
                AdvanceX = glyph.Advance.X.Ceiling(),
                BearingX = _font.Face.Glyph.Metrics.HorizontalBearingX.Ceiling(),
            };
            CurrentX += advanceX + _font.Face.Size.Metrics.NominalWidth;
            return finalCharacter;

            //throw new Exception("Failed to add character to the cache!");
        }
    }
}
