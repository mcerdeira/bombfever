﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using System.Threading;
#endregion

namespace Lolo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LoloGame : Game
    {
        #warning comment this
        // <Fps stuff>
        int _total_frames = 0;
        float _elapsed_time = 0.0f;
        int _fps = 0;
        // </Fps stuff>
        ControlWrapper cwrap1;
        ControlWrapper cwrap2;
        ControlWrapper cwrapReal2;
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
        Credits credits;
        BombManager bombmanager;
        EnemyManager enemymanager;
        SpriteFont mainFont;
        SpriteFont chartFont;
        RoundResults roundR;
        Match cMatch;
        GameOptions gameOPT;
        Effect PauseFX;
        int ScreenWidth = 800;
        int ScreenHeight = 600;
        private Texture2D background;
        private Texture2D menues;
        private Texture2D roundres;
        private Texture2D charselection;
        private Texture2D bombTex;
        private Texture2D bombTex2;
        private Texture2D particleTex;
        private Texture2D pbarTex;
        private Texture2D bubble;
        private SoundEffect sfxExplosion;
        private SoundEffect sfxMiniExplosion;
        private SoundEffect sndfxBigExplode;
        private SoundEffect sndfxItemPick;
        private List<SoundEffect> sndfxBouncingBomb = new List<SoundEffect>();
        private List<Texture2D> PlayerTextures = new List<Texture2D>();
        private List<Texture2D> PlayerSelectionTextures = new List<Texture2D>();
        private bool paused = false;
        private bool pauseKeyDown = false;
        private PlayerActions previousMenuKey = PlayerActions.None;
        private PlayerActions prevKey1 = PlayerActions.None;
        private PlayerActions prevKey2 = PlayerActions.None;
        private bool EnterKeyDown = false;
        private List<SoundEffectInstance> bkMusicList = new List<SoundEffectInstance>();
        private SoundEffectInstance bkMusicInstance;
        private SoundEffectInstance menuMusicInstance;
        private List<SoundEffect> PlayersndFXList = new List<SoundEffect>();
        private SpriteFont titleFont;
        private CharacterSelection charselect;
        private bool PlayerSelected = false;
        private int PlayerSelectedDelay = 0;
        private PlayerTex p1Sel;
        private PlayerTex p2Sel;
        private List<Texture2D> ItemsTx = new List<Texture2D>();
        private List<Texture2D> TilesTx = new List<Texture2D>();
        private List<Texture2D> EnemyTx = new List<Texture2D>();
        private bool bkmusicPaused = false;
        private SoundEffect sfxFreeze;
        private SoundEffect sfxUnFreeze;
        private SoundEffect sfxPortal;
        private int unfreezeDelay = 0;
        private Texture2D GPortal;
        private Thread LoadThread;
        private Vector2 loadingPos;

        GameState CurrentGameState;

        public LoloGame(string[] args) : base()
        {
            graphics = new GraphicsDeviceManager(this);            
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            this.IsMouseVisible = false;
            graphics.IsFullScreen = true;

            if (System.Diagnostics.Debugger.IsAttached 
                || (args.Length > 0 && args[0].ToString().Trim() == "-devmode"))
            {
                this.IsMouseVisible = true;
                graphics.IsFullScreen = false;
            }
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        private void LoadControls()
        {
            string ctrl1 = General.getControlTypes()[gameOPT.p1control];
            string ctrl2 = General.getControlTypes()[gameOPT.p2control];

            if (ctrl1 == "Keyboard" && ctrl2 == "Keyboard")
            {
                ctype1 = ControlType.KeyBoard1;
                ctype2 = ControlType.KeyBoard2;
            }
            else if (ctrl1 == "Joystick" && ctrl2 == "Joystick")
            {
                ctype1 = ControlType.JoyStick1;
                ctype2 = ControlType.JoyStick2;
            }
            else if (ctrl1 == "Keyboard" && ctrl2 == "Joystick")
            {
                ctype1 = ControlType.KeyBoard1;
                ctype2 = ControlType.JoyStick1;
            }
            else if (ctrl1 == "Joystick" && ctrl2 == "Keyboard")
            {
                ctype1 = ControlType.JoyStick1;
                ctype2 = ControlType.KeyBoard1;
            }

            cwrap1 = new ControlWrapper(ctype1);
            cwrap2 = new ControlWrapper(ControlType.KeyBoard1); // Fixed keyboard p2 for menues
            cwrapReal2 = new ControlWrapper(ctype2); // Real p2 controls
        }

        private void BeginPause(bool UserInitiated)
        {
            paused = true;
            bkMusicInstance.Volume = 0.1f;
            menuMusicInstance.Volume = 0.1f;
        }

        private void EndPause()
        {
            bkMusicInstance.Volume = 0.5f;
            menuMusicInstance.Volume = 0.5f;
            paused = false;
        }

        private void checkPauseKey(KeyboardState keyboardState)
        {
            bool pauseKeyDownThisFrame = (keyboardState.IsKeyDown(Keys.P) || keyboardState.IsKeyDown(Keys.Escape));
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

        private PlayerActions charSelectKey(ControlWrapper cwrap, PlayerActions preKey, string player)
        {
            if (cwrap.IsKeyDown(PlayerActions.Up))
            {                
                if (preKey != PlayerActions.Up)
                {
                    charselect.ButtonFocus(-1, player);
                }
                return PlayerActions.Up;
            }
            else if (cwrap.IsKeyDown(PlayerActions.Down))
            {
                if (preKey != PlayerActions.Down)
                {
                    charselect.ButtonFocus(1, player);                    
                }
                return PlayerActions.Down;
            }
            else if (cwrap.IsKeyDown(PlayerActions.Left))
            {
                if (preKey != PlayerActions.Left)
                {
                    charselect.ButtonFocus(-2, player);
                    
                }
                return PlayerActions.Left;
            }
            else if (cwrap.IsKeyDown(PlayerActions.Right))
            {
                if (preKey != PlayerActions.Right)
                {
                    charselect.ButtonFocus(2, player);
                }
                return PlayerActions.Right;
            }
            else if (cwrap.IsKeyDown(PlayerActions.Bomb))
            {
                if (preKey != PlayerActions.Bomb)
                {
                    charselect.ButtonFocus(3, player);
                    if (player == "p1")
                    {
                        p1Sel = charselect.State1;
                    }
                    else
                    {
                        p2Sel = charselect.State2;
                    }
                }
                return PlayerActions.Bomb;
            }
            else
            {
                return PlayerActions.None;
            }
        }

        private void checkMenuKey()
        {
            bool EnterKeyDownThisFrame = (cwrap1.IsKeyDown(PlayerActions.Select) || cwrap2.IsKeyDown(PlayerActions.Select));
            if (cwrap1.IsKeyDown(PlayerActions.Up) || cwrap2.IsKeyDown(PlayerActions.Up))
            {
                if (previousMenuKey != PlayerActions.Up)
                {
                    previousMenuKey = PlayerActions.Up;
                    if (paused)
                    {
                        pauseSprite.ButtonFocus(-1);
                    }
                    else
                    {
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
            }
            else if (cwrap1.IsKeyDown(PlayerActions.Down) || cwrap2.IsKeyDown(PlayerActions.Down))
            {
                if (previousMenuKey != PlayerActions.Down)
                {
                    previousMenuKey = PlayerActions.Down;
                    if (paused)
                    {
                        pauseSprite.ButtonFocus(1);
                    }
                    else
                    {
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
            }
            else if (!EnterKeyDown && EnterKeyDownThisFrame)
            {
                if (paused)
                {
                    GameState tmp = pauseSprite.GetRetState();
                    if (tmp != GameState.None)
                    {
                        CurrentGameState = tmp;                    
                    }
                    EndPause();
                    pauseSprite.Reset();
                }
                else
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
                        options.Reset();
                    }
                    else if (CurrentGameState == GameState.Credits)
                    {
                        CurrentGameState = GameState.MainMenu;
                    }
                }
            }
            else
            {
                previousMenuKey = PlayerActions.None;
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
            CurrentGameState = GameState.Loading;
            mainFont = Content.Load<SpriteFont>("mainfont");             
            spriteBatch = new SpriteBatch(GraphicsDevice);

            loadingPos = new Vector2();
            loadingPos.X = General.getScreenCenterTextX("Loading...", ScreenWidth, mainFont);
            loadingPos.Y = General.getScreenCenterTextY("Loading...", ScreenHeight, mainFont);                
            LoadThread = new Thread(() => RealLoad());
            LoadThread.Start();
        }

        private void RealLoad()
        {            
            gameOPT = new GameOptions();            
            background = Content.Load<Texture2D>("Background");
            chartFont = Content.Load<SpriteFont>("chartsfont");
            pauseSprite = new Pause(ScreenHeight, ScreenWidth, mainFont, chartFont);
            menues = Content.Load<Texture2D>("MainMenu");
            roundres = Content.Load<Texture2D>("RoundResults");
            charselection = Content.Load<Texture2D>("CharSelection");
            titleFont = Content.Load<SpriteFont>("titlefont");
            bubble = Content.Load<Texture2D>("bubble");
            PauseFX = Content.Load<Effect>("Dark.mgfxo");
            PauseFX.Parameters["Percentage"].SetValue(0.30f);
            List<string> cr = new List<string>();
            cr.Add("+++ CONCEPT +++");
            cr.Add("Flor Gigy & Martin Cerdeira");
            //cr.Add(" ");
            cr.Add("+++ ARTWORK +++");
            cr.Add("Flor Gigy");
            cr.Add("http://opengameart.org/");
            //cr.Add(" ");
            cr.Add("+++ CODING +++");
            cr.Add("Martin Cerdeira");
            //cr.Add(" ");
            cr.Add("+++ MUSIC & SFX +++");
            cr.Add("http://www.bfxr.net/");
            cr.Add("http://opengameart.org/");
            //cr.Add(" ");
            credits = new Credits(charselection, mainFont, cr, ScreenHeight, ScreenWidth);
            lvlLoad = new LevelLoader(charselection, mainFont, ScreenHeight, ScreenWidth);
            options = new OptionMenu(charselection, mainFont, ScreenHeight, ScreenWidth);
            gameOPT = options.loadOptions();
            PlayerTextures.Add(Content.Load<Texture2D>("Knight"));
            PlayerTextures.Add(Content.Load<Texture2D>("Girl"));
            PlayerTextures.Add(Content.Load<Texture2D>("King"));
            PlayerTextures.Add(Content.Load<Texture2D>("Man"));
            PlayerTextures.Add(Content.Load<Texture2D>("Skelet"));
            PlayerTextures.Add(Content.Load<Texture2D>("Sorce"));

            PlayerSelectionTextures.Add(Content.Load<Texture2D>("Knight_S"));
            PlayerSelectionTextures.Add(Content.Load<Texture2D>("Girl_S"));
            PlayerSelectionTextures.Add(Content.Load<Texture2D>("King_S"));
            PlayerSelectionTextures.Add(Content.Load<Texture2D>("Man_S"));
            PlayerSelectionTextures.Add(Content.Load<Texture2D>("Skelet_S"));
            PlayerSelectionTextures.Add(Content.Load<Texture2D>("Sorce_S"));

            ItemsTx.Add(Content.Load<Texture2D>("shield"));
            ItemsTx.Add(Content.Load<Texture2D>("skull"));
            ItemsTx.Add(Content.Load<Texture2D>("ghost"));
            ItemsTx.Add(Content.Load<Texture2D>("freeze"));
            ItemsTx.Add(Content.Load<Texture2D>("plus1"));
            ItemsTx.Add(Content.Load<Texture2D>("switch"));
            ItemsTx.Add(Content.Load<Texture2D>("extratime"));
            ItemsTx.Add(Content.Load<Texture2D>("x2"));
            ItemsTx.Add(Content.Load<Texture2D>("dummy")); // BouncingBombs
            ItemsTx.Add(Content.Load<Texture2D>("efire"));
            ItemsTx.Add(Content.Load<Texture2D>("extrabomb"));

            TilesTx.Add(Content.Load<Texture2D>("1"));
            TilesTx.Add(Content.Load<Texture2D>("2"));
            TilesTx.Add(Content.Load<Texture2D>("3"));
            TilesTx.Add(Content.Load<Texture2D>("4"));
            TilesTx.Add(Content.Load<Texture2D>("5"));
            TilesTx.Add(Content.Load<Texture2D>("6"));
            TilesTx.Add(Content.Load<Texture2D>("1pflag"));
            TilesTx.Add(Content.Load<Texture2D>("2pflag"));

            EnemyTx.Add(Content.Load<Texture2D>("Devil1"));
            EnemyTx.Add(Content.Load<Texture2D>("Devil2"));
            EnemyTx.Add(Content.Load<Texture2D>("Spider"));

            LoadControls();
            LoadMusicFX();

            menu = new MainMenu(menues, mainFont, titleFont, ScreenHeight, ScreenWidth, "Boom Hunters", PlayersndFXList[(int)PlayerSndFXs.CharSelect], Content.Load<SoundEffect>("menuselect"));

            charselect = new CharacterSelection(charselection, mainFont, PlayerSelectionTextures, ScreenHeight, ScreenWidth, PlayersndFXList[(int)PlayerSndFXs.CharSelect], PlayersndFXList[(int)PlayerSndFXs.CharSelected], PlayersndFXList[(int)PlayerSndFXs.CharUnSelected]);

            GPortal = Content.Load<Texture2D>("gate");
            bombTex = Content.Load<Texture2D>("bomb");
            bombTex2 = Content.Load<Texture2D>("bomb_red");
            particleTex = Content.Load<Texture2D>("particle");
            pbarTex = Content.Load<Texture2D>("pbar");
            sfxExplosion = Content.Load<SoundEffect>("explosion");
            sfxMiniExplosion = Content.Load<SoundEffect>("miniexplosion");
            sndfxBigExplode = Content.Load<SoundEffect>("explosion_big");
            sndfxItemPick = Content.Load<SoundEffect>("itempick");
            sfxPortal = Content.Load<SoundEffect>("portal");
            cMatch = new Match();

            CurrentGameState = GameState.MainMenu;

            menuMusicInstance.Play();
        }

        private void LoadMusicFX()
        {
            menuMusicInstance = Content.Load<SoundEffect>("menumusic").CreateInstance();
            menuMusicInstance.IsLooped = true;
            menuMusicInstance.Volume = 0.5f;            
            // Load the 6 songs
            for (int i = 1; i < 7; i++)
            {
                bkMusicList.Add(Content.Load<SoundEffect>(String.Format("backmusic{0}", i)).CreateInstance());
            }
           
            // List with all the player soundFXs
            PlayersndFXList.Add(Content.Load<SoundEffect>("placebomb"));
            PlayersndFXList.Add(Content.Load<SoundEffect>("die"));
            PlayersndFXList.Add(Content.Load<SoundEffect>("kick"));
            PlayersndFXList.Add(Content.Load<SoundEffect>("char_select"));
            PlayersndFXList.Add(Content.Load<SoundEffect>("char_selected"));
            PlayersndFXList.Add(Content.Load<SoundEffect>("char_unselected"));            
            // Bouncing bomb sounds
            sndfxBouncingBomb.Add(Content.Load<SoundEffect>("bounce1"));
            sndfxBouncingBomb.Add(Content.Load<SoundEffect>("bounce2"));
            sndfxBouncingBomb.Add(Content.Load<SoundEffect>("bounce3"));

            sfxFreeze = Content.Load<SoundEffect>("freezefx");
            sfxUnFreeze = Content.Load<SoundEffect>("unfreezefx");
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
            #warning comment this
            //<FPS>
            _elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;            
            if (_elapsed_time >= 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }
            //</FPS>
            
            if (CurrentGameState == GameState.MainMenu || 
                CurrentGameState == GameState.LoadFromFile || 
                CurrentGameState == GameState.RoundResults ||
                CurrentGameState == GameState.Options      ||
                CurrentGameState == GameState.Credits)
            {
                checkMenuKey();
            }

            // This is the character selection screen
            if((CurrentGameState == GameState.Start1P || CurrentGameState == GameState.Start2P || CurrentGameState == GameState.StartCoOp) 
                && !PlayerSelected)
            {
                prevKey1 = charSelectKey(cwrap1, prevKey1, "p1");
                prevKey2 = charSelectKey(cwrapReal2, prevKey2, "p2");
            }

            if (CurrentGameState == GameState.Playing1P || CurrentGameState == GameState.Playing2P || CurrentGameState == GameState.PlayingCoOP)
            {
                checkPauseKey(Keyboard.GetState());
                checkMenuKey();
            }

            if (!paused)
            {
                switch (CurrentGameState)
                {
                    case GameState.Loading:
                        break;
                    case GameState.Start1P:
                    case GameState.Start2P:
                    case GameState.StartCoOp:
                        if (PlayerSelected)
                        {
                            if (PlayerSelectedDelay == 0)
                            {
                                unfreezeDelay = 0;
                                charselect.Reset();
                                menuMusicInstance.Stop();

                                Random rnd = new Random();
                                if (bkMusicInstance != null)
                                {
                                    bkMusicInstance.Stop();
                                }
                                int i = rnd.Next(0, 6);
                                bkMusicInstance = bkMusicList[i];
                                bkMusicInstance.IsLooped = true;
                                bkMusicInstance.Volume = 0.5f;
                                bkMusicInstance.Play();
                                // Load game options
                                gameOPT = options.loadOptions();
                                if (CurrentGameState == GameState.StartCoOp || CurrentGameState == GameState.Start1P)
                                {
                                    // Overwrite some game options for the survival modes
                                    gameOPT.gametype = 1; // First hit kills
                                    gameOPT.timelimit = 2; // 80 time limit
                                    enemymanager = new EnemyManager(EnemyTx);
                                    //TODO: Think a way of creating and position enemies
                                }
                                LoadControls();
                                roundTime = float.Parse(General.getRoundTimes()[gameOPT.timelimit]);

                                // In Game objects
                                score = new Score(ScreenHeight, ScreenWidth, mainFont, roundTime, General.getGameTypes()[gameOPT.gametype]);
                                bombmanager = new BombManager(sfxExplosion, sfxMiniExplosion, sndfxBigExplode, sndfxBouncingBomb, bombTex, bombTex2, particleTex, sfxPortal);
                                p1 = new Player(PlayerTextures[(int)p1Sel], new Vector2(50, 50), ctype1, bombmanager, score, "p1", PlayerStyle.Human, PlayersndFXList, mainFont, ScreenHeight, ScreenWidth, bubble, ItemsTx);
                                if (CurrentGameState == GameState.Start1P)
                                {
                                    p2 = new Player(PlayerTextures[(int)p2Sel], new Vector2(702, 500), ctype2, bombmanager, score, "p2", PlayerStyle.Machine, PlayersndFXList, mainFont, ScreenHeight, ScreenWidth, bubble, ItemsTx);
                                    CurrentGameState = GameState.Playing1P;
                                }
                                else
                                {
                                    p2 = new Player(PlayerTextures[(int)p2Sel], new Vector2(702, 500), ctype2, bombmanager, score, "p2", PlayerStyle.Human, PlayersndFXList, mainFont, ScreenHeight, ScreenWidth, bubble, ItemsTx);
                                    if (CurrentGameState == GameState.StartCoOp)
                                    {
                                        CurrentGameState = GameState.PlayingCoOP;
                                    }
                                    else
                                    {
                                        CurrentGameState = GameState.Playing2P;
                                    }
                                }
                                map = new Map(p1, p2, bombmanager, ItemsTx, TilesTx, score, sndfxItemPick, (CurrentGameState == GameState.PlayingCoOP), GPortal);
                                map.GenerateLevel(LevelName);
                                bombmanager.UpdateMap(map, p1, p2);
                                p1.setMap(map);
                                p2.setMap(map);
                            }
                            else
                            {
                                PlayerSelectedDelay--;
                            }
                        }
                        else
                        {
                            PlayerSelected = charselect.Update(gameTime);
                            if (PlayerSelected)
                            {                               
                                menuMusicInstance.Volume = 0.1f;
                                PlayerSelectedDelay = 50;
                            }
                        }
                        break;
                    case GameState.GotoMainMenu:
                        PlayerSelected = false;
                        cMatch = new Match();
                        CurrentGameState = GameState.MainMenu;
                        menuMusicInstance.Volume = 0.5f;
                        if (bkMusicInstance != null)
                        {
                            bkMusicInstance.Volume = 0.5f;
                            bkMusicInstance.Stop();
                            menuMusicInstance.Play();
                        }                       
                        break;
                    case GameState.MainMenu:
                        menu.Update(gameTime);
                        break;
                    case GameState.GotoOptions:
                        gameOPT = options.loadOptions();
                        CurrentGameState = GameState.Options;
                        break;
                    case GameState.Options:
                        options.Update(gameTime);
                        break;
                    case GameState.RoundResults:                        
                        roundR.Update(gameTime);
                        break;
                    case GameState.Playing1P:
                    case GameState.Playing2P:
                    case GameState.PlayingCoOP:
                        bool freezed = (p1.PausedLoop != 0 || p2.PausedLoop != 0);
                        if (freezed)
                        {
                            if (bkMusicInstance.Volume > 0f)
                            {                                
                                sfxFreeze.Play();
                                bkMusicInstance.Volume = 0f;
                                bkmusicPaused = true;
                                unfreezeDelay = 100;
                            }
                        }
                        else
                        {
                            if (bkmusicPaused)
                            {
                                if (unfreezeDelay == 100)
                                {
                                    sfxUnFreeze.Play();
                                }
                                if (unfreezeDelay == 0)
                                {
                                    bkmusicPaused = false;
                                    bkMusicInstance.Volume = 0.5f;
                                }
                                unfreezeDelay--;
                            }
                        }
                        float currTime = score.Update(gameTime, freezed);
                        if (currTime >= 0)
                        {
                            p1.Update(gameTime);
                            p2.Update(gameTime);
                            map.Update(gameTime);
                            bombmanager.Update();
                        }
                        else
                        {
                            // Time is up!
                            GameState st = new GameState();
                            switch (CurrentGameState)
                            {
                                case GameState.Playing1P:
                                    st = GameState.Start1P;
                                    break;
                                case GameState.Playing2P:
                                    st = GameState.Start2P;
                                    break;
                                case GameState.PlayingCoOP:
                                    st = GameState.StartCoOp;
                                    break;
                            }
                            roundR = new RoundResults(roundres, mainFont, chartFont, score, st, cMatch, ScreenHeight, ScreenWidth, pbarTex);
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
            else
            {                
                pauseSprite.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            #warning comment this
            //<FPS>
            _total_frames++;
            this.Window.Title = string.Format("FPS={0}", _fps);
            //</FPS>
            GraphicsDevice.Clear(Color.Transparent);
            if (paused)
            {
                spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, PauseFX);                                
            }
            else
            {
                spriteBatch.Begin();
            }
            switch (CurrentGameState)
            {
                case GameState.Loading:
                    spriteBatch.DrawString(mainFont, "Loading...", loadingPos, Color.Yellow);
                    break;
                case GameState.MainMenu:
                    menu.Draw(spriteBatch);
                    break;
                case GameState.Options:
                    options.Draw(spriteBatch);
                    break;
                case GameState.Playing1P:
                case GameState.Playing2P:
                case GameState.PlayingCoOP:
                    spriteBatch.Draw(background, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
                    map.Draw(spriteBatch);
                    p1.Draw(spriteBatch);
                    p2.Draw(spriteBatch);
                    bombmanager.Draw(spriteBatch);
                    score.Draw(spriteBatch);
                    break;
                case GameState.Start1P:
                case GameState.Start2P:
                case GameState.StartCoOp:
                    charselect.Draw(spriteBatch);                    
                    break;
                case GameState.RoundResults:
                    roundR.Draw(spriteBatch);
                    break;
                case GameState.Quit:
                    // Don't draw anything
                    break;
                case GameState.Credits:
                    credits.Draw(spriteBatch);
                    break;
                case GameState.LoadFromFile:
                    lvlLoad.Draw(spriteBatch);                    
                    break;
            }
            if (paused)
            {                
                spriteBatch.End();
                spriteBatch.Begin();
                pauseSprite.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }        
    }
}
