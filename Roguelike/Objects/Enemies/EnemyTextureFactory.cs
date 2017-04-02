using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Objects
{
    public class AnimationTextureMap : Dictionary<AnimationState, Texture2D>
    {
        public AnimationTextureMap(IDictionary<AnimationState, Texture2D> dict)
            : base(dict)
        {
        }
    };

    public static class EnemyTextureFactory
    {
        private static readonly IReadOnlyDictionary<AnimationState, string> AnimTextureAssetNames = new Dictionary<AnimationState, string>
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

        private static readonly Cache<string, AnimationTextureMap> BaseAnimationTextureMap = new Cache<string, AnimationTextureMap>(enemyName =>
        {
            return LoadAnimationTextures(Global.Content, enemyName, enemyName.ToLower());
        });

        public static AnimationTextureMap GetBaseAnimations(string enemyName)
        {
            return BaseAnimationTextureMap[enemyName];
        }

        private static AnimationTextureMap LoadAnimationTextures(ContentManager content, string assetPrefix, string nameArg)
        {
            return new AnimationTextureMap(AnimTextureAssetNames.ToDictionary(kvp => kvp.Key, kvp =>
            {
                var textureName = "Enemies/{0}/{1}".F(assetPrefix, kvp.Value.F(nameArg));
                return content.Load<Texture2D>(textureName);
            }));
        }
    }
}
