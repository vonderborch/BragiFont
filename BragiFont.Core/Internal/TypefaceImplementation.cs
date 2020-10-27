using System;
using System.Collections.Generic;
using System.Text;

namespace BragiFont.Internal
{
    internal class TypefaceImplementation : Typeface
    {
        public TypefaceImplementation(string name, byte[] typefaceData, bool preGenerateCharacters, char[] charactersToPreGenerate) : base(name, typefaceData, preGenerateCharacters, charactersToPreGenerate)
        {

        }
    }
}
