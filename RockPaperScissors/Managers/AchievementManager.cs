using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RockPaperScissors.Managers
{
    public class AchievementManager
    {
        private List<Achievement> achievements;
        private Queue<Achievement> achievementQueue;
        private Achievement currentAchievement;
        private SpriteFont font;
        private SoundEffect unlockSound;
        private Texture2D unlockBackground;
        private Vector2 unlockPosition;
        private bool isDisplayingUnlock;
        private double displayTimer;
        private bool isSlidingOut;
        private GameStateManager _gameStateManager; // Add reference to GameStateManager


        public AchievementManager(SpriteFont font, SoundEffect unlockSound, Texture2D unlockBackground, GameStateManager gameStateManager)
        {
            achievements = new List<Achievement>();
            achievementQueue = new Queue<Achievement>();
            this.font = font;
            this.unlockSound = unlockSound;
            this.unlockBackground = unlockBackground;
            unlockPosition = new Vector2(-300, 50); // Initial off-screen position
            isDisplayingUnlock = false;
            isSlidingOut = false;
            displayTimer = 0;
            _gameStateManager = gameStateManager; // Initialize GameStateManager

        }

        public void AddAchievement(Achievement achievement)
        {
            achievements.Add(achievement);
        }

        public void UnlockAchievement(string name)
        {
            var achievement = achievements.Find(a => a.Name == name);
            if (achievement != null && !achievement.IsUnlocked)
            {
                achievement.IsUnlocked = true;
                achievementQueue.Enqueue(achievement);
                _gameStateManager.GainXP(achievement.XPReward); // Grant XP reward
                ProcessNextAchievement();
            }
        }

        private void ProcessNextAchievement()
        {
            if (!isDisplayingUnlock && achievementQueue.Count > 0)
            {
                currentAchievement = achievementQueue.Dequeue();
                unlockSound.Play(); // Play the sound effect
                isDisplayingUnlock = true;
                isSlidingOut = false;
                displayTimer = 3.0; // Display for 3 seconds
            }
        }

        public void UnlockAchievementSilently(string name)
        {
            var achievement = achievements.Find(a => a.Name == name);
            if (achievement != null && !achievement.IsUnlocked)
            {
                achievement.IsUnlocked = true;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (isDisplayingUnlock)
            {
                displayTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (displayTimer <= 0 && !isSlidingOut)
                {
                    isSlidingOut = true;
                }
                else if (displayTimer <= -1.0) // 1 second for slide out
                {
                    isDisplayingUnlock = false;
                    isSlidingOut = false;
                    unlockPosition.X = -300; // Reset position
                    ProcessNextAchievement();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isDisplayingUnlock && currentAchievement != null)
            {
                if (!isSlidingOut)
                {
                    unlockPosition.X = MathHelper.Lerp(unlockPosition.X, 50, 0.1f); // Slide in animation
                }
                else
                {
                    unlockPosition.X = MathHelper.Lerp(unlockPosition.X, -300, 0.1f); // Slide out animation
                }

                spriteBatch.Draw(unlockBackground, unlockPosition, Color.White);
                spriteBatch.Draw(currentAchievement.Icon, new Vector2(unlockPosition.X + 10, unlockPosition.Y + 10), Color.White);
                spriteBatch.DrawString(font, currentAchievement.Name, new Vector2(unlockPosition.X + 70, unlockPosition.Y + 10), Color.Black);
                spriteBatch.DrawString(font, currentAchievement.Description, new Vector2(unlockPosition.X + 70, unlockPosition.Y + 40), Color.Black);
            }
        }

        public List<Achievement> GetAchievements()
        {
            return achievements;
        }

        public void ResetAchievements()
        {
            foreach (var achievement in achievements)
            {
                achievement.IsUnlocked = false;
            }
            achievementQueue.Clear();
        }
    }
}
