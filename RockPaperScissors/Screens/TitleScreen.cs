using Microsoft.Xna.Framework.Graphics;
using RockPaperScissors.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RockPaperScissors;

namespace RockPaperScissors.Screens
{
    public class TitleScreen
    {
        private GameStateManager _gameStateManager;
        private SpriteBatch _spriteBatch;
        private Renderer _renderer;
        private InputHandler _inputHandler;


        public TitleScreen(Renderer renderer, SpriteBatch spriteBatch, GameStateManager gameStateManager)
        {
            _spriteBatch = spriteBatch;
            _gameStateManager = gameStateManager;
            _renderer = renderer;
        }

        public TitleScreen(InputHandler inputHandler, GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
            _inputHandler = inputHandler;
        }


        public void HandleInput(KeyboardState currentKeyboardState, MouseState currentMouseState, MouseState _previousMouseState)
        {
            _inputHandler.ExitCondition();
            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.L))
            {
                _gameStateManager.NextModeSound.Play();
                _gameStateManager.IsTransitioning = true;
                // _gameStateManager.CurrentState = _gameStateManager.CurrentState == GameState.Title ? GameState.LS_Title : GameState.Title;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) && _gameStateManager.Level >= 2)
            {
                _gameStateManager.NextModeSound.Play();
                _gameStateManager.IsTransitioning = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Enter))
            {
                _gameStateManager.PlaySound.Play();
                _gameStateManager.CurrentState = GameState.Playing;
            }
            if (currentKeyboardState.IsKeyDown(Keys.T))
            {
                _gameStateManager.ChangeScreenSound.Play();
                _gameStateManager.CurrentState = GameState.Achievements;
            }
            UpdateTitleScreen(currentMouseState, _previousMouseState);
        }

        private void UpdateTitleScreen(MouseState currentMouseState, MouseState _previousMouseState)
        {
            var positions = _gameStateManager.Positions[GameState.Title];

            // Update hover state for rock
            _gameStateManager.IsHoveringRock = _inputHandler.IsHovering(positions["Rock"], _gameStateManager.RockTexture, _gameStateManager.HoverScale);

            // Update hover state for paper
            _gameStateManager.IsHoveringPaper = _inputHandler.IsHovering(positions["Paper"], _gameStateManager.PaperTexture, _gameStateManager.HoverScale);

            // Update hover state for scissors
            _gameStateManager.IsHoveringScissors = _inputHandler.IsHovering(positions["Scissors"], _gameStateManager.ScissorsTexture, _gameStateManager.HoverScale);

            // Check for click on start button
            _gameStateManager.StartButtonRectangle = new Rectangle(
                (int)(_gameStateManager.StartButtonPosition.X - _gameStateManager.StartButtonTexture.Width * _gameStateManager.ButtonScale / 2),
                (int)(_gameStateManager.StartButtonPosition.Y - _gameStateManager.StartButtonTexture.Height * _gameStateManager.ButtonScale / 2),
                (int)(_gameStateManager.StartButtonTexture.Width * _gameStateManager.ButtonScale),
                (int)(_gameStateManager.StartButtonTexture.Height * _gameStateManager.ButtonScale)
            );

            if (_gameStateManager.StartButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.CurrentState = GameState.Playing;
                _gameStateManager.PlaySound.Play();
            }
            if (_gameStateManager.Level >= 2)
            {
                _inputHandler.CheckForNextButtonClick(currentMouseState);
            }
        }

        public void Render()
        {
            var positions = _gameStateManager.Positions[GameState.Title];
            _spriteBatch.Draw(_gameStateManager.TitleTexture, positions["Title"], null, Color.White, 0f, new Vector2(_gameStateManager.TitleTexture.Width / 2, _gameStateManager.TitleTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
            _renderer.DrawTextures(_gameStateManager.Textures, positions);
            

            // _spriteBatch.Draw(_gameStateManager.StartButtonTexture, _gameStateManager.StartButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.StartButtonTexture.Width / 2, _gameStateManager.StartButtonTexture.Height / 2), _gameStateManager.ButtonScale, SpriteEffects.None, 0f);

            Vector2 textSize = _gameStateManager.Font.MeasureString("Press ENTER to Start...");
            _spriteBatch.DrawString(_gameStateManager.Font, "Press ENTER to Start...", _gameStateManager.StartButtonPosition - textSize / 2, Color.Black);
            if (_gameStateManager.Level >= 2)
            {
                _renderer.DrawNextButton();
            }
            _renderer.DrawPlayerInfo();
        }
        public void ProcessLogic()
        {

        }
    }
}
