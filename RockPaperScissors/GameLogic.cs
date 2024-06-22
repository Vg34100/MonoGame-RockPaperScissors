using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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
                // _gameStateManager.WinStreak = 0;
            }
            else if (_winConditions[_gameStateManager.PlayerChoice].Contains(_gameStateManager.ComputerChoice))
            {
                _gameStateManager.ResultMessage = "You win!";
                _gameStateManager.WinStreak++;
                GainXP(10 * _gameStateManager.WinStreak); // Gain 10 XP for a win
            }
            else
            {
                _gameStateManager.ResultMessage = "You lose!";
                _gameStateManager.WinStreak = 0;
            }
            _gameStateManager.SaveGameData();
        }

        private void GainXP(int amount)
        {
            _gameStateManager.XP += amount;
            if (_gameStateManager.XP >= _gameStateManager.XPNeeded)
            {
                _gameStateManager.XP -= _gameStateManager.XPNeeded;
                _gameStateManager.Level++;
            }
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
                _gameStateManager.WinStreak = 0;
            }
            else if (_lsWinConditions[_gameStateManager.PlayerLSChoice].Contains(_gameStateManager.ComputerLSChoice))
            {
                _gameStateManager.ResultMessage = "You win!";
                _gameStateManager.WinStreak++;
                GainXP(10); // Gain 10 XP for a win
            }
            else
            {
                _gameStateManager.ResultMessage = "You lose!";
                _gameStateManager.WinStreak = 0;
            }
        }
    }
}
