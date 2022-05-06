//Author:Anthony Dodier
using System;
using Harmony;
using UnityEngine.SceneManagement;

namespace Game
{
    public static class Loader
    {

        private static Action onLoaderCallback;
        public static void Load(String scene)
        {
            onLoaderCallback = () => { SceneManager.LoadSceneAsync(scene); };
            
            SceneManager.LoadSceneAsync(R.S.Scene.LoadingScene);
        }

        public static void LoaderCallback()
        {
            if (onLoaderCallback != null)
            {
                onLoaderCallback();
                onLoaderCallback = null;
            }
        }
    }
}