using SharpFont;
using System;
using System.Collections.Generic;
using System.Text;

namespace BragiFont
{
    public abstract class Font : IEquatable<Face>, IDisposable
    {
        protected Font(int size, Face face)
        {
            Size = size;
            Face = face;
        }

        public int Size { get; }

        public Face Face { get; }

        public void PreGenerateCharacterGlyphs(char[] characters)
        {

        }
    }
}
