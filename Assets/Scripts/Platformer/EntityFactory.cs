using Binocle;
using UnityEngine;
using App;

namespace App.Platformer
{
    public static class EntityFactory
    {
        public static Game Game;
        public static Entity CreatePlayer(Vector2 startingPosition)
        {
            var e = Game.Scene.CreateEntity("player");
            e.gameObject.layer = LayerMask.NameToLayer("Heroes");
            e.AddComponent<InputComponent>();
            var move = e.AddComponent<MovementComponent>();
            move.MaxVelocity = new Vector2(80, 80);
            move.CollisionLayersMask = 1 << LayerMask.NameToLayer("Blocks");
            var sr = e.AddComponent<SpriteRenderer>();
            sr.sprite = Utils.CreateBoxSprite(8, 8, new Color(1, 1, 1, 1));
            var c = e.AddComponent<BoxCollider2D>();
            c.size = new Vector2(8, 8);
            e.AddComponent<PlayerControlComponent>();
            e.transform.position = startingPosition;
            return e;
        }

        public static Entity CreateMap(Map.Level level)
        {
            var e = Game.Scene.CreateEntity("map");
            e.gameObject.layer = LayerMask.NameToLayer("Blocks");
            var sr = e.AddComponent<SpriteRenderer>();
            sr.sprite = Utils.CreateBoxSprite(16, 8, new Color(1, 0, 0, 1));
            var c = e.AddComponent<BoxCollider2D>();
            c.size = new Vector2(16, 8);
            e.transform.position = new Vector2(120, 80);
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