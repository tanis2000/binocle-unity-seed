using Binocle.AI.FSM;
using Binocle.Sprites;
using UnityEngine;

namespace App.Platformer
{
    public class Pidgeon : Enemy
    {
        private StateMachine<Pidgeon> stateMachine;
        private SpriteAnimator spriteAnimator;
        private Hero target;
        public Vector2 Velocity;

        public class IdleState : State<Pidgeon>
        {
            public override void begin()
            {
                _context.spriteAnimator.Play("idle");
            }
            public override void update(float deltaTime)
            {
                // Check if player is in LOS
                var hero = _context.GetClosestHeroWithSight(900);
                if (hero == null) return;
                _context.target = hero;
                _context.stateMachine.changeState<FlyState>();
            }
        }

        public class FlyState : State<Pidgeon>
        {
            public override void update(float deltaTime)
            {
                _context.spriteAnimator.Play("fly");
                _context.Velocity = (_context.target.transform.position - _context.transform.position).normalized;
                _context.spriteAnimator.FlipTo(Mathf.Sign(_context.Velocity.x));
                _context.Velocity *= 2;
                _context.MoveH(_context.Velocity.x * Game.TimeMult);
                _context.MoveV(_context.Velocity.y * Game.TimeMult);
            }
        }

        public class DeadState : State<Pidgeon>
        {
            public override void update(float deltaTime)
            {
            }
        }

        protected override void Start() 
        {
            base.Start();
            spriteAnimator = GetComponentInChildren<SpriteAnimator>();
            var idleState = new IdleState();
            var flyState = new FlyState();
            var deadState = new DeadState();
            stateMachine = new StateMachine<Pidgeon>(this, idleState);
            stateMachine.addState(flyState);
            stateMachine.addState(deadState);
        }

        protected override void Update()
        {
            base.Update();
            stateMachine.update(Time.deltaTime);
        }

        public override bool OnBallHit(Ball ball) {
            return base.OnBallHit(ball);
        }

        public override void Die() {
            base.Die();
            EntityFactory.CreateFlames(transform.position);
            stateMachine.changeState<DeadState>();
            Destroy(this);
        }

    }
}