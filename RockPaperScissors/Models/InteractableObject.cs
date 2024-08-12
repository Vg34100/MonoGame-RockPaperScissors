using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RockPaperScissors.Models
{
    public class InteractableObject : Sprite
    {
        public Action InteractAction { get; set; }

        public InteractableObject(Texture2D texture, Vector2 position, Action interactAction) : base(texture, position)
        {
            InteractAction = interactAction;
        }

        public bool IsHeroCloseEnough(Hero hero)
        {
            float distance = Vector2.Distance(hero.Position, this.Position);
            return distance < 50; // Interaction distance
        }

        public void Interact()
        {
            InteractAction?.Invoke();
        }
    }
}
