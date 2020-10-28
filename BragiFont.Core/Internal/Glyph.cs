using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BragiFont.Internal
{
    internal class Glyph
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
        public char Character;

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
        public GlyphCache GlyphCache;

        public Glyph(int advanceX, int advanceY, int bearingX, Rectangle boundary, char character, int index, GlyphCache glyphCache)
        {
            AdvanceX = advanceX;
            AdvanceY = advanceY;
            BearingX = bearingX;
            Boundary = boundary;
            Character = character;
            Index = index;
            GlyphCache = glyphCache;
        }

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
            return $"Char: {Character}; X: {Boundary.X}; Y: {Boundary.Y}; Width: {Boundary.Width}, Height: {Boundary.Height}";
        }
    }
}
