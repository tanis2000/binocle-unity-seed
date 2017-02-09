using Binocle;
using UnityEngine;
using App;
using Binocle.TileMaps;
using Binocle.Sprites;

namespace App.Platformer
{
    public static class EntityFactory
    {
        public static Game Game;
        public static EntityPool<Bullet> BulletsPool;

        public static void Init()
        {
            BulletsPool = new EntityPool<Bullet>(Game.Scene);
            BulletsPool.WarmCache(10);
        }

        public static Entity CreatePlayer(Entity parent, Vector2 startingPosition)
        {
            var e = Game.Scene.CreateEntity<Hero>("player");
            e.SetParent(parent);
            e.gameObject.layer = LayerMask.NameToLayer("Heroes");
            e.SpawnPosition = startingPosition;
            e.AddComponent<InputComponent>();
            e.CollisionLayersMask = 1 << LayerMask.NameToLayer("Blocks");
            var ce = Game.Scene.CreateEntity("sprite");
            ce.SetParent(e);
            var sr = ce.AddComponent<SpriteRenderer>();
            //sr.sprite = Utils.CreateBoxSprite(8, 8, new Color(1, 1, 1, 1));
            var c = e.AddComponent<BoxCollider2D>();
            c.size = new Vector2(10, 10);
            ce.AddComponent<ScaleComponent>();
            var spriteAnimator = ce.AddComponent<SpriteAnimator>();
            SpriteAnimation sa = new SpriteAnimation ();
            sa.fps = 6;
            var res = Resources.LoadAll<Sprite>("Sprites/hero-idle");
            sa.AddFrame(res[0]);
            sa.AddFrame(res[1]);
            sa.name = "idle";
            sa.id = 0;
            sa.sequenceCode = "0-1:forever";
            spriteAnimator.AddAnimation(sa);

            sa = new SpriteAnimation ();
            sa.fps = 6;
            res = Resources.LoadAll<Sprite>("Sprites/hero-die");
            sa.AddFrame(res[0]);
            sa.AddFrame(res[1]);
            sa.AddFrame(res[2]);
            sa.name = "die";
            sa.id = 1;
            sa.sequenceCode = "0-2";
            spriteAnimator.AddAnimation(sa);

            spriteAnimator.Play("idle");

            var ball = EntityFactory.CreateBall(e);
            e.Ball = ball as Ball;

            e.transform.localPosition = startingPosition;
            return e;
        }

        public static Entity CreateMapFromJson(Map.Map map)
        {
            var e = Game.Scene.CreateEntity("map");
            e.transform.position = new Vector2(4, 4);
            for (int x = 0; x < map.Level.Width; x++)
            {
                for (int y = 0; y < map.Level.Height; y++)
                {
                    if (map.Level.Tiles[x + y * map.Level.Width] == 1)
                    {
                        var en = Game.Scene.CreateEntity<Solid>(string.Format("tile-{0}-{1}", x, y));
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
                        var en = Game.Scene.CreateEntity<MovingPlatform>(string.Format("moving-{0}-{1}", x, y));
                        en.SetParent(e);
                        en.gameObject.layer = LayerMask.NameToLayer("Blocks");
                        en.CollisionLayersMask = 1 << LayerMask.NameToLayer("Heroes");
                        var sr = en.AddComponent<SpriteRenderer>();
                        sr.sprite = Utils.CreateBoxSprite(8*2, 8, new Color(0, 1, 1, 1));
                        var c = en.AddComponent<BoxCollider2D>();
                        c.size = new Vector2(8*2, 8);
                        en.AddComponent<SolidComponent>();
                        en.transform.localPosition = new Vector2(x * 8 - 4, (map.Level.Height - 1 - y) * 8);
                        en.End = new Vector2(en.transform.position.x + 8*3, en.transform.position.y);
                    }
                    else if (map.Level.Tiles[x + y * map.Level.Width] == 4) {
                        var en = Game.Scene.CreateEntity<MovingPlatform>(string.Format("moving-{0}-{1}", x, y));
                        en.SetParent(e);
                        en.gameObject.layer = LayerMask.NameToLayer("Blocks");
                        en.CollisionLayersMask = 1 << LayerMask.NameToLayer("Heroes");
                        var sr = en.AddComponent<SpriteRenderer>();
                        sr.sprite = Utils.CreateBoxSprite(8*2, 8, new Color(0, 1, 1, 1));
                        var c = en.AddComponent<BoxCollider2D>();
                        c.size = new Vector2(8*2, 8);
                        en.AddComponent<SolidComponent>();
                        en.transform.localPosition = new Vector2(x * 8 - 4, (map.Level.Height - 1 - y) * 8);
                        en.End = new Vector2(en.transform.position.x, en.transform.position.y + 8*3);
                    }
                }
            }
            return e;
        }

        public static TileMap CreateTileMapFromTiledMap(Binocle.Importers.Tiled.TmxMap map, Texture2D tiles)
        {
            TileMap tm = new TileMap(Game.Scene, map.Width, map.Height);
            tm.tileSet = new TileSet(tiles, map.TileWidth, map.TileHeight);
            foreach (var layer in map.Layers) {
                var l = tm.addLayer(layer.Name);
                foreach(var tile in layer.Tiles) {
                    l.addTile(tile.X, tile.Y, tile.Gid);
                }
            }
            return tm;
        }

        public static Map.Map CreateMapFromTiledMap(Binocle.Importers.Tiled.TmxMap map, Entity parent) 
        {
            var m = new Map.Map();
            foreach(var layer in map.ObjectGroups) {
                if (layer.Name == "players") {
                    foreach(var obj in layer.Objects) {
                        if (obj.Name == "player1") {
                            m.PlayerSpawnPosition = new Vector2((float)obj.X, (map.Height - 1) * map.TileHeight - (float)obj.Y);
                            //Debug.Log(m.PlayerSpawnPosition);
                        }
                    }
                } else if (layer.Name == "turrets") {
                    foreach(var obj in layer.Objects) {
                        var pos = new Vector2((float)obj.X, (map.Height - 1) * map.TileHeight - (float)obj.Y);
                        CreateTurret(parent, pos);
                    }
                }
            }
            return m;
        }

        public static Entity CreateMapFromTiledMap(TileMap map, ref Vector2 PlayerSpawnPosition)
        {
            var e = Game.Scene.CreateEntity("map");
            e.transform.position = new Vector2(map.tileSet.TileWidth/2, map.tileSet.TileHeight/2);
            PlayerSpawnPosition = new Vector2(1 * map.tileSet.TileWidth, 1 * map.tileSet.TileHeight);
            foreach(var layer in map.layers) {
                for (int x = 0; x < map.width; x++)
                {
                    for (int y = 0; y < map.height; y++)
                    {
                        var tile = layer.getTile(x, y);
                        if (layer.name == "walls" && tile.TileId != 0)
                        {
                            var en = Game.Scene.CreateEntity<Solid>(string.Format("tile-{0}-{1}", x, y));
                            en.SetParent(e);
                            en.gameObject.layer = LayerMask.NameToLayer("Blocks");
                            var sr = en.AddComponent<SpriteRenderer>();
                            //sr.sprite = Utils.CreateBoxSprite(8, 8, new Color(0, 1, 0, 1));
                            sr.sprite = map.tileSet.GetSprite(tile.TileId-1);
                            var c = en.AddComponent<BoxCollider2D>();
                            c.size = new Vector2(map.tileWidth, map.tileHeight);
                            en.transform.localPosition = new Vector2(x * map.tileWidth, (map.height - 1 - y) * map.tileHeight);
                        }
                        else if (layer.name == "movingh" && tile.TileId != 0) {
                            var en = Game.Scene.CreateEntity<MovingPlatform>(string.Format("moving-{0}-{1}", x, y));
                            en.SetParent(e);
                            en.gameObject.layer = LayerMask.NameToLayer("Blocks");
                            en.CollisionLayersMask = 1 << LayerMask.NameToLayer("Heroes");
                            for (int i = -1 ; i <= 1 ; i+=2) {
                                var sen = Game.Scene.CreateEntity<Entity>("sprite");
                                sen.SetParent(en);
                                sen.transform.localPosition = new Vector2(map.tileWidth/2*i, 0);
                                var sr = sen.AddComponent<SpriteRenderer>();
                                sr.sprite = map.tileSet.GetSprite(tile.TileId-1);
                            }
                            var c = en.AddComponent<BoxCollider2D>();
                            c.size = new Vector2(map.tileWidth*2, map.tileHeight);
                            en.AddComponent<SolidComponent>();
                            en.transform.localPosition = new Vector2(x * map.tileWidth - map.tileWidth/2, (map.height - 1 - y) * map.tileHeight);
                            en.End = new Vector2(en.transform.position.x + map.tileWidth*3, en.transform.position.y);
                        }
                        else if (layer.name == "movingv" && tile.TileId != 0) {
                            var en = Game.Scene.CreateEntity<MovingPlatform>(string.Format("moving-{0}-{1}", x, y));
                            en.SetParent(e);
                            en.gameObject.layer = LayerMask.NameToLayer("Blocks");
                            en.CollisionLayersMask = 1 << LayerMask.NameToLayer("Heroes");
                            for (int i = -1 ; i <= 1 ; i+=2) {
                                var sen = Game.Scene.CreateEntity<Entity>("sprite");
                                sen.SetParent(en);
                                sen.transform.localPosition = new Vector2(map.tileWidth/2*i, 0);
                                var sr = sen.AddComponent<SpriteRenderer>();
                                sr.sprite = map.tileSet.GetSprite(tile.TileId-1);
                            }
                            var c = en.AddComponent<BoxCollider2D>();
                            c.size = new Vector2(map.tileWidth*2, map.tileHeight);
                            en.AddComponent<SolidComponent>();
                            en.transform.localPosition = new Vector2(x * map.tileWidth - map.tileWidth/2, (map.height - 1 - y) * map.tileHeight);
                            en.End = new Vector2(en.transform.position.x, en.transform.position.y + map.tileHeight*3);
                        }
                        else if (layer.name == "bg" && tile.TileId != 0) {
                            var en = Game.Scene.CreateEntity<Entity>(string.Format("bg-{0}-{1}", x, y));
                            en.SetParent(e);
                            en.gameObject.layer = LayerMask.NameToLayer("Backgrounds");
                            var sr = en.AddComponent<SpriteRenderer>();
                            sr.sortingOrder = -1;
                            sr.sprite = map.tileSet.GetSprite(tile.TileId-1);
                            en.transform.localPosition = new Vector2(x * map.tileWidth, (map.height - 1 - y) * map.tileHeight);
                        }
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

        public static Entity CreateTurret(Entity parent, Vector2 startingPosition)
        {
            var e = Game.Scene.CreateEntity<Turret>("turret");
            e.SetParent(parent);
            e.gameObject.layer = LayerMask.NameToLayer("Enemies");
            e.CollisionLayersMask = 1 << LayerMask.NameToLayer("Blocks");
            var ce = Game.Scene.CreateEntity("sprite");
            ce.SetParent(e);
            var sr = ce.AddComponent<SpriteRenderer>();
            //sr.sprite = Utils.CreateBoxSprite(8, 8, new Color(0.8f, 0.8f, 0, 1));
            var c = e.AddComponent<BoxCollider2D>();
            c.size = new Vector2(8, 8);
            ce.AddComponent<ScaleComponent>();
            e.transform.localPosition = startingPosition;
            var spriteAnimator = ce.AddComponent<SpriteAnimator>();
            SpriteAnimation sa = new SpriteAnimation ();
            sa.fps = 6;
            var res = Resources.LoadAll<Sprite>("Sprites/turret");
            sa.AddFrame(res[0]);
            sa.AddFrame(res[1]);
            sa.AddFrame(res[2]);
            sa.AddFrame(res[3]);
            sa.name = "idle";
            sa.id = 0;
            sa.sequenceCode = "0-3:forever";
            spriteAnimator.AddAnimation(sa);

            sa = new SpriteAnimation ();
            sa.fps = 6;
            res = Resources.LoadAll<Sprite>("Sprites/turret-die");
            sa.AddFrame(res[0]);
            sa.AddFrame(res[1]);
            sa.AddFrame(res[2]);
            sa.name = "die";
            sa.id = 1;
            sa.sequenceCode = "0-2";
            spriteAnimator.AddAnimation(sa);

            spriteAnimator.Play("idle");
            return e;
        }

        public static Entity CreateBullet(Vector2 startingPosition, Vector2 velocity)
        {
            var e = BulletsPool.Obtain("bullet");
            //var e = Game.Scene.CreateEntity<Bullet>("bullet");
            e.SetParent((Game.Scene as PlatformerScene).MapEntity);
            e.Velocity = velocity;
            e.transform.position = startingPosition;
            e.Reset();
            //var e = Game.Scene.CreateEntity<Bullet>("bullet");
            /*            
            e.gameObject.layer = LayerMask.NameToLayer("Bullets");
            e.CollisionLayersMask = 1 << LayerMask.NameToLayer("Heroes") | 1 << LayerMask.NameToLayer("Blocks");
            e.Velocity = velocity;
            var ce = Game.Scene.CreateEntity("sprite");
            ce.SetParent(e);
            var sr = ce.AddComponent<SpriteRenderer>();
            sr.sprite = Utils.CreateBoxSprite(4, 4, new Color(0, 0.8f, 0.8f, 1));
            var c = e.AddComponent<BoxCollider2D>();
            c.size = new Vector2(4, 4);
            ce.AddComponent<ScaleComponent>();
            e.transform.position = startingPosition;
            */
            return e;
        }

        public static Entity CreateBall(Hero hero)
        {
            var e = Game.Scene.CreateEntity<Ball>("ball");
            e.gameObject.layer = LayerMask.NameToLayer("Balls");
            e.CollisionLayersMask = 1 << LayerMask.NameToLayer("Blocks") | 1 << LayerMask.NameToLayer("Enemies");
            e.Hero = hero;
            var ce = Game.Scene.CreateEntity("sprite");
            ce.SetParent(e);
            var sr = ce.AddComponent<SpriteRenderer>();
            sr.sprite = Utils.CreateBoxSprite(6, 6, new Color(1, 1f, 1f, 1));
            sr.sortingOrder = 1;
            var c = e.AddComponent<BoxCollider2D>();
            c.size = new Vector2(6, 6);
            ce.AddComponent<ScaleComponent>();
            return e;
        }

        public static Entity CreatePuff(Vector2 position)
        {
            var e = Game.Scene.CreateEntity<Puff>("puff");
            var go = GameObject.Instantiate(Resources.Load("Particles/Dust"), Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(e.transform);
            e.transform.position = position;
            return e;
        }

        public static Entity CreateRain(Vector2 position, float width)
        {
            var e = Game.Scene.CreateEntity<Rain>("rain");
            var go = GameObject.Instantiate(Resources.Load("Particles/Rain"), Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(e.transform);
            e.transform.position = position;
            var systems = go.GetComponentsInChildren<ParticleSystem>();
            foreach(var ps in systems) {
                var s = ps.shape;
                s.box = new Vector3(width, 1, 1);
            }
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