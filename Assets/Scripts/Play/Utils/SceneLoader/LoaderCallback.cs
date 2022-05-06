//Author:Anthony Dodier
using System;
using UnityEngine;

namespace Game
{
    public class LoaderCallback : MonoBehaviour
    {
        private bool isFirstUpdate = true;
        private void Update()
        {
            if (isFirstUpdate)
            {
                isFirstUpdate = false;
                Loader.LoaderCallback();
            }
        }
    }
}