using System;
using UnityEngine;

namespace App.Platformer
{
    public class Enemy : Actor
    {
        public Vector2 SightCheckOffset;

        public int Health { get; protected set; }

        public virtual bool CanSeeHero(Hero hero)
        {
            if (hero == null) return false;
            if (!hero.Detectable((Vector2)transform.position + SightCheckOffset)) return false;
            var hit = Physics2D.Linecast((Vector2)transform.position + SightCheckOffset, hero.transform.position, 1 << LayerMask.NameToLayer("Heroes") | 1 << LayerMask.NameToLayer("Blocks"));
            //Debug.Log(hit.collider);
            if (hit.collider == null) return false;
            var h = hit.collider.GetComponent<Hero>();
            if (h == null) return false;
            if (h == hero) return true;
            return false;
        }

        public Hero GetClosestHeroWithSight(float maxDist)
        {
            Hero hero = null;
            float dist = maxDist;
            foreach (Hero h in FindObjectsOfType<Hero>())
            {
                if (h.Detectable((Vector2)transform.position + SightCheckOffset))
                {
                    float d = Vector2.Distance((Vector2)transform.position + SightCheckOffset, h.transform.position);
                    if ((d < dist) && CanSeeHero(h))
                    {
                        dist = d;
                        hero = h;
                    }
                }
            }
            return hero;
        }

    }
}