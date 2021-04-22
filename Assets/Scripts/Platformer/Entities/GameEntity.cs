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
        public SpriteRenderer SpriteRenderer;

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

        protected virtual void Awake()
        {
            if (Scene is PlatformerScene) {
                GameScene = Scene as PlatformerScene;
            }
        }

        protected virtual void Start()
        {
            if (Scene is PlatformerScene) {
                GameScene = Scene as PlatformerScene;
            }
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected virtual void Update()
        {
            UpdateFlash();
        }

        private void UpdateFlash()
        {
            if (Flashing) {
                flashInterval -= Time.deltaTime;
                if (flashInterval <= 0) {
                    flashInterval = 0.05f;
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
            return PixelCollisions.CollideCheck(transform.position.x + x, transform.position.y + y, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask, oc);
        }

        protected bool CollideCheck(float x, float y)
        {
            var coll = GetComponent<Collider2D>();
            return PixelCollisions.CollideCheck(transform.position.x + x, transform.position.y + y, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
        }

        public void Flash()
        {
            if (!Flashing) {
                Flashing = true;
                flashInterval = 0.05f;
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

        public virtual bool OnBulletHit(Bullet bullet)
        {
            return false;
        }

        public virtual bool OnBallHit(Ball ball)
        {
            return false;
        }

    }
}