using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CoreSystem.Game.ActorComponents
{
    [System.Serializable]
    public class ActorCondition
    {
        public GameActor actor;
        public bool isUnbeatable = false;
        public bool isSuperArmor = false;
        public bool isBlooding = false;
        public bool canSpellSkip = false;
        public bool IsGrounded => actor.brain.MoveEntry.isGrounded;
    }
}
