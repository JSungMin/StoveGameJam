using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game
{
    public class ActorConditionSetter : MonoBehaviour
    {
        public GameActor actor;

        public void SetUnbeatable(int val)
        {
            actor.condition.isUnbeatable = val!=0;
        }

        public void SetBlooding(int val)
        {
            actor.condition.isBlooding = val!=0;
        }
    }
}
