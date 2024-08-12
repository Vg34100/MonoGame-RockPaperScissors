using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockPaperScissors.Managers;
using RockPaperScissors.Screens;
using System;
using System.Collections.Generic;


namespace RockPaperScissors
{
    public class InputHandler
    {
        private GameStateManager _gameStateManager;
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private SettingsScreen _settingsScreen;

        private GameLogic _gameLogic;
        private Game _game; // Add a field for the Game instance

        private double _cooldownTimer; // Timer to handle cooldown
        private const double CooldownDuration = 0.5; // Duration of the cooldown in second
        private Random _random; // Add a random generator

        private HoverItem _previousHoverItem = HoverItem.None;

        private TitleScreen _titleScreen;
        private PlayingScreen _playingScreen;
        private ResultScreen _resultScreen;
        // Lizard Spock GameMode
        private LS_TitleScreen _ls_titleScreen;
        private LS_PlayingScreen _ls_playingScreen;
        private LS_ResultScreen _ls_resultScreen;
        
        // Misc
        private AchievementScreen _achievementScreen;


        public InputHandler(GameStateManager gameStateManager, GameLogic gameLogic, SettingsScreen settingsScreen, Game game)
        {
            _gameStateManager = gameStateManager;
            _gameLogic = gameLogic;
            _settingsScreen = settingsScreen;
            _game = game; // Initialize the Game instance
            _cooldownTimer = 0;
            _random = new Random(); // Initialize the random generator
            _titleScreen = new TitleScreen(this, _gameStateManager);
            _playingScreen = new PlayingScreen(this, _gameStateManager);
            _resultScreen = new ResultScreen(this, _gameStateManager, _gameLogic);

            // Lizard Spock GameMode
            _ls_titleScreen = new LS_TitleScreen(this, _gameStateManager);
            _ls_playingScreen = new LS_PlayingScreen(this, _gameStateManager);
            _ls_resultScreen = new LS_ResultScreen(this, _gameStateManager, _gameLogic);

            // Misc Screen
            _achievementScreen = new AchievementScreen(this, _gameStateManager);


        }

        public void Update(MouseState currentMouseState, KeyboardState currentKeyboardState, GameTime gameTime)
        {
            _cooldownTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.D))
            {
                _gameStateManager.ResetSaveData();
            }
            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.T))
            {
                _gameStateManager.CurrentState = GameState.Settings;
            }
            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.C) && _gameStateManager.CurrentState != GameState.C_Playing)
            {
                _gameStateManager.CurrentState = GameState.C_Playing;
                _gameStateManager.CharacterGameLogic.StartNewGame();

            }
            if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.W) && _gameStateManager.CurrentState != GameState.World)
            {
                _gameStateManager.CurrentState = GameState.World;
                CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);
            }

            var handleInputActions = new Dictionary<GameState, Action>
            {
                { GameState.Title, () => _titleScreen.HandleInput(currentKeyboardState, currentMouseState, _previousMouseState) },
                { GameState.Playing, () => _playingScreen.HandleInput(currentKeyboardState, currentMouseState, _previousMouseState) },
                { GameState.Result, () => _resultScreen.HandleInput(currentKeyboardState, currentMouseState, _previousMouseState) },
                { GameState.LS_Title, () => _ls_titleScreen.HandleInput(currentKeyboardState, currentMouseState, _previousMouseState) },
                { GameState.LS_Playing, () => _ls_playingScreen.HandleInput(currentKeyboardState, currentMouseState, _previousMouseState) },
                { GameState.LS_Result, () => _ls_resultScreen.HandleInput(currentKeyboardState, currentMouseState, _previousMouseState) },
                { GameState.Achievements, () => _achievementScreen.HandleInput(currentKeyboardState, _previousKeyboardState, currentMouseState, _previousMouseState) },
                { GameState.Settings, () => 
                    {
                        _settingsScreen.Update(currentMouseState, _previousMouseState);
                        CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);
                    }
                },
                { GameState.C_Playing, () => HandleCPlayingInput(currentMouseState, currentKeyboardState) }
            };

            if (handleInputActions.TryGetValue(_gameStateManager.CurrentState, out var handleInputAction))
            {
                handleInputAction();
            }
            UpdateHoverSounds(currentMouseState);

            _previousMouseState = currentMouseState;
            _previousKeyboardState = currentKeyboardState;
        }


        private void HandleCPlayingInput(MouseState currentMouseState, KeyboardState currentKeyboardState)
        {
            if (_gameStateManager.CharacterGameLogic.IsGameActive)
            {
                var positions = _gameStateManager.Positions[GameState.C_Playing];
                foreach (Choice choice in new[] { Choice.Rock, Choice.Paper, Choice.Scissors })
                {
                    if (IsChoiceMade(choice, currentMouseState, _previousMouseState, currentKeyboardState, positions))
                    {
                        _gameStateManager.CharacterGameLogic.HandlePlayerChoice(choice);
                        break;
                    }
                }
            }
            else if (IsPlayAgainClicked(currentMouseState))
            {
                _gameStateManager.CharacterGameLogic.StartNewGame();
            }

            CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);
        }

        private bool IsChoiceMade(Choice choice, MouseState currentMouseState, MouseState previousMouseState, KeyboardState currentKeyboardState, Dictionary<string, Vector2> positions)
        {
            if (_gameStateManager.Textures.TryGetValue(choice, out Texture2D texture))
            {
                Vector2 position = positions[choice.ToString()];
                float scale = IsHovering(position, texture, _gameStateManager.NormalScale)
                    ? _gameStateManager.HoverScale
                    : _gameStateManager.NormalScale;

                Rectangle clickableArea = new Rectangle(
                    (int)(position.X - texture.Width * scale / 2),
                    (int)(position.Y - texture.Height * scale / 2),
                    (int)(texture.Width * scale),
                    (int)(texture.Height * scale)
                );

                bool isClicked = clickableArea.Contains(currentMouseState.Position) &&
                                 currentMouseState.LeftButton == ButtonState.Pressed &&
                                 previousMouseState.LeftButton == ButtonState.Released;

                return isClicked ||
                       (choice == Choice.Rock && currentKeyboardState.IsKeyDown(Keys.D1)) ||
                       (choice == Choice.Paper && currentKeyboardState.IsKeyDown(Keys.D2)) ||
                       (choice == Choice.Scissors && currentKeyboardState.IsKeyDown(Keys.D3));
            }

            return false;
        }

        private bool IsPlayAgainClicked(MouseState currentMouseState)
        {
            return _gameStateManager.StartButtonRectangle.Contains(currentMouseState.Position) &&
                   currentMouseState.LeftButton == ButtonState.Pressed &&
                   _previousMouseState.LeftButton == ButtonState.Released;
        }

        public void ExitCondition()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && _cooldownTimer <= 0)
            {
                _game.Exit();
            }
        }

        private void UpdateHoverSounds(MouseState currentMouseState)
        {
            if (!_game.IsActive)
            {
                return;
            }
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


        public void CheckForExitButtonClick(MouseState currentMouseState, GameState state, KeyboardState currentKeyboardState)
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

        public void CheckForNextButtonClick(MouseState currentMouseState)
        {
            if (_gameStateManager.NextButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.NextModeSound.Play();
                _gameStateManager.IsTransitioning = true;

                //_gameStateManager.CurrentState = GameState.LS_Title;
            }
        }

        public void CheckForPrevButtonClick(MouseState currentMouseState)
        {
            if (_gameStateManager.PrevButtonRectangle.Contains(currentMouseState.Position) && currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _gameStateManager.NextModeSound.Play();
                _gameStateManager.IsTransitioning = true;

                // _gameStateManager.CurrentState = GameState.Title;
            }
        }

        public bool IsHovering(Vector2 position, Texture2D texture, float scale)
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
