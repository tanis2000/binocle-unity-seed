using System;
using Binocle;
using UnityEngine;

namespace App.Platformer
{
    public class GameEntity : Entity
    {
        private float flashInterval;
        private float flashCounter;
        private Action onFlashFinish;

        public int CollisionLayersMask;

        public PlatformerScene GameScene
        {
            get;
            private set;
        }

        public bool Flashing
        {
            get;
            private set;
        }

        public SpriteRenderer SpriteRenderer
        {
            get;
            private set;
        }

        protected virtual void Start()
        {
            if (Scene is PlatformerScene) {
                GameScene = Scene as PlatformerScene;
            }
            var s = GetComponent<SpriteRenderer>();
            if (s != null) {
                SpriteRenderer = s;
            }
        }

        void Update()
        {
            UpdateFlash();
        }

        private void UpdateFlash()
        {
            if (Flashing) {
                flashInterval -= Time.deltaTime;
                if (flashInterval <= 0) {
                    flashInterval = 3;
                    OnFlash();
                }
                if (flashCounter > 0) {
                    flashCounter -= Time.deltaTime;
                    if (flashCounter <= 0) {
                        Flashing = false;
                        OnFlashEnd();
                        if (onFlashFinish != null) {
                            onFlashFinish();
                        }
                    }
                }
            }
        }

        protected bool CollideCheck(Entity other, float x, float y)
        {
            var oc = other.GetComponent<Collider2D>();
            var coll = GetComponent<Collider2D>();
            return PixelCollisions.CollideCheck(transform.position.x, transform.position.y + y, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask, oc);
        }

        public void Flash()
        {
            if (!Flashing) {
                Flashing = true;
                flashInterval = 3;
                OnFlashBegin();
            }
            flashCounter = 0;
        }

        public void Flash(int count, Action onFinish = null)
        {
            Flash();
            flashCounter = count;
            onFlashFinish = onFinish;
        }

        protected virtual void OnFlashBegin()
        {
            SpriteRenderer.enabled = false;
        }

        protected virtual void OnFlash()
        {
            SpriteRenderer.enabled = !SpriteRenderer.enabled;
        }

        protected virtual void OnFlashEnd()
        {
            SpriteRenderer.enabled = true;
        }

        protected virtual void OnHeroCollide(Hero hero)
        {
        }
    }
}