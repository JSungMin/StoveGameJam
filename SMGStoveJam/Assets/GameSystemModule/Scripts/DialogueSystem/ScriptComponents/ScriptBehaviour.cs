using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game.Dialogue.Behaviour
{
    [System.Serializable]
    public abstract class ScriptBehaviour : MonoBehaviour, IScriptBehaviour
    {
        private ScriptObject mObj;
        public BehaviourJob job;
        public bool isPlaying = false;

        public virtual void Initialize(ScriptObject obj)
        {
            mObj = obj;
        }

        protected void OnJobComplete(bool comp)
        {
            job.Kill();
            job = null;
        }
        public virtual void Play(Action<bool> onComplete = null)
        {
            if (null == job)
            {
                job = BehaviourJob.Make(Do());
                if (onComplete != null)
                    job.OnComplete += onComplete;
                job.OnComplete += OnJobComplete;
                isPlaying = true;
            }
            else
            {
                if (job.IsPaused)
                    job.UnPause();
                if (isPlaying)
                    job.Start();
            }
        }

        public virtual void Pause()
        {
            job?.Pause();
        }

        public virtual void Stop()
        {
            job?.Kill();
        }
        protected abstract IEnumerator Do();
    }
}
