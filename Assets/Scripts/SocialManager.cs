using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace App
{
    public class SocialManager
    {
        private static SocialManager _instance;
        public static SocialManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SocialManager();
                }
                return _instance;
            }
        }

        public string LeaderboardId = "Default";

        public SocialManager()
        {
        }

        public void Initialize()
        {
#if UNITY_IOS
            Social.localUser.Authenticate(ProcessAuthentication);
#endif
        }

        protected void ProcessAuthentication(bool success)
        {
            if (success)
            {
                Debug.Log("Authenticated, checking achievements");

                // Request loaded achievements, and register a callback for processing them
                Social.LoadAchievements(ProcessLoadedAchievements);
            }
            else
            {
                Debug.Log("Failed to authenticate");
            }
        }

        protected void ProcessLoadedAchievements(IAchievement[] achievements)
        {
            if (achievements.Length == 0)
            {
                Debug.Log("Error: no achievements found");
            }
            else
            {
                Debug.Log("Got " + achievements.Length + " achievements");
            }

            // You can also call into the functions like this
            /*Social.ReportProgress ("Achievement01", 100.0, result => {
                if (result)
                    Debug.Log ("Successfully reported achievement progress");
                else
                    Debug.Log ("Failed to report achievement");
            });*/
        }

        public void ShowLeaderboard()
        {
#if UNITY_IOS
            Social.ShowLeaderboardUI();
#endif
        }

        public void UploadHighScore(long score)
        {
#if UNITY_IOS
            Social.ReportScore(score, LeaderboardId, ReportScoreCallback);
#endif
        }

        protected void ReportScoreCallback(bool success)
        {
            if (success)
            {
                Debug.Log("score submission successful");
            }
            else
            {
                Debug.Log("score submission failed");
            }
        }
    }
}