using Binocle.AI.FSM;
using Binocle.Sprites;
using UnityEngine;

namespace App.Platformer
{
    public class Turret : Enemy
    {
        private float fireCooldown = 1.5f;
        private float fireRate = 1.5f;
        private StateMachine<Turret> stateMachine;
        private SpriteAnimator spriteAnimator;

        public class IdleState : State<Turret>
        {
            public override void update(float deltaTime)
            {
                _context.fireCooldown -= Time.deltaTime;
                // Check if player is in LOS
                var hero = _context.GetClosestHeroWithSight(900);
                // Fire at player
                if (hero == null) return;
                if (_context.fireCooldown <= 0) {
                    _context.fireCooldown = _context.fireRate;
                    Debug.Log("Fire!");
                    var aimDirection = (hero.transform.position - _context.transform.position).normalized;
                    EntityFactory.CreateBullet(_context.transform.position, aimDirection * 5);
                }
            }
        }

        public class DeadState : State<Turret>
        {
            public override void update(float deltaTime)
            {
                _context.spriteAnimator.Play("die", false);
            }
        }

        protected override void Start() 
        {
            spriteAnimator = GetComponentInChildren<SpriteAnimator>();
            var idleState = new IdleState();
            var deadState = new DeadState();
            stateMachine = new StateMachine<Turret>(this, idleState);
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
            EntityFactory.CreatePuff(transform.position);
            stateMachine.changeState<DeadState>();
        }
    }
}