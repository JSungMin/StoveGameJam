using System;
using System.Linq;
using System.Collections.Generic;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem
{
    public partial class ActorProfile : ScriptableObject
    {
        public enum PortraitType
        {
            None,
            Sprite,
            Animation
        }
        [Serializable]
        public class PortraitDesc
        {
            public string name;
            public PortraitType portraitType;
            public AnimationDesc animation;
            public Sprite sprite;


            public PortraitDesc()
            {
                name = "NO_NAME";
                sprite = null;
                animation = new AnimationDesc();
            }
    #if UNITY_EDITOR
            public bool flagFoldoutDesc = true;

            public void UpdatePortraitType()
            {
                if (animation.name != "")
                {
                    sprite = null;
                    portraitType = PortraitType.Animation;
                }
                else if (sprite != null)
                {
                    animation.name = "";
                    portraitType = PortraitType.Sprite;
                }
                else
                    portraitType = PortraitType.None;
            }
    #endif
        }
        [Serializable]
        public class PortraitDescSet
        {
            [SerializeField]
            private List<PortraitDesc> descs = new List<PortraitDesc>();

            public List<PortraitDesc> Descriptions {
                set
                {
                    descs = value;
                }
                get
                {
                    return descs;
                }
            }
            public int Count
            {
                get
                {
                    return descs.Count;
                }
            }

            public void Add(PortraitDesc desc)
            {
                descs.Add(desc);
            }
            public void Remove(PortraitDesc desc)
            {
                descs.Remove(desc);
            }
            public PortraitDesc GetDesc(string name)
            {
                return descs.Where(x => x.name == name).FirstOrDefault();
            }
            public PortraitDesc GetDesc(int idx)
            {
                return descs[idx];
            }
    #if UNITY_EDITOR
            public void UpdateAllPortraitType()
            {
                descs.ForEach(x => x.UpdatePortraitType());
            }
    #endif
        }
    }
}
