using System;
using UnityEngine;

//  This Player Animate Behaviour Depend on Animator
//  If you want to depend on Spine then you could
//  make some parent class that inherit ActorBehaviour
namespace CoreSystem.Game.ActorComponents
{
    [RequireComponent(typeof(Animator))]
    public class FrameAnimatorController : ActorBehaviour
    {
        public Animator animator;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private string[] curAnims;
        
        public bool[] isAnimate = new[] {false};
        public float[] timer;
        public Action<string>[] onStart;
        public Action<string>[] onComplete;

        public ActorBehaviourEntry.AnimateEntry animateEntry;

        public override void LoadWithAwake(GameActor a)
        {
            base.LoadWithAwake(a);
            spriteRenderer = animator.GetComponent<SpriteRenderer>();
            curAnims = new string[animator.layerCount];
            isAnimate = new bool[animator.layerCount];
            timer = new float[animator.layerCount];
            onStart = new Action<string>[animator.layerCount];
            onComplete = new Action<string>[animator.layerCount];
        }

        public void SetBool(string n, bool val) => animator.SetBool(n, val);
        public void SetFloat(string n, float val) => animator.SetFloat(n, val);
        public void SetInt(string n, int val) => animator.SetInteger(n, val);
        public bool GetBool(string n) => animator.GetBool(n);
        public float GetFloat(string n) => animator.GetFloat(n);
        public int GetInt(string n) => animator.GetInteger(n);
        public bool IsFlipX => spriteRenderer.flipX;

        public void FlipX(bool flip)
        {
            var absScaleX = Mathf.Abs(transform.localScale.x);
            Brain.ScaleX(transform, absScaleX);
        }

        public float GetPlaybackRatio(int layer)
        {
            return animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        }
        protected override void Do(ActorBehaviourEntry entry)
        {
            animateEntry = entry.animateEntry;
            SetAnimation(
                    animateEntry.trackIndex,
                    animateEntry.animName,
                    animateEntry.loop,
                    animateEntry.speed,
                    animateEntry.onFrameAnimateStart,
                    animateEntry.onFrameAnimateComplete);
        }

        protected bool SetAnimation(int trackIndex, string animName, bool loop = false, float speed = 1f, Action<string> OnStart = null, Action<string> OnComplete = null)
        {
            var curState = animator.GetCurrentAnimatorStateInfo(trackIndex);
            if (animName == "")
                return false;
            if (curState.IsName(animName) && loop)
                return false;
            onStart[trackIndex] = OnStart;

            animator.Play(animName, trackIndex, 0f);
            curAnims[trackIndex] = animName;
            isAnimate[trackIndex] = true;
            timer[trackIndex] = 0f;
            onComplete[trackIndex] = OnComplete;
            onStart[trackIndex]?.Invoke(animName);
            return true;
        }
        
        private void Update()
        {
            for (var layer = 0; layer < curAnims.Length; layer++)
            {
                if (!isAnimate[layer]) return;
                var curState = animator.GetCurrentAnimatorStateInfo(layer);
                timer[layer] += Time.deltaTime * curState.speed;
                var interrupted = !curState.IsName(curAnims[layer]);
                var timeOut = curState.length <= timer[layer];
                
                if (timeOut)
                {
                    var animName = curAnims[layer];
                    curAnims[layer] = "";
                    isAnimate[layer] = false;
                    timer[layer] = 0f;
                    onComplete[layer]?.Invoke(animName);
                }
            }
        }
    }
}
