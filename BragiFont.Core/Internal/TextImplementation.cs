using System.Collections.Generic;

namespace BragiFont.Internal
{
    public class TextImplementation : Text
    {
        public TextImplementation(string text)
        {
            String = text;
            _characters = new List<TextCharacter>();
        }
    }
}
