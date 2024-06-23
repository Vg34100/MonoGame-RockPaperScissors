using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors
{
    public class Achievement
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsUnlocked { get; set; }
        public bool IsHidden { get; set; }
        public Texture2D Icon { get; set; }
        public int XPReward { get; set; } // Add this line

        public Achievement(string name, string description, Texture2D icon, int xpReward, bool isHidden = false)
        {
            Name = name;
            Description = description;
            Icon = icon;
            IsUnlocked = false;
            IsHidden = isHidden;
            XPReward = xpReward; // Initialize XPReward
        }
    }

}
