using System;
using Binocle.Processors;
using UnityEngine;
using Binocle;
using Binocle.Sprites;

namespace App.Platformer
{
	public class PlayerControlProcessor : BaseEntityProcessor
	{
        // Movement
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

		public PlayerControlProcessor (Matcher matcher) : base(matcher)
		{
		}

		protected override void Begin ()
		{
			base.Begin ();
		}


		public override void Process (Entity entity)
		{
			GameObject go = entity.GameObject;
			InputComponent input = go.GetComponent<InputComponent>();
            MovementComponent move = go.GetComponent<MovementComponent>();
            ScaleComponent scale = go.GetComponent<ScaleComponent>();

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

			if (move.BottomCollided && !move.OnGroundPrev) {
				// Squash + stretch
				scale.Scale.x = 1.5f;
				scale.Scale.y = 0.5f;
			}

			// Apply the correct form of acceleration and friction
			if (move.BottomCollided) {
				tempAccel = groundAccel;
				tempFric  = groundFric;
			} else {
				tempAccel = airAccel;
				tempFric  = airFric;
			}

			// Wall cling to avoid accidental push-off
			if ((!move.RightCollided && !move.LeftCollided) || move.BottomCollided) {
				canStick = true;
				sticking = false;
			} else if (((kRight && move.LeftCollided) || (kLeft && move.RightCollided)) && canStick && !move.BottomCollided) {
				sticking = true;
				canStick = false;
			}

			if ((kRight || kLeft) && sticking) {
				canStick = true;
				sticking = false;
			}

            // Handle gravity
			if (!move.BottomCollided) {
				if ((move.LeftCollided || move.RightCollided) && move.Velocity.y <= 0) {
					// Wall slide
					if(move.OnLeftPrev) move.Velocity.y = 0;
					if(move.OnRightPrev) move.Velocity.y = 0;

					move.Velocity.y = Utils.Approach(move.Velocity.y, -vyMax, -gravSlide);
				} else {
					// Fall normally
					move.Velocity.y = Utils.Approach(move.Velocity.y, -vyMax, -gravNorm);
				}
			}


			// Left
			if (kLeft && !kRight && !sticking)
			{
				move.Velocity.x = Utils.Approach(move.Velocity.x, -vxMax, tempAccel);
            // Right
			} else if (kRight && !kLeft && !sticking) {
				move.Velocity.x = Utils.Approach(move.Velocity.x, vxMax, tempAccel);
			}

			// Friction
			if (!kRight && !kLeft /*&& physicsComponent.vx != 0*/) {
				move.Velocity.x = Utils.Approach(move.Velocity.x, 0, tempFric);
			}

			// Wall jump
			float jumpHeightStickY = jumpHeight * 1.1f;
			if (kJump && move.LeftCollided && !move.BottomCollided) {
				scale.Scale.x = 0.5f;
				scale.Scale.y = 1.5f;
				// Wall jump is different when pushing off/towards the wall
				if (kLeft) {
					move.Velocity.x = jumpHeight * 0.25f;
					move.Velocity.y = jumpHeightStickY;
				} else {
					move.Velocity.x = 0;//vxMax;
					move.Velocity.y = jumpHeightStickY;
				}
			} else if (kJump && move.RightCollided && !move.BottomCollided) {
				scale.Scale.x = 0.5f;
				scale.Scale.y = 1.5f;
				// Wall jump is different when pushing off/towards the wall
				if (kRight) {
					move.Velocity.x = -jumpHeight * 0.25f;
					move.Velocity.y = jumpHeightStickY;
				} else {
					move.Velocity.x = 0;//-vxMax;
					move.Velocity.y = jumpHeightStickY;
				}
			}
			if(kFire){
				move.Velocity.y = 0;
				scale.Scale.x = 0.5f;
				scale.Scale.y = 1.5f;
			}

			// Ladders
			if (move.LadderCollided) {
				move.Velocity.y = 0;
				if (input.Jump) {
					move.Velocity.y = jumpHeight * 0.2f;
				} else if (kDown) {
					move.Velocity.y = -jumpHeight * 0.2f;
				}
			} else if (move.OnLadderPrev && !move.LadderCollided) {
				move.BottomCollided = true;
				move.Velocity.y = 0;
				if (input.Jump) kJump = true;
			}

			// Jump 
			if (kJump) { 
				if (move.LadderCollided) {
				} else if (move.BottomCollided) {
					Debug.Log("jump one");
					doubleJumped = false;
					scale.Scale.x = 0.5f;
					scale.Scale.y = 1.5f;
					move.Velocity.y = jumpHeight;
				} else if (!doubleJumped && !move.LeftCollided && !move.RightCollided) {
					Debug.Log("jump two");
					scale.Scale.x = 0.5f;
					scale.Scale.y = 1.5f;
					move.Velocity.y = jumpHeight;
					doubleJumped = true;
				}
				// Variable jumping
			} /*else if (kDown && !physicsComponent.bottomCollided) {
				physicsComponent.vy = -jumpHeight*2;
			}*/
			else if (kJumpRelease) { 
				Debug.Log("jump release");
				if (move.Velocity.y > 0)
					move.Velocity.y *= 0.25f;
			}

            scale.Scale.x = Utils.Approach(scale.Scale.x, 1.0f, 0.05f);
			scale.Scale.y = Utils.Approach(scale.Scale.y, 1.0f, 0.05f);


		}



	}
}

