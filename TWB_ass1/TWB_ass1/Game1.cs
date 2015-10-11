using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
namespace TWB_ass1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 

    public enum GameState
    {
        InGame,
        GameOver
    }
    public class Game1 : Game
    {

        float x = 0;
        float y = 0;
        public float time { get; protected set; }
        //public area
        public Camera camera { get; protected set; }
        //private area
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ModelManager modelManager;
        Texture2D crosshairTexture;
        BasicEffect effect;
        SpriteFont Arial;
        public SoundEffect backgroundSound;
        public SoundEffectInstance backgroundMusic;
        public SoundEffect movingSound;
        public SoundEffectInstance movingMusic;
        public SoundEffect hitBySound;
        public SoundEffectInstance hitByMusic;
        public SoundEffect dieSound;
        public SoundEffectInstance dieMusic;
        public SoundEffect hitSound;
        public SoundEffectInstance hitMusic;
        public SoundEffect shootSound;
        public SoundEffectInstance shootMusic;

        public GameState currentGameState = new GameState();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            // TODO: Add your initialization logic here
            currentGameState = GameState.InGame;

            camera = new Camera(this, new Vector3(300, 50, 50), new Vector3(0, 50, 0), Vector3.Zero, 10);
            Components.Add(camera);

            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            effect = new BasicEffect(GraphicsDevice);

            this.IsMouseVisible = false;
            base.Initialize();
        }



        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            crosshairTexture = Content.Load<Texture2D>(@"textures\crosshair");
            Arial = Content.Load<SpriteFont>(@"SpriteFonts\Courier New");
            LoadSound();
        }

        public void LoadSound()
        {
            //Royalty Free Music from www.Bensound.com
            backgroundSound = Content.Load<SoundEffect>(@"Audio/bensound-thelounge");
            backgroundMusic = backgroundSound.CreateInstance();
            backgroundMusic.IsLooped = false;

            dieSound = Content.Load<SoundEffect>(@"Audio/Die");
            dieMusic = dieSound.CreateInstance();
            dieMusic.IsLooped = false;

            hitBySound = Content.Load<SoundEffect>(@"Audio/HitByEnemy");
            hitByMusic = hitBySound.CreateInstance();
            hitByMusic.IsLooped = false;

            hitSound = Content.Load<SoundEffect>(@"Audio/HitEnemy");
            hitMusic = hitSound.CreateInstance();
            hitMusic.IsLooped = false;

            movingSound = Content.Load<SoundEffect>(@"Audio/move");
            movingMusic = movingSound.CreateInstance();
            movingMusic.IsLooped = false;

            shootSound = Content.Load<SoundEffect>(@"Audio/Shoot");
            shootMusic = shootSound.CreateInstance();
            shootMusic.IsLooped = false;
        }

        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            time = float.Parse(gameTime.ElapsedGameTime.TotalMilliseconds.ToString()) / 1000;
            backgroundMusic.Play();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(crosshairTexture,
                new Vector2((Window.ClientBounds.Width / 2)
                    - (crosshairTexture.Width / 2),
                    (Window.ClientBounds.Height / 2)
                    - (crosshairTexture.Height / 2)),
                    Color.White);
            spriteBatch.DrawString(Arial, "hi", new Vector2(0, 0), Color.Red);
            spriteBatch.End(); GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);
            if (currentGameState == GameState.GameOver)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Begin();
                spriteBatch.DrawString(Arial, "Score: " + GlobalVariables.score, new Vector2(0, 0), Color.Red);
                spriteBatch.DrawString(Arial, "Game Over!", new Vector2(-100 + GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), Color.Red);
                spriteBatch.End(); GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
        }


    }
}