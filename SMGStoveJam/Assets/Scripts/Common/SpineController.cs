using System;
using UnityEngine;
using Spine.Unity;

namespace CoreSystem.Game.ActorComponents
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class SpineController : ActorBehaviour
    {
        public SkeletonAnimation animator;
        private MeshRenderer meshRenderer;
        [SerializeField]
        private string[] curAnims;
        public int trackIndex = 0;

        public Action<Spine.TrackEntry> onComplete;
        public Action<Spine.TrackEntry, Spine.Event> onEvent;

        public bool IsLoop => animator.AnimationState.GetCurrent(trackIndex).Loop;
        public bool IsComplete { get; set; }

        public int defTrackIndex;
        public string defAnim;


        // Use this for initialization
        public override void LoadWithStart(GameActor a)
        {
            base.LoadWithStart(a);
            animator = GetComponent<SkeletonAnimation>();
            meshRenderer = GetComponent<MeshRenderer>();
            animator.AnimationName = defAnim;
            animator.state.Complete += OnAnimationComplete;
            animator.state.Event += OnAnimationEvent;
            curAnims = new string[animator.AnimationState.Tracks.Count];
        }

        private void OnAnimationComplete(Spine.TrackEntry entry)
        {
            IsComplete = true;
            onComplete?.Invoke(entry);
        }
        private void OnAnimationEvent(Spine.TrackEntry entry, Spine.Event e)
        {
            onEvent?.Invoke(entry, e);
        }
        public void FlipX(bool flipX)
        {
            var absScaleX = Mathf.Abs(ScaleX);
            if (flipX)
                ScaleX = -absScaleX;
            else
                ScaleX = absScaleX;
        }
        public void SetAnimation(int index, string name, bool loop = false, float time = 1.0f)
        {
            if (animator.AnimationState?.GetCurrent(index) == null)
            {
                curAnims[index] = "";
            }
            if (curAnims[index] == name && loop)
            {
                return;
            }
            else if(curAnims[index] != name)
            {
                animator.state.SetAnimation(index, name, loop).TimeScale = time;
                curAnims[index] = name;
                IsComplete = false;
            }
            else if (!loop && IsComplete)
            {
                animator.state.SetAnimation(index, name, false).TimeScale = time;
                curAnims[index] = name;
                IsComplete = false;
            }
        }

        public float GetPlaybackRatio(int layer)
        {
            var current = animator.AnimationState.GetCurrent(layer);
            return current.TrackTime / current.AnimationEnd;
        }
        public Color OverlayColor
        {
            get => meshRenderer.material.GetColor("_OverlayColor");
            set => meshRenderer.material.SetColor("_OverlayColor", value);
        }
        public Color MainColor
        {
            get => meshRenderer.material.color;
            set => meshRenderer.material.color = value;
        }
        public float ScaleX
        {
            set => animator.skeleton.ScaleX = value;
            get => animator.skeleton.ScaleX;
        }

        protected override void Do(ActorBehaviourEntry entry)
        {
            var animateEntry = entry.animateEntry;
            SetAnimation(animateEntry.trackIndex, animateEntry.animName, animateEntry.loop, animateEntry.speed);
        }
    }
}
