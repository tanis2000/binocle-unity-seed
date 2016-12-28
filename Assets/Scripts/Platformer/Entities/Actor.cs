using System;
using Binocle;
using UnityEngine;

namespace App.Platformer
{
    public class Actor : GameEntity
    {
        private Vector2 SubPixelCounter = Vector2.zero;
        protected Vector2 tempPos = Vector2.zero;
        protected Collider2D coll;

        public bool FinishPushOnSquish;
        public bool NaivePush = false;
        public bool Pushable = true;

        public virtual bool FinishPushOnSquishRiding
        {
            get
            {
                return false;
            }
        }

        public Vector2 ActualPosition
        {
            get
            {
                return new Vector2(transform.position.x + SubPixelCounter.x, transform.position.y + SubPixelCounter.y);
            }
        }
        

        protected override void Start()
        {
            base.Start();
            tempPos = transform.position;
            coll = GetComponent<Collider2D>();
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
                    Collider2D c = PixelCollisions.CollideCheck(transform.position.x + dir, transform.position.y, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
                    if (c != null)
                    {
                        //Debug.Log("Collision with " + c.name + " " + dir);
                        Entity entity = c.GetComponent<Entity>();
                        SubPixelCounter.x = 0f;
                        if (onCollide != null)
                        {
                            onCollide(entity as Solid);
                        }
                        return true;
                    }
                    tempPos = transform.position;
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
                    Collider2D c = PixelCollisions.CollideCheck(transform.position.x, transform.position.y + 1, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
                    if (c != null)
                    {
                        //Debug.Log("Collision with " + c.name);
                        entity = c.GetComponent<Entity>();
                        SubPixelCounter.y = 0f;
                        if (onCollide != null)
                        {
                            onCollide(entity as Solid);
                        }
                        return true;
                    }
                    tempPos = transform.position;
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
                    Collider2D c = PixelCollisions.CollideCheck(transform.position.x, transform.position.y - 1, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
                    if (c != null)
                    {
                        //Debug.Log("Collision with " + c.name);
                        entity = c.GetComponent<Entity>();
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
                    tempPos = transform.position;
                    tempPos.y -= 1;
                    transform.position = tempPos;
                    dy += 1;
                }
            }
            return false;
        }

        public virtual void MoveExactH(int move, Action<Solid> onCollide = null)
        {
            int dir = Math.Sign(move);
            while (move != 0)
            {
                Collider2D c = PixelCollisions.CollideCheck(transform.position.x + dir, transform.position.y, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
                if (c != null)
                {
                    Entity entity = c.GetComponent<Entity>();
                    if (onCollide != null)
                    {
                        onCollide(entity as Solid);
                    }
                    if (FinishPushOnSquish)
                    {
                        tempPos = transform.position;
                        tempPos.x += move;
                        transform.position = tempPos;
                    }
                    break;
                }
                tempPos = transform.position;
                tempPos.x += dir;
                transform.position = tempPos;
                move -= dir;
            }
        }

        public virtual void MoveExactV(int move, Action<Solid> onCollide = null)
        {
            Entity entity;
            // Moving upwards
            if (move > 0)
            {
                while (move != 0)
                {
                    Collider2D c = PixelCollisions.CollideCheck(transform.position.x, transform.position.y + 1, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
                    if (c != null)
                    {
                        entity = c.GetComponent<Entity>();
                        if (onCollide != null)
                        {
                            onCollide(entity as Solid);
                        }
                        if (this.FinishPushOnSquish)
                        {
                            tempPos = transform.position;
                            tempPos.y += move;
                            transform.position = tempPos;
                        }
                        break;
                    }
                    tempPos = transform.position;
                    tempPos.y += 1;
                    transform.position = tempPos;
                    move -= 1;
                }
            }
            // Moving downwards
            else if (move < 0)
            {
                while (move != 0)
                {
                    Collider2D c = PixelCollisions.CollideCheck(transform.position.x, transform.position.y - 1, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
                    if (c != null)
                    {
                        entity = c.GetComponent<Entity>();
                        if (onCollide != null)
                        {
                            onCollide(entity as Solid);
                        }
                        if (this.FinishPushOnSquish)
                        {
                            tempPos = transform.position;
                            tempPos.y += move;
                            transform.position = tempPos;
                        }
                        break;
                    }
                    /*
                    if (!this.IgnoreJumpThrus && ((entity = base.CollideFirstOutside(GameTags.JumpThru, base.X, base.Y + 1f)) != null))
                    {
                        if (onCollide != null)
                        {
                            onCollide(entity as Solid);
                        }
                        if (this.FinishPushOnSquish)
                        {
                            base.Y += move;
                        }
                        break;
                    }
                    */
                    tempPos = transform.position;
                    tempPos.y -= 1;
                    transform.position = tempPos;
                    move += 1;
                }
            }
        }
        
        public void MoveTowards(Vector2 target, float maxAmount, Action<Solid> onCollideH = null, Action<Solid> onCollideV = null)
        {
            Vector2 vector = Utils.Approach(ActualPosition, target, maxAmount);
            Move(vector - ActualPosition, null, null);
        }


        public virtual void DisableSolids()
        {
        }

        public virtual void EnableSolids()
        {
        }

        public virtual void OnSquishDown(Solid solid)
        {
            Destroy(this);
        }

        public virtual void OnSquishLeft(Solid solid)
        {
            Destroy(this);
        }

        public virtual void OnSquishRight(Solid solid)
        {
            Destroy(this);
        }

        public virtual void OnSquishUp(Solid solid)
        {
            Destroy(this);
        }


    }
}