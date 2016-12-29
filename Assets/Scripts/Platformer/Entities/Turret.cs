using UnityEngine;

namespace App.Platformer
{
    public class Turret : Enemy
    {
        void Update()
        {
            // Check if player is in LOS
            var hero = GetClosestHeroWithSight(900);
            // Fire at player
            if (hero == null) return;
            Debug.Log("Fire!");
        }
    }
}