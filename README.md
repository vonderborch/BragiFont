# BragiFont
An alternative solution for Monogame/FNA/XNA-derived frameworks that utilizes SharpFont to draw text rather than the traditional SpriteFont approach.

# Installation
There are nuget packages available for Monogame and FNA.
- Monogame: [BragiFont.Monogame](https://www.nuget.org/packages/BragiFont.Monogame/)
- FNA: [BragiFont.FNA](https://www.nuget.org/packages/BragiFont.FNA/)

# Usage
Approach 1: Draw Text Directly
```
var fontSize = 48;
// Bragi.Core.Initialize(GraphicsDevice); // Optional, if not called you'll need to add the GraphicsDevice to the first Bragi.Core.GetFont call that is made to set the GraphicsDevice.
var font = Bragi.Core.GetFont("pathToFontFile", fontSize, GraphicsDevice);
_spriteBatch.DrawString(font, "Hello World!", new Vector2(50, 50), Color.White);
```

Approach 2: Cache Text (quicker since we don't need to rebuild the text glyph list on subsequent calls)
```
var fontSize = 48;
// Bragi.Core.Initialize(GraphicsDevice); // Optional, if not called you'll need to add the GraphicsDevice to the first Bragi.Core.GetFont call that is made to set the GraphicsDevice.
var font = Bragi.Core.GetFont("pathToFontFile", fontSize, GraphicsDevice);
var text = font.MakeText("Hello World!");
_spriteBatch.DrawString(text, new Vector2(50, 50), Color.White);

```

# Example
Code:
```
using BragiFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaldurUI
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private string testString = "Hello\nWorld!";
        private string file = "Content\\PlayfairDisplayRegular-ywLOY.ttf";
        private Text text;


        BragiFont.Font font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Bragi.Core.Initialize(GraphicsDevice);

            font = Bragi.Core.GetFont(file, 80);
            text = font.MakeText(testString);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, testString, new Vector2(50, 50), Color.Blue);
            _spriteBatch.DrawString(text, new Vector2(50, 250), Color.Red);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

```

Screenshot:
![Screenshot](https://github.com/vonderborch/BragiFont/blob/master/Example.PNG?raw=true)


# Future Plans
- Add support for embedding some HTML or Markdown style commands in text to control boldness, italic characters, underlines, and colors
- Add built-in support for bidirectional text handling (i.e. a mix of RTL and LTR text in the same string)
