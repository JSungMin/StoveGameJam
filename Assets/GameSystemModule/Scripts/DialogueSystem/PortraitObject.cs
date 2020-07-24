using System.Collections;
using CoreSystem.ProfileComponents;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace CoreSystem.Game.Dialogue
{
    public class PortraitObject : MonoBehaviour
    {
        public PortraitManager manager;
        public RectTransform rectTransform;
        public RectTransform followTarget;
        public ActorProfile actorProfile;
        public ActorProfile.PortraitDesc desc;

        private Image portraitImage;
        private SkeletonGraphic spineAnimator;
        private Animator frameAnimator;

        public Transform shakeTransform;
        public Vector3 offsetScale;

        public bool useFollow = true;
        
        public string Name => desc.name;
        public ActorProfile.PortraitType Type => desc.portraitType;
        public Sprite Portrait => desc.sprite;
        public AnimationDesc Animation => desc.animation;

        private BehaviourJob stageJob;
        private BehaviourJob followJob;
        private BehaviourJob fadeJob;

        public Color CurrentColor => portraitImage.color;

        public void Stage(PortraitManager m, ActorProfile profile, string portraitName, RectTransform follow)
        {
            manager = m;
            actorProfile = profile;
            var shakeObj = new GameObject("ShakeAvatar");
            shakeObj.transform.SetParent(transform);
            shakeTransform = shakeObj.transform;
            if (actorProfile != null)
                desc = actorProfile.GetPortraitDesc(portraitName);
            followTarget = follow;
            stageJob = BehaviourJob.Make(IStage());
        }

        public void SwapPortraitDesc(ActorProfile.PortraitDesc d)
        {
            if (d.portraitType == ActorProfile.PortraitType.Sprite)
            {
                if (null != spineAnimator)
                    DestroyImmediate(spineAnimator);
                if (null == portraitImage)
                    portraitImage = gameObject.AddComponent<Image>();
                portraitImage.sprite = d.sprite;
                portraitImage.SetNativeSize();
                //var nativeSize = rectTransform.sizeDelta;
                //var adjustSize = nativeSize * manager.portraitMagnification;
                rectTransform.sizeDelta = new Vector2(1024f,1280f);
            }
            else if (d.portraitType == ActorProfile.PortraitType.Animation)
            {
                if (d.animation.animationType == AnimationType.Frame)
                {
                    if (null == frameAnimator)
                        frameAnimator = gameObject.AddComponent<Animator>();
                    frameAnimator.runtimeAnimatorController = d.animation.frameAnimator;
                    frameAnimator.Play(d.animation.name);
                    frameAnimator.StopPlayback();
                }
                else if (d.animation.animationType == AnimationType.Spine)
                {
                    if (null != portraitImage)
                        DestroyImmediate(portraitImage);
                    if (null == spineAnimator)
                        spineAnimator = gameObject.AddComponent<SkeletonGraphic>();
                    spineAnimator.skeletonDataAsset = d.animation.spineDataAsset;
                    spineAnimator.Initialize(true);
                    spineAnimator.startingAnimation = d.animation.name;
                    spineAnimator.AnimationState.SetAnimation(0, d.animation.name, true);
                }
            }
            desc = d;
        }
        public void GoFront()
        {
            FadeColor(portraitImage.color, Color.white, 0.25f);
            followTarget.SetAsLastSibling();
        }

        public BehaviourJob FadeColor(Color bColor, Color eColor, float duration)
        {
            fadeJob?.Kill();
            fadeJob = BehaviourJob.Make(IFade(bColor, eColor, duration));
            return fadeJob;
        }
        public void UnStage()
        {
            BehaviourJob.Make(IUnStage());
        }

        private void DestroyPortrait()
        {
            Destroy(followTarget.gameObject);
            Destroy(gameObject);
        }

        
        private IEnumerator IFollow()
        {
            while (useFollow)
            {
                transform.position = Vector3.Lerp(transform.position, followTarget.position, Time.deltaTime * 2f);
                yield return null;
            }
        }

        private IEnumerator IFade(Color bColor, Color eColor, float duration)
        {
            portraitImage.color = bColor;
            var timer = 0f;
            while (timer <= duration)
            {
                portraitImage.color = Color.Lerp(bColor, eColor, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            portraitImage.color = eColor;
        }
        private IEnumerator IStage()
        {
            yield return new WaitForEndOfFrame();
            rectTransform.position = followTarget.position;
            SwapPortraitDesc(desc);

            fadeJob = BehaviourJob.Make(IFade(Color.clear,Color.white,0.25f));
            followJob = BehaviourJob.Make(IFollow(),true);
        }

        private IEnumerator IUnStage()
        {
            stageJob?.Kill();
            followJob?.Kill();
            yield return FadeColor(portraitImage.color, Color.clear, 0.25f);
            fadeJob?.Kill();
            DestroyPortrait();
        }
    }
}
