//Author: Olivier Beauséjour

using UnityEngine;

namespace Game
{
    public class ContactDestroyable : MonoBehaviour, IContactDestroyable
    {
        public void DestroyByContact()
        {
            Destroy(gameObject);
        }
    }
}