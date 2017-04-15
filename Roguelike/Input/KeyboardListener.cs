using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Roguelike.Input
{
    public class KeyboardListener
    {
        private static readonly Keys[] AllKeys = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToArray();

        private KeyboardState _previousState;

        public event EventHandler<Keys> KeyPressed;
        public event EventHandler<Keys> KeyReleased;

        public void Update(GameTime gameTime)
        {
            var currentState = Keyboard.GetState();

            RaisePressedEvents(gameTime, currentState);
            RaiseReleasedEvents(currentState);

            _previousState = currentState;
        }

        private void RaisePressedEvents(GameTime gameTime, KeyboardState currentState)
        {
            if (currentState.IsKeyDown(Keys.LeftAlt) || currentState.IsKeyDown(Keys.RightAlt))
                return;

            var pressedKeys = AllKeys.Where(key => currentState.IsKeyDown(key) && _previousState.IsKeyUp(key));
            foreach (var key in pressedKeys)
                KeyPressed?.Invoke(this, key);
        }

        private void RaiseReleasedEvents(KeyboardState currentState)
        {
            var releasedKeys = AllKeys.Where(key => currentState.IsKeyUp(key) && _previousState.IsKeyDown(key));
            foreach (var key in releasedKeys)
                KeyReleased?.Invoke(this, key);
        }
    }
}
