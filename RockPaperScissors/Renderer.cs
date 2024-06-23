using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

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
            if (_gameStateManager.IsTransitioning)
            {
                DrawTransitionScreen();
            }
            else
            {
                if (_gameStateManager.CurrentState == GameState.Title)
                {
                    DrawTitleScreen();
                }
                else if (_gameStateManager.CurrentState == GameState.LS_Title)
                {
                    DrawLSTitleScreen();
                }
                else if (_gameStateManager.CurrentState == GameState.Playing)
                {
                    DrawPlayingScreen();
                }
                else if (_gameStateManager.CurrentState == GameState.LS_Playing)
                {
                    DrawLSPlayingScreen();
                }
                else if (_gameStateManager.CurrentState == GameState.Result)
                {
                    DrawResultScreen();
                }
                else if (_gameStateManager.CurrentState == GameState.LS_Result)
                {
                    DrawLSResultScreen();
                }
                else if (_gameStateManager.CurrentState == GameState.Achievements)
                {
                    DrawAchievementsScreen();
                }
            }

            DrawResetMessage(gameTime);
        }


        private void DrawResetMessage(GameTime gameTime)
        {
            if (_gameStateManager.ShowResetMessage)
            {
                Vector2 messagePosition = new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width / 2, 1.9f * _spriteBatch.GraphicsDevice.Viewport.Height / 2);
                string resetMessage = "Save data has been reset!";
                Vector2 textSize = _gameStateManager.Font.MeasureString(resetMessage);
                _spriteBatch.DrawString(_gameStateManager.Font, resetMessage, messagePosition - textSize / 2, Color.Black);
                _gameStateManager.UpdateResetMessageTimer(gameTime); // Update the timer
            }
        }

        private void DrawTransitionScreen()
        {
            float offset = _gameStateManager.TransitionProgress * _spriteBatch.GraphicsDevice.Viewport.Width;

            var positionsTitle = _gameStateManager.Positions[GameState.Title];
            var positionsLSTitle = _gameStateManager.Positions[GameState.LS_Title];

            if (_gameStateManager.CurrentState == GameState.Title)
            {
                // Transition from Title to LS_Title
                DrawTransitionItems(positionsTitle, -offset, true);
                DrawTransitionItems(positionsLSTitle, _spriteBatch.GraphicsDevice.Viewport.Width - offset, false);
            }
            else
            {
                // Transition from LS_Title to Title
                DrawTransitionItems(positionsLSTitle, offset, false);
                DrawTransitionItems(positionsTitle, -_spriteBatch.GraphicsDevice.Viewport.Width + offset, true);
            }
        }


        private void DrawTransitionItems(Dictionary<string, Vector2> positions, float offset, bool isTitle)
        {
            foreach (var kvp in positions)
            {
                Texture2D texture = null;

                if (Enum.TryParse(kvp.Key, out Choice choice) && _gameStateManager.Textures.TryGetValue(choice, out var choiceTexture))
                {
                    texture = choiceTexture;
                }
                else if (Enum.TryParse(kvp.Key, out LS_Choice lsChoice) && _gameStateManager.LSTextures.TryGetValue(lsChoice, out var lsChoiceTexture))
                {
                    texture = lsChoiceTexture;
                }
                else if (kvp.Key == "Title" && isTitle)
                {
                    texture = _gameStateManager.TitleTexture;
                }
                else if (kvp.Key == "LSTitle" && !isTitle)
                {
                    texture = _gameStateManager.LSTitleTexture;
                }

                if (texture != null)
                {
                    var position = kvp.Value;
                    position.X += offset;
                    _spriteBatch.Draw(texture, position, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
                }
            }
        }




        private void DrawAchievementsScreen()
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

                // Vector2 textSize = _gameStateManager.Font.MeasureString("Press ENTER to Start...");
                // _spriteBatch.DrawString(_gameStateManager.Font, "Press ENTER to Start...", _gameStateManager.StartButtonPosition - textSize / 2, Color.Black);

                // _spriteBatch.Draw(computerTexture, computerPosition, null, Color.Gray, 0f, new Vector2(computerTexture.Width / 2, computerTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);

                if (achievement.IsUnlocked)
                {
                    _spriteBatch.Draw(achievement.Icon, position, null, Color.White, 0f, new Vector2(achievement.Icon.Width/2, achievement.Icon.Height/2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);
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
                    _spriteBatch.DrawString(_gameStateManager.Font, "???", (position + new Vector2(0,50)) - _gameStateManager.Font.MeasureString("???") / 2, Color.Black);

                }

                if (achievement.IsUnlocked)
                {
                    _spriteBatch.DrawString(_gameStateManager.Font, achievement.Description, (position + new Vector2(0, 70)) - _gameStateManager.Font.MeasureString(achievement.Description) / 2, Color.Black);
                }

            }

            // Draw navigation buttons if needed
            DrawNavigationButtons();

            DrawExitButton();

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

        private void DrawTitleScreen()
        {
            var positions = _gameStateManager.Positions[GameState.Title];

            _spriteBatch.Draw(_gameStateManager.TitleTexture, positions["Title"], null, Color.White, 0f, new Vector2(_gameStateManager.TitleTexture.Width / 2, _gameStateManager.TitleTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);

            DrawTextures(_gameStateManager.Textures, positions);

            // _spriteBatch.Draw(_gameStateManager.StartButtonTexture, _gameStateManager.StartButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.StartButtonTexture.Width / 2, _gameStateManager.StartButtonTexture.Height / 2), _gameStateManager.ButtonScale, SpriteEffects.None, 0f);

            Vector2 textSize = _gameStateManager.Font.MeasureString("Press ENTER to Start...");
            _spriteBatch.DrawString(_gameStateManager.Font, "Press ENTER to Start...", _gameStateManager.StartButtonPosition - textSize / 2, Color.Black);

            if (_gameStateManager.Level >= 2)
            {
                DrawNextButton();
            }

            DrawPlayerInfo();
        }

        private void DrawLSTitleScreen()
        {
            var positions = _gameStateManager.Positions[GameState.LS_Title];

            _spriteBatch.Draw(_gameStateManager.LSTitleTexture, positions["LSTitle"], null, Color.White, 0f, new Vector2(_gameStateManager.LSTitleTexture.Width / 2, _gameStateManager.LSTitleTexture.Height / 2), _gameStateManager.NormalScale, SpriteEffects.None, 0f);

            DrawTextures(_gameStateManager.LSTextures, positions);

            // _spriteBatch.Draw(_gameStateManager.StartButtonTexture, _gameStateManager.StartButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.StartButtonTexture.Width / 2, _gameStateManager.StartButtonTexture.Height / 2), _gameStateManager.ButtonScale, SpriteEffects.None, 0f);

            Vector2 textSize = _gameStateManager.Font.MeasureString("Press ENTER to Start...");
            _spriteBatch.DrawString(_gameStateManager.Font, "Press ENTER to Start...", _gameStateManager.StartButtonPosition - textSize / 2, Color.Black);

            if (_gameStateManager.Level >= 2)
            {
                DrawPrevButton();
            }

            DrawPlayerInfo();
        }

        private void DrawPlayingScreen()
        {
            var positions = _gameStateManager.Positions[GameState.Playing];
            DrawTextures(_gameStateManager.Textures, positions);
            DrawPlayerInfo();
            DrawExitButton();
        }

        private void DrawLSPlayingScreen()
        {
            var positions = _gameStateManager.Positions[GameState.LS_Playing];
            DrawTextures(_gameStateManager.LSTextures, positions);
            DrawPlayerInfo();
            DrawExitButton();
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
            DrawExitButton();
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

        private void DrawExitButton()
        {
            _gameStateManager.ExitButtonRectangle = new Rectangle(
                (int)(_gameStateManager.ExitButtonPosition.X - _gameStateManager.ExitButtonTexture.Width / 2),
                (int)(_gameStateManager.ExitButtonPosition.Y - _gameStateManager.ExitButtonTexture.Height / 2),
                _gameStateManager.ExitButtonTexture.Width,
                _gameStateManager.ExitButtonTexture.Height
            );

            _spriteBatch.Draw(_gameStateManager.ExitButtonTexture, _gameStateManager.ExitButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.ExitButtonTexture.Width / 2, _gameStateManager.ExitButtonTexture.Height / 2), 1f, SpriteEffects.None, 0f);

            Vector2 textSize = _gameStateManager.Font.MeasureString("Exit");
            _spriteBatch.DrawString(_gameStateManager.Font, "Exit", _gameStateManager.ExitButtonPosition - textSize / 2, Color.Black);
        }

        private void DrawNextButton()
        {
            _gameStateManager.NextButtonRectangle = new Rectangle(
                (int)(_gameStateManager.NextButtonPosition.X - _gameStateManager.NextButtonTexture.Width / 2),
                (int)(_gameStateManager.NextButtonPosition.Y - _gameStateManager.NextButtonTexture.Height / 2),
                _gameStateManager.NextButtonTexture.Width,
                _gameStateManager.NextButtonTexture.Height
            );

            _spriteBatch.Draw(_gameStateManager.NextButtonTexture, _gameStateManager.NextButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.NextButtonTexture.Width / 2, _gameStateManager.NextButtonTexture.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        private void DrawPrevButton()
        {
            _gameStateManager.PrevButtonRectangle = new Rectangle(
                (int)(_gameStateManager.PrevButtonPosition.X - _gameStateManager.PrevButtonTexture.Width / 2),
                (int)(_gameStateManager.PrevButtonPosition.Y - _gameStateManager.PrevButtonTexture.Height / 2),
                _gameStateManager.PrevButtonTexture.Width,
                _gameStateManager.PrevButtonTexture.Height
            );

            _spriteBatch.Draw(_gameStateManager.PrevButtonTexture, _gameStateManager.PrevButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.PrevButtonTexture.Width / 2, _gameStateManager.PrevButtonTexture.Height / 2), 1f, SpriteEffects.None, 0f);
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
