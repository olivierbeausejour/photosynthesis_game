// Author : Anthony Dodier

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject objectsToSpawn;
        [SerializeField] private Transform poolProjectile;
        [SerializeField] private int nbObjectsToSpawn;
        private List<GameObject> objects;


        public int NbObjectsToSpawn => nbObjectsToSpawn;

        private void Start()
        {
            objects = new List<GameObject>();
            for (int i = 0; i < nbObjectsToSpawn; i++)
            {
                GameObject obj = Instantiate(objectsToSpawn,poolProjectile);
                obj.SetActive(false);
                objects.Add(obj);
            }
        }

        public GameObject Spawn()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (!objects[i].activeInHierarchy)
                {
                    return objects[i];
                }
            }
            return null;
        }
    }
}