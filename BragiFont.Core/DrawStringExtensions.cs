using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BragiFont;

//  This class is deliberately not in a namespace so you can use it without "using" anything

public static class DrawStringExtension
{
    /// <summary>
    /// Draw textto SpriteBatch with the given Font.
    /// This method is SLOWER than the override that takes a BragiText instead of a string, as it will create and discard the glyphs every time.
    /// </summary>
    /// <param name="spriteBatch">This</param>
    /// <param name="font">The Font to use when rendering the string</param>
    /// <param name="text">The string to render.</param>
    /// <param name="position">Position at which to render the string</param>
    /// <param name="color">Color with which to render the string</param>
    public static void DrawString(this SpriteBatch spriteBatch, Font font, string text, Vector2 position, Color color)
    {
        //spriteBatch.DrawString(font.CreateText(text), position, color);
        font.Draw(spriteBatch, text, color, new Rectangle((int)position.X, (int)position.Y, 0, 0));
    }

    /// <summary>
    /// Draw text to SpriteBatch with the given Font.
    /// </summary>
    /// <param name="spriteBatch">This</param>
    /// <param name="font">The Font to use when rendering the string</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">Position at which to render the text</param>
    /// <param name="color">Color with which to render the text</param>
    public static void DrawString(this SpriteBatch spriteBatch, Text text, Vector2 position, Color color)
    {
        text.Draw(spriteBatch, position, color);
    }
}
