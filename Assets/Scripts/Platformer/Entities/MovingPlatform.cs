using UnityEngine;

namespace App.Platformer
{
    public class MovingPlatform : Solid
    {
        private Vector2 start;
        private Vector2 end;
        private Vector2 moveTarget;


        public Vector2 End {
            get {
                return end;
            }
            set {
                end = value;
                moveTarget = end;
            }
        }

        protected override void Start()
        {
            base.Start();
            start = transform.position;
        }

        protected override void Update()
        {
            base.Update();

            MoveTowards(moveTarget, 0.4f * Game.TimeMult);
            if (transform.position.x == moveTarget.x && transform.position.y == moveTarget.y) {
                if (moveTarget == End) {
                    moveTarget = start;
                } else {
                    moveTarget = End;
                }
            }
        }

    }
}