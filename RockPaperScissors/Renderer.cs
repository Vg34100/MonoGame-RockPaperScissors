using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RockPaperScissors
{
    public class Renderer
    {
        private SpriteBatch _spriteBatch;
        private GameStateManager _gameStateManager;

        public Renderer(SpriteBatch spriteBatch, GameStateManager gameStateManager)
        {
            _spriteBatch = spriteBatch;
            _gameStateManager = gameStateManager;
        }

        public void Draw(GameTime gameTime)
        {
            if (_gameStateManager.CurrentState == GameState.Title)
            {
                DrawTitleScreen();
            }
            else if (_gameStateManager.CurrentState == GameState.Playing)
            {
                DrawPlayingScreen();
            }
            else if (_gameStateManager.CurrentState == GameState.Result)
            {
                DrawResultScreen();
            }
            else if (_gameStateManager.CurrentState == GameState.LS_Title)
            {
                DrawLSTitleScreen();
            }
            else if (_gameStateManager.CurrentState == GameState.LS_Playing)
            {
                DrawLSPlayingScreen();
            }
            else if (_gameStateManager.CurrentState == GameState.LS_Result)
            {
                DrawLSResultScreen();
            }
        }

        private void DrawTitleScreen()
        {
            var positions = _gameStateManager.Positions[GameState.Title];

            _spriteBatch.Draw(_gameStateManager.TitleTexture, positions["Title"], null, Color.White, 0f, new Vector2(_gameStateManager.TitleTexture.Width / 2, _gameStateManager.TitleTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);

            DrawTextures(_gameStateManager.Textures, positions);

            _spriteBatch.Draw(_gameStateManager.StartButtonTexture, _gameStateManager.StartButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.StartButtonTexture.Width / 2, _gameStateManager.StartButtonTexture.Height / 2), _gameStateManager.ButtonScale, SpriteEffects.None, 0f);

            Vector2 textSize = _gameStateManager.Font.MeasureString("Start");
            _spriteBatch.DrawString(_gameStateManager.Font, "Start", _gameStateManager.StartButtonPosition - textSize / 2, Color.Black);

            DrawPlayerInfo();
        }

        private void DrawLSTitleScreen()
        {
            var positions = _gameStateManager.Positions[GameState.LS_Title];

            _spriteBatch.Draw(_gameStateManager.LSTitleTexture, positions["LSTitle"], null, Color.White, 0f, new Vector2(_gameStateManager.LSTitleTexture.Width / 2, _gameStateManager.LSTitleTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);

            DrawTextures(_gameStateManager.LSTextures, positions);

            _spriteBatch.Draw(_gameStateManager.StartButtonTexture, _gameStateManager.StartButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.StartButtonTexture.Width / 2, _gameStateManager.StartButtonTexture.Height / 2), _gameStateManager.ButtonScale, SpriteEffects.None, 0f);

            Vector2 textSize = _gameStateManager.Font.MeasureString("Start");
            _spriteBatch.DrawString(_gameStateManager.Font, "Start", _gameStateManager.StartButtonPosition - textSize / 2, Color.Black);

            DrawPlayerInfo();
        }

        private void DrawPlayingScreen()
        {
            var positions = _gameStateManager.Positions[GameState.Playing];
            DrawTextures(_gameStateManager.Textures, positions);
            DrawPlayerInfo();
        }

        private void DrawLSPlayingScreen()
        {
            var positions = _gameStateManager.Positions[GameState.LS_Playing];
            DrawTextures(_gameStateManager.LSTextures, positions);
            DrawPlayerInfo();
        }

        private void DrawResultScreen()
        {
            var positions = _gameStateManager.Positions[GameState.Playing];
            Vector2 computerPosition = new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width / 2, _spriteBatch.GraphicsDevice.Viewport.Height / 2 + 100);

            if (_gameStateManager.Textures.TryGetValue(_gameStateManager.PlayerChoice, out Texture2D playerTexture))
            {
                _spriteBatch.Draw(playerTexture, positions[_gameStateManager.PlayerChoice.ToString()], null, Color.White, 0f, new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
            }

            if (_gameStateManager.Textures.TryGetValue(_gameStateManager.ComputerChoice, out Texture2D computerTexture))
            {
                _spriteBatch.Draw(computerTexture, computerPosition, null, Color.Gray, 0f, new Vector2(computerTexture.Width / 2, computerTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
            }

            DrawResultScreenCommon();
        }

        private void DrawLSResultScreen()
        {
            var positions = _gameStateManager.Positions[GameState.LS_Playing];
            Vector2 computerPosition = new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width / 2, _spriteBatch.GraphicsDevice.Viewport.Height / 2 + 100);

            if (_gameStateManager.LSTextures.TryGetValue(_gameStateManager.PlayerLSChoice, out Texture2D playerTexture))
            {
                _spriteBatch.Draw(playerTexture, positions[_gameStateManager.PlayerLSChoice.ToString()], null, Color.White, 0f, new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
            }

            if (_gameStateManager.LSTextures.TryGetValue(_gameStateManager.ComputerLSChoice, out Texture2D computerTexture))
            {
                _spriteBatch.Draw(computerTexture, computerPosition, null, Color.Gray, 0f, new Vector2(computerTexture.Width / 2, computerTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
            }

            DrawResultScreenCommon();
        }

        private void DrawResultScreenCommon()
        {
            Vector2 resultPosition = new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width / 2, _spriteBatch.GraphicsDevice.Viewport.Height / 2);
            Vector2 resultSize = _gameStateManager.Font.MeasureString(_gameStateManager.ResultMessage);
            _spriteBatch.DrawString(_gameStateManager.Font, _gameStateManager.ResultMessage, resultPosition - resultSize / 2, Color.Black);

            _spriteBatch.Draw(_gameStateManager.StartButtonTexture, _gameStateManager.StartButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.StartButtonTexture.Width / 2, _gameStateManager.StartButtonTexture.Height / 2), _gameStateManager.ButtonScale, SpriteEffects.None, 0f);

            Vector2 textSize = _gameStateManager.Font.MeasureString("Play Again");
            _spriteBatch.DrawString(_gameStateManager.Font, "Play Again", _gameStateManager.StartButtonPosition - textSize / 2, Color.Black);

            DrawPlayerInfo();
        }

        private void DrawTextures<T>(Dictionary<T, Texture2D> textures, Dictionary<string, Vector2> positions)
        {
            foreach (var kvp in textures)
            {
                string key = kvp.Key.ToString();
                if (positions.TryGetValue(key, out Vector2 position))
                {
                    _spriteBatch.Draw(kvp.Value, position, null, Color.White, 0f, new Vector2(kvp.Value.Width / 2, kvp.Value.Height / 2), GetScaleForChoice(kvp.Key), SpriteEffects.None, 0f);
                }
            }
        }

        private float GetScaleForChoice<T>(T choice)
        {
            if ((choice.Equals(Choice.Rock) || choice.Equals(LS_Choice.Rock)) && _gameStateManager.IsHoveringRock) return _gameStateManager.HoverScale;
            if ((choice.Equals(Choice.Paper) || choice.Equals(LS_Choice.Paper)) && _gameStateManager.IsHoveringPaper) return _gameStateManager.HoverScale;
            if ((choice.Equals(Choice.Scissors) || choice.Equals(LS_Choice.Scissors)) && _gameStateManager.IsHoveringScissors) return _gameStateManager.HoverScale;
            if (choice.Equals(LS_Choice.Lizard) && _gameStateManager.IsHoveringLizard) return _gameStateManager.HoverScale;
            if (choice.Equals(LS_Choice.Spock) && _gameStateManager.IsHoveringSpock) return _gameStateManager.HoverScale;
            return _gameStateManager.NormalScale;
        }

        private void DrawPlayerInfo()
        {
            string levelText = $"Level: {_gameStateManager.Level}";
            string xpText = $"XP: {_gameStateManager.XP}/{_gameStateManager.XPNeeded}";
            string winStreakText = $"Win Streak: {_gameStateManager.WinStreak}";

            Vector2 levelPosition = new Vector2(10, 10);
            Vector2 xpPosition = new Vector2(10, 40);
            Vector2 winStreakPosition = new Vector2(10, 70);

            _spriteBatch.DrawString(_gameStateManager.Font, levelText, levelPosition, Color.Black);
            _spriteBatch.DrawString(_gameStateManager.Font, xpText, xpPosition, Color.Black);
            _spriteBatch.DrawString(_gameStateManager.Font, winStreakText, winStreakPosition, Color.Black);
        }
    }
}
