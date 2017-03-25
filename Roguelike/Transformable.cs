using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike
{
    /// <summary>
    /// Decomposed transform defined by a position, a rotation and a scale
    /// </summary>
    public abstract class Transformable
    {
        /// <summary>
        /// Position of the object
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Orientation of the object
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Local origin of the object
        /// </summary>
        public Vector2 Origin { get; set; }
    }
}
