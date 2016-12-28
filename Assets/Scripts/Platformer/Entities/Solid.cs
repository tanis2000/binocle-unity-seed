using System;
using System.Collections.Generic;
using Binocle;
using UnityEngine;

namespace App.Platformer
{
    public class Solid : GameEntity
    {
        public static Solid Mover;
        private static HashSet<Actor> riding = new HashSet<Actor>();
        private Vector2 SubPixelCounter = Vector2.zero;
        private Vector2 tempPos = Vector2.zero;
        protected Collider2D coll;

        protected override void Start()
        {
            base.Start();
            tempPos = transform.position;
            coll = GetComponent<Collider2D>();
        }


        public Hero GetHeroRiding()
        {
            foreach (Hero hero in FindObjectsOfType(typeof(Hero)))
            {
                if (hero.IsRiding(this))
                {
                    return hero;
                }
            }
            return null;
        }

        public bool IsHeroRiding()
        {
            return GetHeroRiding() != null;
        }

        public void Move(Vector2 moveAmount)
        {
            Mover = this;
            SubPixelCounter += moveAmount;
            int dx = (int) Math.Round((double) SubPixelCounter.x);
            int dy = (int) Math.Round((double) SubPixelCounter.y);
            if ((dx != 0) || (dy != 0))
            {
                riding.Clear();
                foreach (Actor actor in FindObjectsOfType<Actor>())
                {
                    if (actor.IsRiding(this))
                    {
                        //Debug.Log("riding");
                        riding.Add(actor);
                        if (actor.FinishPushOnSquishRiding == this)
                        {
                            actor.FinishPushOnSquish = true;
                        }
                    }
                }
                //coll.enabled = false;
                if (dx != 0)
                {
                    SubPixelCounter.x -= dx;
                    tempPos = transform.position;
                    tempPos.x += dx;
                    transform.position = tempPos;
                    if (dx > 0)
                    {
                        foreach (Actor actor in FindObjectsOfType(typeof(Actor)))
                        {
                            actor.EnableSolids();
                            var ac = actor.GetComponent<Collider2D>();
                            if (actor.Pushable && PixelCollisions.CollideCheck(transform.position.x, transform.position.y, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask, ac))
                            {
                                if (actor.NaivePush)
                                {
                                    tempPos = actor.transform.position;
                                    tempPos.x += dx;
                                    actor.transform.position = tempPos;
                                }
                                else
                                {
                                    var right = transform.position.x + coll.bounds.extents.x;
                                    var left = actor.transform.position.x - ac.bounds.extents.x;
                                    //Debug.Log("sp: " + transform.position.x + "ap: " + actor.transform.position.x + " sext: " + coll.bounds.extents.x + " aext: " + ac.bounds.extents.x + " Right: " + right + " Left: " + left);
                                    actor.MoveExactH((int) (right - left), new Action<Solid>(actor.OnSquishRight));
                                }
                            }
                            else if (riding.Contains(actor))
                            {
                                //Debug.Log(dx);
                                actor.MoveExactH(dx, null);
                            }
                            actor.DisableSolids();
                        }
                    }
                    else
                    {
                        foreach (Actor actor in FindObjectsOfType(typeof(Actor)))
                        {
                            actor.EnableSolids();
                            var ac = actor.GetComponent<Collider2D>();
                            if (actor.Pushable && PixelCollisions.CollideCheck(transform.position.x, transform.position.y, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask, ac))
                            {
                                if (actor.NaivePush)
                                {
                                    tempPos = actor.transform.position;
                                    tempPos.x += dx;
                                    actor.transform.position = tempPos;
                                }
                                else
                                {
                                    var right = actor.transform.position.x + ac.bounds.extents.x;
                                    var left = transform.position.x - coll.bounds.extents.x;
                                    //Debug.Log("sp: " + transform.position.x + "ap: " + actor.transform.position.x + " sext: " + coll.bounds.extents.x + " aext: " + ac.bounds.extents.x + " Right: " + right + " Left: " + left);
                                    actor.MoveExactH((int) (left - right), new Action<Solid>(actor.OnSquishLeft));
                                }
                            }
                            else if (riding.Contains(actor))
                            {
                                actor.MoveExactH(dx, null);
                            }
                            actor.DisableSolids();
                        }
                    }
                }
                if (dy != 0)
                {
                    SubPixelCounter.y -= dy;
                    tempPos = transform.position;
                    tempPos.y += dy;
                    transform.position = tempPos;
                    // Moving downwards
                    if (dy < 0)
                    {
                        foreach (Actor actor in FindObjectsOfType(typeof(Actor)))
                        {
                            actor.EnableSolids();
                            var ac = actor.GetComponent<Collider2D>();
                            if (actor.Pushable && PixelCollisions.CollideCheck(transform.position.x, transform.position.y, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask, ac))
                            {
                                if (actor.NaivePush)
                                {
                                    tempPos = actor.transform.position;
                                    tempPos.y += dy;
                                    actor.transform.position = tempPos;
                                }
                                else
                                {
                                    var top = actor.transform.position.y + ac.bounds.extents.y;
                                    var bottom = transform.position.y - coll.bounds.extents.y;
                                    actor.MoveExactV((int) (bottom - top), new Action<Solid>(actor.OnSquishDown));
                                }
                            }
                            else if (riding.Contains(actor))
                            {
                                actor.MoveExactV(dy, null);
                            }
                            actor.DisableSolids();
                        }
                    }
                    // Moving upwards
                    else
                    {
                        foreach (Actor actor in FindObjectsOfType(typeof(Actor)))
                        {
                            actor.EnableSolids();
                            var ac = actor.GetComponent<Collider2D>();
                            if (actor.Pushable && PixelCollisions.CollideCheck(transform.position.x, transform.position.y, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask, ac))
                            {
                                if (actor.NaivePush)
                                {
                                    tempPos = actor.transform.position;
                                    tempPos.y += dy;
                                    actor.transform.position = tempPos;
                                }
                                else
                                {
                                    var top = transform.position.y + coll.bounds.extents.y;
                                    var bottom = actor.transform.position.y - ac.bounds.extents.y;
                                    actor.MoveExactV((int) (top - bottom), new Action<Solid>(actor.OnSquishUp));
                                }
                            }
                            else if (riding.Contains(actor))
                            {
                                actor.MoveExactV(dy, null);
                            }
                            actor.DisableSolids();
                        }
                    }
                }
                foreach (Actor actor in riding)
                {
                    if (actor.FinishPushOnSquishRiding)
                    {
                        actor.FinishPushOnSquish = false;
                    }
                }
                //coll.enabled = true;
            }
            Mover = null;
        }
        
        public void MoveTo(Vector2 target)
        {
            Move(target - new Vector2(transform.position.x + SubPixelCounter.x, transform.position.y + SubPixelCounter.y));
        }

        public void MoveTowards(Vector2 target, float maxAmount)
        {
            var v = new Vector2(transform.position.x + SubPixelCounter.x, transform.position.y + SubPixelCounter.y);
            MoveTo(Utils.Approach(v, target, maxAmount));
        }




    }
}