using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RockPaperScissors
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameStateManager _gameStateManager;
        private InputHandler _inputHandler;
        private Renderer _renderer;
        private GameLogic _gameLogic;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gameStateManager = new GameStateManager();
            _gameLogic = new GameLogic(_gameStateManager);
            _inputHandler = new InputHandler(_gameStateManager, _gameLogic);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load resources into the GameStateManager
            _gameStateManager.LoadContent(Content, GraphicsDevice);

            // Initialize other components
            _renderer = new Renderer(_spriteBatch, _gameStateManager);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _inputHandler.Update(Mouse.GetState(), Keyboard.GetState());
            _gameLogic.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _renderer.Draw(gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
