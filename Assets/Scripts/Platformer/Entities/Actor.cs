using System;
using Binocle;
using UnityEngine;

namespace App.Platformer
{
    public class Actor : GameEntity
    {
        private Vector2 SubPixelCounter = Vector2.zero;
        private Vector2 tempPos = Vector2.zero;

        protected override void Start()
        {
            base.Start();
            tempPos = transform.position;
        }

        public virtual bool IsRiding(Solid solid)
        {
            return CollideCheck(solid, transform.position.x, transform.position.y - 1);
        }

        public void Move(Vector2 amount, Action<Solid> onCollideH = null, Action<Solid> onCollideV = null)
        {
            MoveH(amount.x, onCollideH);
            MoveV(amount.y, onCollideV);
        }

        public bool MoveH(float moveH, Action<Solid> onCollide = null)
        {
            SubPixelCounter.x += moveH;
            int dx = (int) Math.Round((double) SubPixelCounter.x);
            if (dx != 0)
            {
                int dir = Math.Sign(dx);
                SubPixelCounter.x -= dx;
                while (dx != 0)
                {
                    Collider2D coll = PixelCollisions.CollideCheck(transform.position.x + dir, transform.position.y, 8, 8, CollisionLayersMask);
                    if (coll != null)
                    {
                        Debug.Log("Collision with " + coll.name + " " + dir);
                        Entity entity = coll.GetComponent<Entity>();
                        SubPixelCounter.x = 0f;
                        if (onCollide != null)
                        {
                            onCollide(entity as Solid);
                        }
                        return true;
                    }
                    tempPos.x += dir;
                    transform.position = tempPos;
                    dx -= dir;
                }
            }
            return false;
        }

        public bool MoveV(float moveV, Action<Solid> onCollide = null)
        {
            Entity entity;
            SubPixelCounter.y += moveV;
            int dy = (int) Math.Round((double) SubPixelCounter.y);
            // Moving upwards
            if (dy > 0)
            {
                SubPixelCounter.y -= dy;
                while (dy != 0)
                {
                    Collider2D coll = PixelCollisions.CollideCheck(transform.position.x, transform.position.y + 1, 8, 8, CollisionLayersMask);
                    if (coll != null)
                    {
                        Debug.Log("Collision with " + coll.name);
                        entity = coll.GetComponent<Entity>();
                        SubPixelCounter.y = 0f;
                        if (onCollide != null)
                        {
                            onCollide(entity as Solid);
                        }
                        return true;
                    }
                    tempPos.y += 1f;
                    transform.position = tempPos;
                    dy -= 1;
                }
            }
            // Moving downwards
            else if (dy < 0)
            {
                SubPixelCounter.y -= dy;
                while (dy != 0)
                {
                    Collider2D coll = PixelCollisions.CollideCheck(transform.position.x, transform.position.y - 1, 8, 8, CollisionLayersMask);
                    if (coll != null)
                    {
                        Debug.Log("Collision with " + coll.name);
                        entity = coll.GetComponent<Entity>();
                        SubPixelCounter.y = 0f;
                        if (onCollide != null)
                        {
                            onCollide(entity as Solid);
                        }
                        return true;
                    }
                    /*
                    if (!this.IgnoreJumpThrus && ((entity = base.CollideFirstOutside(GameTags.JumpThru, base.X, base.Y + 1f)) != null))
                    {
                        SubPixelCounter.y = 0f;
                        if (onCollide != null)
                        {
                            onCollide(entity as Solid);
                        }
                        return true;
                    }
                    */
                    tempPos.y -= 1;
                    transform.position = tempPos;
                    dy -= 1;
                }
            }
            return false;
        }



    }
}