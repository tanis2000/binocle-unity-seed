using System;
using Binocle.Processors;
using UnityEngine;
using Binocle;
using Binocle.Sprites;

namespace App.Platformer
{
    public class MovementProcessor : BaseEntityProcessor
    {
        private float skin = 0.005f;

        public MovementProcessor(Matcher matcher) : base(matcher)
        {
        }

        protected override void Begin()
        {
            base.Begin();
        }


        public override void Process(Entity entity)
        {
            GameObject go = entity.GameObject;
            MovementComponent move = go.GetComponent<MovementComponent>();
            ScaleComponent scale = go.GetComponent<ScaleComponent>();
            BoxCollider2D coll = go.GetComponent<BoxCollider2D>();

            // Store current position
            move.LastPosition = go.transform.position;

            // Handle sub-pixel movement
            move.SubPosition.x += move.Velocity.x * Time.deltaTime * 45;
            move.SubPosition.y += move.Velocity.y * Time.deltaTime * 45;

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

            
            var pos = go.transform.position;
            //movimento l'ogggetto in Y  pixel per pixel fino al termine della velocità o
			//fino alla collisione con un altro oggetto che non si può muovere
			if (Mathf.Abs(vyNew) != 0)
			{
				bool top = (vyNew >= 0.0f) ? true : false;
				int vyABS = Mathf.Abs(vyNew);
				for (int i = 0; i < vyABS; i++)
				{
					bool collided = PixelCollisions.Collide(pos.x, pos.y, move.OriginalWidth, move.OriginalHeight, top? DirectionUnit.Top : DirectionUnit.Bottom, move.CollisionLayersMask);
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
					bool collided = PixelCollisions.Collide(pos.x, pos.y, move.OriginalWidth, move.OriginalHeight, right? DirectionUnit.Right : DirectionUnit.Left, move.CollisionLayersMask);

					if(collided){
						move.Velocity.x = 0;
						break;
					}else{
						pos.x += right?1:-1;
					}
				}
			}

            go.transform.position = pos;
            go.transform.localScale = scale.Scale;
            
            move.OnGroundPrev = move.BottomCollided;
			move.OnTopPrev = move.TopCollided;
			move.OnLeftPrev = move.LeftCollided;
			move.OnRightPrev = move.RightCollided;
			move.OnLadderPrev = move.LadderCollided;
			move.BottomCollided = PixelCollisions.Collide(go.transform.position.x,go.transform.position.y,move.OriginalWidth,move.OriginalHeight,DirectionUnit.Bottom,move.CollisionLayersMask);
			move.LeftCollided = PixelCollisions.Collide(go.transform.position.x,go.transform.position.y,move.OriginalWidth,move.OriginalHeight,DirectionUnit.Left,move.CollisionLayersMask);
			move.TopCollided =  PixelCollisions.Collide(go.transform.position.x,go.transform.position.y,move.OriginalWidth,move.OriginalHeight,DirectionUnit.Top,move.CollisionLayersMask);
			move.RightCollided = PixelCollisions.Collide(go.transform.position.x,go.transform.position.y,move.OriginalWidth,move.OriginalHeight,DirectionUnit.Right,move.CollisionLayersMask);
            //Bounds b = go.GetComponent<Collider2D>().bounds;
			//move.LadderCollided = Physics2D.OverlapArea(b.min, b.max, laddersMask) != null ? true : false;


            /*
            var pos = go.transform.position;
            // Resolve any possible collisions below and above the entity.
            if (vyNew != 0) {
                vyNew = (int)yAxisCollisions(move, coll, vyNew, Mathf.Sign(vxNew), pos);
            }
            // Resolve any possible collisions left and right of the entity.
            if (vxNew != 0)
            {
                vxNew = (int)xAxisCollisions(move, coll, vxNew, pos);
            }
            go.transform.position = new Vector2(pos.x + vxNew, pos.y + vyNew);
            */
        }

        private float xAxisCollisions(MovementComponent move, BoxCollider2D coll, float deltaX, Vector3 entityPosition)
        {
            bool sideCollision = false;
            float i;
            // If we are on the ground, perform just three, normal sized raycasts.
            if (move.Grounded)
            {
                i = 0;
                // Else, perform a larger range of raycasts that extend slightly outside of
                // the box collider in order to prevent falling through corners in the Collisions layermask.
            }
            else
            {
                i = -0.5f;
            }
            for (; i < 3; ++i)
            {
                float dirX = Mathf.Sign(deltaX);

                float x = entityPosition.x + coll.offset.x + coll.size.x / 2 * dirX - skin * dirX;
                float y = (entityPosition.y + coll.offset.y - coll.size.y / 2) + coll.size.y / 2 * i;
                if (i == 0) {
                    y += skin;
                } else {
                    y -= skin;
                }

                RaycastHit2D hit;
                Ray2D rayX = new Ray2D(new Vector2(x, y), new Vector2(dirX, 0));

                Debug.DrawRay(rayX.origin, rayX.direction, Color.cyan);

                hit = Physics2D.Raycast(rayX.origin, rayX.direction, Mathf.Abs(deltaX), move.CollisionLayersMask);
                if (hit.collider != null)
                {
                    Debug.Log(hit.point);
                    Debug.DrawRay(rayX.origin, rayX.direction, Color.red);
                    deltaX = 0;
                    sideCollision = true;
                    break;
                }
            }

            return deltaX;
        }

        private float yAxisCollisions(MovementComponent move, BoxCollider2D coll, float deltaY, float dirX, Vector3 entityPosition)
        {
            move.Grounded = false;
            // To prevent falling through collision layers by a gap in the corner
            // if we are facing right, peform y-axis raycasts starting on the right.
            int facingRight = 1;
            if (dirX == facingRight)
            {
                for (int i = 2; i > -1; --i)
                {
                    if (yAxisRaycasts(move, coll, i, ref deltaY, entityPosition))
                    {
                        break;
                    }
                }
                // else we are facing left, peform y-axis raycasts starting on the left
            }
            else
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (yAxisRaycasts(move, coll, i, ref deltaY, entityPosition))
                    {
                        break;
                    }
                }
            }

            return deltaY;
        }

        private bool yAxisRaycasts(MovementComponent move, BoxCollider2D coll, int i, ref float deltaY, Vector3 entityPosition)
        {
            float dirY = Mathf.Sign(deltaY);
            // Start at the left or the right of the boxCollider, depending on the value of i.
            float x = (entityPosition.x + coll.offset.x - coll.size.x / 2) + coll.size.x / 2 * i;
            // Bottom or top of boxCollider, depending on if dirY is positive or negative
            float y = entityPosition.y + coll.offset.y + coll.size.y / 2 * dirY;

            if (i == 0) {
                x += skin;
            } else {
                x -= skin;
            }

            RaycastHit2D hit;
            Ray2D ray = new Ray2D(new Vector2(x, y), new Vector2(0, dirY));

            Debug.DrawRay(ray.origin, ray.direction, Color.magenta);

            hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Abs(deltaY), move.CollisionLayersMask);
            if (hit.collider != null)
            {
                Debug.Log(ray.origin + " - " + ray.direction + " - " + hit.point);
                Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
                // Get Distance between entity and ground
                float distance = Vector2.Distance(ray.origin, hit.point);
                // Stop entity's downward movement after coming within skin width of a boxCollider
                if (distance > skin)
                {
                    deltaY = distance * dirY;// + skin;
                }
                else
                {
                    deltaY = 0;
                }
                move.Grounded = true;
                return true;
            }

            return false;
        }

    }
}

