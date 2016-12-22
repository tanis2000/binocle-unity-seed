using System;

namespace App.Platformer.Map
{
    /// <summary>
    /// The map as it is being serialized/deserialized by the level editor
    /// </summary>
    [Serializable]
    public class Level
    {
        public int Width;
        public int Height;
        public int[] Tiles;
    }
}