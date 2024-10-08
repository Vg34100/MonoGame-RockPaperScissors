﻿namespace RockPaperScissors
{
    public enum GameState { Title, Playing, Result, LS_Title, LS_Playing, LS_Result, Achievements, Settings, C_Playing, World }
    public enum Choice { None, Rock, Paper, Scissors }
    public enum LS_Choice { None, Rock, Paper, Scissors, Lizard, Spock }
    public enum HoverItem
    {
        None,
        Rock,
        Paper,
        Scissors,
        Lizard,
        Spock
    }
    public enum GameResult
    {
        Tie,
        Win,
        Lose
    }
}
