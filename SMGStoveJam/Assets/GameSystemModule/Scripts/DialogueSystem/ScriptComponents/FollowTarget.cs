using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.Dialogue;
using UnityEngine;

namespace CoreSystem.Game.Dialogue.Behaviour
{
    public class FollowTarget : ScriptBehaviour
    {
        public Transform followTarget;
        public Vector3 offset;

        public override void Initialize(ScriptObject obj)
        {
            base.Initialize(obj);
            if (null != followTarget)
                return;
            //  Default Follow Target is Staged Actor
            if (null != obj.stagedActor)
            {
                followTarget = obj.stagedActor.transform;
            }
        }
        
        protected override IEnumerator Do()
        {
            while (isPlaying)
            {
                var screenPoint = Camera.main.WorldToScreenPoint(followTarget.position + offset);
                transform.position = screenPoint;
                yield return null;
            }
        }
    }
}
