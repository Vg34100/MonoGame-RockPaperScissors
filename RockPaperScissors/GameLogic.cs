using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using RockPaperScissors.Managers;
using System;
using System.Collections.Generic;
using System.IO;

namespace RockPaperScissors
{
    public class GameLogic
    {
        private GameStateManager _gameStateManager;
        private Random _random;
        private bool _resultCalculated = false;

        private readonly Dictionary<Choice, List<Choice>> _winConditions = new Dictionary<Choice, List<Choice>>
        {
            { Choice.Rock, new List<Choice> { Choice.Scissors } },
            { Choice.Paper, new List<Choice> { Choice.Rock } },
            { Choice.Scissors, new List<Choice> { Choice.Paper } }
        };

        private readonly Dictionary<LS_Choice, List<LS_Choice>> _lsWinConditions = new Dictionary<LS_Choice, List<LS_Choice>>
        {
            { LS_Choice.Rock, new List<LS_Choice> { LS_Choice.Scissors, LS_Choice.Lizard } },
            { LS_Choice.Paper, new List<LS_Choice> { LS_Choice.Rock, LS_Choice.Spock } },
            { LS_Choice.Scissors, new List<LS_Choice> { LS_Choice.Paper, LS_Choice.Lizard } },
            { LS_Choice.Lizard, new List<LS_Choice> { LS_Choice.Spock, LS_Choice.Paper } },
            { LS_Choice.Spock, new List<LS_Choice> { LS_Choice.Scissors, LS_Choice.Rock } }
        };

        public GameLogic(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
            _random = new Random();
        }

        public void Update(GameTime gameTime)
        {
            if (!_resultCalculated)
            {
                if (_gameStateManager.CurrentState == GameState.Result)
                {
                    MakeComputerChoice<Choice>();
                    DetermineResult(_gameStateManager.PlayerChoice, _gameStateManager.ComputerChoice, _winConditions);
                    _resultCalculated = true;
                }
                else if (_gameStateManager.CurrentState == GameState.LS_Result)
                {
                    MakeComputerChoice<LS_Choice>();
                    DetermineResult(_gameStateManager.PlayerLSChoice, _gameStateManager.ComputerLSChoice, _lsWinConditions);
                    _resultCalculated = true;

                }
            }
        }

        private void MakeComputerChoice<T>()
        {
            if (typeof(T) == typeof(Choice))
                _gameStateManager.ComputerChoice = (Choice)_random.Next(1, 4);
            if (typeof(T) == typeof(LS_Choice))
                _gameStateManager.ComputerLSChoice = (LS_Choice)_random.Next(1, 6);

        }

        private void DetermineResult<T>(T playerChoice, T computerChoice, Dictionary<T, List<T>> winConditions)
        {

            if (object.Equals(playerChoice, computerChoice))
            {
                HandleGameResult(GameResult.Tie, "It's a tie!", _gameStateManager.LoseSound, 0, playerChoice);
            }
            if (winConditions[playerChoice].Contains(computerChoice))
            {
                HandleGameResult(GameResult.Win, "You win!", _gameStateManager.WinSound, 10 * _gameStateManager.WinStreak, playerChoice);
            }
            else
            {
                HandleGameResult(GameResult.Lose, "You lose!", _gameStateManager.LoseSound, 0, playerChoice);

            }
            CheckAchievements();
            _gameStateManager.SaveGameData();
        }

        private void HandleGameResult<T>(GameResult result, string message, SoundEffect sound, int xp, T choice)
        {
            _gameStateManager.ResultMessage = message;
            sound.Play();
            switch (result)
            {
                case GameResult.Win:
                    _gameStateManager.WinStreak++;
                    _gameStateManager.OverallWins++;
                    if (choice is Choice)
                    {
                        _gameStateManager.IncrementWinCount((Choice)(object)choice);
                        _gameStateManager.ModeWins[GameState.Playing]++;
                    }
                    else if (choice is LS_Choice)
                    {
                        _gameStateManager.IncrementLSWinCount((LS_Choice)(object)choice);
                        _gameStateManager.ModeWins[GameState.LS_Playing]++;
                    }
                    break;
                case GameResult.Tie:
                    _gameStateManager.OverallTies++;
                    break;
                case GameResult.Lose:
                    _gameStateManager.OverallLoses++;
                    _gameStateManager.WinStreak = 0;
                    break;
            }
            _gameStateManager.GainXP(xp);
        }

        public void ResetRound()
        {
            _resultCalculated = false;
            _gameStateManager.PlayerChoice = Choice.None;
            _gameStateManager.ComputerChoice = Choice.None;
            _gameStateManager.PlayerLSChoice = LS_Choice.None;
            _gameStateManager.ComputerLSChoice = LS_Choice.None;
            _gameStateManager.ResultMessage = "";
        }

        // Check for achievements
        private void CheckAchievements()
        {
            if (_gameStateManager.ModeWins[GameState.Playing] >= 1)
            {
                _gameStateManager.AchievementManager.UnlockAchievement("First Win");
            }
            if (_gameStateManager.ModeWins[GameState.LS_Playing] >= 1)
            {
                _gameStateManager.AchievementManager.UnlockAchievement("Lizard and Spock?");
            }

            if (_gameStateManager.WinStreak >= 5)
                _gameStateManager.AchievementManager.UnlockAchievement("Win Streak");

            if (_gameStateManager.OverallLoses >= 20)
                _gameStateManager.AchievementManager.UnlockAchievement("Unlucky...");

            CheckSpecificAchievement(Choice.Rock, "Rock Builder", 5);
            CheckSpecificAchievement(Choice.Paper, "Paper Fanatic", 5);
            CheckSpecificAchievement(Choice.Scissors, "Scissor Crazy", 5);
            CheckSpecificAchievement(LS_Choice.Lizard, "Lizard Luck", 5);
            CheckSpecificAchievement(LS_Choice.Spock, "Spocked!?", 5);
        }

        // Check for specific achievements
        private void CheckSpecificAchievement<T>(T choice, string achievement, int count) where T : Enum
        {
            // Calculate the number of wins for the given choice.
            // If the choice is of type Choice, sum the wins from both WinsWith and LSWinsWith dictionaries.
            // If the choice is of type LS_Choice, get the wins from the LSWinsWith dictionary.

            int wins = choice is Choice
                ? _gameStateManager.WinsWith[(Choice)(object)choice] + _gameStateManager.LSWinsWith[(LS_Choice)(object)choice]
                : _gameStateManager.LSWinsWith[(LS_Choice)(object)choice];

            // If the total wins for the given choice are greater than or equal to the specified count,
            // unlock the corresponding achievement.
            if (wins >= count)
                _gameStateManager.AchievementManager.UnlockAchievement(achievement);
        }

    }


}
