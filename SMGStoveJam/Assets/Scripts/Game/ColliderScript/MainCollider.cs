using System;
using System.Collections.Generic;
using CoreSystem.Game.Skill;
using UnityEngine;

namespace CoreSystem.Game
{
    public class MainCollider : MonoBehaviour
    {
        public bool isInit = false;
        public bool isLoaded = false;
        public string mName;
        private SkillBase skillBase;

        public GameActor Speller => skillBase.speller;
        public SkillProfile SkillProfile => skillBase.skillProfile;
        private Action<UnityEngine.Object, GameActor> mSkillAction;
        public List<SubCollider> subColliderList = new List<SubCollider>();
        public List<GameActor> victimCache = new List<GameActor>();

        public void OnSubTriggerEvent(Collider col)
        {
            if (!(isInit && isLoaded))
                return;
            //  태그를 통해 스킬에 영향을 받을 객체를 걸러냄
            var useInvTag = SkillProfile.skillType == SkillType.Buff ||
                              SkillProfile.skillType == SkillType.Avoid;
            var equalTag = Speller.gameObject.tag == col.tag;
            if (useInvTag &&!equalTag)
                return;
            if (!useInvTag && equalTag)
                return;
            var victimActor = col.GetComponent<GameActor>();
            if (null == victimActor)
                return;
            var cached = victimCache.Find(x => x == victimActor);
            if (null != cached)
                return;
            victimCache.Add(victimActor);
            BattleLogic.CreateBehaveSpellMsg(skillBase, mSkillAction, Speller, victimActor);
        }
        
        public void Activate()
        {
            Speller.brain.SpellEntry.isColliderActivated = true;
            victimCache.Clear();
            subColliderList.ForEach(x => x.Activate());
        }

        public void DeActivate()
        {
            Speller.brain.SpellEntry.isColliderActivated = false;
            victimCache.Clear();
            subColliderList.ForEach(x => x.DeActivate());
        }
        public MainCollider Initialize(bool activate)
        {
            isInit = true;
            subColliderList.ForEach(x => x.Initialize(this));
            //Speller.brain.SpellEntry.skillColliders.Add(this);
            return this;
        }

        public MainCollider LoadSkillDesc(SkillBase skill, Action<UnityEngine.Object, GameActor> skillAction)
        {
            isLoaded = true;
            skillBase = skill;
            mSkillAction = skillAction;
            return this;
        }
        public void Destroy()
        {
            //Speller.brain.SpellEntry.skillColliders.Remove(this);
            victimCache.Clear();
            Destroy(gameObject);
        }
        public static MainCollider CreateGlobalCollider(SkillBase @base, Action<UnityEngine.Object, GameActor> skillAction, bool activate = true)
        {
            var instance = new GameObject("Col_Main_"+@base.SkillName, typeof(BoxCollider), typeof(MainCollider));
            if(!@base.IsGlobalSkill)
                instance.transform.SetParent(@base.speller.transform);
            //  TODO : Adjust Collider Size
            //  TODO : Set Collider Destroy Time

            var result = instance.GetComponent<MainCollider>();
            result.Initialize(activate).LoadSkillDesc(@base, skillAction);
            return result;
        }
    }
}
