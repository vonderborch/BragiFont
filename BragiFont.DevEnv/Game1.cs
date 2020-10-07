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
