using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Roguelike.Objects
{
    public sealed class Humanoid : Enemy
    {
        public HumanoidType HumanoidType { get; } = (HumanoidType)Rand.Next(EnumExtensions.GetCount<HumanoidType>());

        [Flags]
        public enum ArmorFlags
        {
            None = 0,
            Helmet = 1,
            Torso = 2,
            Legs = 4
        }

        public Humanoid()
        {
            var armors = ArmorFlags.None;
            if (Rand.Next(5) == 0)
                armors |= ArmorFlags.Helmet;
            if (Rand.Next(5) == 0)
                armors |= ArmorFlags.Torso;
            if (Rand.Next(5) == 0)
                armors |= ArmorFlags.Legs;

            var textures = CreateTextures(armors);
            foreach (var kvp in textures)
                Textures[(int)kvp.Key] = kvp.Value;

            SetSprite(Textures[(int)(AnimationState.WalkUp)], frames: 8, frameSpeed: 12);
        }

        private AnimationTextureMap CreateTextures(ArmorFlags armors)
        {
            var baseTextures = EnemyTextureFactory.GetBaseTextures(HumanoidType.ToString());

            if (armors == ArmorFlags.None)
                return baseTextures;

            var finalTextures = new AnimationTextureMap();
            foreach (AnimationState animationState in Enum.GetValues(typeof(AnimationState)))
            {
                var baseTexture = baseTextures[animationState];
                var renderTarget = new RenderTarget2D(Global.GraphicsDevice, baseTexture.Width, baseTexture.Height);

                var textures = GetTexturesToCompose(animationState, armors);
                DrawTexturesToRenderTarget(textures, renderTarget);

                finalTextures.Add(animationState, renderTarget);
            }

            return finalTextures;
        }

        private IEnumerable<Texture2D> GetTexturesToCompose(AnimationState animationState, ArmorFlags armors)
        {
            var baseTextures = EnemyTextureFactory.GetBaseTextures(HumanoidType.ToString());
            yield return baseTextures[animationState];

            if (armors.HasFlag(ArmorFlags.Helmet))
                yield return EnemyTextureFactory.GetHelmetTextures()[animationState];

            if (armors.HasFlag(ArmorFlags.Torso))
                yield return EnemyTextureFactory.GetTorsoTextures()[animationState];

            if (armors.HasFlag(ArmorFlags.Legs))
                yield return EnemyTextureFactory.GetLegsTextures()[animationState];
        }

        private static void DrawTexturesToRenderTarget(IEnumerable<Texture2D> textures, RenderTarget2D renderTarget)
        {
            var graphicsDevice = Global.GraphicsDevice;
            var spriteBatch = Global.SpriteBatch;

            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();

            foreach (var texture in textures)
                spriteBatch.Draw(texture, Vector2.Zero, Color.White);

            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);
        }
    }

    public enum HumanoidType
    {
        Goblin,
        Skeleton
    }
}
