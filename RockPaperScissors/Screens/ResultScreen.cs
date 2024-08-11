using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockPaperScissors.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors.Screens
{
    public class ResultScreen
    {
        private SpriteBatch _spriteBatch;
        private GameStateManager _gameStateManager;
        private Renderer _renderer;
        private InputHandler _inputHandler;

        private GameLogic _gameLogic;

        public ResultScreen(Renderer renderer, GameStateManager gameStateManager, SpriteBatch spriteBatch)
        {
            _renderer = renderer;
            _gameStateManager = gameStateManager;
            _spriteBatch = spriteBatch;
        }

        public ResultScreen(InputHandler inputHandler, GameStateManager gameStateManager, GameLogic gameLogic) 
        { 
            _inputHandler = inputHandler;
            _gameStateManager = gameStateManager;
            _gameLogic = gameLogic;


        }

        public void HandleInput(KeyboardState currentKeyboardState, MouseState currentMouseState, MouseState _previousMouseState)
        {
            // Check for click on start button to restart the game
            if ((_gameStateManager.StartButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.Enter))
            {
                _gameLogic.ResetRound();
                _gameStateManager.CurrentState = GameState.Playing;
            }
            _inputHandler.CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);
        }

        public void Render()
        {
            var positions = _gameStateManager.Positions[GameState.Playing];
            Vector2 computerPosition = new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width / 2, _spriteBatch.GraphicsDevice.Viewport.Height / 2 + 100);

            if (_gameStateManager.Textures.TryGetValue(_gameStateManager.PlayerChoice, out Texture2D playerTexture))
            {
                _spriteBatch.Draw(playerTexture, positions[_gameStateManager.PlayerChoice.ToString()], null, Color.White, 0f, new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
            }
            if (_gameStateManager.Textures.TryGetValue(_gameStateManager.ComputerChoice, out Texture2D computerTexture))
            {
                _spriteBatch.Draw(computerTexture, computerPosition, null, Color.Gray, 0f, new Vector2(computerTexture.Width / 2, computerTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
            }
            _renderer.DrawResultScreenCommon();
        }

    }
}
