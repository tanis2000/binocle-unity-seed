using System;
using Binocle;
using UnityEngine.UI;
using UnityEngine;

namespace App
{
	public class MainMenuScene : Scene
	{
		public static MainMenuScene Instance;

		public Text newGameText;
		public Text continueGameText;
		public Text quitText;
		public Text instructionsText;

		public Text selectedMenu;
		public Text VersionText;

		public override void Start()
		{
			base.Start();
			Instance = this;

			int tx = 0;
			int ty = 64;


			var e = CreateEntity("newgame-text");
			e.GameObject.transform.SetParent(((App.Game)Game).GUICanvas.transform);
			newGameText = e.AddComponent<Text>();
			newGameText.font = Resources.Load<Font>("Fonts/semp");
			newGameText.text = "PLATFORMER GAME";
			newGameText.alignment = TextAnchor.MiddleCenter;
			newGameText.fontSize = 11;
			newGameText.rectTransform.localPosition = new Vector3(tx, ty, 0);
			ty -= 16;

			e = CreateEntity("continue-text");
			e.GameObject.transform.SetParent(((App.Game)Game).GUICanvas.transform);
			continueGameText = e.AddComponent<Text>();
			continueGameText.font = Resources.Load<Font>("Fonts/semp");
			continueGameText.text = "TOP DOWN GAME";
			continueGameText.alignment = TextAnchor.MiddleCenter;
			continueGameText.fontSize = 11;
			continueGameText.rectTransform.localPosition = new Vector3(tx, ty, 0);
			ty -= 16;

			e = CreateEntity("quit-text");
			e.GameObject.transform.SetParent(((App.Game)Game).GUICanvas.transform);
			quitText = e.AddComponent<Text>();
			quitText.font = Resources.Load<Font>("Fonts/semp");
			quitText.text = "QUIT";
			quitText.alignment = TextAnchor.MiddleCenter;
			quitText.fontSize = 11;
			quitText.rectTransform.localPosition = new Vector3(tx, ty, 0);
			ty -= 32;

			e = CreateEntity("instructions-text");
			e.GameObject.transform.SetParent(((App.Game)Game).GUICanvas.transform);
			instructionsText = e.AddComponent<Text>();
			instructionsText.font = Resources.Load<Font>("Fonts/semp");
			instructionsText.text = "UP/DOWN TO MOVE,\nSPACE TO SELECT";
			instructionsText.alignment = TextAnchor.MiddleCenter;
			instructionsText.fontSize = 11;
			instructionsText.rectTransform.localPosition = new Vector3(tx, ty, 0);
			ty -= 32;

      selectedMenu = newGameText;
			
			var m = CreateEntity("music");
			var audioSource = m.AddComponent<AudioSource>();
			audioSource.loop = true;
			audioSource.clip = Resources.Load<AudioClip>("Music/Menu");
			audioSource.Play();
			
			e = CreateEntity("version-text");
			e.GameObject.transform.SetParent(((App.Game)Game).GUICanvas.transform);
			VersionText = e.AddComponent<Text>();
			VersionText.font = Resources.Load<Font>("Fonts/semp");
			VersionText.text = "version " + ((App.Game)Game).Version;
			VersionText.alignment = TextAnchor.MiddleCenter;
			VersionText.fontSize = 11;
			VersionText.rectTransform.localPosition = new Vector3(tx, ty, 0);
			ty -= 16;

		}

		public override void Update ()
		{
			base.Update ();

			newGameText.color = Color.gray;
			continueGameText.color = Color.grey;
			quitText.color = Color.grey;

			selectedMenu.color = Color.yellow;

			if (Input.GetKeyUp (KeyCode.Space)) {
				if (selectedMenu == newGameText) {
					((App.Game)Game).continueSelected = false;
					((App.Game)Game).gameState = GameState.PlayPlatformer;
				} else if (selectedMenu == continueGameText) {
					((App.Game)Game).continueSelected = true;
					((App.Game)Game).gameState = GameState.PlayTopDown;
				} else if (selectedMenu == quitText) {
					((App.Game)Game).gameState = GameState.Quit;
				}
			}

			if (Input.GetKeyUp (KeyCode.DownArrow)) {
				if (selectedMenu == newGameText) {
					selectedMenu = continueGameText;
				} else if (selectedMenu == continueGameText) {
					selectedMenu = quitText;
				} else if (selectedMenu == quitText) {
					selectedMenu = newGameText;
				}
			}

			if (Input.GetKeyUp (KeyCode.UpArrow)) {
				if (selectedMenu == newGameText) {
					selectedMenu = quitText;
				} else if (selectedMenu == continueGameText) {
					selectedMenu = newGameText;
				} else if (selectedMenu == quitText) {
					selectedMenu = continueGameText;
				}
			}

		}

		public override void Remove ()
		{
			base.Remove ();
			GameObject.Destroy(newGameText.gameObject);
			GameObject.Destroy(continueGameText.gameObject);
			GameObject.Destroy(quitText.gameObject);
			GameObject.Destroy(instructionsText.gameObject);
			GameObject.Destroy(VersionText.gameObject);
			GameObject.Destroy(this.gameObject);
		}

	}
}

