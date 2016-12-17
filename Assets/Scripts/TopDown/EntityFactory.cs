using Binocle;
using UnityEngine;
using App;

namespace App.TopDown
{
    public static class EntityFactory
    {
        public static Game Game;
        public static Vector2 MapGridSize = new Vector2(9, 9);
        public static Vector2 MapSize = new Vector2(MapGridSize.x * 16, MapGridSize.y * 16);

        public static Entity CreatePlayer()
        {
            var e = Game.Scene.CreateEntity("player");
            e.AddComponent<PlayerStatsComponent>();
            return e;
        }

        public static Entity CreateMap(Level level)
        {
            var e = Game.Scene.CreateEntity("map");
            //e.transform.position = new Vector2((Game.DesignWidth-16*9)/2 + 16/2, 32);
            e.transform.position = new Vector2(Game.DesignWidth / 2, Game.DesignHeight / 2);
            var bc = e.AddComponent<BoxCollider2D>();
            bc.size = MapSize;
            var gc = e.AddComponent<MapComponent>();
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    var lc = level.tiles[x + y * 9];
                    var ci = new CellInfo();
                    switch (lc)
                    {
                        case 0:
                        default:
                            {
                                ci.CellType = CellType.None;
                                break;
                            }
                        case 1:
                            {
                                ci.CellType = CellType.Water;
                                break;
                            }
                        case 2:
                            {
                                ci.CellType = CellType.Earth;
                                break;
                            }
                    }
                    var bg = CreateBackgroundTile(x, y, ci.CellType, e);
                    ci.Tile = bg;
                    gc.Cells[x, y] = ci;
                }
            }
            return e;
        }

        public static Entity CreateBackgroundTile(int column, int row, CellType cellType, Entity parent)
        {
            if (cellType == CellType.None) return null;

            var e = Game.Scene.CreateEntity("background-tile-" + column + "-" + row);
            e.SetParent(parent);
            var sr = e.AddComponent<SpriteRenderer>();
            var pos = e.AddComponent<PositionComponent>();
            pos.Position = new Vector2((-MapGridSize.x / 2 + column) * 16 + 8, (-MapGridSize.y / 2 + row) * 16 + 8);
            float r = column / 9.0f;
            float g = row / 9.0f;
            float b = 1;
            float a = 1;
            if (cellType == CellType.Earth)
            {
                r = 0.1f;
                g = 0.4f;
                b = 0.1f;
            }
            else if (cellType == CellType.Water)
            {
                r = 0.0f;
                g = 0.1f;
                b = 0.4f;
            }

            sr.sprite = Utils.CreateBoxSprite(16, 16, new Color(r, g, b, a));
            return e;
        }

        public static Entity CreateTownHall(Vector2 gridPosition, Entity parent)
        {
            var e = Game.Scene.CreateEntity("townhall");
            e.SetParent(parent);
            var gp = e.AddComponent<GridPositionComponent>();
            gp.Position = gridPosition;
            var unit = e.AddComponent<UnitComponent>();
            unit.UnitType = UnitType.TownHall;
            var hp = e.AddComponent<HealthComponent>();
            hp.MaxHealth = 10;
            hp.Health = hp.MaxHealth;
            var sr = e.AddComponent<SpriteRenderer>();
            sr.sprite = Utils.CreateBoxSprite(8, 8, new Color(1, 1, 1, 1));
            sr.sortingLayerName = "units";
            var pos = e.AddComponent<PositionComponent>();
            return e;
        }

        public static Entity CreateBoat(Vector2 gridPosition, Entity parent)
        {
            var e = Game.Scene.CreateEntity("boat");
            e.SetParent(parent);
            var gp = e.AddComponent<GridPositionComponent>();
            gp.Position = gridPosition;
            var unit = e.AddComponent<UnitComponent>();
            unit.UnitType = UnitType.Boat;
            unit.SailUnit = true;
            var hp = e.AddComponent<HealthComponent>();
            hp.MaxHealth = 5;
            hp.Health = hp.MaxHealth;
            var sr = e.AddComponent<SpriteRenderer>();
            sr.sprite = Utils.CreateBoxSprite(8, 4, new Color(1, 0, 0, 1));
            sr.sortingLayerName = "units";
            var pos = e.AddComponent<PositionComponent>();
            return e;
        }


        public static Level LoadLevel(int number)
        {
            var fileName = string.Format("Levels/{0:D4}", number);
            Debug.Log(fileName);
            var json = Resources.Load<TextAsset>(fileName);
            Debug.Log(json);
            var l = JsonUtility.FromJson<Level>(json.ToString());
            return l;
        }

        /// <summary>
        /// Actually just dumps to the console the representation of a Level as a JSON string
        /// </summary>
        /// <param name="level">the level to save</param>
        public static void SaveLevel(Level level)
        {
            var l = JsonUtility.ToJson(level);
            Debug.Log(l);
        }

    }

}