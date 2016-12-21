using UnityEngine;
using Binocle.Components;

namespace App.Platformer
{
    public class MovementComponent : BaseMonoBehaviour
    {
        public Vector2 Velocity;
        public Vector2 LastPosition;
        public Vector2 MaxVelocity;
        /// <summary>
        /// Used for sub-pixel movement
        /// </summary>
        public Vector2 SubPosition;
        public int CollisionLayersMask;
        public bool Grounded;
        public bool BottomCollided;
        public bool LeftCollided;
        public bool RightCollided;
        public bool TopCollided;
        public bool LadderCollided;
        public bool OnLeftPrev;
        public bool OnRightPrev;
        public bool OnGroundPrev;
        public bool OnTopPrev;
        public bool OnLadderPrev;
        public float OriginalWidth;
        public float OriginalHeight;
        public void Start()
        {
            var img = GetComponentInChildren<SpriteRenderer>();
            OriginalWidth = 1;
            OriginalHeight = 1;
            if (img != null)
            {
                OriginalWidth = img.bounds.size.x;
                OriginalHeight = img.bounds.size.y;
            }

        }
    }

}