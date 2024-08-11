using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockPaperScissors.Managers;
using System;

namespace RockPaperScissors.Screens
{
    public class AchievementScreen
    {
        private SpriteBatch _spriteBatch;
        private GameStateManager _gameStateManager;
        private Renderer _renderer;
        private InputHandler _inputHandler;



        public AchievementScreen(Renderer renderer, GameStateManager gameStateManager, SpriteBatch spriteBatch)
        {
            _renderer = renderer;
            _gameStateManager = gameStateManager;
            _spriteBatch = spriteBatch;
        }

        public AchievementScreen(InputHandler inputHandler, GameStateManager gameStateManager)
        {
            _inputHandler = inputHandler;
            _gameStateManager = gameStateManager;
        }

        public void HandleInput(KeyboardState currentKeyboardState, KeyboardState _previousKeyboardState, MouseState currentMouseState, MouseState _previousMouseState)
        {
            if (currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                _gameStateManager.CurrentAchievementPage = (_gameStateManager.CurrentAchievementPage + 1) % _gameStateManager.TotalAchievementPages();
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                _gameStateManager.CurrentAchievementPage = (_gameStateManager.CurrentAchievementPage - 1 + _gameStateManager.TotalAchievementPages()) % _gameStateManager.TotalAchievementPages();
            }


            // Check for exit to title screen
            if (currentKeyboardState.IsKeyDown(Keys.Back))
            {
                _gameStateManager.CurrentState = GameState.Title;
            }


            _inputHandler.CheckForExitButtonClick(currentMouseState, GameState.Title, currentKeyboardState);
        }

        public void Render()
        {
            var achievements = _gameStateManager.AchievementManager.GetAchievements();
            int startIndex = _gameStateManager.CurrentAchievementPage * _gameStateManager.AchievementsPerPage;
            int endIndex = Math.Min(startIndex + _gameStateManager.AchievementsPerPage, achievements.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                var achievement = achievements[i];
                int row = (i - startIndex) / 4;
                int col = (i - startIndex) % 4;
                Vector2 position = new Vector2(100 + col * 200, 100 + row * 200);

                if (achievement.IsUnlocked)
                {
                    _spriteBatch.Draw(achievement.Icon, position, null, Color.White, 0f, new Vector2(achievement.Icon.Width / 2, achievement.Icon.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
                }
                else if (!achievement.IsUnlocked)
                {
                    _spriteBatch.Draw(_gameStateManager.UnknownTexture, position, null, Color.White, 0f, new Vector2(_gameStateManager.UnknownTexture.Width / 2, _gameStateManager.UnknownTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);

                }

                if (!achievement.IsHidden || achievement.IsUnlocked)
                {
                    _spriteBatch.DrawString(_gameStateManager.Font, achievement.Name, (position + new Vector2(0, 50)) - _gameStateManager.Font.MeasureString(achievement.Name) / 2, Color.Black);
                }
                else if (achievement.IsHidden)
                {
                    _spriteBatch.DrawString(_gameStateManager.Font, "???", (position + new Vector2(0, 50)) - _gameStateManager.Font.MeasureString("???") / 2, Color.Black);

                }

                if (achievement.IsUnlocked)
                {
                    _spriteBatch.DrawString(_gameStateManager.Font, achievement.Description, (position + new Vector2(0, 70)) - _gameStateManager.Font.MeasureString(achievement.Description) / 2, Color.Black);
                }

            }

            // Draw navigation buttons if needed
            DrawNavigationButtons();

            _renderer.DrawExitButton();
        }
        private void DrawNavigationButtons()
        {
            int totalPages = _gameStateManager.TotalAchievementPages();
            if (totalPages > 1)
            {
                if (_gameStateManager.CurrentAchievementPage > 0)
                {
                    // Draw left navigation button
                    _spriteBatch.Draw(_gameStateManager.PrevButtonTexture, _gameStateManager.PrevButtonPosition, Color.White);
                }

                if (_gameStateManager.CurrentAchievementPage < totalPages - 1)
                {
                    // Draw right navigation button
                    _spriteBatch.Draw(_gameStateManager.NextButtonTexture, _gameStateManager.NextButtonPosition, Color.White);
                }
            }
        }
    }
}
