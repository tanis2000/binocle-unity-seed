using System;
using UnityEngine;
using Binocle;

namespace App.Platformer
{
    public class Bullet : Actor
    {
        public int Damage = 1;
        public Vector2 Velocity;

        protected override void Start()
        {
            base.Start();
            var startPos = transform.position;
            gameObject.layer = LayerMask.NameToLayer("Bullets");
            CollisionLayersMask = 1 << LayerMask.NameToLayer("Heroes") | 1 << LayerMask.NameToLayer("Blocks");
            transform.position = Vector2.zero;
            var ce = Scene.CreateEntity("sprite");
            ce.SetParent(this);
            var sr = ce.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "units";
            sr.sprite = Utils.CreateBoxSprite(4, 4, new Color(0, 0.8f, 0.8f, 1));
            var c = AddComponent<BoxCollider2D>();
            c.size = new Vector2(4, 4);
            ce.AddComponent<ScaleComponent>();
            transform.position = startPos;

            // We force this as the base can't initialize
            tempPos = transform.position;
            coll = c;
        }

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

        public void Reset()
        {
            tempPos = transform.position;
        }
        
    }
}