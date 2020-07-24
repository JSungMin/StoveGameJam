using System.Collections;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    [System.Serializable]
    public abstract class ActorBehaviour : MonoBehaviour
    {
        public string behaviourName;
        public int entryBitIndex = -1;
        
        protected GameActor actor;
        protected ActorBrain Brain => actor.brain;
        protected ActorBehaviourEntry BehaviourEntry => Brain.behaviourEntry;

        [SerializeField]
        protected bool isPlaying = false;
        [SerializeField]
        protected bool isPaused = false;

        public virtual void LoadWithAwake(GameActor a)
        {
            actor = a;
            entryBitIndex = 0;
        }

        public virtual void LoadWithStart(GameActor a)
        {
            actor = a;
            entryBitIndex = 0;
        }
        public virtual void SetEntryToDirty(ActorBehaviourEntry entry)
        {
            entry.SetDirty(entryBitIndex);
        }
        public virtual IEnumerator BehaveAsCoroutine()
        {
            if (!isPlaying)
                yield break;
            if (isPaused)
                yield break;
            Do(BehaviourEntry);
            SetEntryToDirty(BehaviourEntry);
        }

        public virtual void Behave()
        {
            if (!isPlaying)
                return;
            if (isPaused)
                return;
            Do(BehaviourEntry);
            SetEntryToDirty(BehaviourEntry);
        }
        

        public virtual void Play()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                isPaused = false;
            }
            else if(isPaused)
                Resume();
        }

        public virtual void Pause()
        {
            isPaused = true;
        }

        public virtual void Resume()
        {
            isPaused = false;
        }
        public virtual void Stop()
        {
            isPlaying = false;
            isPaused = false;
        }
        protected abstract void Do(ActorBehaviourEntry entry);
    }
}
