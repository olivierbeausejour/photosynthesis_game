// Author : Derek Pouliot

using System.Collections;
using UnityEngine;

namespace Game
{
    public class EnemiesSpawner : MonoBehaviour
    {
        [SerializeField] private float totalSecondsBeforeSpawn = 5f;

        private Spawner spawner;

        private void Awake()
        {
            spawner = GetComponentInChildren<Spawner>();
        }

        private void Update()
        {
            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(totalSecondsBeforeSpawn);
                
                GameObject newEnemy = spawner.Spawn();
                newEnemy.transform.position = transform.position;
                newEnemy.SetActive(true);
            }
        }
    }
}