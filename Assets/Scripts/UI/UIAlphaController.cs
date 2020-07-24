using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAlphaController
{
    public static BehaviourJob StartFadeOut(RectTransform rectTransform, float duration)
    {
        var images = new List<MaskableGraphic>();
        TravelChilds(rectTransform, delegate (Transform t)
        {
            var i = t.GetComponent<MaskableGraphic>();
            if (i != null)
                images.Add(i);
        });
        return BehaviourJob.Make(IFadeOut(images, duration), true);
    }
    public static BehaviourJob StartFadeIn<T>(RectTransform rectTransform, float duration) where T : MaskableGraphic
    {
        var images = new List<T>();
        TravelChilds(rectTransform, delegate (Transform t)
        {
            var i = t.GetComponent<T>();
            if (i != null)
                images.Add(i);
        });
        return BehaviourJob.Make(IFadeIn(images, duration), true);
    }
    public static BehaviourJob Transition<T>(RectTransform rectTransform, Color eColor, float duration, bool useRGB, bool enterChild = true) where T : MaskableGraphic
    {
        var images = new List<MaskableGraphic>();
        if (enterChild)
        {
            TravelChilds(rectTransform, delegate (Transform t)
            {
                var i = t.GetComponent<T>();
                if (i != null)
                    images.Add(i);
            });
        }
        else
        {
            var comp = rectTransform.GetComponent<T>();
            if (null != comp)
                images.Add(comp);
        }
        return BehaviourJob.Make(ITransitionColor(images, eColor, duration, useRGB));
    }
    public static void SetAlphaColor<T>(RectTransform rectTransform, float alpha) where T : MaskableGraphic
    {
        TravelChilds(rectTransform, delegate(Transform t)
        {
            var i = t.GetComponent<T>();
            if (i == null) return;
            var resultColor = i.color;
            resultColor.a = alpha;
            i.color = resultColor;
        });
    }
    private static void TravelChilds(Transform t, Action<Transform> action)
    {
        action?.Invoke(t);
        for(int i = 0; i < t.childCount; i++)
        {
            TravelChilds(t.GetChild(i), action);
        }
    }

    private static IEnumerator ITransitionColor<T>(List<T> images, Color eColor, float duration, bool useRGB) where T: MaskableGraphic
    {
        var originColors = new Color[images.Count];
        var targetColors = new Color[images.Count];
        for (var i = 0; i < images.Count; i++)
        {
            originColors[i] = images[i].color;
            if (useRGB)
                targetColors[i] = eColor;
            else
            {
                targetColors[i] = images[i].color;
                targetColors[i].a = eColor.a;
            }
            
        }
        var timer = 0f;
        while (timer <= duration)
        {
            for (var i = 0; i < images.Count; i++)
            {
                images[i].color = Color.Lerp(originColors[i], targetColors[i], timer / duration);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        for (var i = 0; i < images.Count; i++)
        {
            images[i].color = targetColors[i];
        }
    }
    private static IEnumerator IFadeOut<T>(List<T> images, float duration) where T: MaskableGraphic
    {
        var originColors = new Color[images.Count];
        var targetColors = new Color[images.Count];
        for (int i = 0; i < images.Count; i++)
        {
            originColors[i] = images[i].color;
            targetColors[i] = images[i].color;
            targetColors[i].a = 0f;
        }
        var timer = 0f;
        while(timer <= duration)
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].color = Color.Lerp(originColors[i], targetColors[i], timer / duration);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < images.Count; i++)
        {
            images[i].color = targetColors[i];
        }
    }
    private static IEnumerator IFadeIn<T>(List<T> images, float duration) where T : MaskableGraphic
    {
        var originColors = new Color[images.Count];
        var targetColors = new Color[images.Count];
        for (int i = 0; i < images.Count; i++)
        {
            originColors[i] = images[i].color;
            originColors[i].a = 0f;
            targetColors[i] = images[i].color;
            targetColors[i].a = 1f;
        }
        var timer = 0f;
        while (timer <= duration)
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].color = Color.Lerp(originColors[i], targetColors[i], timer / duration);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < images.Count; i++)
        {
            images[i].color = targetColors[i];
        }
    }
}
