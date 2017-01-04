using System;
using UnityEngine;
using Binocle;

namespace App.Platformer
{
    public class Bullet : Actor
    {
        public int Damage = 1;
        public Vector2 Velocity;

        protected override void Update()
        {
            base.Update();
            CheckForTargetCollisions();
            MoveH(Velocity.x * Game.TimeMult);
            MoveV(Velocity.y * Game.TimeMult);
        }

        protected virtual bool CheckForTargetCollisions()
        {
            var colliders = PixelCollisions.CollideCheckAll(transform.position.x + Velocity.x * Game.TimeMult, transform.position.y + Velocity.y * Game.TimeMult, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
            if (colliders == null) return false;
            foreach(var c in colliders) {
                Debug.Log(c);
                var entity = c.GetComponent<GameEntity>();
                if (entity != null) {
                    entity.OnBulletHit(this);
                    return true;
                }
            }
            return false;
        }
        
    }
}