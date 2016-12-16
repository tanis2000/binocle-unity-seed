using Binocle;
using Binocle.Processors;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    public class MainScene : Scene
    {
        public static MainScene Instance;

        public Entity Map;

        public bool SetupDone = false;

        public Text VersionText;

        public override void Awake()
        {
            base.Awake();
            Instance = this;
            Debug.Log("MainScene - Awake");
        }

        public override void Update()
        {
            base.Update();
            if (!SetupDone)
            {
                // Everything that needs at least the processors to be set up should go in there
                var level = EntityFactory.LoadLevel(1);
                EntityFactory.CreatePlayer();
                Map = EntityFactory.CreateMap(level);
                // Hack to force initilization on assignment
                GetEntityProcessor<MapProcessor>().Map = Map.GetComponent<MapComponent>();
                //EntityFactory.CreateStartingUnits(Map);
                EntityFactory.CreateTownHall(new Vector2(2, 7), Map);
                EntityFactory.CreateBoat(new Vector2(8, 0), Map);

                SetupDone = true;
            }
        }

        public void Setup(bool newGame)
        {
            VersionText = GameObject.Find("VersionText").GetComponent<Text>();
            VersionText.text = ((App.Game)Game).Version;

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
            AddEntityProcessor(new MapProcessor(new Matcher().All(typeof(MapComponent))));

            // Input/logic
            AddEntityProcessor(new InputProcessor(new Matcher().All(typeof(InputComponent))));
            AddEntityProcessor(new AIProcessor(new Matcher().All(typeof(AIComponent))));

            // Physics
            AddEntityProcessor(new ScaleProcessor(new Matcher().All(typeof(ScaleComponent))));
            AddEntityProcessor(new PositionProcessor(new Matcher().All(typeof(PositionComponent))));
            /*			AddEntityProcessor (new PhysicsProcessor ());
                        AddEntityProcessor(new MovementProcessor(new Matcher ().All (typeof(MovementComponent))));
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