using BragiFont.Internal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BragiFont
{
    public abstract class Text
    {
        public float Width { get; internal set; }

        public float Height { get; internal set; }

        public string String { get; internal set; }

        internal List<TextCharacter> _characters;

        internal void AddCharacter(TextCharacter character)
        {
            _characters.Add(character);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            for (var i = 0; i < _characters.Count; i++)
            {
                spriteBatch.Draw(_characters[i].Character.Texture, _characters[i].Position + position, _characters[i].Character.Boundary, color);
            }
        }
    }
}
