using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockPaperScissors.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors.Models
{
    public class Hero : Sprite
    {

        private const float SPEED = 500;
        private Vector2 _minPos, _maxPos;
        private Map _map;

        public Hero(Texture2D texture, Vector2 position, Map map) : base(texture, position)
        {
            _map = map;
        }


        public void SetBounds(Point mapSize, Point tileSize)
        {
            _minPos = new Vector2((-tileSize.X / 2) + Origin.X, (-tileSize.Y / 2) + Origin.Y);
            _maxPos = new Vector2(mapSize.X - (tileSize.X / 2) - Origin.X, mapSize.Y - (tileSize.Y / 2) - Origin.Y);
        }

        public void Update()
        {
            Vector2 newPosition = Position + PawnInputManager.Direction * Globals.Time * SPEED;

            // Check for collision at the new position
            if (!_map.CheckCollision(newPosition))
            {
                Position = newPosition;
            }

            Position = Vector2.Clamp(Position, _minPos, _maxPos);
        }

        public void TryInteract(List<InteractableObject> interactableObjects)
        {
            foreach (var interactableObject in interactableObjects)
            {
                if (interactableObject.IsHeroCloseEnough(this))
                {
                    interactableObject.Interact();
                    break; // Stop after the first successful interaction
                }
            }
        }

    }

}
