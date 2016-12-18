using UnityEngine;
using Binocle.Components;

namespace App.Platformer
{
    public class MovementComponent : BaseMonoBehaviour
    {
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Vector2 LastPosition;
        public Vector2 MaxVelocity;
        /// <summary>
        /// Used for sub-pixel movement
        /// </summary>
        public Vector2 SubPosition;
        public int CollisionLayersMask;
    }
}