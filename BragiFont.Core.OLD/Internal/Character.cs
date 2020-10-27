using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BragiFont.Internal
{
    /// <summary>
    /// A character in the font system.
    /// </summary>
    internal class Character
    {
        /// <summary>
        /// The X Advance of the character.
        /// </summary>
        public int AdvanceX;

        /// <summary>
        /// The Y Advance of the character.
        /// </summary>
        public int AdvanceY;

        /// <summary>
        /// The X bearing of the character.
        /// </summary>
        public int BearingX;

        /// <summary>
        /// The position and boundaries of the Character on the GlyphCache's Texture.
        /// </summary>
        public Rectangle Boundary;

        /// <summary>
        /// The Character.
        /// </summary>
        public char Char;

        /// <summary>
        /// The index of the Character.
        /// </summary>
        public int Index;

        /// <summary>
        /// Kerning for the character.
        /// </summary>
        public Dictionary<char, int> Kerning = new Dictionary<char, int>();

        /// <summary>
        /// The GlyphCache that the character is associated with.
        /// </summary>
        public GlyphCache Texture;

        /// <summary>
        /// The Bearing of the Character with the Width added.
        /// </summary>
        public int BearingWithWidth => BearingX + Boundary.Width;

        /// <summary>
        /// Converts the character to a string for debug purposes.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Char: {Char}; X: {Boundary.X}; Y: {Boundary.Y}; Width: {Boundary.Width}, Height: {Boundary.Height}";
        }
    }
}
