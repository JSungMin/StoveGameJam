using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CoreSystem.Game.Helper
{
    public class AreaTrigger : MonoBehaviour
    {
        public Vector3 extents;
        private BoxCollider boxCollider;

        public Collider[] actableColliders;
        public ActorActivator actorActivator;

        public bool unActiveOnStart;
        public int enterMaxCount;
        public UnityEvent onTriggerEnter;
        public UnityEvent onTriggerStay;
        public int exitMaxCount;
        public UnityEvent onTriggerExit;


	    // Use this for initialization
	    void Start ()
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = extents;

            if (unActiveOnStart)
                UnActiveActors();
	    }
        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, extents);
            Gizmos.color = Color.green;
            for (int i = 0; i < actorActivator.actors.Length; i++)
            {
                var actor = actorActivator.actors[i];
                var col = actor.GetComponent<Collider>();
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.extents);
            }
            Gizmos.color = Color.cyan;
            for (int i = 0; i < actableColliders.Length; i++)
            {
                var col = actableColliders[i];
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.extents);
            }
        }

        public void ActiveActors()
        {
            actorActivator.ActiveActors();
        }
        public void UnActiveActors()
        {
            actorActivator.UnActiveActors();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (enterMaxCount <= 0)
            {
                return;
            }
            enterMaxCount--;
            for (int i = 0; i < actableColliders.Length; i++)
            {
                if (actableColliders[i] == other)
                    onTriggerEnter?.Invoke();
            }
        }
        public void OnTriggerStay(Collider other)
        {
            for (int i = 0; i < actableColliders.Length; i++)
            {
                if (actableColliders[i] == other)
                    onTriggerStay?.Invoke();   
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (exitMaxCount <= 0)
            {
                return;
            }
            exitMaxCount--;
            for (int i = 0; i < actableColliders.Length; i++)
            {
                if (actableColliders[i] == other)
                    onTriggerExit?.Invoke();
            }
        }
    }
}
