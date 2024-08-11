using Microsoft.Xna.Framework.Input;
using RockPaperScissors.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors.Screens
{
    public class LS_PlayingScreen
    {
        private GameStateManager _gameStateManager;
        private Renderer _renderer;

        private InputHandler _inputHandler;

        public LS_PlayingScreen(Renderer renderer, GameStateManager gameStateManager) 
        { 
            _renderer = renderer;
            _gameStateManager = gameStateManager;
        }

        public LS_PlayingScreen(InputHandler inputHandler, GameStateManager gameStateManager)
        {
            _inputHandler = inputHandler;
            _gameStateManager = gameStateManager;
        }

        public void HandleInput(KeyboardState currentKeyboardState, MouseState currentMouseState, MouseState _previousMouseState)
        {
            var positions = _gameStateManager.Positions[GameState.LS_Playing];

            // Update hover state for rock
            _gameStateManager.IsHoveringRock = _inputHandler.IsHovering(positions["Rock"], _gameStateManager.RockTexture, _gameStateManager.HoverScale);

            // Update hover state for paper
            _gameStateManager.IsHoveringPaper = _inputHandler.IsHovering(positions["Paper"], _gameStateManager.PaperTexture, _gameStateManager.HoverScale);

            // Update hover state for scissors
            _gameStateManager.IsHoveringScissors = _inputHandler.IsHovering(positions["Scissors"], _gameStateManager.ScissorsTexture, _gameStateManager.HoverScale);

            _gameStateManager.IsHoveringLizard = _inputHandler.IsHovering(positions["Lizard"], _gameStateManager.LizardTexture, _gameStateManager.HoverScale);
            _gameStateManager.IsHoveringSpock = _inputHandler.IsHovering(positions["Spock"], _gameStateManager.SpockTexture, _gameStateManager.HoverScale);



            // Check for player's choice
            if ((_gameStateManager.IsHoveringRock && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.D1))
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Rock;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
            else if ((_gameStateManager.IsHoveringPaper && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.D2))
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Paper;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
            else if ((_gameStateManager.IsHoveringScissors && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.D3))
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Scissors;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
            else if ((_gameStateManager.IsHoveringLizard && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.D4))
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Lizard;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
            else if ((_gameStateManager.IsHoveringSpock && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.D5))
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Spock;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
            _inputHandler.CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);
        }

        public void Render()
        {
            var positions = _gameStateManager.Positions[GameState.LS_Playing];
            _renderer.DrawTextures(_gameStateManager.LSTextures, positions);
            _renderer.DrawPlayerInfo();
            _renderer.DrawExitButton();
        }
    }
}
