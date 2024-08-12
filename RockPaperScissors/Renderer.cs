using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockPaperScissors.Managers;
using RockPaperScissors.Screens;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace RockPaperScissors
{
    public class Renderer
    {
        private SpriteBatch _spriteBatch;
        private GameStateManager _gameStateManager;
        private SettingsScreen _settingsScreen;
        private TitleScreen _titleScreen;
        private PlayingScreen _playingScreen;
        private ResultScreen _resultScreen;
        // Lizard Spock GameMode
        private LS_TitleScreen _ls_titleScreen;
        private LS_PlayingScreen _ls_playingScreen;
        private LS_ResultScreen _ls_resultScreen;

        // Misc
        private AchievementScreen _achievementScreen;

        public Renderer(SpriteBatch spriteBatch, GameStateManager gameStateManager, SettingsScreen settingsScreen)
        {
            _spriteBatch = spriteBatch;
            _gameStateManager = gameStateManager;
            _settingsScreen = settingsScreen;
            _titleScreen = new TitleScreen(this, spriteBatch, gameStateManager);
            _playingScreen = new PlayingScreen(this, _gameStateManager);
            _resultScreen = new ResultScreen(this, _gameStateManager, _spriteBatch);
            
            // Lizard Spock GameMode
            _ls_titleScreen = new LS_TitleScreen(this, spriteBatch, gameStateManager);
            _ls_playingScreen = new LS_PlayingScreen(this, _gameStateManager);
            _ls_resultScreen = new LS_ResultScreen(this, _gameStateManager, _spriteBatch);

            // Misc Screens
            _achievementScreen = new AchievementScreen(this, _gameStateManager, _spriteBatch);

        }

        public void Draw(GameTime gameTime)
        {
            if (_gameStateManager.IsTransitioning)
            {
                DrawTransitionScreen();
            }
            else
            {
                var renderActions = new Dictionary<GameState, Action>
                {
                    { GameState.Title, () => _titleScreen.Render() },
                    { GameState.LS_Title, () => _ls_titleScreen.Render() },
                    { GameState.Playing, () => _playingScreen.Render() },
                    { GameState.LS_Playing, () => _ls_playingScreen.Render() },
                    { GameState.Result, () => _resultScreen.Render() },
                    { GameState.LS_Result, () => _ls_resultScreen.Render() },
                    { GameState.Achievements, () => _achievementScreen.Render() },
                    { GameState.Settings, () => _settingsScreen.Draw(_spriteBatch) },
                    { GameState.C_Playing, () =>  _gameStateManager.CharacterGameLogic.Draw(_spriteBatch)},
                    { GameState.World, () =>
                    {

                            // Calculate depth based on Y position (higher Y means closer to the front)
                                Vector2 cameraPosition = new Vector2(-_gameStateManager._translation.Translation.X, -_gameStateManager._translation.Translation.Y);

                    float heroDepth = 1f - ((_gameStateManager._hero.Position.Y - cameraPosition.Y) /_gameStateManager._map.MapSize.Y);
                    // Draw background layers
                    _gameStateManager._map.DrawBackgroundLayers();

                    // Draw hero
                    _gameStateManager._hero.Draw(heroDepth);

                    // Draw foreground layers
                    _gameStateManager._map.DrawForegroundLayers();

                    // Interactable Depth Calculation
                    foreach (var interactable in _gameStateManager._interactableObjects)
                    {
                         float treeDepth = 1f - ((interactable.Position.Y - cameraPosition.Y) /_gameStateManager._map.MapSize.Y);
                        interactable.Draw(treeDepth);
                    }



                    } }
                };

                if (renderActions.TryGetValue(_gameStateManager.CurrentState, out var renderAction))
                {
                    renderAction();
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

        public void DrawResultScreenCommon()
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

        public void DrawTextures<T>(Dictionary<T, Texture2D> textures, Dictionary<string, Vector2> positions)
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

        public void DrawExitButton()
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

        public void DrawNextButton()
        {
            _gameStateManager.NextButtonRectangle = new Rectangle(
                (int)(_gameStateManager.NextButtonPosition.X - _gameStateManager.NextButtonTexture.Width / 2),
                (int)(_gameStateManager.NextButtonPosition.Y - _gameStateManager.NextButtonTexture.Height / 2),
                _gameStateManager.NextButtonTexture.Width,
                _gameStateManager.NextButtonTexture.Height
            );

            _spriteBatch.Draw(_gameStateManager.NextButtonTexture, _gameStateManager.NextButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.NextButtonTexture.Width / 2, _gameStateManager.NextButtonTexture.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public void DrawPrevButton()
        {
            _gameStateManager.PrevButtonRectangle = new Rectangle(
                (int)(_gameStateManager.PrevButtonPosition.X - _gameStateManager.PrevButtonTexture.Width / 2),
                (int)(_gameStateManager.PrevButtonPosition.Y - _gameStateManager.PrevButtonTexture.Height / 2),
                _gameStateManager.PrevButtonTexture.Width,
                _gameStateManager.PrevButtonTexture.Height
            );

            _spriteBatch.Draw(_gameStateManager.PrevButtonTexture, _gameStateManager.PrevButtonPosition, null, Color.White, 0f, new Vector2(_gameStateManager.PrevButtonTexture.Width / 2, _gameStateManager.PrevButtonTexture.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public void DrawPlayerInfo()
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
