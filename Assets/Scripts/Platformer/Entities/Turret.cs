using UnityEngine;

namespace App.Platformer
{
    public class Turret : Enemy
    {
        private float fireCooldown = 1.5f;
        private float fireRate = 1.5f;
        void Update()
        {
            fireCooldown -= Time.deltaTime;
            // Check if player is in LOS
            var hero = GetClosestHeroWithSight(900);
            // Fire at player
            if (hero == null) return;
            if (fireCooldown <= 0) {
                fireCooldown = fireRate;
                Debug.Log("Fire!");
                var aimDirection = (hero.transform.position - transform.position).normalized;
                EntityFactory.CreateBullet(transform.position, aimDirection * 5);
            }
        }
    }
}