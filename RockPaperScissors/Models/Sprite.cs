using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors.Models
{
    public class Sprite
    {
        public readonly Texture2D _texture;
        public Vector2 Position { get; protected set; }
        public Vector2 Origin { get; protected set; }
        public float Depth { get; set; }

        public Sprite(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
            Origin = new(_texture.Width / 2, _texture.Height / 2);
        }

        private readonly Rectangle _sourceRect;

        public Sprite(Texture2D texture, Vector2 position, Rectangle sourceRect)
        {
            _texture = texture;
            Position = position;
            _sourceRect = sourceRect;
            Origin = new(_sourceRect.Width / 2, _sourceRect.Height / 2);
        }

        public void Draw(float depth)
        {
            Globals.SpriteBatch.Draw(_texture, Position, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, depth);
        }
    }
}
