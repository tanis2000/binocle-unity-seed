using System;
using UnityEngine;
using Binocle;
using Binocle.AI.FSM;

namespace App.Platformer
{
    public class Ball : Actor
    {
        public int Damage = 1;
        public Vector2 Velocity = Vector2.zero;
        public Hero Hero;
        public Vector3 Offset = Vector3.zero;

        private StateMachine<Ball> stateMachine;
        private IdleState idleState;
        private ShootingState shootingState;

        public class IdleState : State<Ball>
        {
            private float counter;
            private Vector3 floatingPos = Vector3.zero;
            private int OriginalCollisionLayersMask;

            public override void begin()
            {
                _context.transform.position = _context.Hero.transform.position + _context.Offset * _context.Hero.Facing + floatingPos;
                OriginalCollisionLayersMask = _context.CollisionLayersMask;
                _context.CollisionLayersMask = 0;
                counter = 0;
                _context.ResetSubpixelCounter();
            }

            public override void update(float deltaTime)
            {
                counter += deltaTime * 4.0f;
                var y = Mathf.Sin(counter) * 8;
                floatingPos.y = Mathf.RoundToInt(y);

                /*_context.MoveV(y);
                var pos = _context.transform.position;
                pos.x = _context.Hero.transform.position.x + _context.Offset.x * _context.Hero.Facing;
                _context.transform.position = pos;*/
                _context.transform.position = _context.Hero.transform.position + _context.Offset * _context.Hero.Facing + floatingPos;
            }

            public override void end()
            {
                _context.CollisionLayersMask = OriginalCollisionLayersMask;
            }
        }

        public class ShootingState : State<Ball>
        {

            public override void begin()
            {
                _context.transform.position = _context.Hero.transform.position + _context.Offset * _context.Hero.Facing;
            }

            public override void update(float deltaTime)
            {
                _context.CheckForTargetCollisions();
                _context.MoveH(_context.Velocity.x * Game.TimeMult);
                _context.MoveV(_context.Velocity.y * Game.TimeMult);
            }
        }

        protected override void Start()
        {
            base.Start();
            Offset.x = 12;
            idleState = new IdleState();
            shootingState = new ShootingState();
            stateMachine = new StateMachine<Ball>(this, idleState);
            stateMachine.addState(shootingState);
        }

        protected override void Update()
        {
            base.Update();
            stateMachine.update(Time.deltaTime);
        }

        protected virtual bool CheckForTargetCollisions()
        {
            var colliders = PixelCollisions.CollideCheckAll(transform.position.x + Velocity.x * Game.TimeMult, transform.position.y + Velocity.y * Game.TimeMult, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
            if (colliders == null) return false;
            foreach(var c in colliders) {
                Debug.Log(c);
                var entity = c.GetComponent<GameEntity>();
                if (entity != null) {
                    entity.OnBallHit(this);
                    return true;
                }
            }
            return false;
        }

        public void Shoot()
        {
            if (stateMachine.currentState != idleState) return;
            Velocity.x = 10 * Hero.Facing;
            stateMachine.changeState<ShootingState>();
        }

        public void ReturnHome()
        {
            Velocity.x = 0;
            Velocity.y = 0;
            stateMachine.changeState<IdleState>();
        }
        
    }
}