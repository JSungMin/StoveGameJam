using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game
{
    public class AudioManager : SingletonGameObject<AudioManager>
    {
        public List<SoundWrapper> requestQ = new List<SoundWrapper>();
        public List<AudioSource> cachedSources = new List<AudioSource>();
        public List<AudioSource> playingSources = new List<AudioSource>();
        public List<AudioSource> remainSources = new List<AudioSource>();

        public int maxPlayAmount = 10;
        public BehaviourJob manageJob;

        public static void DelayedPlay(GameActor actor, SoundProfile profile, int priority, bool canOverlap = true)
        {
            //  한계치를 넘어 우선순위가 낮은 것을 뺀다.
            if (Instance.requestQ.Count >= Instance.maxPlayAmount)
            {
                if (Instance.requestQ[0].priority < priority)
                {
                    Instance.requestQ.RemoveAt(0);
                }
            }

            if (!canOverlap)
            {
                var overlap = Instance.requestQ.FirstOrDefault(x => x.profile == profile);
                if (null != overlap)
                    return;
            }
            Instance.requestQ.Add(actor == null
                ? new SoundWrapper(priority, profile, Camera.main.transform.position)
                : new SoundWrapper(priority, profile, actor.transform.position));
            Instance.requestQ = Instance.requestQ.OrderBy(x => x.priority).ToList();
        }

        public void Awake()
        {
            for (var i = 0; i < maxPlayAmount; i++)
            {
                var newObj = new GameObject("C_Audio");
                newObj.transform.SetParent(transform);
                var source = newObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                cachedSources.Add(source);
                remainSources.Add(source);
            }
        }
        public void Update()
        {
            for (var i = 0; i < playingSources.Count; i++)
            {
                var source = playingSources[i];
                if (source.isPlaying) continue;
                playingSources.RemoveAt(i);
                remainSources.Add(source);
            }
            for (var i = requestQ.Count - 1; i >= 0; i--)
            {
                var re = requestQ[i];
                requestQ.RemoveAt(i);
                if (remainSources.Count <= 0) continue;
                var top = remainSources[0];
                top.priority = re.priority;
                top.clip = re.profile.clip;
                top.outputAudioMixerGroup = re.profile.mixerGroup;
                top.pitch = re.profile.pitch;
                top.volume = re.profile.volume;
                top.transform.position = re.point;
                top.Play();
                remainSources.RemoveAt(0);
                playingSources.Add(top);
            }
        }
        [System.Serializable]
        public class SoundWrapper
        {
            public int priority;
            public SoundProfile profile;
            public Vector3 point;

            public SoundWrapper()
            {
                priority = 0;
                profile = null;
                point = Vector3.zero;
            }

            public SoundWrapper(int pri, SoundProfile pro, Vector3 p)
            {
                priority = pri;
                profile = pro;
                point = p;
            }
        }
    }
}
