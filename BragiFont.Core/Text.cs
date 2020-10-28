using System.Collections.Generic;
using BragiFont.Internal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BragiFont
{
    /// <summary>
    /// A pre-generated list of characters that can be rendered
    /// </summary>
    public abstract class Text
    {
        /// <summary>
        /// The characters that are used by this Text object
        /// </summary>
        internal List<TextCharacter> Characters;

        /// <summary>
        /// The font that is used by this Text object
        /// </summary>
        protected Font Font;

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width { get; internal set; }

        /// <summary>
        /// Gets the width as an int.
        /// </summary>
        /// <value>
        /// The width int.
        /// </value>
        public int WidthInt => Helpers.ConvertFloatToInt(Width);

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height { get; internal set; }

        /// <summary>
        /// Gets the height as an int.
        /// </summary>
        /// <value>
        /// The height int.
        /// </value>
        public int HeightInt => Helpers.ConvertFloatToInt(Height);

        /// <summary>
        /// Gets the string that will be printed by this Text object.
        /// </summary>
        /// <value>
        /// The string.
        /// </value>
        public string String { get; internal set; }

        /// <summary>
        /// Gets the size of the object.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Vector2 Size { get; internal set; }

        /// <summary>
        /// Adds a character to the Text object.
        /// </summary>
        /// <param name="character">The character to add.</param>
        internal void AddCharacter(TextCharacter character)
        {
            Characters.Add(character);
        }

        /// <summary>
        /// Draws the text to the string.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="position">The position to draw the text at.</param>
        /// <param name="color">The color to draw the text with.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < Characters.Count; i++)
            {
                spriteBatch.Draw(Characters[i].Character.GlyphCache.Texture, Characters[i].Position + position, Characters[i].Character.Boundary, color);
            }
        }

    }
}
