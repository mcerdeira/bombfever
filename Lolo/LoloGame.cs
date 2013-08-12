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
        float roundTime;
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
        RoundResults roundR;
        int ScreenWidth = 800;
        int ScreenHeight = 600;
        private Texture2D background;
        private Texture2D menues;
        private bool paused = false;
        private bool pauseKeyDown = false;
        private Keys previousMenuKey = Keys.None;
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
            roundTime = 120;
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
                    else if (CurrentGameState == GameState.RoundResults)
                    {
                        roundR.ButtonFocus(-1);
                    }
                    else if (CurrentGameState == GameState.Options)
                    {
                        options.ButtonFocus(-1);
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
                    else if (CurrentGameState == GameState.RoundResults)
                    {
                        roundR.ButtonFocus(1);
                    }
                    else if (CurrentGameState == GameState.Options)
                    {
                        options.ButtonFocus(1);
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
                else if (CurrentGameState == GameState.RoundResults)
                {
                    CurrentGameState = roundR.GetRetState();
                }
                else if (CurrentGameState == GameState.Options)
                {
                    GameState tmp = options.GetRetState();
                    if (tmp != GameState.None)
                    {
                        CurrentGameState = tmp;
                    }
                    else
                    {
                        options.CheckBoxClicked();
                    }
                }
            }
            else
            {
                previousMenuKey = Keys.None;
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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("Background");
            pauseSprite = new Pause(ScreenHeight, ScreenWidth, Content.Load<SpriteFont>("mainfont")); 
            menues = Content.Load<Texture2D>("MainMenu");
            mainFont = Content.Load<SpriteFont>("mainfont");
            menu = new MainMenu(menues, mainFont, ScreenHeight, ScreenWidth);
            lvlLoad = new LevelLoader(menues, mainFont, ScreenHeight, ScreenWidth);
            options = new OptionMenu(menues, mainFont, ScreenHeight, ScreenWidth);
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

            if (CurrentGameState == GameState.MainMenu || 
                CurrentGameState == GameState.LoadFromFile || 
                CurrentGameState == GameState.RoundResults ||
                CurrentGameState == GameState.Options)
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
                        score = new Score(ScreenHeight, ScreenWidth, Content.Load<SpriteFont>("mainfont"), roundTime);
                        bombmanager = new BombManager(Content);
                        p1 = new Player(Content.Load<Texture2D>("Player"), new Vector2(0, 0), ctype1, bombmanager, score, "p1", PlayerStyle.Human);
                        if (CurrentGameState == GameState.Start1P)
                        {
                            p2 = new Player(Content.Load<Texture2D>("Player"), new Vector2(770, 566), ctype2, bombmanager, score, "p2", PlayerStyle.Machine, p1);
                            CurrentGameState = GameState.Playing1P;
                        }
                        else
                        {
                            p2 = new Player(Content.Load<Texture2D>("Player"), new Vector2(770, 566), ctype2, bombmanager, score, "p2", PlayerStyle.Human);
                            CurrentGameState = GameState.Playing2P;
                        }
                        map = new Map(p1, p2);
                        map.GenerateLevel(Content, LevelName);
                        bombmanager.UpdateMap(map, p1, p2);                        
                        break;
                    case GameState.MainMenu:
                        menu.Update(gameTime);
                        break;
                    case GameState.Options:
                        options.Update(gameTime);
                        break;
                    case GameState.RoundResults:                        
                        roundR.Update(gameTime);
                        break;
                    case GameState.Playing1P:
                    case GameState.Playing2P:
                        if (score.Update(gameTime) >= 0)
                        {
                            p1.Update(gameTime);
                            p2.Update(gameTime);
                            map.Update();
                            bombmanager.Update();
                        }
                        else
                        {
                            // Time is up!
                            GameState st;
                            st = (CurrentGameState == GameState.Playing1P) ? GameState.Start1P : GameState.Start2P;                   
                            roundR = new RoundResults(Content.Load<Texture2D>("MainMenu"), mainFont, score, st, ScreenHeight, ScreenWidth);
                            CurrentGameState = GameState.RoundResults;
                        }
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
                    spriteBatch.Draw(background, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White); 
                    map.Draw(spriteBatch);
                    p1.Draw(spriteBatch);
                    p2.Draw(spriteBatch);                    
                    bombmanager.Draw(spriteBatch);
                    score.Draw(spriteBatch);
                    break;
                case GameState.RoundResults:
                    roundR.Draw(spriteBatch);
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
