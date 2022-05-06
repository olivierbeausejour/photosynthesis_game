//Authors:
//Benjamin Lemelin
//Olivier Beauséjour

using Harmony;
using UnityEngine;

namespace Game
{
    public class Stimuli : MonoBehaviour
    {
        [Header("Stimuli collider")]
        [SerializeField] private new Collider2D collider;

        [SerializeField] private GameObject prefabToAffect;

        public GameObject PrefabToAffect => prefabToAffect;

        public event StimuliEventHandler OnDestroyed;

        private void OnDestroy()
        {
            NotifyDestroyed();
        }

        private void NotifyDestroyed()
        {
            if (OnDestroyed != null) OnDestroyed(transform.parent.gameObject);
        }
    }

    public delegate void StimuliEventHandler(GameObject otherObject);
}