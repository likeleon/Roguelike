using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Roguelike.Sounds
{
    public sealed class AmbientSound
    {
        private readonly SoundEffectInstance _instance;
        private readonly AudioListener _listener;
        private readonly AudioEmitter _emitter = new AudioEmitter();

        public AmbientSound(SoundEffect soundEffect, AudioListener listener)
        {
            _instance = soundEffect.CreateInstance();
            _instance.IsLooped = true;

            _listener = listener;

            _instance.Play();
            _instance.Apply3D(_listener, _emitter);
        }

        public void SetPosition(Vector2 position)
        {
            _emitter.Position = new Vector3(position, 0.0f);
            _instance.Apply3D(_listener, _emitter);
        }
    }
}
