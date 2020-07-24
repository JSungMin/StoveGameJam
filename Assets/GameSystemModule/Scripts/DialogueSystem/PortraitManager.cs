using System;
using System.Collections.Generic;
using System.Linq;
using CoreSystem.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace CoreSystem.Game.Dialogue
{
    public class PortraitManager : MonoBehaviour
    {
        public enum PivotType
        {
            LEFT,
            CENTER,
            RIGHT
        }

        public RectTransform rectTransform;
        public RectTransform[] pivots;
        public RectTransform imagePool;
        public PortraitObject curTalker;

        public ShakeManager shakeManager;

        public List<PortraitObject> stagedObjects = new List<PortraitObject>();

        public static PortraitManager Employ(RectTransform employee)
        {
            var obj = new GameObject("PortraitManager");
            obj.transform.parent = employee.transform;

            var leftPivot = new GameObject("LeftPivot");
            leftPivot.transform.SetParent(obj.transform);
            var leftRect = leftPivot.AddComponent<RectTransform>();
            leftRect.localPosition = Vector3.left * 600 + Vector3.down * 500;
            leftRect.localScale = Vector3.one;
            var centerPivot = new GameObject("CenterPivot");
            centerPivot.transform.SetParent(obj.transform);
            var centerRect = centerPivot.AddComponent<RectTransform>();
            centerRect.localPosition = Vector3.down * 500;
            centerRect.localScale = Vector3.one;
            var rightPivot = new GameObject("RightPivot");
            rightPivot.transform.SetParent(obj.transform);
            var rightRect = rightPivot.AddComponent<RectTransform>();
            rightRect.localPosition = Vector3.right * 600 + Vector3.down * 500;
            rightRect.localScale = Vector3.one;

            var imagePool = new GameObject("ImagePool");
            imagePool.transform.SetParent(obj.transform);
            var imageRect = imagePool.AddComponent<RectTransform>();
            imageRect.localPosition = Vector3.zero;
            imageRect.localScale = Vector3.one;

            var manager = obj.AddComponent<PortraitManager>();
            manager.pivots = new RectTransform[3];
            manager.pivots[0] = leftRect;
            manager.pivots[1] = centerRect;
            manager.pivots[2] = rightRect;
            manager.imagePool = imageRect;

            foreach (var pivot in manager.pivots)
            {
                pivot.sizeDelta = Vector2.right * 400;
                var layout = pivot.gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.childAlignment = TextAnchor.LowerCenter;
            }

            manager.rectTransform = obj.AddComponent<RectTransform>();
            manager.rectTransform.localScale = Vector3.one;
            manager.rectTransform.anchorMin = Vector2.zero;
            manager.rectTransform.anchorMax = Vector2.one;
            manager.rectTransform.anchoredPosition = Vector2.zero;
            manager.rectTransform.offsetMin = Vector2.zero;
            manager.rectTransform.offsetMax = new Vector2(0, -200);

            manager.shakeManager = obj.AddComponent<ShakeManager>();
            return manager;
        }

        public PortraitObject FindPortrait(ActorProfile profile)
        {
            return stagedObjects.FirstOrDefault(x => x.actorProfile.actorName == profile.actorName);
        }

        public void ShakePortrait(ActorProfile profile, int index, float amp, float freq, float duration, Action<bool> onComplete)
        {
            if (profile == null)
                return;
            var element = stagedObjects.FirstOrDefault(x => x.actorProfile.actorName == profile.actorName);
            if (null == element)
                return;
            shakeManager.Shake(index, element.transform, amp, freq, duration, onComplete);
        }
        public void AddPortrait(ActorProfile profile, string portraitName, PivotType pType)
        {
            if (profile == null)
                return;
            var pivot = pivots[(int)pType];

            var followTarget = new GameObject("Cell_" + portraitName);
            followTarget.transform.SetParent(pivot);
            var followRect = followTarget.AddComponent<RectTransform>();
            
            var obj = new GameObject("Portrait_"+ portraitName);
            obj.transform.position = pivot.position;
            obj.transform.SetParent(imagePool);

            var objRect = obj.AddComponent<RectTransform>();
            objRect.localScale = Vector3.one;
            objRect.pivot = new Vector2(0.5f,0f);
            
            var portraitObject = obj.AddComponent<PortraitObject>();
            portraitObject.rectTransform = objRect;
            portraitObject.Stage(this, profile, portraitName, followRect);
            stagedObjects.Add(portraitObject);
            if (null != curTalker)
                SetToWaiter(curTalker.actorProfile);
            curTalker = portraitObject;
        }

        public void SetToTalker(ActorProfile profile, ActorProfile.PortraitDesc desc = null, PivotType pType = PivotType.LEFT)
        {
            var element = stagedObjects.FirstOrDefault(x => x.actorProfile.actorName == profile.actorName);
            if (element == null)return;
            if (desc != null)
                element.SwapPortraitDesc(desc);
            element.GoFront();
        }

        public void SetToWaiter(ActorProfile profile)
        {
            var element = stagedObjects.FirstOrDefault(x => x.actorProfile.actorName == profile.actorName);
            if (element == null)
            {
                return;
            }
            element.FadeColor(element.CurrentColor, new Color(0.4f,0.4f,0.4f,1f), 0.25f);
        }

        public void RemovePortrait(PortraitObject portrait)
        {
            if (null == portrait) return;
            portrait.UnStage();
            stagedObjects.Remove(portrait);
        }

        public void RemoveAllPortrait()
        {
            foreach (var portraitObject in stagedObjects)
            {
                portraitObject.UnStage();
            }
            stagedObjects.Clear();
        }
    }
}
