using Microsoft.Xna.Framework.Input;
using RockPaperScissors.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors.Screens
{
    public class PlayingScreen
    {
        private GameStateManager _gameStateManager;
        private Renderer _renderer;

        private InputHandler _inputHandler;

        public PlayingScreen(Renderer renderer, GameStateManager gameStateManager) 
        { 
            _renderer = renderer;
            _gameStateManager = gameStateManager;
        }

        public PlayingScreen(InputHandler inputHandler, GameStateManager gameStateManager)
        {
            _inputHandler = inputHandler;
            _gameStateManager = gameStateManager;
        }

        public void HandleInput(KeyboardState currentKeyboardState, MouseState currentMouseState, MouseState _previousMouseState)
        {
            var positions = _gameStateManager.Positions[GameState.Playing];

            // Update hover state for rock
            _gameStateManager.IsHoveringRock = _inputHandler.IsHovering(positions["Rock"], _gameStateManager.RockTexture, _gameStateManager.HoverScale);

            // Update hover state for paper
            _gameStateManager.IsHoveringPaper = _inputHandler.IsHovering(positions["Paper"], _gameStateManager.PaperTexture, _gameStateManager.HoverScale);

            // Update hover state for scissors
            _gameStateManager.IsHoveringScissors = _inputHandler.IsHovering(positions["Scissors"], _gameStateManager.ScissorsTexture, _gameStateManager.HoverScale);

            // Check for player's choice
            if ((_gameStateManager.IsHoveringRock && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.D1))
            {
                _gameStateManager.PlayerChoice = Choice.Rock;
                _gameStateManager.CurrentState = GameState.Result;
            }
            else if ((_gameStateManager.IsHoveringPaper && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.D2))
            {
                _gameStateManager.PlayerChoice = Choice.Paper;
                _gameStateManager.CurrentState = GameState.Result;
            }
            else if ((_gameStateManager.IsHoveringScissors && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.D3))
            {
                _gameStateManager.PlayerChoice = Choice.Scissors;
                _gameStateManager.CurrentState = GameState.Result;
            }
           _inputHandler.CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);
        }

        public void Render()
        {
            var positions = _gameStateManager.Positions[GameState.Playing];
            _renderer.DrawTextures(_gameStateManager.Textures, positions);
            _renderer.DrawPlayerInfo();
            _renderer.DrawExitButton();
        }
    }
}
