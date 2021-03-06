﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Torch : Object
    {
        public float Brightness { get; set; } = 1.0f;

        public Torch()
        {
            SetSprite(Global.Content.Load<Texture2D>("spr_torch"), frames: 5, frameSpeed: 12);
        }

        public override void Update(GameTime gameTime)
        {
            Brightness = Rand.Next(80, 121) / 100.0f;
        }
    }
}
