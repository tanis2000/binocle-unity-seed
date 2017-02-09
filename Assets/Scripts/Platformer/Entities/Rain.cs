using System;
using Binocle;
using UnityEngine;

namespace App.Platformer
{
    public class Rain : Entity
    {
        private float cooldown = 5.0f;

        protected virtual void Update()
        {
            UpdateParticles();
        }

        private void UpdateParticles()
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0) {
                //Destroy(this);
            }
        }


    }
}