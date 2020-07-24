using System;
using System.Collections.Generic;
using CoreSystem.Game.Skill;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    public class SkillChainController : MonoBehaviour
    {
        public GameActor actor;
        [SerializeField]
        protected SkillChainProfile[] skillChainProfiles;
        [SerializeField]
        protected List<SkillChain> skillChains = new List<SkillChain>();
        protected Dictionary<string, SkillBase> skillBaseMap = new Dictionary<string, SkillBase>();
        protected Dictionary<string, SkillChain> skillChainMap = new Dictionary<string, SkillChain>();
        public List<SkillChain> skillChainQ = new List<SkillChain>();
        public SkillChain current = null;

        #region Initialize Region
        public SkillChainController Initialize(GameActor a, SkillView view)
        {
            actor = a;
            MappingSkillWithName(view);
            MappingProfileWithChain(view);
            return this;
        }
        public void MappingSkillWithName(SkillView view)
        {
            skillBaseMap = new Dictionary<string, SkillBase>();
            foreach (var skill in view.skillEntries)
            {
                skillBaseMap[skill.SkillName] = skill;
            }
        }
        public void MappingProfileWithChain(SkillView view)
        {
            skillChainMap = new Dictionary<string, SkillChain>();
            foreach (var profile in skillChainProfiles)
            {
                var loadedChain = profile.CreateSkillChain(actor, null);
                skillChains.Add(loadedChain);
                skillChainMap[profile.chainName] = loadedChain;
            }
        }
        #endregion

        public SkillChain GetChain(string name) => skillChainMap[name];
        public SkillBase GetSkill(string name) => skillBaseMap[name];

        public SkillChainController SetFinishChainAction(string chainName, Action<SkillChain> onAction, bool add = true)
        {
            GetChain(chainName)?.SetFinishPatternAction(onAction, add);
            return this;
        }
        public SkillChainController SetGlobalFinishChainAction(Action<SkillChain> onAction, bool add = true)
        {
            foreach (var chain in skillChains)
            {
                chain.SetFinishPatternAction(onAction, add);
            }
            return this;
        }
        public bool IsCoolDowned(string name)
        {
            var chain = GetChain(name);
            return (chain != null) && chain.IsCoolDowned;
        }

        public SkillChain CoolDown(string name)
        {
            var chain = skillChainMap[name];
            if (null == chain) return null;
            chain.CoolDown();
            return chain;
        }

        public SkillChain CoolDown(string name, bool autoPush)
        {
            var chain = CoolDown(name);
            if (!autoPush) return chain;
            if (chain?.IsCoolDowned == true)
                PushToQ(chain);
            return chain;
        }

        public SkillChainController ResetCool()
        {
            if (current == null) return this;
            current.ResetCool();
            return this;
        }
        public SkillChain PushToQ(SkillChain chain)
        {
            if (skillChainQ.Find(x => x.Name == chain.Name) != null)
                return chain;
            skillChainQ.Add(chain);
            return chain;
        }

        public SkillChain UseWithQ()
        {
            if (skillChainQ.Count == 0) return null;
            if (skillChainQ.Count >= 2)
            {
                skillChainQ.Sort(((x, y) => x.priority >= y.priority ? 1 : -1));
            }

            if (current != null && current.state != SkillChain.ChainState.FINISH) return current;

            var chain = skillChainQ[0];
            if (chain.state != SkillChain.ChainState.FINISH) return current;
            current = chain;
            current.Start(false);
            return current;
        }

        public SkillChain Use(string name, bool useCool)
        {
            if (current != null && current.CurrentlyRunning) return current;
            var chain = GetChain(name);
            if (chain.state != SkillChain.ChainState.FINISH) return current;
            current = chain;
            current.Start(false);
            return current;
        }

        public SkillChain PauseChain()
        {
            if (current == null) return null;
            current.Pause();
            return current;
        }

        public SkillChain StartChain()
        {
            if (current == null) return null;
            current.Start(false);
            return current;
        }

        public SkillChain StopChain()
        {
            if (current == null) return null;
            current.Stop();
            return current;
        }
        public SkillChainController PopFromQ(SkillChain chain)
        {
            skillChainQ.Remove(chain);
            return this;
        }
    }
}
