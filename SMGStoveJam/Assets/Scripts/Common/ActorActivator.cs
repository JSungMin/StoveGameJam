using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game.Helper
{
    [System.Serializable]
    public class ActorActivator
    {
        public GameActor[] actors;

        public void ActiveActors()
        {
            for(int i = 0; i < actors.Length; i++)
            {
                actors[i].gameObject.SetActive(true);
            }
        }
        public void UnActiveActors()
        {
            for (int i = 0; i < actors.Length; i++)
            {
                actors[i].gameObject.SetActive(false);
            }
        }
    }
}
