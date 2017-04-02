using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Roguelike.Objects
{
    public sealed class Humanoid : Enemy
    {
        private static readonly IReadOnlyDictionary<ArmorTier, Color> ArmorTierColors = new Dictionary<ArmorTier, Color>
        {
            [ArmorTier.Bronze] = new Color(110, 55, 28),
            [ArmorTier.Silver] = new Color(209, 208, 201),
            [ArmorTier.Gold] = new Color(229, 192, 21),
        };

        public HumanoidType HumanoidType { get; } = (HumanoidType)Rand.Next(EnumExtensions.GetCount<HumanoidType>());

        public enum ArmorType
        {
            Helmet,
            Torso,
            Legs
        }

        public enum ArmorTier
        {
            Bronze,
            Silver,
            Gold
        }

        public Humanoid()
        {
            var armorOptions = new Dictionary<ArmorType, ArmorTier>();

            if (Rand.Next(5) == 0)
                armorOptions.Add(ArmorType.Helmet, RandomSelectArmorTier());
            if (Rand.Next(5) == 0)
                armorOptions.Add(ArmorType.Torso, RandomSelectArmorTier());
            if (Rand.Next(5) == 0)
                armorOptions.Add(ArmorType.Legs, RandomSelectArmorTier());

            var textures = CreateTextures(armorOptions);
            foreach (var kvp in textures)
                Textures[(int)kvp.Key] = kvp.Value;

            SetSprite(Textures[(int)(AnimationState.WalkUp)], frames: 8, frameSpeed: 12);
        }

        private static ArmorTier RandomSelectArmorTier()
        {
            var tierValue = Rand.Next(0, 100);
            if (tierValue < 50)
                return ArmorTier.Bronze;
            else if (tierValue < 85)
                return ArmorTier.Silver;
            else
                return ArmorTier.Gold;
        }

        private AnimationTextureMap CreateTextures(IReadOnlyDictionary<ArmorType, ArmorTier> armorOptions)
        {
            var baseTextures = EnemyTextureFactory.GetBaseTextures(HumanoidType.ToString());

            if (armorOptions.Count <= 0)
                return baseTextures;

            var finalTextures = new AnimationTextureMap();
            foreach (AnimationState animationState in Enum.GetValues(typeof(AnimationState)))
            {
                var baseTexture = baseTextures[animationState];
                var renderTarget = new RenderTarget2D(Global.GraphicsDevice, baseTexture.Width, baseTexture.Height);

                var armorTextures = GetArmorTextures(animationState, armorOptions);
                DrawTexturesToRenderTarget(baseTexture, armorTextures, renderTarget);

                finalTextures.Add(animationState, renderTarget);
            }

            return finalTextures;
        }

        private IEnumerable<Tuple<Texture2D, ArmorTier>> GetArmorTextures(
            AnimationState animationState, 
            IReadOnlyDictionary<ArmorType, ArmorTier> armorOptions)
        {
            foreach (var armorOption in armorOptions)
            {
                var armorType = armorOption.Key;
                var armorTier = armorOption.Value;

                var texture = GetArmorAnimationTextureMap(armorType)[animationState];
                yield return Tuple.Create(texture, armorTier);
            }
        }

        private static AnimationTextureMap GetArmorAnimationTextureMap(ArmorType armorType)
        {
            switch (armorType)
            {
                case ArmorType.Helmet:
                    return EnemyTextureFactory.GetHelmetTextures();
                case ArmorType.Torso:
                    return EnemyTextureFactory.GetTorsoTextures();
                case ArmorType.Legs:
                    return EnemyTextureFactory.GetLegsTextures();
                default:
                    throw new InvalidOperationException($"Invalid {nameof(ArmorType)} '{armorType}'");
            }
        }

        private static void DrawTexturesToRenderTarget(
            Texture2D baseTexture,
            IEnumerable<Tuple<Texture2D, ArmorTier>> armorTexturesWithTier, 
            RenderTarget2D renderTarget)
        {
            var graphicsDevice = Global.GraphicsDevice;
            var spriteBatch = Global.SpriteBatch;

            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();

            spriteBatch.Draw(baseTexture, Vector2.Zero, Color.White);
            foreach (var kvp in armorTexturesWithTier)
            {
                var armorTexture = kvp.Item1;
                var armorTier = kvp.Item2;
                spriteBatch.Draw(armorTexture, Vector2.Zero, ArmorTierColors[armorTier]);
            }

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
