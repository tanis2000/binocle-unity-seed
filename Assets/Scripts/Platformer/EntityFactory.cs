using Binocle;
using UnityEngine;
using App;

namespace App.Platformer
{
    public static class EntityFactory
    {
        public static Game Game;
        public static Entity CreatePlayer(Entity parent, Vector2 startingPosition)
        {
            var e = Game.Scene.CreateEntity("player");
            e.SetParent(parent);
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
            e.AddComponent<ScaleComponent>();
            e.transform.localPosition = startingPosition;
            return e;
        }

        public static Entity CreateMap(Map.Map map)
        {
            var e = Game.Scene.CreateEntity("map");
            e.transform.position = new Vector2(4, 4);
            for (int x = 0; x < map.Level.Width; x++)
            {
                for (int y = 0; y < map.Level.Height; y++)
                {
                    if (map.Level.Tiles[x + y * map.Level.Width] == 1)
                    {
                        var en = Game.Scene.CreateEntity(string.Format("tile-{0}-{1}", x, y));
                        en.SetParent(e);
                        en.gameObject.layer = LayerMask.NameToLayer("Blocks");
                        var sr = en.AddComponent<SpriteRenderer>();
                        sr.sprite = Utils.CreateBoxSprite(8, 8, new Color(0, 1, 0, 1));
                        var c = en.AddComponent<BoxCollider2D>();
                        c.size = new Vector2(8, 8);
                        en.transform.localPosition = new Vector2(x * 8, (map.Level.Height - 1 - y) * 8);
                    }
                    else if (map.Level.Tiles[x + y * map.Level.Width] == 2)
                    {
                        map.PlayerSpawnPosition = new Vector2(x * 8, (map.Level.Height - 1 - y) * 8);
                    }
                    else if (map.Level.Tiles[x + y * map.Level.Width] == 3) {
                        var en = Game.Scene.CreateEntity(string.Format("moving-{0}-{1}", x, y));
                        en.SetParent(e);
                        en.gameObject.layer = LayerMask.NameToLayer("Blocks");
                        var sr = en.AddComponent<SpriteRenderer>();
                        sr.sprite = Utils.CreateBoxSprite(8, 8, new Color(0, 1, 1, 1));
                        var c = en.AddComponent<BoxCollider2D>();
                        c.size = new Vector2(8, 8);
                        en.AddComponent<SolidComponent>();
                        en.transform.localPosition = new Vector2(x * 8, (map.Level.Height - 1 - y) * 8);
                    }
                }
            }
            return e;
        }
        public static void CreateCamera(Transform target)
        {
            var cameraFollow = Camera.main.gameObject.AddComponent<CameraFollow>();
            cameraFollow.target = target;
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