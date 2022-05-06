//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    public class PullDestroyable : MonoBehaviour, IPullDestroyable
    {
        private EnemyIsDoneBeingPulledEventChannel enemyIsDoneBeingPulledEventChannel;
        private SpriteRenderer[] spriteRenderers;
        private BoxCollider2D[] colliders2D;
        private void Awake()
        {
            enemyIsDoneBeingPulledEventChannel = Finder.EnemyIsDoneBeingPulledEventChannel;
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            colliders2D = GetComponentsInChildren<BoxCollider2D>();
        }

        public void DestroyByPull()
        {
            transform.gameObject.GetComponent<BaseEnemyController>().DisableEnemy();
            enemyIsDoneBeingPulledEventChannel.NotifyEnemyDoneBeingPulled();
        }
    }
}