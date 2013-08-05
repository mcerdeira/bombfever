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
        MainMenu menu;
        OptionMenu options;
        BombManager bombmanager;
        int ScreenWidth = 800;
        int ScreenHeight = 600;
        private Texture2D background;
        private bool paused = false;
        private bool pauseKeyDown = false;

        GameState CurrentGameState = GameState.MainMenu;

        public LoloGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            #warning Put fullscreen back
            //ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
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
            menu = new MainMenu(Content.Load<Texture2D>("MainMenu"));
            options = new OptionMenu();
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
            #warning remove this
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            checkPauseKey(Keyboard.GetState());

            if (!paused)
            {
                switch (CurrentGameState)
                {
                    case GameState.MainMenu:
                        menu.Update(gameTime);
                        break;
                    case GameState.Options:
                        options.Update(gameTime);
                        break;
                    case GameState.Playing:
                        p1.Update(gameTime);
                        //en.Update();
                        map.Update();
                        bombmanager.Update();
                        break;
                    case GameState.Quit:
                        Exit();
                        break;
                }
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
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    menu.Draw(spriteBatch);
                    break;
                case GameState.Options:
                    options.Draw(spriteBatch);
                    break;
                case GameState.Playing:
                    spriteBatch.Draw(background, new Rectangle(0, 0,ScreenWidth, ScreenHeight), Color.White); 
                    map.Draw(spriteBatch);
                    p1.Draw(spriteBatch);
                    //en.Draw(spriteBatch);
                    bombmanager.Draw(spriteBatch);
                    break;
                case GameState.Quit:
                    // Don't draw anything
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
