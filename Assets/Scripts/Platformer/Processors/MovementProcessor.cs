using System;
using Binocle.Processors;
using UnityEngine;
using Binocle;
using Binocle.Sprites;

namespace App.Platformer
{
	public class MovementProcessor : BaseEntityProcessor
	{
		public MovementProcessor (Matcher matcher) : base(matcher)
		{
		}

		protected override void Begin ()
		{
			base.Begin ();
		}


		public override void Process (Entity entity)
		{
			GameObject go = entity.GameObject;
            MovementComponent move = go.GetComponent<MovementComponent>();
			var img = go.GetComponentInChildren<SpriteRenderer> ();
            float originalWidth = 1;
            float originalHeight = 1;
			if (img != null) {
				originalWidth = img.bounds.size.x;
				originalHeight = img.bounds.size.y;
			}

            // Store current position
            move.LastPosition = go.transform.position;

            // Update velocity
            move.Velocity.x = move.Acceleration.x * Time.deltaTime;
            move.Velocity.y = move.Acceleration.y * Time.deltaTime;

            // Check for max velocity
            if (Math.Abs(move.Velocity.x) > Math.Abs(move.MaxVelocity.x)) {
                move.Velocity.x = move.MaxVelocity.x * Math.Sign(move.Velocity.x);
            }
            if (Math.Abs(move.Velocity.y) > Math.Abs(move.MaxVelocity.y)) {
                move.Velocity.y = move.MaxVelocity.y * Math.Sign(move.Velocity.y);
            }

            // Handle sub-pixel movement
			move.SubPosition.x += move.Velocity.x * Time.deltaTime;
			move.SubPosition.y += move.Velocity.y * Time.deltaTime;

			// Compute the integer distance
			int vxNew = Mathf.RoundToInt(move.SubPosition.x);
			int vyNew = Mathf.RoundToInt(move.SubPosition.y);

            // Difference to sum during the next frame
			move.SubPosition.x -= vxNew;
			move.SubPosition.y -= vyNew;

            /*
            long newX = (long)(go.transform.position.x + vxNew);
            long newY = (long)(go.transform.position.y + vyNew);
            int d = Math.Max(Math.Abs(vxNew), Math.Abs(vyNew));
            bool coll = false;
            //Debug.Log("d: "+d);
            var lastPos = go.transform.position;
            for (int i = 0 ; i < d ; i++) {
                var pos = Vector2.Lerp(go.transform.position, new Vector2(newX, newY), 1.0f/d * i+1);
                Debug.Log(pos);
                var pa = new Vector2(pos.x - originalWidth / 2, pos.y - originalHeight / 2);
                var pb = new Vector2(pos.x + originalWidth / 2, pos.y + originalHeight / 2);
                //Debug.Log(pa);
                //Debug.Log(pb);
                DebugX.DrawPoint(pa, Color.cyan, 1);
                DebugX.DrawPoint(pb, Color.magenta, 1);
                //DebugX.DrawRect(new Rect(pos/2, new Vector2(originalWidth, originalHeight)), Color.green);
                Collider2D[] colliders = Physics2D.OverlapAreaAll(pa, pb, move.CollisionLayersMask);
			    if (colliders.Length > 0) {
                    //Debug.Log("Collision");
                    go.transform.position = lastPos;
                    coll = true;
                    break;
                }
                lastPos = pos;
            }

            // Update the position
            if (!coll) {
                go.transform.position = new Vector2(newX, newY);
            }
            */

            /*
            var pos = go.transform.position;
            //movimento l'ogggetto in Y  pixel per pixel fino al termine della velocità o
			//fino alla collisione con un altro oggetto che non si può muovere
			if (Mathf.Abs(vyNew) != 0)
			{
				bool top = (vyNew >= 0.0f) ? true : false;
				int vyABS = Mathf.Abs(vyNew);
				for (int i = 0; i < vyABS; i++)
				{
					bool collided = PixelCollisions.Collide(pos.x, pos.y, originalWidth, originalHeight, top? DirectionUnit.Top : DirectionUnit.Bottom, move.CollisionLayersMask);
					if(collided){
						move.Velocity.y = 0;
						break;
					}else{
						pos.y += top? 1:-1;
					}
				}
			}
			//ripeto la stessa procedura fatta sulla Y
			if (Mathf.Abs(vxNew) != 0)
			{
				bool right = (vxNew >= 0.0f) ? true : false;
				int vxABS = Mathf.Abs(vxNew);
				for (int i = 0; i < vxABS ; i++)
				{
					bool collided = PixelCollisions.Collide(pos.x, pos.y, originalWidth, originalHeight, right? DirectionUnit.Right : DirectionUnit.Left, move.CollisionLayersMask);

					if(collided){
						move.Velocity.x = 0;
						break;
					}else{
						pos.x += right?1:-1;
					}
				}
			}

            go.transform.position = pos;
            */

            long newX = (long)(go.transform.position.x + vxNew);
            long newY = (long)(go.transform.position.y + vyNew);
            var pos = new Vector2(newX, newY);

            var hit = Physics2D.Raycast(go.transform.position, Vector2.right, pos.x-go.transform.position.x+originalWidth/2, move.CollisionLayersMask);
            Debug.DrawRay(go.transform.position, Vector2.right * (pos.x-go.transform.position.x+originalWidth/2), Color.magenta);
            if (hit.collider != null) {
                Debug.Log("Collision");
                pos.x = hit.point.x - originalWidth/2;
            }

            go.transform.position = pos;
		}



	}
}

