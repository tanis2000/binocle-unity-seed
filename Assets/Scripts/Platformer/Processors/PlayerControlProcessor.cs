using System;
using Binocle.Processors;
using UnityEngine;
using Binocle;
using Binocle.Sprites;

namespace App.Platformer
{
	public class PlayerControlProcessor : BaseEntityProcessor
	{
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

            var a = 3000;

            if (input.Right) {
                move.Acceleration.x = a;
            } else if (input.Left) {
                move.Acceleration.x = -a;
            } else {
                move.Acceleration.x = 0;
            }

            if (input.Jump && !input.WasJump && move.Grounded) {
                move.Acceleration.y = a;
            } else if (!move.Grounded) {
                move.Acceleration.y -= 100;
            }
		}



	}
}

