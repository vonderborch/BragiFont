﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BragiFont.MonogameDevEnv
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private string testString = "Hello\nWorld! 123 () *&^$%#";
        private string fontFile1 = "Content\\PlayfairDisplayRegular-ywLOY.ttf";
        private string fontFile2 = "Content\\Trueno-wml2.otf";
        private Text text1;
        private Text text2;


        BragiFont.Font font1;
        BragiFont.Font font2;

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

            font1 = Bragi.Core.GetFont(fontFile1, 80);
            text1 = font1.MakeText(testString);

            font2 = Bragi.Core.GetFont(fontFile2, 80);
            text2 = font2.MakeText(testString);
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

            _spriteBatch.DrawString(font1, testString, new Vector2(0, -15), Color.Blue);
            _spriteBatch.DrawString(font1, testString, new Vector2(150, -15), Color.Pink, 0.1f, Vector2.Zero, new Vector2(.5f, .5f), SpriteEffects.None, 1f);
            _spriteBatch.DrawString(text2, new Vector2(50, 150), Color.Red);
            _spriteBatch.DrawString(text1, new Vector2(150, 300), Color.Black, 0.1f, Vector2.Zero, new Vector2(.5f, .5f), SpriteEffects.None, 1f);
            _spriteBatch.DrawString(text1, new Vector2(150, 300), Color.Black, 0.1f, new Vector2(50, 50), new Vector2(.5f, .5f), SpriteEffects.FlipVertically, 1f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
