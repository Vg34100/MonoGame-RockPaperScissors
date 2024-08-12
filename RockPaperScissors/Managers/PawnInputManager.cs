using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RockPaperScissors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors.Managers
{
    public static class PawnInputManager
    {
        private static Vector2 _direction;
        public static Vector2 Direction => _direction;

        public static void Update(GameStateManager gameStateManager)
        {
            var keyboardState = Keyboard.GetState();

            _direction = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.W)) _direction.Y--;
            if (keyboardState.IsKeyDown(Keys.S)) _direction.Y++;
            if (keyboardState.IsKeyDown(Keys.A)) _direction.X--;
            if (keyboardState.IsKeyDown(Keys.D)) _direction.X++;
            if (keyboardState.IsKeyDown(Keys.E)) gameStateManager._hero.TryInteract(gameStateManager._interactableObjects);



            if (_direction != Vector2.Zero)
            {
                _direction.Normalize();
            }
        }
    }
}
