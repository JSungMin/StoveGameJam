using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game
{
    [RequireComponent(typeof(Collider))]
    public class SubCollider : MonoBehaviour
    {
        [HideInInspector]
        public MainCollider mainCollider;
        public Collider mCollider;
        
        private void OnTriggerEnter(Collider col)
        {
            mainCollider.OnSubTriggerEvent(col);
        }

        private void OnTriggerStay(Collider col)
        {
            mainCollider.OnSubTriggerEvent(col);
        }

        private void OnTriggerExit(Collider col)
        {
            mainCollider.OnSubTriggerEvent(col);
        }
        public void Initialize(MainCollider main)
        {
            mCollider = GetComponent<Collider>();
            mCollider.isTrigger = true;
            mainCollider = main;
            DeActivate();
        }

        public void Activate()
        {
            mCollider.enabled = true;
        }

        public void DeActivate()
        {
            mCollider.enabled = false;
        }
        public void Destroy()
        {
            mainCollider.subColliderList.Remove(this);
            Destroy(gameObject);
        }
        public static SubCollider Create(MainCollider main)
        {
            var instance = new GameObject("Col_Sub_" + main.SkillProfile.skillName, typeof(BoxCollider),typeof(SubCollider));
            var result = instance.GetComponent<SubCollider>();
            result.Initialize(main);
            main.subColliderList.Add(result);
            return result;
        }

    }
}
