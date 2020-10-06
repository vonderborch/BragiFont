using Microsoft.Xna.Framework;

namespace BragiFont.Internal
{
    internal class TextCharacter
    {
        public Character Character;
        public Vector2 Position;

        public TextCharacter(Character character, Vector2 position)
        {
            Character = character;
            Position = position;
        }
    }
}
