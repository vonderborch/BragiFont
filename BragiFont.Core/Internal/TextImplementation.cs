using System.Collections.Generic;

namespace BragiFont.Internal
{
    /// <summary>
    /// The implementation of the Text object.
    /// </summary>
    /// <seealso cref="BragiFont.Text" />
    public class TextImplementation : Text
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextImplementation"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        public TextImplementation(string text, Font font)
        {
            String = text;
            Characters = new List<TextCharacter>();
            Font = font;
        }

    }

}
