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
            int move = (int) Math.Round((double) SubPixelCounter.x);
            int num2 = (int) Math.Round((double) SubPixelCounter.y);
            if ((move != 0) || (num2 != 0))
            {
                riding.Clear();
                foreach (Actor actor in FindObjectsOfType(typeof(Actor)))
                {
                    if (actor.IsRiding(this))
                    {
                        riding.Add(actor);
                        if (actor.FinishPushOnSquishRiding == this)
                        {
                            actor.FinishPushOnSquish = true;
                        }
                    }
                }
                coll.enabled = false;
                if (move != 0)
                {
                    SubPixelCounter.x -= move;
                    tempPos = transform.position;
                    tempPos.x += move;
                    transform.position = tempPos;
                    if (move > 0)
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
                                    tempPos.x += move;
                                    actor.transform.position = tempPos;
                                }
                                else
                                {
                                    var right = transform.position.x + coll.bounds.extents.x;
                                    var left = actor.transform.position.x - ac.bounds.extents.x;
                                    actor.MoveExactH((int) (right - left), new Action<Solid>(actor.OnSquishRight));
                                }
                            }
                            else if (riding.Contains(actor))
                            {
                                actor.MoveExactH(move, null);
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
                                    tempPos.x += move;
                                    actor.transform.position = tempPos;
                                }
                                else
                                {
                                    var right = actor.transform.position.x + ac.bounds.extents.x;
                                    var left = transform.position.x - coll.bounds.extents.x;
                                    actor.MoveExactH((int) (left - right), new Action<Solid>(actor.OnSquishLeft));
                                }
                            }
                            else if (riding.Contains(actor))
                            {
                                actor.MoveExactH(move, null);
                            }
                            actor.DisableSolids();
                        }
                    }
                }
                if (num2 != 0)
                {
                    SubPixelCounter.y -= num2;
                    tempPos = transform.position;
                    tempPos.y += num2;
                    transform.position = tempPos;
                    if (num2 > 0)
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
                                    tempPos.y += num2;
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
                                actor.MoveExactV(num2, null);
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
                                    tempPos.y += num2;
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
                                actor.MoveExactV(num2, null);
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
                coll.enabled = true;
            }
            Mover = null;
        }
        

    }
}