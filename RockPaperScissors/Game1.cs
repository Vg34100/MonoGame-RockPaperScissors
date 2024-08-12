using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockPaperScissors.Managers;

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
        private SettingsScreen _settingsScreen;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Globals.WindowSize = new(1024, 768);
            _graphics.PreferredBackBufferWidth = Globals.WindowSize.X;
            _graphics.PreferredBackBufferHeight = Globals.WindowSize.Y;
            _graphics.ApplyChanges();

            Globals.Content = Content;

            _gameStateManager = new GameStateManager();
            _gameLogic = new GameLogic(_gameStateManager);
            _settingsScreen = new SettingsScreen(_gameStateManager);
            _inputHandler = new InputHandler(_gameStateManager, _gameLogic, _settingsScreen, this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = _spriteBatch;
            Globals.GameStateManager = _gameStateManager;

            // Load resources into the GameStateManager
            _gameStateManager.LoadContent(Content, GraphicsDevice);
            _settingsScreen.LoadContent();

            // Initialize other components
            _renderer = new Renderer(_spriteBatch, _gameStateManager, _settingsScreen);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_gameStateManager.IsTransitioning)
            {
                _gameStateManager.TransitionProgress += (float)gameTime.ElapsedGameTime.TotalSeconds * GameStateManager.TransitionSpeed;
                if (_gameStateManager.TransitionProgress >= 1f)
                {
                    _gameStateManager.TransitionProgress = 0f;
                    _gameStateManager.IsTransitioning = false;
                    // Switch to the next state
                    _gameStateManager.CurrentState = _gameStateManager.CurrentState == GameState.Title ? GameState.LS_Title : GameState.Title;
                }
            }
            else
            {
                _inputHandler.Update(Mouse.GetState(), Keyboard.GetState(), gameTime);
                _gameLogic.Update(gameTime);
                _gameStateManager.AchievementManager.Update(gameTime);
            }
            Globals.Update(gameTime);
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            Color myCustomColor = new Color(133, 104, 94); // Example: purple color

            GraphicsDevice.Clear(myCustomColor);

            if (_gameStateManager.CurrentState == GameState.World)
            {
                _spriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: _gameStateManager._translation);

            }
            else
            {
                _spriteBatch.Begin();

            }
            _renderer.Draw(gameTime);
            _gameStateManager.AchievementManager.Draw(_spriteBatch); // Draw the achievement notifications
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
