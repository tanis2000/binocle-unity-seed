using Binocle;
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
        public Vector3 LastPosition;
        public Vector3 StartingPosition;

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
                if (hero == null || hero.Dead) return;
                _context.target = hero;
                _context.stateMachine.changeState<FlyState>();
            }
        }

        public class FlyState : State<Pidgeon>
        {
            public override void update(float deltaTime)
            {
                if (_context.target.Dead) {
                    _context.stateMachine.changeState<FlyBack>();
                }
                _context.LastPosition = _context.transform.position;
                _context.spriteAnimator.Play("fly");
                _context.Velocity = (_context.target.transform.position - _context.transform.position).normalized;
                _context.spriteAnimator.FlipTo(Mathf.Sign(_context.Velocity.x));
                _context.Velocity *= 1.2f;
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

        public class FlyBack : State<Pidgeon>
        {
            public override void update(float deltaTime)
            {
                _context.LastPosition = _context.transform.position;
                _context.spriteAnimator.Play("fly");
                _context.Velocity = (_context.StartingPosition - _context.transform.position).normalized;
                if (_context.Velocity.x == 0 && _context.Velocity.y == 0) {
                    _context.stateMachine.changeState<IdleState>();
                    return;
                }
                _context.spriteAnimator.FlipTo(Mathf.Sign(_context.Velocity.x));
                _context.Velocity *= 1.2f;
                _context.MoveH(_context.Velocity.x * Game.TimeMult);
                _context.MoveV(_context.Velocity.y * Game.TimeMult);
            }
        }


        protected override void Start() 
        {
            base.Start();
            StartingPosition = transform.position;
            spriteAnimator = GetComponentInChildren<SpriteAnimator>();
            var idleState = new IdleState();
            var flyState = new FlyState();
            var deadState = new DeadState();
            var flyBack = new FlyBack();
            stateMachine = new StateMachine<Pidgeon>(this, idleState);
            stateMachine.addState(flyState);
            stateMachine.addState(deadState);
            stateMachine.addState(flyBack);
        }

        protected override void Update()
        {
            base.Update();
            stateMachine.update(Time.deltaTime);
            if (CheckForTargetCollisions()) {
                Bump();
                stateMachine.changeState<FlyBack>();
            }
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

        protected virtual bool CheckForTargetCollisions()
        {
            var colliders = PixelCollisions.CollideCheckAll(transform.position.x + Velocity.x * Game.TimeMult, transform.position.y + Velocity.y * Game.TimeMult, coll.bounds.size.x, coll.bounds.size.y, CollisionLayersMask);
            if (colliders == null) return false;
            foreach(var c in colliders) {
                //Debug.Log(c);
                var entity = c.GetComponent<Hero>();
                if (entity != null && !entity.Dead) {
                    entity.Hurt(DamageType.Melee);
                    return true;
                }
            }
            return false;
        }

        protected void Bump()
        {
            var direction = (LastPosition - transform.position).normalized;
            //Debug.Log(direction);
            Move(direction*20);
        }

    }
}