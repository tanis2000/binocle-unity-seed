using UnityEngine;
using UnityEngine.UI;
using Binocle;

namespace App
{

    public enum GameState
    {
        MainMenu,
        PlayTopDown,
        PlayPlatformer,
        GameOver,
        Quit,
        LevelStats
    }

    public class Game : Binocle.Game
    {
        public GameState gameState;

        public GameObject WorldCanvas;
        public GameObject GUICanvas;
        public CanvasScaler GUIScaler;

        /// <summary>
        /// The current scene being in use
        /// </summary>
        public Scene Scene;

        public bool mainSceneSetup = false;
        public bool platformerSceneSetup = false;

        public bool continueSelected = false;

        public string Version = "Unknown";

        public override void Start()
        {
            DesignWidth = 320;
            DesignHeight = 240;
            base.Start();
            TopDown.EntityFactory.Game = this;
            Platformer.EntityFactory.Game = this;

            Version = Utils.GetVersion();
            Debug.LogFormat("Game {0}", Version);

            WorldCanvas = GameObject.Find("WorldCanvas");
            GUICanvas = GameObject.Find("GUICanvas");
            GUIScaler = GUICanvas.GetComponent<CanvasScaler>();

            gameState = GameState.MainMenu;

            SocialManager.Instance.Initialize();

            TouchKit.instance.designTimeResolution = new Vector2(DesignWidth, DesignHeight);
        }

        public override void Update()
        {
            base.Update();

            GUIScaler.scaleFactor = pixelCamera.Scaling;

            switch (gameState)
            {
                case GameState.MainMenu:
                    {
                        if (Scene == null || Scene != MainMenuScene.Instance)
                        {
                            if (Scene != null)
                            {
                                Scene.Remove();
                            }
                            Scene = CreateScene<MainMenuScene>("MainMenuScene");
                        }
                    }
                    break;
                case GameState.PlayTopDown:
                    {

                        if (Application.isEditor && Input.GetKeyUp(KeyCode.Alpha1))
                        {
                            gameState = GameState.LevelStats;
                        }

                        bool newGame = true;
                        if (Scene == null || Scene != MainScene.Instance)
                        {
                            if (!mainSceneSetup)
                            {
                                if (Scene != null)
                                {
                                    Scene.Remove();
                                }
                                Scene = CreateScene<MainScene>("MainScene");
                                MainScene.Instance.Setup(newGame);
                                mainSceneSetup = true;
                            }
                        }
                    }
                    break;

                case GameState.PlayPlatformer:
                    {
                        bool newGame = true;
                        if (Scene == null || Scene != PlatformerScene.Instance)
                        {
                            if (!platformerSceneSetup)
                            {
                                if (Scene != null)
                                {
                                    Scene.Remove();
                                }
                                Scene = CreateScene<PlatformerScene>("PlatformerScene");
                                PlatformerScene.Instance.Setup(newGame);
                                platformerSceneSetup = true;
                            }
                        }
                    }
                    break;

                /*
      case GameState.PlayerDead:
        {
          if (Input.GetKeyUp (KeyCode.Space)) {
            Debug.Log("switching to gameover");
            gameState = GameState.GameOver;
          }
        }
        break;
      case GameState.GameOver:
        {
          // TODO: add a stats scene
          Debug.Log("inside gameover");
          gameState = GameState.MainMenu;
          mainSceneSetup = false;
        }
        break;*/
                case GameState.Quit:
                    {
                        Application.Quit();
                    }
                    break;
                    /*
                case GameState.LevelStats:
                    {
                        mainSceneSetup = false;
                        if (scene == null || scene != LevelStatsScene.instance) {
                            scene.Remove();
                            scene = CreateScene<LevelStatsScene>("LevelStatsScene");
                        }
                    }
                    break;*/
            }
        }

    }
}
