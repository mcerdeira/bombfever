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
        Player p2;
        string LevelName = "";
        Score score;
        ControlType ctype1;
        ControlType ctype2;
        LevelLoader lvlLoad;
        Pause pauseSprite;
        Map map;        
        MainMenu menu;
        OptionMenu options;
        BombManager bombmanager;
        SpriteFont mainFont;
        int ScreenWidth = 800;
        int ScreenHeight = 600;
        private Texture2D background;
        private bool paused = false;
        private bool pauseKeyDown = false;
        private Keys previousMenuKey = Keys.Zoom;
        private bool EnterKeyDown = false;        

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

        private void LoadControls()
        {
            #warning Here I must load the config file
            ctype1 = ControlType.KeyBoard1;
            ctype2 = ControlType.KeyBoard2;
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

        private void checkMenuKey(KeyboardState keyboardState)
        {
            bool EnterKeyDownThisFrame = keyboardState.IsKeyDown(Keys.Enter);

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (previousMenuKey != Keys.Up)
                {
                    previousMenuKey = Keys.Up;
                    if (CurrentGameState == GameState.LoadFromFile)
                    {
                        lvlLoad.ButtonFocus(-1);
                    }
                    else if (CurrentGameState == GameState.MainMenu)
                    {
                        menu.ButtonFocus(-1);
                    }
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (previousMenuKey != Keys.Down)
                {
                    previousMenuKey = Keys.Down;
                    if (CurrentGameState == GameState.LoadFromFile)
                    {
                        lvlLoad.ButtonFocus(1);
                    }
                    else if (CurrentGameState == GameState.MainMenu)
                    {
                        menu.ButtonFocus(1);
                    }
                }
            }
            else if (!EnterKeyDown && EnterKeyDownThisFrame)
            {
                if (CurrentGameState == GameState.LoadFromFile)
                {
                    LevelName = lvlLoad.getCaption();
                    if (LevelName == "Cancel")
                    {
                        LevelName = "";
                    }
                    CurrentGameState = lvlLoad.GetRetState();
                }
                else if (CurrentGameState == GameState.MainMenu)
                {
                    CurrentGameState = menu.GetRetState();
                }
            }
            else
            {
                previousMenuKey = Keys.Zoom;
            }

            EnterKeyDown = EnterKeyDownThisFrame;
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
            LoadControls();
            // Create a new SpriteBatch, which can be used to draw textures.
            score = new Score(ScreenHeight, ScreenWidth, Content.Load<SpriteFont>("mainfont"));
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pauseSprite = new Pause(ScreenHeight, ScreenWidth, Content.Load<SpriteFont>("mainfont")); 

            mainFont = Content.Load<SpriteFont>("mainfont");
            menu = new MainMenu(Content.Load<Texture2D>("MainMenu"), Content.Load<Texture2D>("btn1"), mainFont, ScreenHeight, ScreenWidth);
            lvlLoad = new LevelLoader(Content.Load<Texture2D>("MainMenu"), Content.Load<Texture2D>("btn1"), mainFont, ScreenHeight, ScreenWidth);
            options = new OptionMenu();
            background = Content.Load<Texture2D>("Background");          
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
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

            if (CurrentGameState == GameState.MainMenu || CurrentGameState == GameState.LoadFromFile)
            {
                checkMenuKey(Keyboard.GetState());
            }
            if (CurrentGameState == GameState.Playing1P || CurrentGameState == GameState.Playing2P)
            {
                checkPauseKey(Keyboard.GetState());
            }

            if (!paused)
            {
                switch (CurrentGameState)
                {
                    case GameState.Start1P:
                    case GameState.Start2P:                        
                        // In Game objects                        
                        map = new Map();
                        bombmanager = new BombManager(Content);
                        p1 = new Player(Content.Load<Texture2D>("Player"), new Vector2(0, 0), ctype1, bombmanager, score, "p1", PlayerStyle.Human);
                        if (CurrentGameState == GameState.Start1P)
                        {
                            p2 = new Player(Content.Load<Texture2D>("Player"), new Vector2(770, 566), ctype2, bombmanager, score, "p2", PlayerStyle.Machine);
                            CurrentGameState = GameState.Playing1P;
                        }
                        else
                        {
                            p2 = new Player(Content.Load<Texture2D>("Player"), new Vector2(770, 566), ctype2, bombmanager, score, "p2", PlayerStyle.Human);
                            CurrentGameState = GameState.Playing2P;
                        }
                        map.GenerateLevel(Content, p1, p2, LevelName);
                        bombmanager.UpdateMap(map, p1, p2);          
                        break;
                    case GameState.MainMenu:
                        menu.Update(gameTime);
                        break;
                    case GameState.Options:
                        options.Update(gameTime);
                        break;
                    case GameState.Playing1P:
                    case GameState.Playing2P:
                        score.Update(gameTime);
                        p1.Update(gameTime);
                        p2.Update(gameTime);                        
                        map.Update();
                        bombmanager.Update();
                        break;
                    case GameState.Credits:
                        break;
                    case GameState.LoadFromFile:
                        lvlLoad.Update(gameTime);
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
                case GameState.Playing1P:
                case GameState.Playing2P:
                    spriteBatch.Draw(background, new Rectangle(0, 0,ScreenWidth, ScreenHeight), Color.White); 
                    map.Draw(spriteBatch);
                    p1.Draw(spriteBatch);
                    p2.Draw(spriteBatch);                    
                    bombmanager.Draw(spriteBatch);
                    score.Draw(spriteBatch);
                    break;
                case GameState.Quit:
                    // Don't draw anything
                    break;
                case GameState.Credits:
                    break;
                case GameState.LoadFromFile:
                    lvlLoad.Draw(spriteBatch);
                    break;
            }            
            if (paused)
            {
                pauseSprite.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
