using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RockPaperScissors
{
    public class InputHandler
    {
        private GameStateManager _gameStateManager;
        private MouseState _previousMouseState;
        private GameLogic _gameLogic;

        public InputHandler(GameStateManager gameStateManager, GameLogic gameLogic)
        {
            _gameStateManager = gameStateManager;
            _gameLogic = gameLogic;
        }

        public void Update(MouseState currentMouseState, KeyboardState currentKeyboardState)
        {
            if (_gameStateManager.CurrentState == GameState.Title)
            {
                if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.L))
                {
                    _gameStateManager.CurrentState = _gameStateManager.CurrentState == GameState.Title ? GameState.LS_Title : GameState.Title;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Right) && _gameStateManager.Level >= 2)
                {
                    _gameStateManager.CurrentState = _gameStateManager.CurrentState == GameState.Title ? GameState.LS_Title : GameState.Title;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    _gameStateManager.CurrentState = GameState.Playing;
                }
                UpdateTitleScreen(currentMouseState);
            }
            else if (_gameStateManager.CurrentState == GameState.Playing)
            {
                UpdatePlayingScreen(currentMouseState);
                CheckForExitButtonClick(currentMouseState, GameState.Title);
            }
            else if (_gameStateManager.CurrentState == GameState.Result)
            {
                UpdateResultScreen(currentMouseState);
                CheckForExitButtonClick(currentMouseState, GameState.Title);

            }
            else if (_gameStateManager.CurrentState == GameState.LS_Title)
            {
                if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.S))
                {
                    _gameStateManager.CurrentState = _gameStateManager.CurrentState == GameState.Title ? GameState.LS_Title : GameState.Title;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Left))
                {
                    _gameStateManager.CurrentState = _gameStateManager.CurrentState == GameState.Title ? GameState.LS_Title : GameState.Title;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    _gameStateManager.CurrentState = GameState.LS_Playing;
                }
                UpdateLSTitleScreen(currentMouseState);
            }
            else if (_gameStateManager.CurrentState == GameState.LS_Playing)
            {
                UpdateLSPlayingScreen(currentMouseState);
                CheckForExitButtonClick(currentMouseState, GameState.LS_Title);

            }
            else if (_gameStateManager.CurrentState == GameState.LS_Result)
            {
                UpdateLSResultScreen(currentMouseState);
                CheckForExitButtonClick(currentMouseState, GameState.LS_Title);

            }

            _previousMouseState = currentMouseState;
        }

        private void CheckForExitButtonClick(MouseState currentMouseState, GameState state)
        {
            if (_gameStateManager.ExitButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.CurrentState = state;
            }
        }

        private void UpdateTitleScreen(MouseState currentMouseState)
        {
            var positions = _gameStateManager.Positions[GameState.Title];

            // Update hover state for rock
            _gameStateManager.IsHoveringRock = IsHovering(positions["Rock"], _gameStateManager.RockTexture, _gameStateManager.HoverScale);

            // Update hover state for paper
            _gameStateManager.IsHoveringPaper = IsHovering(positions["Paper"], _gameStateManager.PaperTexture, _gameStateManager.HoverScale);

            // Update hover state for scissors
            _gameStateManager.IsHoveringScissors = IsHovering(positions["Scissors"], _gameStateManager.ScissorsTexture, _gameStateManager.HoverScale);

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
            }
            if (_gameStateManager.Level >= 2)
            {
                CheckForNextButtonClick(currentMouseState);
            }
        }
        private void UpdateLSTitleScreen(MouseState currentMouseState)
        {
            var positions = _gameStateManager.Positions[GameState.LS_Title];

            // Update hover state for rock
            _gameStateManager.IsHoveringRock = IsHovering(positions["Rock"], _gameStateManager.RockTexture, _gameStateManager.HoverScale);

            // Update hover state for paper
            _gameStateManager.IsHoveringPaper = IsHovering(positions["Paper"], _gameStateManager.PaperTexture, _gameStateManager.HoverScale);

            // Update hover state for scissors
            _gameStateManager.IsHoveringScissors = IsHovering(positions["Scissors"], _gameStateManager.ScissorsTexture, _gameStateManager.HoverScale);

            _gameStateManager.IsHoveringLizard = IsHovering(positions["Lizard"], _gameStateManager.LizardTexture, _gameStateManager.HoverScale);
            _gameStateManager.IsHoveringSpock = IsHovering(positions["Spock"], _gameStateManager.SpockTexture, _gameStateManager.HoverScale);


            // Check for click on start button
            _gameStateManager.StartButtonRectangle = new Rectangle(
                (int)(_gameStateManager.StartButtonPosition.X - _gameStateManager.StartButtonTexture.Width * _gameStateManager.ButtonScale / 2),
                (int)(_gameStateManager.StartButtonPosition.Y - _gameStateManager.StartButtonTexture.Height * _gameStateManager.ButtonScale / 2),
                (int)(_gameStateManager.StartButtonTexture.Width * _gameStateManager.ButtonScale),
                (int)(_gameStateManager.StartButtonTexture.Height * _gameStateManager.ButtonScale)
            );

            if (_gameStateManager.StartButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.CurrentState = GameState.LS_Playing;
            }
            if (_gameStateManager.Level >= 2)
            {
                CheckForPrevButtonClick(currentMouseState);
            }
        }

        private void UpdatePlayingScreen(MouseState currentMouseState)
        {
            var positions = _gameStateManager.Positions[GameState.Playing];

            // Update hover state for rock
            _gameStateManager.IsHoveringRock = IsHovering(positions["Rock"], _gameStateManager.RockTexture, _gameStateManager.HoverScale);

            // Update hover state for paper
            _gameStateManager.IsHoveringPaper = IsHovering(positions["Paper"], _gameStateManager.PaperTexture, _gameStateManager.HoverScale);

            // Update hover state for scissors
            _gameStateManager.IsHoveringScissors = IsHovering(positions["Scissors"], _gameStateManager.ScissorsTexture, _gameStateManager.HoverScale);

            // Check for player's choice
            if (_gameStateManager.IsHoveringRock && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.PlayerChoice = Choice.Rock;
                _gameStateManager.CurrentState = GameState.Result;
            }
            else if (_gameStateManager.IsHoveringPaper && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.PlayerChoice = Choice.Paper;
                _gameStateManager.CurrentState = GameState.Result;
            }
            else if (_gameStateManager.IsHoveringScissors && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.PlayerChoice = Choice.Scissors;
                _gameStateManager.CurrentState = GameState.Result;
            }
        }

        private void UpdateLSPlayingScreen(MouseState currentMouseState)
        {
            var positions = _gameStateManager.Positions[GameState.LS_Playing];

            // Update hover state for rock
            _gameStateManager.IsHoveringRock = IsHovering(positions["Rock"], _gameStateManager.RockTexture, _gameStateManager.HoverScale);

            // Update hover state for paper
            _gameStateManager.IsHoveringPaper = IsHovering(positions["Paper"], _gameStateManager.PaperTexture, _gameStateManager.HoverScale);

            // Update hover state for scissors
            _gameStateManager.IsHoveringScissors = IsHovering(positions["Scissors"], _gameStateManager.ScissorsTexture, _gameStateManager.HoverScale);

            _gameStateManager.IsHoveringLizard = IsHovering(positions["Lizard"], _gameStateManager.LizardTexture, _gameStateManager.HoverScale);
            _gameStateManager.IsHoveringSpock = IsHovering(positions["Spock"], _gameStateManager.SpockTexture, _gameStateManager.HoverScale);



            // Check for player's choice
            if (_gameStateManager.IsHoveringRock && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Rock;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
            else if (_gameStateManager.IsHoveringPaper && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Paper;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
            else if (_gameStateManager.IsHoveringScissors && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Scissors;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
            else if (_gameStateManager.IsHoveringLizard && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Lizard;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
            else if (_gameStateManager.IsHoveringSpock && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.PlayerLSChoice = LS_Choice.Spock;
                _gameStateManager.CurrentState = GameState.LS_Result;
            }
        }

        private void UpdateResultScreen(MouseState currentMouseState)
        {
            // Check for click on start button to restart the game
            if (_gameStateManager.StartButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameLogic.ResetRound();
                _gameStateManager.CurrentState = GameState.Playing;
            }
        }
        private void UpdateLSResultScreen(MouseState currentMouseState)
        {
            // Check for click on start button to restart the game
            if (_gameStateManager.StartButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameLogic.ResetRound();
                _gameStateManager.CurrentState = GameState.LS_Playing;
            }
        }

        private void CheckForNextButtonClick(MouseState currentMouseState)
        {
            if (_gameStateManager.NextButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.CurrentState = GameState.LS_Title;
            }
        }

        private void CheckForPrevButtonClick(MouseState currentMouseState)
        {
            if (_gameStateManager.PrevButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.CurrentState = GameState.Title;
            }
        }

        private bool IsHovering(Vector2 position, Texture2D texture, float scale)
        {
            Rectangle rectangle = new Rectangle(
                (int)(position.X - texture.Width * scale / 2),
                (int)(position.Y - texture.Height * scale / 2),
                (int)(texture.Width * scale),
                (int)(texture.Height * scale)
            );

            return rectangle.Contains(Mouse.GetState().Position);
        }
    }
}
