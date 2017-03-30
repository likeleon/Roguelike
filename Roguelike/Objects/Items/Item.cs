using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Utils;

namespace Roguelike.Objects
{
    public abstract class Item : Object
    {
        private SpriteFont _font;
        private Vector2 _textOffset;

        public string Name { get; private set; }
        public ItemType ItemType { get; }

        protected Item(ItemType itemType)
        {
            ItemType = itemType;
        }

        protected void SetName(string name, SpriteFont font)
        {
            Name = name;

            _font = font;
            _textOffset = font.MeasureString(name) / 2.0f + new Vector2(0.0f, 30.0f);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            if (!Name.IsNullOrEmpty())
                spriteBatch.DrawString(_font, Name, Position - _textOffset, Color.White);
        }
    }
}
