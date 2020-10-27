using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BragiFont.Internal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;

namespace BragiFont
{
    /// <summary>
    /// A Font that can be used to draw text to the screen
    /// </summary>
    /// <seealso cref="System.IEquatable{Font}" />
    /// <seealso cref="System.IDisposable" />
    public abstract class Typeface : IEquatable<Typeface>, IDisposable
    {
        private static readonly Dictionary<int, Font> Fonts;

        protected Typeface(string name, byte[] typefaceData, bool preGenerateCharacters, char[] charactersToPreGenerate)
        {
            Name = name;
            TypefaceData = typefaceData;
            PreGenerateCharacters = preGenerateCharacters;
            CharactersToPreGenerate = charactersToPreGenerate;
        }

        public string Name { get; }

        public byte[] TypefaceData { get; }

        public bool PreGenerateCharacters { get; set; }

        public char[] CharactersToPreGenerate { get; set; }

        public Font GetFont(int size, bool? preGenerateCharacters = null, char[] charactersToPreGenerate = null)
        {
            if (!Fonts.TryGetValue(size, out var font))
            {
                var face = new Face(Bragi.Core.BragiLibrary, TypefaceData, 0);
                face.SetCharSize(size, size, 0, 0);
                face.SetTransform();
                font = new FontImplementation(size, face);

                preGenerateCharacters = preGenerateCharacters ?? PreGenerateCharacters;
                charactersToPreGenerate = charactersToPreGenerate ?? CharactersToPreGenerate;
                if ((bool)preGenerateCharacters && charactersToPreGenerate != null)
                {
                    font.PreGenerateCharacterGlyphs(charactersToPreGenerate);
                }

                Fonts.Add(size, font);
            }

            return font;
        }

        public void RemoveFont(int size)
        {
            Fonts.Remove(size);
        }
    }
}
