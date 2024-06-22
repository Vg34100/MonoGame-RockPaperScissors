using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace RockPaperScissors
{
    public class GameStateManager
    {
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

        public Texture2D RockTexture { get; private set; }
        public Texture2D PaperTexture { get; private set; }
        public Texture2D ScissorsTexture { get; private set; }
        public Texture2D LizardTexture { get; private set; }
        public Texture2D SpockTexture { get; private set; }

        public Texture2D TitleTexture { get; private set; }
        public Texture2D LSTitleTexture { get; private set; }


        public Texture2D StartButtonTexture { get; private set; }
        public SpriteFont Font { get; private set; }

        // Level System
        public int Level { get; set; } = 1;
        public int XP { get; set; } = 0;
        public int XPNeeded => Level * 100; // XP needed to level up
        public int WinStreak { get; set; } = 0;


        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            RockTexture = content.Load<Texture2D>("rock");
            PaperTexture = content.Load<Texture2D>("paper");
            ScissorsTexture = content.Load<Texture2D>("scissors");
            LizardTexture = content.Load<Texture2D>("lizard");
            SpockTexture = content.Load<Texture2D>("spock");
            TitleTexture = content.Load<Texture2D>("RPS-logo");
            LSTitleTexture = content.Load<Texture2D>("RPSLS-logo");


            StartButtonTexture = content.Load<Texture2D>("startButton");
            Font = content.Load<SpriteFont>("File");

            int windowWidth = graphicsDevice.Viewport.Width;
            int windowHeight = graphicsDevice.Viewport.Height;

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
                        { "Rock", new Vector2(4 * windowWidth / 16, windowHeight / 3) },
                        { "Paper", new Vector2(7 * windowWidth / 16, windowHeight / 3) },
                        { "Scissors", new Vector2(9 * windowWidth / 16, windowHeight / 3) },
                        { "Lizard", new Vector2(11 * windowWidth / 16, windowHeight / 3) },
                        { "Spock", new Vector2(13 * windowWidth / 16, windowHeight / 3) }
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

            // Load saved data
            LoadGameData();
        }

        public void SaveGameData()
        {
            var data = new SaveData
            {
                Level = this.Level,
                XP = this.XP,
                WinStreak = this.WinStreak
            };

            string json = System.Text.Json.JsonSerializer.Serialize(data);
            File.WriteAllText("savegame.json", json);
        }

        public void LoadGameData()
        {
            if (File.Exists("savegame.json"))
            {
                string json = File.ReadAllText("savegame.json");
                var data = System.Text.Json.JsonSerializer.Deserialize<SaveData>(json);

                this.Level = data.Level;
                this.XP = data.XP;
                this.WinStreak = data.WinStreak;
            }
        }

        private class SaveData
        {
            public int Level { get; set; }
            public int XP { get; set; }
            public int WinStreak { get; set; }
        }

    }
}
