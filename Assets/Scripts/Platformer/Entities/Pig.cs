using System.Collections.Generic;
using Binocle.AI.FSM;
using Binocle.Sprites;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace App.Platformer
{
    public class Pig: Enemy
    {
        public Vector2 Velocity;
        public Vector3 LastPosition;
        public Vector3 StartingPosition;

        private StateMachine<Pig> stateMachine;
        private SpriteAnimator spriteAnimator;
        private bool onGround;
        private static HashSet<Actor> riding = new HashSet<Actor>();
        private Vector3 target;
        
        public class IdleState : State<Pig>
        {
            public override void begin()
            {
                _context.spriteAnimator.Play("idle");
            }
            public override void update(float deltaTime)
            {
                riding.Clear();
                var actors = FindObjectsOfType<Actor>();
                foreach (var actor in actors)
                {
                    if (actor.IsRiding(_context))
                    {
                        riding.Add(actor);
                    }
                }

                if (riding.Count > 0)
                {
                    _context.stateMachine.changeState<WalkState>();
                }
            }
        }

        public class WalkState : State<Pig>
        {
            private void ObtainNewTarget()
            {
                var dir = Random.value > 0.5f ? 1 : -1;
                _context.target = _context.transform.position + Vector3.right * dir * 16;
            }

            private void EmitPuff()
            {
                var pos = _context.transform.position;
                pos.y -= _context.coll.bounds.size.y/2;
                EntityFactory.CreatePuff(pos);
            }
            
            public override void begin()
            {
                ObtainNewTarget();
                EmitPuff();
            }

            public override void update(float deltaTime)
            {
                riding.Clear();
                var actors = FindObjectsOfType<Actor>();
                foreach (var actor in actors)
                {
                    if (actor.IsRiding(_context))
                    {
                        riding.Add(actor);
                    }
                }

                if (riding.Count == 0) {
                    _context.stateMachine.changeState<IdleState>();
                }
                
                _context.LastPosition = _context.transform.position;
                _context.spriteAnimator.Play("walk");
                _context.Velocity = (_context.target - _context.transform.position).normalized;

                if (_context.Velocity.x == 0 && _context.Velocity.y == 0) {
                    ObtainNewTarget();
                    EmitPuff();
                }

                _context.spriteAnimator.FlipTo(Mathf.Sign(_context.Velocity.x));
                _context.Velocity *= 1.2f;
                _context.MoveH(_context.Velocity.x * Game.TimeMult);
                foreach (var actor in riding)
                {
                    actor.MoveH(_context.Velocity.x * Game.TimeMult, null);
                }
            }
        }

        protected override void Start() 
        {
            base.Start();
            StartingPosition = transform.position;
            spriteAnimator = GetComponentInChildren<SpriteAnimator>();
            var idleState = new IdleState();
            var walkState = new WalkState();
            // var deadState = new DeadState();
            // var flyBack = new FlyBack();
            stateMachine = new StateMachine<Pig>(this, idleState);
            stateMachine.addState(walkState);
            // stateMachine.addState(deadState);
            // stateMachine.addState(flyBack);
        }

        protected override void Update()
        {
            base.Update();
            stateMachine.update(Time.deltaTime);
            onGround = CheckBelow();
            if (!onGround)
            {
                Speed.y = -3.5f;
            }
            Move(Speed * Game.TimeMult);
        }
    }
}