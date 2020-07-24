using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using CoreSystem.Game;

namespace CoreSystem.ProfileComponents
{
    [Serializable]
    public class AnimationDesc
    {
        public string name;
        public float speedMultiplier;
        public AnimationType animationType;
        public RuntimeAnimatorController frameAnimator;
        public SkeletonDataAsset spineDataAsset = null;

        public AnimationDesc()
        {
            name = "";
            speedMultiplier = 1.0f;
            animationType = AnimationType.Frame;
        }
    }
    [System.Serializable]
    public class Stat
    {
        public List<float> elements = new List<float>();
        public List<string> Names => CoreDataSet.MetaStatData.Names;

        public void Initialize()
        {
            if (Count != CoreDataSet.MetaStatData.Count)
                elements = CoreDataSet.MetaStatData.Values;
        }
        public int Count => elements.Count;
        public static Stat operator +(Stat a, Stat b)
        {
            var sumStat = new Stat();
            sumStat.Initialize();
            for (int i = 0; i < sumStat.Count; i++)
            {
                sumStat.elements[i] = a.elements[i] + b.elements[i];
            }
            return sumStat;
        }
        public static Stat operator -(Stat a, Stat b)
        {
            var minStat = new Stat();
            minStat.Initialize();
            for (int i = 0; i < minStat.Count; i++)
            {
                minStat.elements[i] = a.elements[i] - b.elements[i];
            }
            return minStat;
        }
    }
    public abstract class ElementsView<T>
    {
        protected GameActor viewOwner;
        [SerializeField]
        private List<T> elements = new List<T>();
        public Action<T> OnAddElement;
        public Action<T> OnRemoveElement;

        public virtual void Initialize(GameActor actor)
        {
            viewOwner = actor;
        }

        public virtual void AddElement(T e)
        {
            elements.Add(e);
            OnAddElement?.Invoke(e);
        }
        public virtual void RemoveElement(int id)
        {
            if (id < 0 || id >= elements.Count)
                return;
            var e = elements[id];
            elements.RemoveAt(id);
            OnRemoveElement?.Invoke(e);
        }
        public virtual void RemoveElement(T e)
        {
            elements.Remove(e);
            OnRemoveElement?.Invoke(e);
        }
        public virtual void ReplaceElement(int prevID, T cur)
        {
            if (prevID < 0 || prevID >= elements.Count)
                return;
            OnRemoveElement?.Invoke(elements[prevID]);
            OnAddElement?.Invoke(cur);
            elements[prevID] = cur;
        }
        public virtual void ReplaceElement(T prev, T cur)
        {
            var prevID = elements.IndexOf(prev);
            ReplaceElement(prevID, cur);
        }
        public bool IsEmpty(int idx)
        {
            return elements[idx] == null;
        }
        public bool Contains(T e)
        {
            return elements.Contains(e);
        }

        public virtual T Find(Predicate<T> match)
        {
            return elements.Find(match);
        }
        public List<T> Elements
        {
            get
            {
                return elements;
            }
#if UNITY_EDITOR
            set
            {
                elements = value;
            }
#endif
        }
    }

    [Serializable]
    public class InventoryView : ElementsView<InventoryElement>
    {
        public float money;
        public Action<InventoryElement, int> OnRootItem;
        public Action<InventoryElement, int> OnDropItem;

        public bool Contains(ItemProfile profile)
        {
            return Elements.Exists(x => x.Profile == profile);
        }
        public InventoryElement FindElement(ItemProfile profile)
        {
            return Find(x => x.Profile == profile);
        }
        public void UseItem(GameActor actor, ItemProfile profile)
        {
            switch (profile.itemType)
            {
                // case ItemType.Consumables:
                //     actor.Profile.HP.CurrentGauge += profile.hp.CurrentGauge;
                //     actor.Profile.MP.CurrentGauge += profile.mp.CurrentGauge;
                //     actor.Profile.basicStat += profile.influence;
                //     break;
                case ItemType.장비:
                    actor.Profile.HP.DefaultGauge += profile.hp.DefaultGauge;
                    actor.TakeEquipment(FindElement(profile));
                    break;
            }
        }
        public InventoryElement RootItem(ItemProfile profile, int delta)
        {
            if (delta <= 0)
                return null;
            var element = Elements.Find(x => x.Profile == profile);
            if (element != null)
            {
                OnRootItem?.Invoke(element, delta);
                element.Amount += delta;
            }
            else
            {
                element = new InventoryElement(profile, delta);
                AddElement(element);
                OnRootItem?.Invoke(element, delta);
            }
            return element;
        }
        /// <summary>
        /// Return Object that is Dropped Elements
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public InventoryElement DropItem(ItemProfile profile, int delta)
        {
            if (delta <= 0)
                return null;
            var element = Elements.Find(x => x.Profile == profile);
            if (element == null)
                return null;
            OnDropItem?.Invoke(element, delta);
            element.Amount -= delta;
            if (element.Amount <= 0)
                Elements.Remove(element);
            return new InventoryElement(profile, delta);
        }

        public override void Initialize(GameActor actor)
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public class EquipmentView : ElementsView<ItemProfile>
    {
        public void ConfirmMetaData()
        {
            if (Elements.Count == CoreDataSet.MetaEquipPartData.Count)
                return;
            for (int i = 0; i < CoreDataSet.MetaEquipPartData.Count; i++)
            {
                AddElement(null);
            }
        }
        public void ApplyInfulence(Stat additiveStat)
        {
            for (int i = 0; i < additiveStat.Count; i++)
            {
                additiveStat.elements[i] = 0f;
            }
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i] == null)
                    continue;
                for (int j = 0; j < additiveStat.Count; j++)
                {
                    var prevVal = additiveStat.elements[j];
                    var influence = Elements[i].influence.elements[j];
                    additiveStat.elements[j] = prevVal + influence;
                }
            }
        }
        public float AdditiveHP
        {
            get
            {
                float result = 0f;
                for (int i = 0; i < Elements.Count; i++)
                {
                    result += Elements[i].hp.DefaultGauge;
                }
                return result;
            }
        }
    }


}