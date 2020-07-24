using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    public class ColliderManager : MonoBehaviour
    {
        public List<MainCollider> managedList = new List<MainCollider>();
        private Dictionary<string, MainCollider> nameColliderMap = new Dictionary<string, MainCollider>();

        public void Initialize()
        {
            foreach (var element in managedList)
            {
                nameColliderMap[element.mName] = element;
                element.Initialize(false);
            }
        }

        public MainCollider GetCollider(string n)
        {
            return nameColliderMap.ContainsKey(n) ? nameColliderMap[n] : null;
        } 
    }
}
