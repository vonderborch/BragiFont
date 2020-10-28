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
        private static readonly Dictionary<int, Font> Fonts = new Dictionary<int, Font>();

        protected Typeface(string name, byte[] typefaceData, bool preGenerateCharacters, char[] charactersToPreGenerate, bool storeTypefaceFileData)
        {
            Name = name;

            if (storeTypefaceFileData)
            {
                TypefaceData = typefaceData;
            }
            else
            {
                TypefaceData = null;
            }

            PreGenerateCharacters = preGenerateCharacters;
            CharactersToPreGenerate = charactersToPreGenerate;
        }

        public string Name { get; }

        public byte[] TypefaceData { get; }

        public bool PreGenerateCharacters { get; set; }

        public char[] CharactersToPreGenerate { get; set; }

        public void Dispose()
        {
            DisposeFinal();
            Bragi.Core.RemoveTypeface(Name);
        }

        public void DisposeFinal()
        {
            for (var i = 0; i < Fonts.Count; i++)
            {
                Fonts[i].DisposeFinal();
            }
        }

        public bool Equals(Typeface other)
        {
            return !(other is null) && Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return obj != null && (obj is Typeface || obj is TypefaceImplementation) && Equals((Typeface)obj);
        }

        public Font GetFont(int size, bool? preGenerateCharacters = null, char[] charactersToPreGenerate = null)
        {
            if (!Fonts.TryGetValue(size, out var font))
            {
                var face = GetFace();
                face.SetCharSize(size, size, 0, 0);
                face.SetTransform();
                font = new FontImplementation(size, face, Name);

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

        private Face GetFace()
        {
            if (TypefaceData == null)
            {
                return Bragi.Core.BragiLibrary.NewFace(Name, 0);
            }

            return new Face(Bragi.Core.BragiLibrary, TypefaceData, 0);
        }
    }
}
