using Microsoft.Xna.Framework.Graphics;
using Roguelike.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Objects
{
    public class AnimationTextureMap : Dictionary<AnimationState, Texture2D>
    {
        public AnimationTextureMap()
        {
        }

        public AnimationTextureMap(IDictionary<AnimationState, Texture2D> dict)
            : base(dict)
        {
        }
    };

    public static class EnemyTextureFactory
    {
        private static readonly IReadOnlyDictionary<AnimationState, string> TextureNames4Dirs = new Dictionary<AnimationState, string>
        {
            [AnimationState.WalkUp] = "spr_{0}_walk_up",
            [AnimationState.WalkDown] = "spr_{0}_walk_down",
            [AnimationState.WalkRight] = "spr_{0}_walk_right",
            [AnimationState.WalkLeft] = "spr_{0}_walk_left",
            [AnimationState.IdleUp] = "spr_{0}_idle_up",
            [AnimationState.IdleDown] = "spr_{0}_idle_down",
            [AnimationState.IdleRight] = "spr_{0}_idle_right",
            [AnimationState.IdleLeft] = "spr_{0}_idle_left"
        };

        private static readonly IReadOnlyDictionary<AnimationState, string> TextureNames2Dirs = new Dictionary<AnimationState, string>
        {
            [AnimationState.WalkUp] = "spr_{0}_walk_front",
            [AnimationState.WalkDown] = "spr_{0}_walk_front",
            [AnimationState.WalkRight] = "spr_{0}_walk_side",
            [AnimationState.WalkLeft] = "spr_{0}_walk_side",
            [AnimationState.IdleUp] = "spr_{0}_idle_front",
            [AnimationState.IdleDown] = "spr_{0}_idle_front",
            [AnimationState.IdleRight] = "spr_{0}_idle_side",
            [AnimationState.IdleLeft] = "spr_{0}_idle_side"
        };

        private static readonly Cache<string, AnimationTextureMap> BaseAnimationTextureMap = new Cache<string, AnimationTextureMap>(enemyName =>
        {
            return LoadAnimationTextures(TextureNames4Dirs, "Enemies", enemyName);
        });

        private static readonly Lazy<AnimationTextureMap> HelmetTextures = new Lazy<AnimationTextureMap>(() =>
        {
            return LoadAnimationTextures(TextureNames2Dirs, "Armor", "Helmet");
        });

        private static readonly Lazy<AnimationTextureMap> TorsoTextures = new Lazy<AnimationTextureMap>(() =>
        {
            return LoadAnimationTextures(TextureNames2Dirs, "Armor", "Torso");
        });

        private static readonly Lazy<AnimationTextureMap> LegsTextures = new Lazy<AnimationTextureMap>(() =>
        {
            return LoadAnimationTextures(TextureNames2Dirs, "Armor", "Legs");
        });

        private static AnimationTextureMap LoadAnimationTextures(IReadOnlyDictionary<AnimationState, string> textureNameFormats, string category, string name)
        {
            return new AnimationTextureMap(textureNameFormats.ToDictionary(kvp => kvp.Key, kvp =>
            {
                var textureName = "{0}/{1}/{2}".F(category, name, kvp.Value.F(name.ToLower()));
                return Global.Content.Load<Texture2D>(textureName);
            }));
        }

        public static AnimationTextureMap GetBaseTextures(string enemyName) => BaseAnimationTextureMap[enemyName];

        public static AnimationTextureMap GetHelmetTextures() => HelmetTextures.Value;

        public static AnimationTextureMap GetTorsoTextures() => TorsoTextures.Value;

        public static AnimationTextureMap GetLegsTextures() => LegsTextures.Value;
    }
}
