using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace RockPaperScissors
{
    public class InputHandler
    {
        private GameStateManager _gameStateManager;
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;

        private GameLogic _gameLogic;
        private Game _game; // Add a field for the Game instance

        private double _cooldownTimer; // Timer to handle cooldown
        private const double CooldownDuration = 0.5; // Duration of the cooldown in second
        private Random _random; // Add a random generator

        private HoverItem _previousHoverItem = HoverItem.None;


        public InputHandler(GameStateManager gameStateManager, GameLogic gameLogic, Game game)
        {
            _gameStateManager = gameStateManager;
            _gameLogic = gameLogic;
            _game = game; // Initialize the Game instance
            _cooldownTimer = 0;
            _random = new Random(); // Initialize the random generator

        }

        public void Update(MouseState currentMouseState, KeyboardState currentKeyboardState, GameTime gameTime)
        {
            _cooldownTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.D))
            {
                _gameStateManager.ResetSaveData();
            }

            if (_gameStateManager.CurrentState == GameState.Title)
            {
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
                    // _gameStateManager.CurrentState = _gameStateManager.CurrentState == GameState.Title ? GameState.LS_Title : GameState.Title;
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
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) && _cooldownTimer <= 0)
                {
                    _game.Exit();
                }
                UpdateTitleScreen(currentMouseState);
            }
            else if (_gameStateManager.CurrentState == GameState.Playing)
            {
                UpdatePlayingScreen(currentMouseState, currentKeyboardState);
                CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);
            }
            else if (_gameStateManager.CurrentState == GameState.Result)
            {
                UpdateResultScreen(currentMouseState, currentKeyboardState);
                CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);

            }
            else if (_gameStateManager.CurrentState == GameState.LS_Title)
            {
                if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.S))
                {
                    _gameStateManager.NextModeSound.Play();
                    _gameStateManager.IsTransitioning = true;

                    // _gameStateManager.CurrentState = _gameStateManager.CurrentState == GameState.Title ? GameState.LS_Title : GameState.Title;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Left))
                {
                    _gameStateManager.NextModeSound.Play();
                    _gameStateManager.IsTransitioning = true;

                    // _gameStateManager.CurrentState = _gameStateManager.CurrentState == GameState.Title ? GameState.LS_Title : GameState.Title;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    _gameStateManager.PlaySound.Play();
                    _gameStateManager.CurrentState = GameState.LS_Playing;

                }
                UpdateLSTitleScreen(currentMouseState);
            }
            else if (_gameStateManager.CurrentState == GameState.LS_Playing)
            {
                UpdateLSPlayingScreen(currentMouseState, currentKeyboardState);
                CheckForExitButtonClick(currentMouseState, GameState.LS_Title, currentKeyboardState);

            }
            else if (_gameStateManager.CurrentState == GameState.LS_Result)
            {
                UpdateLSResultScreen(currentMouseState, currentKeyboardState);
                CheckForExitButtonClick(currentMouseState, GameState.LS_Title, currentKeyboardState);

            }
            else if (_gameStateManager.CurrentState == GameState.Achievements)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
                {
                    _gameStateManager.CurrentAchievementPage = (_gameStateManager.CurrentAchievementPage + 1) % _gameStateManager.TotalAchievementPages();
                }
                else if (currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
                {
                    _gameStateManager.CurrentAchievementPage = (_gameStateManager.CurrentAchievementPage - 1 + _gameStateManager.TotalAchievementPages()) % _gameStateManager.TotalAchievementPages();
                }
                UpdateAchievementsScreen(currentMouseState, currentKeyboardState);
                CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);
            }
            UpdateHoverSounds(currentMouseState);

            _previousMouseState = currentMouseState;
            _previousKeyboardState = currentKeyboardState;
        }

        private void UpdateHoverSounds(MouseState currentMouseState)
        {
            HoverItem currentHoverItem = HoverItem.None;

            if (_gameStateManager.IsHoveringRock)
            {
                currentHoverItem = HoverItem.Rock;
            }
            else if (_gameStateManager.IsHoveringPaper)
            {
                currentHoverItem = HoverItem.Paper;
            }
            else if (_gameStateManager.IsHoveringScissors)
            {
                currentHoverItem = HoverItem.Scissors;
            }
            else if (_gameStateManager.IsHoveringLizard)
            {
                currentHoverItem = HoverItem.Lizard;
            }
            else if (_gameStateManager.IsHoveringSpock)
            {
                currentHoverItem = HoverItem.Spock;
            }

            if (currentHoverItem != HoverItem.None && currentHoverItem != _previousHoverItem)
            {
                PlayRandomSelectSound();
            }

            _previousHoverItem = currentHoverItem;
        }



        private void PlayRandomSelectSound()
        {
            int index = _random.Next(_gameStateManager.SelectSounds.Length);
            _gameStateManager.SelectSounds[index].Play();
        }


        private void UpdateAchievementsScreen(MouseState currentMouseState, KeyboardState currentKeyboardState)
        {
            // Check for navigation through achievements
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                // Move to next set of achievements
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                // Move to previous set of achievements
            }

            // Check for exit to title screen
            if (currentKeyboardState.IsKeyDown(Keys.Back))
            {
                _gameStateManager.CurrentState = GameState.Title;
            }
        }

        private void CheckForExitButtonClick(MouseState currentMouseState, GameState state, KeyboardState currentKeyboardState)
        {
            if ((_gameStateManager.ExitButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                || (currentKeyboardState.IsKeyDown(Keys.Escape) && _cooldownTimer <= 0))
            {
                _cooldownTimer = CooldownDuration;
                _gameStateManager.ExitSound.Play();
                _gameStateManager.CurrentState = state;
                _gameLogic.ResetRound();
                
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
                _gameStateManager.PlaySound.Play();
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
                _gameStateManager.PlaySound.Play();
                _gameStateManager.CurrentState = GameState.LS_Playing;
            }
            if (_gameStateManager.Level >= 2)
            {
                CheckForPrevButtonClick(currentMouseState);
            }
        }

        private void UpdatePlayingScreen(MouseState currentMouseState, KeyboardState currentKeyboardState)
        {
            var positions = _gameStateManager.Positions[GameState.Playing];

            // Update hover state for rock
            _gameStateManager.IsHoveringRock = IsHovering(positions["Rock"], _gameStateManager.RockTexture, _gameStateManager.HoverScale);

            // Update hover state for paper
            _gameStateManager.IsHoveringPaper = IsHovering(positions["Paper"], _gameStateManager.PaperTexture, _gameStateManager.HoverScale);

            // Update hover state for scissors
            _gameStateManager.IsHoveringScissors = IsHovering(positions["Scissors"], _gameStateManager.ScissorsTexture, _gameStateManager.HoverScale);

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
        }

        private void UpdateLSPlayingScreen(MouseState currentMouseState, KeyboardState currentKeyboardState)
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
        }

        private void UpdateResultScreen(MouseState currentMouseState, KeyboardState currentKeyboardState)
        {
            // Check for click on start button to restart the game
            if ((_gameStateManager.StartButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.Enter))
            {
                _gameLogic.ResetRound();
                _gameStateManager.CurrentState = GameState.Playing;
            }
        }
        private void UpdateLSResultScreen(MouseState currentMouseState, KeyboardState currentKeyboardState)
        {
            // Check for click on start button to restart the game
            if ((_gameStateManager.StartButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) || currentKeyboardState.IsKeyDown(Keys.Enter))
            {
                _gameLogic.ResetRound();
                _gameStateManager.CurrentState = GameState.LS_Playing;
            }
        }

        private void CheckForNextButtonClick(MouseState currentMouseState)
        {
            if (_gameStateManager.NextButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.NextModeSound.Play();
                _gameStateManager.IsTransitioning = true;

                //_gameStateManager.CurrentState = GameState.LS_Title;
            }
        }

        private void CheckForPrevButtonClick(MouseState currentMouseState)
        {
            if (_gameStateManager.PrevButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.NextModeSound.Play();
                _gameStateManager.IsTransitioning = true;

                // _gameStateManager.CurrentState = GameState.Title;
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
