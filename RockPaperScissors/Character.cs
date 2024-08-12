using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors
{
    public class Character
    {
        // Basic character information
        public string Name { get; set; }
        public Texture2D Portrait { get; set; }

        // Probabilities for choosing each option in RPS and RPSLS
        private Dictionary<Choice, float> _rpsProbabilities;
        private Dictionary<LS_Choice, float> _rpslsProbabilities;

        // Dialogue options
        private List<string> _randomDialogue;
        private Queue<string> _orderedDialogue;
        private Queue<string> _originalOrderedDialogue;
        private Dictionary<string, List<string>> _conditionalDialogue;

        // Character's current mood (can affect dialogue and choices)
        public enum Mood { Neutral, Happy, Angry, Excited, Sad }
        public Mood CurrentMood { get; private set; }

        // Character's experience and level
        public int Experience { get; private set; }
        public int Level { get; private set; }

        // Random number generator for probabilistic choices
        private Random _random;

        public Character(string name, Texture2D portrait)
        {
            Name = name;
            Portrait = portrait;
            _random = new Random();

            // Initialize probabilities with equal distribution
            InitializeProbabilities();

            // Initialize dialogue lists
            _randomDialogue = new List<string>();
            _orderedDialogue = new Queue<string>();
            _originalOrderedDialogue = new Queue<string>();
            _conditionalDialogue = new Dictionary<string, List<string>>();

            CurrentMood = Mood.Neutral;
            Experience = 0;
            Level = 1;
        }

        private void InitializeProbabilities()
        {
            _rpsProbabilities = new Dictionary<Choice, float>
            {
                { Choice.Rock, 1f / 3 },
                { Choice.Paper, 1f / 3 },
                { Choice.Scissors, 1f / 3 }
            };

            _rpslsProbabilities = new Dictionary<LS_Choice, float>
            {
                { LS_Choice.Rock, 1f / 5 },
                { LS_Choice.Paper, 1f / 5 },
                { LS_Choice.Scissors, 1f / 5 },
                { LS_Choice.Lizard, 1f / 5 },
                { LS_Choice.Spock, 1f / 5 }
            };
        }

        // Method to add random dialogue
        public void AddRandomDialogue(string dialogue)
        {
            _randomDialogue.Add(dialogue);
        }

        // Method to add ordered dialogue
        public void AddOrderedDialogue(string dialogue)
        {
            _orderedDialogue.Enqueue(dialogue);
            _originalOrderedDialogue.Enqueue(dialogue);
        }

        // Method to add conditional dialogue
        public void AddConditionalDialogue(string condition, string dialogue)
        {
            if (!_conditionalDialogue.ContainsKey(condition))
            {
                _conditionalDialogue[condition] = new List<string>();
            }
            _conditionalDialogue[condition].Add(dialogue);
        }

        // Method to get next dialogue line
        public string GetNextDialogue(string condition = null)
        {
            // First, check for conditional dialogue
            if (condition != null && _conditionalDialogue.ContainsKey(condition) && _conditionalDialogue[condition].Count > 0)
            {
                int index = _random.Next(_conditionalDialogue[condition].Count);
                return _conditionalDialogue[condition][index];
            }

            // Then, check for ordered dialogue
            if (_orderedDialogue.Count > 0)
            {
                return _orderedDialogue.Dequeue();
            }

            // Finally, fall back to random dialogue
            if (_randomDialogue.Count > 0)
            {
                return _randomDialogue[_random.Next(_randomDialogue.Count)];
            }

            // If no dialogue is available, return a default message
            return "...";
        }

        // Method to make a choice in RPS
        public Choice MakeRPSChoice()
        {
            float randomValue = (float)_random.NextDouble();
            float cumulativeProbability = 0f;

            foreach (var kvp in _rpsProbabilities)
            {
                cumulativeProbability += kvp.Value;
                if (randomValue <= cumulativeProbability)
                {
                    return kvp.Key;
                }
            }

            // This should never happen if probabilities sum to 1, but just in case
            return Choice.Rock;
        }

        // Method to make a choice in RPSLS
        public LS_Choice MakeRPSLSChoice()
        {
            float randomValue = (float)_random.NextDouble();
            float cumulativeProbability = 0f;

            foreach (var kvp in _rpslsProbabilities)
            {
                cumulativeProbability += kvp.Value;
                if (randomValue <= cumulativeProbability)
                {
                    return kvp.Key;
                }
            }

            // This should never happen if probabilities sum to 1, but just in case
            return LS_Choice.Rock;
        }

        // Method to adjust probabilities based on player's choice
        public void AdjustProbabilities(Choice playerChoice, float adjustmentFactor = 0.1f)
        {
            foreach (var choice in _rpsProbabilities.Keys)
            {
                if (choice == playerChoice)
                {
                    _rpsProbabilities[choice] += adjustmentFactor;
                }
                else
                {
                    _rpsProbabilities[choice] -= adjustmentFactor / 2;
                }
            }

            NormalizeProbabilities(_rpsProbabilities);
        }

        // Method to adjust RPSLS probabilities
        public void AdjustRPSLSProbabilities(LS_Choice playerChoice, float adjustmentFactor = 0.1f)
        {
            foreach (var choice in _rpslsProbabilities.Keys)
            {
                if (choice == playerChoice)
                {
                    _rpslsProbabilities[choice] += adjustmentFactor;
                }
                else
                {
                    _rpslsProbabilities[choice] -= adjustmentFactor / 4;
                }
            }

            NormalizeProbabilities(_rpslsProbabilities);
        }

        // Helper method to normalize probabilities
        private void NormalizeProbabilities<T>(Dictionary<T, float> probabilities)
        {
            float sum = 0f;
            foreach (var prob in probabilities.Values)
            {
                sum += prob;
            }

            foreach (var key in probabilities.Keys.ToList())
            {
                probabilities[key] /= sum;
            }
        }

        // Method to change the character's mood
        public void ChangeMood(Mood newMood)
        {
            CurrentMood = newMood;
            // You could add logic here to adjust probabilities or trigger specific dialogue based on mood
        }

        // Method to gain experience and potentially level up
        public void GainExperience(int amount)
        {
            Experience += amount;
            while (Experience >= 100 * Level)
            {
                Experience -= 100 * Level;
                Level++;
                // You could add special effects or dialogue for leveling up
            }
        }

        // Method to draw the character's portrait
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Portrait, position, Color.White);
        }

        public void ResetOrderedDialogue()
        {
            _orderedDialogue = new Queue<string>(_originalOrderedDialogue);
        }
    }

}
