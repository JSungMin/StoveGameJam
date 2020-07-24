using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoreSystem.Game.Dialogue.Behaviour
{
    public class FadeController : ScriptBehaviour
    {
        public RectTransform rect;
        public Color endColor;
        public float duration;
        public bool useRGB = false;
        public bool childAffection = false;

        public List<RectTransform> affectionGroup = new List<RectTransform>();

        public override void Initialize(ScriptObject obj)
        {
            base.Initialize(obj);
            rect = obj.panelRect;
        }

        protected override IEnumerator Do()
        {
            for (var i = 0; i < affectionGroup.Count; i++)
            {
                UIAlphaController.Transition<MaskableGraphic>(affectionGroup[i], endColor, duration, useRGB, childAffection);
            }
            yield return null;
        }
    }
}
