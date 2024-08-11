using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace RockPaperScissors.Managers
{
    public class GameStateManager
    {

        public AchievementManager AchievementManager { get; private set; }


        public GameState CurrentState { get; set; } = GameState.Title;
        public Choice PlayerChoice { get; set; } = Choice.None;
        public Choice ComputerChoice { get; set; } = Choice.None;
        public LS_Choice PlayerLSChoice { get; set; } = LS_Choice.None;
        public LS_Choice ComputerLSChoice { get; set; } = LS_Choice.None;
        public string ResultMessage { get; set; } = "";
        public bool IsHoveringRock { get; set; } = false;
        public bool IsHoveringPaper { get; set; } = false;
        public bool IsHoveringScissors { get; set; } = false;
        public bool IsHoveringLizard { get; set; } = false;
        public bool IsHoveringSpock { get; set; } = false;


        public Dictionary<GameState, Dictionary<string, Vector2>> Positions { get; private set; }
        public Dictionary<Choice, Texture2D> Textures { get; private set; }

        public Dictionary<LS_Choice, Texture2D> LSTextures { get; private set; }



        public Vector2 StartButtonPosition { get; set; }
        public Rectangle StartButtonRectangle { get; set; }

        public float HoverScale { get; set; } = 3.5f;
        public float NormalScale { get; set; } = 3f;
        public float ButtonScale { get; set; } = 1.5f;

        public Texture2D UnknownTexture { get; private set; }

        public Texture2D RockTexture { get; private set; }
        public Texture2D PaperTexture { get; private set; }
        public Texture2D ScissorsTexture { get; private set; }
        public Texture2D LizardTexture { get; private set; }
        public Texture2D SpockTexture { get; private set; }

        public Texture2D TitleTexture { get; private set; }
        public Texture2D LSTitleTexture { get; private set; }

        public Texture2D KnobTexture { get; private set; }
        public Texture2D SliderTexture { get; private set; }
        public Texture2D StartButtonTexture { get; private set; }
        public SpriteFont Font { get; private set; }

        public Texture2D ExitButtonTexture { get; private set; }

        public SoundEffect PlaySound { get; private set; }
        public SoundEffect ExitSound { get; private set; }

        public SoundEffect ChangeScreenSound { get; private set; }
        public SoundEffect LoseSound { get; private set; }
        public SoundEffect WinSound { get; private set; }
        public SoundEffect NextModeSound { get; private set; }
        public SoundEffect[] SelectSounds { get; private set; } // Add this line
        public float SoundEffectVolume { get; set; } = 1.0f; // Default volume (1.0 is full volume)






        public Vector2 ExitButtonPosition { get; set; }
        public Rectangle ExitButtonRectangle { get; set; }

        // Level System
        public int Level { get; set; } = 1;
        public int XP { get; set; } = 0;
        public int XPNeeded => Level * 100; // XP needed to level up
        public int WinStreak { get; set; } = 0;
        public int OverallWins { get; set; } = 0;
        public Dictionary<GameState, int> ModeWins { get; private set; }
        public int OverallLoses { get; set; } = 0;
        public int OverallTies { get; set; } = 0;


        public Dictionary<Choice, int> WinsWith { get; private set; }
        public Dictionary<LS_Choice, int> LSWinsWith { get; private set; }

        public int CurrentAchievementPage { get; set; } = 0;
        public int AchievementsPerPage { get; private set; } = 8;


        public Texture2D NextButtonTexture { get; private set; }
        public Texture2D PrevButtonTexture { get; private set; }
        public Vector2 NextButtonPosition { get; set; }
        public Vector2 PrevButtonPosition { get; set; }
        public Rectangle NextButtonRectangle { get; set; }
        public Rectangle PrevButtonRectangle { get; set; }



        public bool IsTransitioning { get; set; }
        public float TransitionProgress { get; set; }
        public const float TransitionSpeed = 2f; // Adjust the speed of the transition as needed



        public GameStateManager()
        {
            InitializeDictionaries();
        }

        private void InitializeDictionaries()
        {
            WinsWith = new Dictionary<Choice, int>
            {
                { Choice.Rock, 0 },
                { Choice.Paper, 0 },
                { Choice.Scissors, 0 }
            };

            LSWinsWith = new Dictionary<LS_Choice, int>
            {
                { LS_Choice.Rock, 0 },
                { LS_Choice.Paper, 0 },
                { LS_Choice.Scissors, 0 },
                { LS_Choice.Lizard, 0 },
                { LS_Choice.Spock, 0 }
            };

            ModeWins = new Dictionary<GameState, int>
            {
                { GameState.Playing, 0 },
                { GameState.LS_Playing, 0 },
            };
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            SoundEffect.MasterVolume = 0.8f;
            RockTexture = content.Load<Texture2D>("rock");
            PaperTexture = content.Load<Texture2D>("paper");
            ScissorsTexture = content.Load<Texture2D>("scissors");
            LizardTexture = content.Load<Texture2D>("lizard");
            SpockTexture = content.Load<Texture2D>("spock");
            TitleTexture = content.Load<Texture2D>("RPS-logo");
            LSTitleTexture = content.Load<Texture2D>("RPSLS-logo");

            UnknownTexture = content.Load<Texture2D>("unknown");

            StartButtonTexture = content.Load<Texture2D>("startButton");
            ExitButtonTexture = content.Load<Texture2D>("startButton"); // Load the exit button texture

            NextButtonTexture = content.Load<Texture2D>("nextButton");
            PrevButtonTexture = content.Load<Texture2D>("prevButton");

            KnobTexture = content.Load<Texture2D>("rock");
            SliderTexture = content.Load<Texture2D>("paper");

            Font = content.Load<SpriteFont>("File");

            int windowWidth = graphicsDevice.Viewport.Width;
            int windowHeight = graphicsDevice.Viewport.Height;

            NextButtonPosition = new Vector2(windowWidth - NextButtonTexture.Width / 2 - 10, windowHeight / 2);
            PrevButtonPosition = new Vector2(PrevButtonTexture.Width / 2 + 10, windowHeight / 2);



            PlaySound = content.Load<SoundEffect>("play");
            ExitSound = content.Load<SoundEffect>("exit");
            ChangeScreenSound = content.Load<SoundEffect>("change_screen");
            LoseSound = content.Load<SoundEffect>("lose");
            WinSound = content.Load<SoundEffect>("win");
            NextModeSound = content.Load<SoundEffect>("next_mode");
            // Load the select sounds
            SelectSounds = new SoundEffect[3];
            SelectSounds[0] = content.Load<SoundEffect>("select1");
            SelectSounds[1] = content.Load<SoundEffect>("select2");
            SelectSounds[2] = content.Load<SoundEffect>("select3");


            Positions = new Dictionary<GameState, Dictionary<string, Vector2>>()
            {
                {
                    GameState.Title, new Dictionary<string, Vector2>()
                    {
                        { "Rock", new Vector2(10.5f * windowWidth / 16, windowHeight / 4.7f) },
                        { "Paper", new Vector2(5.1f * windowWidth / 16, 3 * windowHeight / 8) },
                        { "Scissors", new Vector2(12.5f * windowWidth / 16,  4 * windowHeight / 8) },
                        { "Title", new Vector2(windowWidth / 2, windowHeight / 2) }
                    }
                },
                {
                    GameState.Playing, new Dictionary<string, Vector2>()
                    {
                        { "Rock", new Vector2(5 * windowWidth / 16, windowHeight / 3) },
                        { "Paper", new Vector2(windowWidth / 2, windowHeight / 3) },
                        { "Scissors", new Vector2(11 * windowWidth / 16, windowHeight / 3) }
                    }
                },
                {
                    GameState.Result, new Dictionary<string, Vector2>()
                    {
                        { "Rock", new Vector2(5 * windowWidth / 16, windowHeight / 3) },
                        { "Paper", new Vector2(windowWidth / 2, windowHeight / 3) },
                        { "Scissors", new Vector2(11 * windowWidth / 16, windowHeight / 3) }
                    }
                },
                {
                    GameState.LS_Title, new Dictionary<string, Vector2>()
                    {
                        { "Rock", new Vector2(10.5f * windowWidth / 16, windowHeight / 4.7f) },
                        { "Paper", new Vector2(5.1f * windowWidth / 16, 3 * windowHeight / 8) },
                        { "Scissors", new Vector2(12.5f * windowWidth / 16,  4 * windowHeight / 8) },
                        { "Lizard", new Vector2(5.2f * windowWidth / 16,  5 * windowHeight / 8) },
                        { "Spock", new Vector2(10.3f * windowWidth / 16,  6 * windowHeight / 8) },
                        { "LSTitle", new Vector2(windowWidth / 2, windowHeight / 2) }
                    }
                },
                {
                    GameState.LS_Playing, new Dictionary<string, Vector2>()
                    {
                        { "Rock", new Vector2(7f * windowWidth / 32, windowHeight / 3) },
                        { "Paper", new Vector2(11.5f * windowWidth / 32, windowHeight / 3) },
                        { "Scissors", new Vector2(16f * windowWidth / 32, windowHeight / 3) },
                        { "Lizard", new Vector2(20.5f * windowWidth / 32, windowHeight / 3) },
                        { "Spock", new Vector2(25f * windowWidth / 32, windowHeight / 3) }
                    }
                }
            };

            Textures = new Dictionary<Choice, Texture2D>
            {
                { Choice.Rock, RockTexture },
                { Choice.Paper, PaperTexture },
                { Choice.Scissors, ScissorsTexture }
            };

            LSTextures = new Dictionary<LS_Choice, Texture2D>
            {
                { LS_Choice.Rock, RockTexture },
                { LS_Choice.Paper, PaperTexture },
                { LS_Choice.Scissors, ScissorsTexture },
                { LS_Choice.Lizard, LizardTexture },
                { LS_Choice.Spock, SpockTexture }
            };

            StartButtonPosition = new Vector2(windowWidth / 2, 7 * windowHeight / 8);
            ExitButtonPosition = new Vector2(windowWidth - ExitButtonTexture.Width / 2 - 10, windowHeight - ExitButtonTexture.Height / 2 - 10); // Position it at the bottom right corner with some padding



            AchievementManager = new AchievementManager(
                content.Load<SpriteFont>("File"), // Load the font
                content.Load<SoundEffect>("achievement_unlock"), // Load the sound effect
                content.Load<Texture2D>("unlockBackground"), // Load the unlock background texture
                this
            );

            // Example achievements
            AchievementManager.AddAchievement(new Achievement("First Win", "Win your first game.", content.Load<Texture2D>("firstWinIcon"), 20));
            AchievementManager.AddAchievement(new Achievement("Win Streak", "Get a win streak of 5.", content.Load<Texture2D>("winStreakIcon"), 50));
            AchievementManager.AddAchievement(new Achievement("Unlucky...", "Lost 20 times...", content.Load<Texture2D>("winStreakIcon"), 30));
            AchievementManager.AddAchievement(new Achievement("Rock Builder", "Get a 5 wins using Rock", content.Load<Texture2D>("rock"), 20));
            AchievementManager.AddAchievement(new Achievement("Paper Fanatic", "Get a 5 wins using Paper", content.Load<Texture2D>("paper"), 20));
            AchievementManager.AddAchievement(new Achievement("Scissor Crazy", "Get a 5 wins using Scissor", content.Load<Texture2D>("scissors"), 20));
            AchievementManager.AddAchievement(new Achievement("Secret Achievement", "You found a secret!", content.Load<Texture2D>("secretIcon"), 100, true));
            AchievementManager.AddAchievement(new Achievement("Lizard and Spock?", "Win your first Lizard Spock game.", content.Load<Texture2D>("firstWinIcon"), 20, true));
            AchievementManager.AddAchievement(new Achievement("Lizard Luck", "Get a 5 wins using Lizard", content.Load<Texture2D>("lizard"), 30, true));
            AchievementManager.AddAchievement(new Achievement("Spocked!?", "Get a 5 wins using Spock", content.Load<Texture2D>("spock"), 30, true));



            // Load saved data
            LoadGameData();
        }

        public void SetVolume(float volume)
        {
            SoundEffectVolume = MathHelper.Clamp(volume, 0.0f, 1.0f); // Ensure volume is between 0 and 1

            SoundEffect.MasterVolume = SoundEffectVolume;
        }

        public void SaveGameData()
        {
            var data = new SaveData
            {
                Level = Level,
                XP = XP,
                WinStreak = WinStreak,
                OverallWins = OverallWins,
                OverallLoses = OverallLoses,
                OverallTies = OverallTies,
                UnlockedAchievements = new List<string>(),
                WinsWith = new Dictionary<Choice, int>(WinsWith),
                LSWinsWith = new Dictionary<LS_Choice, int>(LSWinsWith),
                ModeWins = new Dictionary<GameState, int>(ModeWins)
            };

            foreach (var achievement in AchievementManager.GetAchievements())
            {
                if (achievement.IsUnlocked)
                {
                    data.UnlockedAchievements.Add(achievement.Name);
                }
            }

            string json = System.Text.Json.JsonSerializer.Serialize(data);
            File.WriteAllText("savegame.json", json);
        }

        public void LoadGameData()
        {
            if (File.Exists("savegame.json"))
            {
                string json = File.ReadAllText("savegame.json");
                var data = System.Text.Json.JsonSerializer.Deserialize<SaveData>(json);

                Level = data.Level;
                XP = data.XP;
                WinStreak = data.WinStreak;
                OverallWins = data.OverallWins;
                OverallLoses = data.OverallLoses;
                OverallTies = data.OverallTies;
                WinsWith = new Dictionary<Choice, int>(data.WinsWith);
                LSWinsWith = new Dictionary<LS_Choice, int>(data.LSWinsWith);
                ModeWins = new Dictionary<GameState, int>(data.ModeWins);

                foreach (var achievementName in data.UnlockedAchievements)
                {
                    AchievementManager.UnlockAchievementSilently(achievementName); // Unlock achievements without triggering notification
                }
            }
        }

        // Reset message properties
        public bool ShowResetMessage { get; private set; } = false;
        public double ResetMessageTimer { get; private set; } = 0;
        private const double ResetMessageDuration = 3.0; // 3 seconds

        // Other methods...

        public void ResetSaveData()
        {
            if (File.Exists("savegame.json"))
            {
                File.Delete("savegame.json");
            }

            // Reset in-game variables
            Level = 1;
            XP = 0;
            WinStreak = 0;
            OverallWins = 0;
            OverallLoses = 0;
            OverallTies = 0;
            InitializeDictionaries();
            AchievementManager.ResetAchievements();



            // Set the reset message timer and flag
            ShowResetMessage = true;
            ResetMessageTimer = ResetMessageDuration;

            // Optionally, save the reset data
            SaveGameData();
        }

        public void UpdateResetMessageTimer(GameTime gameTime)
        {
            if (ShowResetMessage)
            {
                ResetMessageTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (ResetMessageTimer <= 0)
                {
                    ShowResetMessage = false;
                }
            }
        }


        private class SaveData
        {
            public int Level { get; set; }
            public int XP { get; set; }
            public int WinStreak { get; set; }
            public int OverallWins { get; set; }
            public int OverallLoses { get; set; }
            public int OverallTies { get; set; }
            public List<string> UnlockedAchievements { get; set; } = new List<string>();
            public Dictionary<Choice, int> WinsWith { get; set; } = new Dictionary<Choice, int>();
            public Dictionary<LS_Choice, int> LSWinsWith { get; set; } = new Dictionary<LS_Choice, int>();
            public Dictionary<GameState, int> ModeWins { get; set; } = new Dictionary<GameState, int>();

        }

        public void IncrementWinCount(Choice choice)
        {
            WinsWith[choice]++;
        }

        public void IncrementLSWinCount(LS_Choice choice)
        {
            if (LSWinsWith.ContainsKey(choice))
            {
                LSWinsWith[choice]++;
            }
        }

        public int TotalAchievementPages()
        {
            return (int)Math.Ceiling((double)AchievementManager.GetAchievements().Count / AchievementsPerPage);
        }

        public void GainXP(int amount)
        {
            XP += amount;
            if (XP >= XPNeeded)
            {
                XP -= XPNeeded;
                Level++;
            }
        }

    }
}
