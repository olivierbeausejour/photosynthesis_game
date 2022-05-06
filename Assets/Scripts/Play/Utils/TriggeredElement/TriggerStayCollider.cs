﻿//Authors:
//Charles Tremblay
//Anthony Dodier

using Harmony;
using UnityEngine;

namespace Game
{
    public class TriggerStayCollider : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(R.S.Layer.Player)&& other.gameObject.CompareTag(R.S.Tag.PhysicalCollider))
                transform.parent.GetComponentInChildren<ITriggerStay>().OnTriggerStayDetected(other);
        }
    }
}