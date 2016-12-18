using Binocle.Components;

namespace App.Platformer
{
    public class InputComponent : BaseMonoBehaviour
    {
        public bool Left;
        public bool Right;
        public bool Jump;
        public bool Down;
        public bool Fire;

        public bool WasLeft;
        public bool WasRight;
        public bool WasJump;
        public bool WasDown;
        public bool WasFire;

    }
}