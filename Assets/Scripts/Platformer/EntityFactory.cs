using Binocle;
using UnityEngine;
using App;

namespace App.Platformer
{
    public static class EntityFactory
    {
        public static Game Game;
        public static Entity CreatePlayer()
        {
            var e = Game.Scene.CreateEntity("player");
            return e;
        }

        public static Entity CreateMap(Map.Level level)
        {
          var e = Game.Scene.CreateEntity("map");
          return e;
        }

        public static Map.Level LoadLevel(int number)
        {
            var fileName = string.Format("Levels/platformer-{0:D4}", number);
            Debug.Log(fileName);
            var json = Resources.Load<TextAsset>(fileName);
            Debug.Log(json);
            var l = JsonUtility.FromJson<Map.Level>(json.ToString());
            return l;
        }

        /// <summary>
        /// Actually just dumps to the console the representation of a Level as a JSON string
        /// </summary>
        /// <param name="level">the level to save</param>
        public static void SaveLevel(Map.Level level)
        {
            var l = JsonUtility.ToJson(level);
            Debug.Log(l);
        }

    }

}