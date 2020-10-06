using SharpFont;
using System;

namespace BragiFont.Internal
{
    internal class FontImplementation : Font
    {
        public FontImplementation(Tuple<string, int> key, Face face, int fontSize) : base(key, face, fontSize)
        {
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
