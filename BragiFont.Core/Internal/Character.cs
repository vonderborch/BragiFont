using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BragiFont.Internal
{
    internal class Character
    {
        public Texture2D Texture;
        public Rectangle Boundary;
        public int Index;
        public char? PreCharacter;
        public char Char;
        public int AdvanceX;
        public int BearingX;

        public Dictionary<char, int> Kerning = new Dictionary<char, int>();

        public int BearingWithWidth
        {
            get { return BearingX + Boundary.Width; }
        }

        public override string ToString()
        {
            return $"Char: {Char}; X: {Boundary.X}; Y: {Boundary.Y}; Width: {Boundary.Width}, Height: {Boundary.Height}";
        }
    }
}
