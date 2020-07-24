using System;
using System.Linq;
using UnityEngine;

using CoreSystem.ProfileComponents;
using CoreSystem.Game.ActorComponents;
using CoreSystem.Game.Skill;

namespace CoreSystem.Game
{
    public class GameActor : ManagedSceneObject
    {
        public bool isLoaded = false;
        public enum ActorType
        {
            Enemy,
            Player,
            NPC
        }
        public ActorType actorType;
        public ActorProfile copiedProfile;
        public ActorBrain brain;
        public ActorCondition condition;

        [SerializeField]
        private ActorFSM fsm;
        [SerializeField]
        private ColliderManager colliderManager;
        [Tooltip("If ActorType Is Enemy Then Automatically This Property will be allocated")]
        [SerializeField]
        private ParryReactor parryReactor;

        //  Called From StagedObjectManager
        public override void LoadWithAwake()
        {
            if (isLoaded) return;
            brain?.LoadWithAwake(this);
            SkillView.Initialize(this);
            isLoaded = true;
        }

        public override void LoadWithStart()
        {
            colliderManager?.Initialize();
            brain?.LoadWithStart(this);
        }

        #region Exposed Actor Behaviour Funcs
        public virtual SkillBase SpellSkill(SkillProfile skill, bool useCondition)
        {
            return brain.SpellSkill(skill, useCondition);
        }

        public virtual SkillBase SpellSkill(SkillBase skill, bool useCondition)
        {
            return brain.SpellSkill(skill, useCondition);
        }
        
        public void Move(Vector3 direction, float moveSpeed, bool useJump)
        {
            brain.Move(direction, moveSpeed, useJump);
        }

        public void Damage(SkillBase skillBase, float dmg)
        {
            brain.Damage(skillBase, dmg);
        }

        public GameActor LookAt(Vector3 pos)
        {

            return this;
        }

        public bool? FSM_TransState(int idx) => fsm?.TransState(idx);
        public bool? FSM_TransState(string state) => fsm?.TransState(state);
        public void FSM_SetState(int idx) => fsm?.SetState(idx);
        public void FSM_SetState(string state) => fsm?.SetState(state);
        #endregion

        public MainCollider GetCollider(string colName)
        {
            return colliderManager.GetCollider(colName);
        }

        public ParryReactor ParryReactor => parryReactor;
        public ActorFSM FSM => fsm;

        #region Inventory Wrap Functions
        public bool HasItem(ItemProfile profile)
        {
            return Inventory.Contains(profile);
        }
        public InventoryElement FindItem(ItemProfile profile)
        {
            return Inventory.FindElement(profile);
        }
        public InventoryElement RootItem(ItemProfile profile, int amount)
        {
            return Inventory.RootItem(profile, amount);
        }
        public InventoryElement DropItem(ItemProfile profile, int amount)
        {
            return Inventory.DropItem(profile, amount);
        }
        public void UseItem(ItemProfile profile)
        {
            Inventory.UseItem(this, profile);
        }
        #endregion
        
        #region Equipment Wrap Functions
        public bool TakeEquipment(InventoryElement element)
        {
            return Profile.TakeEquipment(element);
        }
        public void TakeOffEquipment(InventoryElement element)
        {
            Profile.TakeOffEquipment(element);
        }
        public void TakeOffEquipment(int partID)
        {
            Profile.TakeOffEquipment(partID);
        }
        public void ApplyEquipmentInfulence()
        {
            Equipments.ApplyInfulence(Profile.additionalStat);
        }
        #endregion

        #region Static Funcs Region
        public static GameActor SpawnGameActor(ActorProfile profile, ActorType actorType, bool isSaveable, bool useClone, GameObject existObj = null)
        {
            if (existObj == null)
                existObj = new GameObject("GA_" + profile.actorName);
            else
            {
                existObj = Instantiate(existObj, Vector3.zero, Quaternion.identity);
                existObj.name = "GA_" + profile.actorName;
                var existActor = existObj.GetComponent<GameActor>();
                DestroyImmediate(existActor);
            }
            if (isSaveable)
            {
                var ga = existObj.AddComponent<GameActorWithSaveable>();
                ga.actorType = actorType;
                ga.copiedProfile = ActorProfileSet.GetCloneElement(profile);
                ga.LoadWithAwake();
                StagedObjectManager.AddGameActor(ga);
                StagedObjectManager.AddSaveable(ga);
            }
            else
            {
                var ga = existObj.AddComponent<GameActor>();
                ga.actorType = actorType;
                ga.copiedProfile = ActorProfileSet.GetCloneElement(profile);
                ga.LoadWithAwake();
                StagedObjectManager.AddGameActor(ga);
            }

            var result = existObj.GetComponent<GameActor>();
            if (actorType == ActorType.Enemy)
            {
                 result.parryReactor = existObj.AddComponent<ParryReactor>();
                 result.parryReactor.actor = result;
            }

            return result;
        }

        public static bool KillGameActor(GameActor actor)
        {
            return StagedObjectManager.RemoveGameActor(actor);
        }
        #endregion
        public ActorProfile Profile => copiedProfile;

        public InventoryView Inventory
        {
            get => Profile.inventory;
            set => Profile.inventory = value;
        }
        public EquipmentView Equipments
        {
            get => Profile.equipment;
            set => Profile.equipment = value;
        }
        public SkillView SkillView
        {
            get => Profile.skills;
            set => Profile.skills = value;
        }
    }
}
