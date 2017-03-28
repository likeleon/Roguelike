using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Roguelike
{
    public enum OrderType
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        Attack,
        Cancel
    }

    public static class Order
    {
        public static bool IsOrderIssued(OrderType orderType)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One);

            switch (orderType)
            {
                case OrderType.MoveLeft:
                    return keyboardState.IsKeyDown(Keys.Left) ||
                        keyboardState.IsKeyDown(Keys.A) ||
                        gamePadState.ThumbSticks.Left.X < -0.8f;

                case OrderType.MoveRight:
                    return keyboardState.IsKeyDown(Keys.Right) ||
                        keyboardState.IsKeyDown(Keys.D) ||
                        gamePadState.ThumbSticks.Left.X > 0.8f;

                case OrderType.MoveUp:
                    return keyboardState.IsKeyDown(Keys.Up) ||
                        keyboardState.IsKeyDown(Keys.W) ||
                        gamePadState.ThumbSticks.Left.Y > 0.8f;

                case OrderType.MoveDown:
                    return keyboardState.IsKeyDown(Keys.Down) ||
                        keyboardState.IsKeyDown(Keys.S) ||
                        gamePadState.ThumbSticks.Left.Y < -0.8f;

                case OrderType.Attack:
                    return keyboardState.IsKeyDown(Keys.Space) ||
                        mouseState.LeftButton == ButtonState.Pressed;

                case OrderType.Cancel:
                    return keyboardState.IsKeyDown(Keys.Escape) ||
                        gamePadState.Buttons.Back == ButtonState.Pressed;

                default:
                    return false;
            }
        }
    }
}
