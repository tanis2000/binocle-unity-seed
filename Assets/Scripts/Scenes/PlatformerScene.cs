using App.Platformer.Map;
using Binocle;
using Binocle.Importers.Tiled;
using Binocle.Processors;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    public class PlatformerScene : Scene
    {
        public static PlatformerScene Instance;

        public Entity MapEntity;

        public bool SetupDone = false;

        public Text VersionText;
        public Map Map;
        public Entity Hero;
        public Texture2D TilesTexture;
        public Vector2 PlayerSpawnPosition = Vector2.zero;


        public override void Awake()
        {
            base.Awake();
            Instance = this;
            Debug.Log("PlatformerScene - Awake");
        }

        public override void Update()
        {
            base.Update();
            if (!SetupDone)
            {
                // Everything that needs at least the processors to be set up should go in there
                var mapFile = Resources.Load<TextAsset>("Levels/level-002");
                Debug.Log(mapFile);
                var tiledMap = new TmxMap(mapFile.text);
                Debug.Log(tiledMap);
                var tm = Platformer.EntityFactory.CreateTileMapFromTiledMap(tiledMap, TilesTexture);
                Debug.Log(tm.tileSet.GetSprite(0));
                MapEntity = Platformer.EntityFactory.CreateMapFromTiledMap(tm, ref PlayerSpawnPosition);
                Platformer.EntityFactory.CreateRain(new Vector2((MapEntity.transform.position.x + tiledMap.Width*tiledMap.TileWidth) / 2, MapEntity.transform.position.y + tiledMap.Height*tiledMap.TileHeight), tiledMap.Width*tiledMap.TileWidth);
                Map = Platformer.EntityFactory.CreateMapFromTiledMap(tiledMap, MapEntity);
                Hero = Platformer.EntityFactory.CreatePlayer(MapEntity, Map.PlayerSpawnPosition);
                //var level = Platformer.EntityFactory.LoadLevel(1);
                //Map = new Map();
                //Map.Level = level;
                //MapEntity = Platformer.EntityFactory.CreateMapFromJson(Map);
                //Hero = Platformer.EntityFactory.CreatePlayer(MapEntity, Map.PlayerSpawnPosition);
                Platformer.EntityFactory.CreateCamera(Hero.gameObject.transform);
                // Hack to force initilization on assignment
                //GetEntityProcessor<MapProcessor>().Map = Map.GetComponent<MapComponent>();

                SetupDone = true;
            }
        }

        public void Setup(bool newGame)
        {
            VersionText = GameObject.Find("VersionText").GetComponent<Text>();
            VersionText.text = ((App.Game)Game).Version;
            Application.targetFrameRate = 60; // we try to chieve 60 FPS
            Platformer.EntityFactory.Init();
            
            TilesTexture = Resources.Load ("Sprites/tileset3", typeof(Texture2D)) as Texture2D;

            /*
                  gameOverText = GameObject.Find("gameover-label").GetComponent<Text>();
                  scoreText = GameObject.Find("score-label").GetComponent<Text>();
                  levelText = GameObject.Find("level-label").GetComponent<Text>();
                  energyText = GameObject.Find("energy-label").GetComponent<Text>();

                  gameOverText.enabled = false;
                  scoreText.enabled = true;
                  levelText.enabled = true;
                  energyText.enabled = true;

                  if (newGame) {
                      new StatsManager (this);
                      StatsManager.instance.reset ();
                  }*/

            new AudioManager();

            // Map stuff
            //AddEntityProcessor(new MapProcessor(new Matcher().All(typeof(MapComponent))));

            // Input/logic
            AddEntityProcessor(new Platformer.InputProcessor(new Matcher().All(typeof(Platformer.InputComponent))));
            //AddEntityProcessor(new AIProcessor(new Matcher().All(typeof(AIComponent))));
            //AddEntityProcessor(new Platformer.PlayerControlProcessor(new Matcher().All(typeof(Platformer.PlayerControlComponent))));

            // Physics
            //AddEntityProcessor(new ScaleProcessor(new Matcher().All(typeof(ScaleComponent))));
            //AddEntityProcessor(new PositionProcessor(new Matcher().All(typeof(PositionComponent))));
            //AddEntityProcessor (new PhysicsProcessor ());
            //AddEntityProcessor(new Platformer.MovementProcessor(new Matcher ().All (typeof(Platformer.MovementComponent))));
                        /*
                        AddEntityProcessor(new BouncingProcessor(new Matcher ().All (typeof(BouncingComponent))));			

                        // Death stuff
                        AddEntityProcessor(new CatchAreaProcessor(new Matcher ().All (typeof(CatchAreaComponent))));
                        AddEntityProcessor(new LevelProcessor(new Matcher ().All (typeof(LevelComponent))));

                        // Post movement
                        AddEntityProcessor(new BulletCollisionProcessor(new Matcher().All(typeof(BulletComponent))));

                        hero = EntityFactory.createHero(Game.DesignWidth/2, 50);
                        var ca = EntityFactory.createCatchArea();
                        var rnd = new System.Random(42);
                        for (int i = 0 ; i < 50 ; i++) {
                            EntityFactory.createRat(ca, rnd.Next(-Game.DesignWidth/2+16, Game.DesignWidth/2-16), rnd.Next(Game.DesignHeight/2-32, Game.DesignHeight/2-8));
                        }*/


            //var testBlock = EntityFactory.CreateBackgroundTile(0, 0, CellType.Normal);

            Camera.main.gameObject.transform.position = new Vector3(Game.DesignWidth / 2, Game.DesignHeight / 2, -10);
            /*
                        LevelInfo li = Levels[Levels.Count-1];			
                        if (StatsManager.instance.currLevel <= Levels.Count) {
                            li = Levels[StatsManager.instance.currLevel-1];
                        }

                        var lg = new LevelGenerator(this, li);
                        lg.Generate();

                        StatsManager.instance.updateLevel();

                        StatsManager.instance.updateEnergy();

                        var m = CreateEntity("music");
                        var audioSource = m.AddComponent<AudioSource>();
                        audioSource.volume = 0.5f;
                        audioSource.loop = true;
                        audioSource.clip = Resources.Load<AudioClip>("Music/GattoMagicoLevel");
                        audioSource.Play();
            */
        }

    }
}