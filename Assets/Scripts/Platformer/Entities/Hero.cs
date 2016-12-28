using Binocle;
using UnityEngine;

namespace App.Platformer
{
    public class Hero : Actor
    {
        // Movement
        private InputComponent input;
        private ScaleComponent scale;

		private float groundAccel = 1.00f;
		private float groundFric  = 3.00f;
		private float airAccel    = 0.75f;
		private float airFric     = 0.53f;
		private float vxMax       = 3.0f;
		private float vyMax       = 7.0f;
		private float jumpHeight  = 8.0f;
		private float gravNorm    = -0.76f;
		private float gravSlide   = -0.55f;
		private bool sticking = false;
		private bool canStick = false;
        private bool doubleJumped = false;

        public bool Grounded;
        public bool BottomCollided;
        public bool LeftCollided;
        public bool RightCollided;
        public bool TopCollided;
        public bool LadderCollided;
        public bool OnLeftPrev;
        public bool OnRightPrev;
        public bool OnGroundPrev;
        public bool OnTopPrev;
        public bool OnLadderPrev;

        public Vector2 Velocity = Vector2.zero;

        public bool Dead { get; private set; }

        protected override void Start()
        {
            base.Start();
            input = GetComponent<InputComponent>();
            scale = GetComponentInChildren<ScaleComponent>();
        }
        
        void Update()
        {
            // Init the controls
			bool kLeft = input.Left;
			bool kRight = input.Right;
			if (kRight) kLeft = false;
			bool kDown = input.Down;
			bool kJump = input.Jump && !input.WasJump; // check that jump has just been pressed
			bool kJumpRelease = !input.Jump && input.WasJump; // check that jump has just been released
			bool kFire = false;
            // Temporary vars
			float tempAccel, tempFric;

			if (BottomCollided && !OnGroundPrev) {
				// Squash + stretch
				scale.Scale.x = 1.5f;
				scale.Scale.y = 0.5f;
			}

			// Apply the correct form of acceleration and friction
			if (BottomCollided) {
				tempAccel = groundAccel;
				tempFric  = groundFric;
			} else {
				tempAccel = airAccel;
		    	tempFric  = airFric;
			}

			// Wall cling to avoid accidental push-off
			if ((!RightCollided && !LeftCollided) || BottomCollided) {
				canStick = true;
				sticking = false;
			} else if (((kRight && LeftCollided) || (kLeft && RightCollided)) && canStick && !BottomCollided) {
				sticking = true;
				canStick = false;
			}

			if ((kRight || kLeft) && sticking) {
				canStick = true;
				sticking = false;
			}

            // Handle gravity
			if (!BottomCollided) {
				if ((LeftCollided || RightCollided) && Velocity.y <= 0) {
					// Wall slide
					if(OnLeftPrev) Velocity.y = 0;
					if(OnRightPrev) Velocity.y = 0;

					Velocity.y = Utils.Approach(Velocity.y, -vyMax, -gravSlide);
				} else {
					// Fall normally
					Velocity.y = Utils.Approach(Velocity.y, -vyMax, -gravNorm);
				}
			}

			// Left
			if (kLeft && !kRight && !sticking)
			{
				Velocity.x = Utils.Approach(Velocity.x, -vxMax, tempAccel);
            // Right
			} else if (kRight && !kLeft && !sticking) {
				Velocity.x = Utils.Approach(Velocity.x, vxMax, tempAccel);
			}

			// Friction
			if (!kRight && !kLeft /*&& physicsComponent.vx != 0*/) {
				Velocity.x = Utils.Approach(Velocity.x, 0, tempFric);
			}

			// Wall jump
			float jumpHeightStickY = jumpHeight * 1.1f;
			if (kJump && LeftCollided && !BottomCollided) {
				scale.Scale.x = 0.5f;
				scale.Scale.y = 1.5f;
				// Wall jump is different when pushing off/towards the wall
				if (kLeft) {
					Velocity.x = jumpHeight * 0.25f;
					Velocity.y = jumpHeightStickY;
				} else {
					Velocity.x = 0;//vxMax;
					Velocity.y = jumpHeightStickY;
				}
			} else if (kJump && RightCollided && !BottomCollided) {
				scale.Scale.x = 0.5f;
				scale.Scale.y = 1.5f;
				// Wall jump is different when pushing off/towards the wall
				if (kRight) {
					Velocity.x = -jumpHeight * 0.25f;
					Velocity.y = jumpHeightStickY;
				} else {
					Velocity.x = 0;//-vxMax;
					Velocity.y = jumpHeightStickY;
				}
			}
			if(kFire){
				Velocity.y = 0;
				scale.Scale.x = 0.5f;
				scale.Scale.y = 1.5f;
			}

			// Ladders
			if (LadderCollided) {
				Velocity.y = 0;
				if (input.Jump) {
					Velocity.y = jumpHeight * 0.2f;
				} else if (kDown) {
					Velocity.y = -jumpHeight * 0.2f;
				}
			} else if (OnLadderPrev && !LadderCollided) {
				BottomCollided = true;
				Velocity.y = 0;
				if (input.Jump) kJump = true;
			}

			// Jump 
			if (kJump) { 
				if (LadderCollided) {
				} else if (BottomCollided) {
					Debug.Log("jump one");
					doubleJumped = false;
					scale.Scale.x = 0.5f;
					scale.Scale.y = 1.5f;
					Velocity.y = jumpHeight;
				} else if (!doubleJumped && !LeftCollided && !RightCollided) {
					Debug.Log("jump two");
					scale.Scale.x = 0.5f;
					scale.Scale.y = 1.5f;
					Velocity.y = jumpHeight;
					doubleJumped = true;
				}
				// Variable jumping
			} /*else if (kDown && !physicsComponent.bottomCollided) {
				physicsComponent.vy = -jumpHeight*2;
			}*/
			else if (kJumpRelease) { 
				Debug.Log("jump release");
				if (Velocity.y > 0)
					Velocity.y *= 0.25f;
			}

            scale.Scale.x = Utils.Approach(scale.Scale.x, 1.0f, 0.05f);
			scale.Scale.y = Utils.Approach(scale.Scale.y, 1.0f, 0.05f);

            MoveH(Velocity.x * (Time.deltaTime / 0.01666667f));
            MoveV(Velocity.y * (Time.deltaTime / 0.01666667f));

            scale.transform.localScale = scale.Scale;

            OnGroundPrev = BottomCollided;
			OnTopPrev = TopCollided;
			OnLeftPrev = LeftCollided;
			OnRightPrev = RightCollided;
			OnLadderPrev = LadderCollided;
			BottomCollided = PixelCollisions.Collide(transform.position.x,transform.position.y,coll.bounds.size.x,coll.bounds.size.y,DirectionUnit.Bottom,CollisionLayersMask);
			LeftCollided = PixelCollisions.Collide(transform.position.x,transform.position.y,coll.bounds.size.x,coll.bounds.size.y,DirectionUnit.Left,CollisionLayersMask);
			TopCollided =  PixelCollisions.Collide(transform.position.x,transform.position.y,coll.bounds.size.x,coll.bounds.size.y,DirectionUnit.Top,CollisionLayersMask);
			RightCollided = PixelCollisions.Collide(transform.position.x,transform.position.y,coll.bounds.size.x,coll.bounds.size.y,DirectionUnit.Right,CollisionLayersMask);

        }


        public override void OnSquishDown(Solid platform)
        {
            DoSquish(platform as Solid, Vector2.down);
        }

        public override void OnSquishLeft(Solid platform)
        {
            DoSquish(platform as Solid, Vector2.left);
        }

        public override void OnSquishRight(Solid platform)
        {
            DoSquish(platform as Solid, Vector2.right);
        }

        public override void OnSquishUp(Solid platform)
        {
            DoSquish(platform as Solid, Vector2.up);
        }
        
        private void DoSquish(Solid solid, Vector2 direction)
        {
            Debug.Log(direction);
            /*
            var coll = Solid.Mover.GetComponent<Collider2D>();
            tempPos = transform.position;
            tempPos.x += direction.x * 8;
            tempPos.y += direction.y * 8;
            transform.position = tempPos;
            */
            
            if (!Dead)
            {
                var coll = Solid.Mover.GetComponent<Collider2D>();
                //coll.enabled = true;
                for (int i = 0; i <= 2; i++)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        if ((i != 0) || (j != 0))
                        {
                            for (int k = -1; k <= 1; k += 2)
                            {
                                for (int m = -1; m <= 1; m += 2)
                                {
                                    int dx = (i * 3) * k;
                                    int dy = (j * 3) * m;
                                    //Debug.Log("dx: " + dx + " dy: " + dy);
                                    var c = GetComponent<Collider2D>();
                                    if (!PixelCollisions.CollideCheck(transform.position.x + dx, transform.position.y + dy, c.bounds.size.x, c.bounds.size.y, CollisionLayersMask, coll))
                                    {
                                        tempPos = transform.position;
                                        tempPos.x += dx;
                                        tempPos.y += dy;
                                        transform.position = tempPos;
                                        if (dx != 0)
                                        {
                                            MoveExactH(-dx, null);
                                        }
                                        if (dy != 0)
                                        {
                                            MoveExactV(-dy, null);
                                        }
                                        //coll.enabled = false;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                Die();
                //coll.enabled = false;
            }
            
        }

        public void Die()
        {
            Dead = true;
            Debug.Log("DEAD!");
        }

    }
}