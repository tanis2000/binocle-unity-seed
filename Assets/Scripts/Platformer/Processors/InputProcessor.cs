using System;
using Binocle.Processors;
using UnityEngine;
using Binocle;
using Binocle.Sprites;

namespace App.Platformer
{
	public class InputProcessor : BaseEntityProcessor
	{
		public InputProcessor (Matcher matcher) : base(matcher)
		{
		}

		protected override void Begin ()
		{
			base.Begin ();
		}


		public override void Process (Entity entity)
		{
			GameObject go = entity.GameObject;
			InputComponent input = go.GetComponent<InputComponent> ();;

			input.WasLeft = input.Left;
			input.WasRight = input.Right;
			input.WasJump = input.Jump;
			input.WasDown = input.Down;
			input.WasFire = input.Fire;

			input.Left = Input.GetKey (KeyCode.A);
			input.Right = Input.GetKey (KeyCode.D);
			input.Jump = Input.GetKey (KeyCode.W);
			input.Down = Input.GetKey (KeyCode.S);
			input.Fire = Input.GetKey (KeyCode.Space);

		}



	}
}

