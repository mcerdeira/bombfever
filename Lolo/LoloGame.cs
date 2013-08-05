#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Lolo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LoloGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player p1;
        Map map;
        Enemy en;        
        BombManager bombmanager;
        private Texture2D background;
        private bool paused = false;
        private bool pauseKeyDown = false;

        public LoloGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            #warning Put fullscreen back
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        private void BeginPause(bool UserInitiated)
        {
            paused = true;
            //TODO: Volume down
        }

        private void EndPause()
        {
            //TODO: Resume audio            
            paused = false;
        }

        private void checkPauseKey(KeyboardState keyboardState)
        {
            bool pauseKeyDownThisFrame = keyboardState.IsKeyDown(Keys.P);
            // If key was not down before, but is down now, we toggle the
            // pause setting
            if (!pauseKeyDown && pauseKeyDownThisFrame)
            {
                if (!paused)
                    BeginPause(true);
                else
                    EndPause();
            }
            pauseKeyDown = pauseKeyDownThisFrame;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {            
            paused = false;            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);           
            map = new Map();
            bombmanager = new BombManager(Content);
            p1 = new Player(Content.Load<Texture2D>("Player"), new Vector2(0, 0), bombmanager);           
            map.GenerateLevel(Content, p1);
            bombmanager.UpdateMap(map);
            //en = new Enemy(Content.Load<Texture2D>("Enemy"), new Vector2(19, 19));
            background = Content.Load<Texture2D>("Background");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            checkPauseKey(Keyboard.GetState());

            if (!paused)
            {
                p1.Update(gameTime);
                //en.Update();
                map.Update();
                bombmanager.Update();
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White); 
            map.Draw(spriteBatch);
            p1.Draw(spriteBatch);
            //en.Draw(spriteBatch);
            bombmanager.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
