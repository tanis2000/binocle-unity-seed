using Binocle.AI.FSM;
using Binocle.Sprites;
using UnityEngine;

namespace App.Platformer
{
    public class Cat : Actor
    {
        public override bool OnBallHit(Ball ball) {
            ball.ReturnHome();
            return true;
        }
    }
}