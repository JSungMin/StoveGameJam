using System.Collections.Generic;
using CoreSystem.Game.Skill;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    public abstract class EnemyBrain : ActorBrain
    {
        [SerializeField]
        protected SkillChainController mPatterns;
        public static GameActor Player => BattleLogic.Instance.player;
        public float DisToPlayer => Vector3.Distance(Player.transform.position, actor.transform.position);
        public Vector3 DirToPlayer => (Player.transform.position - actor.transform.position);
        public float CurHp => actor.Profile.HP.CurrentGauge;
        public float DefHp => actor.Profile.HP.DefaultGauge;

        protected List<SkillBase> SkillEntries => actor.SkillView.skillEntries;

        public int phase = 0;
        protected virtual void BuildSkillChain(SkillView view)
        {
            
        }

        public override void LoadWithStart(GameActor a)
        {
            base.LoadWithStart(a);
            mPatterns.Initialize(a, SkillView);
            BuildSkillChain(SkillView);
            actor.ParryReactor.onSuccessAction += OnParried;
        }
        public abstract void OnParried(SkillBase ps, SkillBase es);
    }
}
