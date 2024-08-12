using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockPaperScissors.Managers;

namespace RockPaperScissors
{
    public enum GamePhase
    {
        PlayerTurn,
        CharacterTurn,
        Result,
        GameOver
    }

    public class CharacterGameLogic
    {
        private GameStateManager _gameStateManager;
        private Character _currentCharacter;
        private int _playerWins;
        private int _characterWins;
        private int _roundsPlayed;
        private const int TotalRounds = 3;
        private string _currentDialogue;
        private string _currentDirection = "...";
        private Choice _playerChoice;
        private Choice _characterChoice;
        public bool IsGameActive { get; private set; }
        public GamePhase CurrentPhase { get; private set; }

        private float _phaseTimer;
        private const float CharacterTurnDuration = 2f; // 2 seconds for character to "think"
        private const float ResultDuration = 3f; // 3 seconds to show the result
        private const float GameOverDuration = 3f;

        private Dictionary<Choice, Vector2> _choicePositions;
        private Dictionary<Choice, float> _choiceOpacities;
        private Vector2 _playerChoiceTargetPosition;

        private Vector2 _characterChoicePosition;
        private Vector2 _characterChoiceTargetPosition;
        private const float CharacterChoiceSlideSpeed = 10f; 

        private const float TransitionSpeed = 7f;
        private const float FadeSpeed = 4f;

        private GameState previousGameState = GameState.LS_Title;


        public CharacterGameLogic(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
            _choicePositions = new Dictionary<Choice, Vector2>();
            _choiceOpacities = new Dictionary<Choice, float>();
            _characterChoicePosition = new Vector2(
            gameStateManager.windowWidth + 100, // Start off-screen to the right
            gameStateManager.windowHeight / 2);
        }

        private void InitializeChoicePositions()
        {
            var positions = _gameStateManager.Positions[GameState.C_Playing];
            _choicePositions[Choice.Rock] = positions["Rock"];
            _choicePositions[Choice.Paper] = positions["Paper"];
            _choicePositions[Choice.Scissors] = positions["Scissors"];
            _playerChoiceTargetPosition = positions["PlayerChoice"];
            _characterChoiceTargetPosition = positions["CharacterChoice"];

            foreach (Choice choice in Enum.GetValues(typeof(Choice)))
            {
                if (choice != Choice.None)
                {
                    _choiceOpacities[choice] = 1f;
                }
            }

            _characterChoiceTargetPosition = _gameStateManager.Positions[GameState.C_Playing]["CharacterChoice"];
            _characterChoicePosition = new Vector2(
                _gameStateManager.windowWidth + 100,
                _characterChoiceTargetPosition.Y);
        }

        public void StartNewGame()
        {
            previousGameState = GameState.Title;

            _currentCharacter = SelectRandomCharacter();
            _currentCharacter.ResetOrderedDialogue();
            _playerWins = 0;
            _characterWins = 0;
            _roundsPlayed = 0;
            _currentDialogue = _currentCharacter.GetNextDialogue("GameStart");
            _playerChoice = Choice.None;
            _characterChoice = Choice.None;
            IsGameActive = true;
            CurrentPhase = GamePhase.PlayerTurn;
            _currentDirection = "Make a choice!";
            InitializeChoicePositions();
        }

        public void StartNewGame(Character specificCharacter, GameState gameState)
        {
            previousGameState = gameState;


            _currentCharacter = specificCharacter;
            _currentCharacter.ResetOrderedDialogue();
            _playerWins = 0;
            _characterWins = 0;
            _roundsPlayed = 0;
            _currentDialogue = _currentCharacter.GetNextDialogue("GameStart");
            _playerChoice = Choice.None;
            _characterChoice = Choice.None;
            IsGameActive = true;
            CurrentPhase = GamePhase.PlayerTurn;
            _currentDirection = "Make a choice!";
            InitializeChoicePositions();
        }

        private Character SelectRandomCharacter()
        {
            Random rng = new Random();
            return _gameStateManager._characters[rng.Next(_gameStateManager._characters.Count)];
        }

        public void Update(GameTime gameTime)
        {
            if (!IsGameActive) return;

            var positions = _gameStateManager.Positions[GameState.C_Playing];
            _gameStateManager.IsHoveringRock = IsHovering(positions["Rock"], _gameStateManager.RockTexture, _gameStateManager.NormalScale);
            _gameStateManager.IsHoveringPaper = IsHovering(positions["Paper"], _gameStateManager.PaperTexture, _gameStateManager.NormalScale);
            _gameStateManager.IsHoveringScissors = IsHovering(positions["Scissors"], _gameStateManager.ScissorsTexture, _gameStateManager.NormalScale);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateChoicePositions(elapsed);
            UpdateChoiceOpacities(elapsed);
            UpdateCharacterChoicePosition(elapsed);

            switch (CurrentPhase)
            {
                case GamePhase.PlayerTurn:
                    // Wait for player input, handled in HandlePlayerChoice
                    break;
                case GamePhase.CharacterTurn:
                    _phaseTimer -= elapsed;
                    if (_phaseTimer <= 0)
                    {
                        _characterChoice = _currentCharacter.MakeRPSChoice();
                        CurrentPhase = GamePhase.Result;
                        ResolveRound();
                        _phaseTimer = ResultDuration;
                    }
                    break;
                case GamePhase.Result:
                    _phaseTimer -= elapsed;

                    if (_phaseTimer <= 0)
                    {
                        if (_playerWins > TotalRounds / 2 || _characterWins > TotalRounds / 2)
                        {
                            HandleGameOver();
                            _phaseTimer = GameOverDuration;
                        }
                        else
                        {
                            StartNewRound();
                            _currentDirection = "Make a Choice...";
                        }
                    }
                    break;
                case GamePhase.GameOver:

                    _phaseTimer -= elapsed;
                    if (_phaseTimer <= 0)
                    {
                        IsGameActive = false; // Ensure game is marked as inactive
                        _gameStateManager.CurrentState = previousGameState; // Or GameState.Title, depending on your needs
                    }
                    break;
            }
        }


        private void UpdateCharacterChoicePosition(float elapsed)
        {
            if (CurrentPhase == GamePhase.CharacterTurn || CurrentPhase == GamePhase.Result)
            {
                _characterChoicePosition = Vector2.Lerp(_characterChoicePosition, _characterChoiceTargetPosition, CharacterChoiceSlideSpeed * elapsed);
            }
            else
            {
                _characterChoicePosition.X = _gameStateManager.windowWidth + 100;
            }
        }

        private void UpdateChoicePositions(float elapsed)
        {
            foreach (Choice choice in Enum.GetValues(typeof(Choice)))
            {
                if (choice != Choice.None)
                {
                    Vector2 targetPosition = _choicePositions[choice];

                    // Update positions only when it's not the player's turn
                    if (CurrentPhase == GamePhase.CharacterTurn || CurrentPhase == GamePhase.Result)
                    {
                        targetPosition = choice == _playerChoice ? _playerChoiceTargetPosition :
                                         
                                         _choicePositions[choice];
                    }

                    _choicePositions[choice] = Vector2.Lerp(_choicePositions[choice], targetPosition, TransitionSpeed * elapsed);
                }
            }
        }



        private void UpdateChoiceOpacities(float elapsed)
        {
            foreach (Choice choice in Enum.GetValues(typeof(Choice)))
            {
                if (choice != Choice.None)
                {
                    float targetOpacity = 1f;

                    // Start fading only after the player has chosen and it's no longer the player's turn
                    if (CurrentPhase == GamePhase.CharacterTurn || CurrentPhase == GamePhase.Result || CurrentPhase == GamePhase.GameOver)
                    {
                        targetOpacity = (choice == _playerChoice) ? 1f : 0f;
                    }

                    _choiceOpacities[choice] = MathHelper.Lerp(_choiceOpacities[choice], targetOpacity, FadeSpeed * elapsed);
                }
            }
        }



        public void HandlePlayerChoice(Choice choice)
        {
            if (CurrentPhase == GamePhase.PlayerTurn && _playerChoice == Choice.None)
            {
                _playerChoice = choice;

                // Start the zoom-in effect by setting the target position for the player's choice
                CurrentPhase = GamePhase.CharacterTurn;
                _phaseTimer = CharacterTurnDuration;
                _currentDialogue = _currentCharacter.GetNextDialogue("Thinking");
                _currentDirection = "Opponent is making a choice...";
            }
        }


        private void ResolveRound()
        {
            if (_playerChoice == _characterChoice)
            {
                _currentDialogue = _currentCharacter.GetNextDialogue("Tie");
                _currentDirection = "You Tied!";
            }
            else if ((_playerChoice == Choice.Rock && _characterChoice == Choice.Scissors) ||
                     (_playerChoice == Choice.Paper && _characterChoice == Choice.Rock) ||
                     (_playerChoice == Choice.Scissors && _characterChoice == Choice.Paper))
            {
                _playerWins++;
                _currentDialogue = _currentCharacter.GetNextDialogue("Lose");
                _currentCharacter.AdjustProbabilities(_playerChoice);
                _currentDirection = "You Won!";

            }
            else
            {
                _characterWins++;
                _currentDialogue = _currentCharacter.GetNextDialogue("Win");
                _currentDirection = "Your Opponent Won!";

            }

            _roundsPlayed++;

            // Debug logging
            Console.WriteLine($"Round {_roundsPlayed} result: Player {_playerChoice}, Character {_characterChoice}");
            Console.WriteLine($"Current dialogue: {_currentDialogue}");

            // Reset character choice position for the next round
            _characterChoicePosition.X = _gameStateManager.windowWidth + 100;
        }


        private void StartNewRound()
        {
            _playerChoice = Choice.None;
            _characterChoice = Choice.None;
            CurrentPhase = GamePhase.PlayerTurn;
            _currentDialogue = _currentCharacter.GetNextDialogue("NewRound");
            InitializeChoicePositions();  // Reset positions and opacities
        }

        private void HandleGameOver()
        {
            CurrentPhase = GamePhase.GameOver;
            if (_playerWins > _characterWins)
            {
                _currentDialogue = _currentCharacter.GetNextDialogue("GameLose");
                _gameStateManager.GainXP(50);
            }
            else if (_characterWins > _playerWins)
            {
                _currentDialogue = _currentCharacter.GetNextDialogue("GameWin");
            }
            else
            {
                _currentDialogue = _currentCharacter.GetNextDialogue("GameTie");
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_currentCharacter == null) return;

            // Draw character portrait
            spriteBatch.Draw(_currentCharacter.Portrait,
                _gameStateManager.Positions[GameState.C_Playing]["CharacterPortrait"],
                null,
                Color.White,
                0f,
                new Vector2(_currentCharacter.Portrait.Width, _currentCharacter.Portrait.Height),
                _gameStateManager.NormalScale * 0.5f,
                SpriteEffects.None,
                0f);

            // Draw player direction
            Vector2 directionTextSize = _gameStateManager.Font.MeasureString(_currentDirection);
            spriteBatch.DrawString(_gameStateManager.Font,
                _currentDirection,
                _gameStateManager.Positions[GameState.C_Playing]["PlayerDirection"] - directionTextSize / 2,
                Color.Black);

            // Draw character dialogue
            Vector2 dialogueTextSize = _gameStateManager.Font.MeasureString(_currentDialogue);
            spriteBatch.DrawString(_gameStateManager.Font,
                _currentDialogue,
                _gameStateManager.Positions[GameState.C_Playing]["CharacterDialogue"] - dialogueTextSize / 2,
                Color.White);

            // Draw scores
            spriteBatch.DrawString(_gameStateManager.Font, $"Player: {_playerWins}", new Vector2(50, 50), Color.White);
            spriteBatch.DrawString(_gameStateManager.Font, $"{_currentCharacter.Name}: {_characterWins}", new Vector2(650, 50), Color.White);

            // Draw choices with transitions
            foreach (var choice in new[] { Choice.Rock, Choice.Paper, Choice.Scissors })
            {
                DrawChoice(spriteBatch, choice, _choicePositions[choice], _choiceOpacities[choice]);
            }

            // Draw player's choice
            if (_playerChoice != Choice.None)
            {
                DrawChoice(spriteBatch, _playerChoice, _choicePositions[_playerChoice], 1f);
            }

            // Draw character's choice separately
            if (CurrentPhase != GamePhase.PlayerTurn && _characterChoice != Choice.None)
            {
                DrawCharacterChoice(spriteBatch, _characterChoice, _characterChoicePosition);
            }

            // Draw current phase (for debugging)
            spriteBatch.DrawString(_gameStateManager.Font, $"Phase: {CurrentPhase}", new Vector2(50, 80), Color.White);

            // Draw "Play Again" button if the game is over


        }

        private void DrawCharacterChoice(SpriteBatch spriteBatch, Choice choice, Vector2 position)
        {
            if (_gameStateManager.Textures.TryGetValue(choice, out Texture2D texture))
            {
                float scale = _gameStateManager.NormalScale * 1.2f; // Slightly larger than normal
                spriteBatch.Draw(texture, position, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0f);
            }
        }

        private void DrawChoice(SpriteBatch spriteBatch, Choice choice, Vector2 position, float opacity = 1.0f)
        {
            if (choice != Choice.None && _gameStateManager.Textures.TryGetValue(choice, out Texture2D texture))
            {
                float scale = GetScale(position, texture);

                // Apply a zoom-in effect when the choice is selected
                if (choice == _playerChoice || choice == _characterChoice)
                {
                    scale *= 1.2f; // Increase the scale for the zoom-in effect
                }

                spriteBatch.Draw(texture, position, null, Color.White * opacity, 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0f);
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


        private float GetScale(Vector2 position, Texture2D texture)
        {
            return IsHovering(position, texture, _gameStateManager.NormalScale) ? _gameStateManager.HoverScale : _gameStateManager.NormalScale;
        }



    }
}