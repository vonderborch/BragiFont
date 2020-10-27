using System.Text;
using BragiFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
// ReSharper disable once UnusedMember.Global
public static class DrawStringExtension
{
    /// <summary>
    ///     Draws text to the screen with the given font.
    ///     This method is SLOWER than the override that takes a Text object instead of a string, as it will create and discard
    ///     the glyphs every time.
    /// </summary>
    /// <param name="spriteBatch">The spritebatch</param>
    /// <param name="font">The Font to use when rendering the string</param>
    /// <param name="text">The string to render</param>
    /// <param name="position">Position at which to render the string</param>
    /// <param name="color">Color with which to render the string</param>
    public static void DrawString(this SpriteBatch spriteBatch, Font font, string text, Vector2 position, Color color)
    {
        font.Draw(spriteBatch, text, color, new Rectangle((int) position.X, (int) position.Y, 0, 0));
    }

    /// <summary>
    ///     Draws text to the screen with the given font.
    ///     This method is SLOWER than the override that takes a Text object instead of a string, as it will create and discard
    ///     the glyphs every time.
    /// </summary>
    /// <param name="spriteBatch">The spritebatch</param>
    /// <param name="font">The Font to use when rendering the string</param>
    /// <param name="text">The string to render.</param>
    /// <param name="position">Position at which to render the string</param>
    /// <param name="color">Color with which to render the string</param>
    public static void DrawString(this SpriteBatch spriteBatch, Font font, StringBuilder text, Vector2 position, Color color)
    {
        font.Draw(spriteBatch, text.ToString(), color, new Rectangle((int) position.X, (int) position.Y, 0, 0));
    }

    /// <summary>
    ///     Draw text to the screen with the given Font.
    /// </summary>
    /// <param name="spriteBatch">The spritebatch</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">Position at which to render the text</param>
    /// <param name="color">Color with which to render the text</param>
    public static void DrawString(this SpriteBatch spriteBatch, Text text, Vector2 position, Color color)
    {
        text.Draw(spriteBatch, position, color);
    }

}
