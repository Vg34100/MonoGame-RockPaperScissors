using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace RockPaperScissors
{
    public class GameLogic
    {
        private GameStateManager _gameStateManager;
        private Random _random;
        private bool _resultCalculated;

        private Dictionary<Choice, List<Choice>> _winConditions;
        private Dictionary<LS_Choice, List<LS_Choice>> _lsWinConditions;

        public GameLogic(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
            _random = new Random();
            _resultCalculated = false;

            InitializeWinConditions();
            InitializeLSWinConditions();
        }

        private void InitializeWinConditions()
        {
            _winConditions = new Dictionary<Choice, List<Choice>>
            {
                { Choice.Rock, new List<Choice> { Choice.Scissors } },
                { Choice.Paper, new List<Choice> { Choice.Rock } },
                { Choice.Scissors, new List<Choice> { Choice.Paper } }
            };
        }

        private void InitializeLSWinConditions()
        {
            _lsWinConditions = new Dictionary<LS_Choice, List<LS_Choice>>
            {
                { LS_Choice.Rock, new List<LS_Choice> { LS_Choice.Scissors, LS_Choice.Lizard } },
                { LS_Choice.Paper, new List<LS_Choice> { LS_Choice.Rock, LS_Choice.Spock } },
                { LS_Choice.Scissors, new List<LS_Choice> { LS_Choice.Paper, LS_Choice.Lizard } },
                { LS_Choice.Lizard, new List<LS_Choice> { LS_Choice.Spock, LS_Choice.Paper } },
                { LS_Choice.Spock, new List<LS_Choice> { LS_Choice.Scissors, LS_Choice.Rock } }
            };
        }

        public void Update(GameTime gameTime)
        {
            if (_gameStateManager.CurrentState == GameState.Result && !_resultCalculated)
            {
                MakeComputerChoice();
                DetermineResult();
                _resultCalculated = true;
            }
            if (_gameStateManager.CurrentState == GameState.LS_Result && !_resultCalculated)
            {
                MakeComputerLSChoice();
                DetermineLSResult();
                _resultCalculated = true;
            }
        }

        private void MakeComputerChoice()
        {
            int choice = _random.Next(1, 4); // 1 to 3
            _gameStateManager.ComputerChoice = (Choice)choice;
        }

        private void DetermineResult()
        {
            if (_gameStateManager.PlayerChoice == _gameStateManager.ComputerChoice)
            {
                _gameStateManager.ResultMessage = "It's a tie!";
                _gameStateManager.LoseSound.Play();
                _gameStateManager.OverallTies++;
                // _gameStateManager.WinStreak = 0;
            }
            else if (_winConditions[_gameStateManager.PlayerChoice].Contains(_gameStateManager.ComputerChoice))
            {
                _gameStateManager.ResultMessage = "You win!";
                _gameStateManager.WinSound.Play();
                _gameStateManager.WinStreak++;
                _gameStateManager.OverallWins++;
                _gameStateManager.IncrementWinCount(_gameStateManager.PlayerChoice);
                _gameStateManager.GainXP(10 * _gameStateManager.WinStreak); // Gain 10 XP for a win
                _gameStateManager.AchievementManager.UnlockAchievement("First Win");
            }
            else
            {
                _gameStateManager.ResultMessage = "You lose!";
                _gameStateManager.LoseSound.Play();
                _gameStateManager.WinStreak = 0;
                _gameStateManager.OverallLoses++;
            }
            AchievementCheck();
            _gameStateManager.SaveGameData();
        }




        public void ResetRound()
        {
            _resultCalculated = false;
            _gameStateManager.PlayerChoice = Choice.None;
            _gameStateManager.ComputerChoice = Choice.None;
            _gameStateManager.ResultMessage = "";
        }

        // Add method for Lizard-Spock mode
        private void MakeComputerLSChoice()
        {
            int choice = _random.Next(1, 6); // 1 to 5 for LS mode
            _gameStateManager.ComputerLSChoice = (LS_Choice)choice;
        }


        private void DetermineLSResult()
        {
            if (_gameStateManager.PlayerLSChoice == _gameStateManager.ComputerLSChoice)
            {
                _gameStateManager.ResultMessage = "It's a tie!";
                _gameStateManager.LoseSound.Play();
                _gameStateManager.WinStreak = 0;
                _gameStateManager.OverallTies++;
            }
            else if (_lsWinConditions[_gameStateManager.PlayerLSChoice].Contains(_gameStateManager.ComputerLSChoice))
            {
                _gameStateManager.ResultMessage = "You win!";
                _gameStateManager.WinSound.Play();
                _gameStateManager.WinStreak++;
                _gameStateManager.IncrementLSWinCount(_gameStateManager.PlayerLSChoice);
                _gameStateManager.GainXP(20 * _gameStateManager.WinStreak); // Gain 20 XP for a win
                _gameStateManager.OverallWins++;
                _gameStateManager.AchievementManager.UnlockAchievement("Lizard and Spock?");

            }
            else
            {
                _gameStateManager.ResultMessage = "You lose!";
                _gameStateManager.LoseSound.Play();
                _gameStateManager.WinStreak = 0;
                _gameStateManager.OverallLoses++;

            }
            AchievementCheck();
            _gameStateManager.SaveGameData();

        }
        private void AchievementCheck()
        {
            _gameStateManager.WinsWith.TryGetValue(Choice.Rock, out int rockval2);
            Console.WriteLine($"Computer choice: {rockval2}");
            if (_gameStateManager.WinStreak >= 5)
            {
                _gameStateManager.AchievementManager.UnlockAchievement("Win Streak");
            }
            if (_gameStateManager.OverallLoses >= 20)
            {
                _gameStateManager.AchievementManager.UnlockAchievement("Unlucky...");
            }
            if (_gameStateManager.WinsWith.TryGetValue(Choice.Rock, out int rockval) && _gameStateManager.LSWinsWith.TryGetValue(LS_Choice.Rock, out int lsrockval))
            {
                if (rockval + lsrockval >= 5)
                {
                    _gameStateManager.AchievementManager.UnlockAchievement("Rock Builder");
                }
            }
            if (_gameStateManager.WinsWith.TryGetValue(Choice.Paper, out int paperval) && _gameStateManager.LSWinsWith.TryGetValue(LS_Choice.Paper, out int lspaperval))
            {
                if (paperval + lspaperval >= 5)
                {
                    _gameStateManager.AchievementManager.UnlockAchievement("Paper Fanatic");
                }
            }
            if (_gameStateManager.WinsWith.TryGetValue(Choice.Scissors, out int scissorval) && _gameStateManager.LSWinsWith.TryGetValue(LS_Choice.Scissors, out int lsscissorval))
            {
                if (scissorval + lsscissorval >= 5)
                {
                    _gameStateManager.AchievementManager.UnlockAchievement("Scissor Crazy");
                }
            }
            if (_gameStateManager.LSWinsWith.TryGetValue(LS_Choice.Lizard, out int lslizardval))
            {
                if (lslizardval >= 5)
                {
                    _gameStateManager.AchievementManager.UnlockAchievement("Lizard Luck");
                }
            }
            if (_gameStateManager.LSWinsWith.TryGetValue(LS_Choice.Spock, out int lsspockval))
            {
                if (lsspockval >= 5)
                {
                    _gameStateManager.AchievementManager.UnlockAchievement("Spocked!?");
                }
            }
        }
    }


}
