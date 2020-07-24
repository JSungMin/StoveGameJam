using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorResourceSet : SingletonScriptableObject<EditorResourceSet> {
    [System.Serializable]
    public class CGUISkin
    {
        public GUISkin skin;
        public Color bgColor;
        public Texture bgTexture;
        public Color contentBG = Color.white;

        public string Name
        {
            get
            {
                return (skin != null) ? skin.name : "";
            }
        }
    }
    [System.Serializable]
    public class TaggedColor
    {
        public string name;
        public Color color;

        public TaggedColor()
        {
            name = "";
            color = Color.clear;
        }
        public TaggedColor(string n, Color c)
        {
            name = n;
            color = c;
        }
    }
    public List<TaggedColor> colors = new List<TaggedColor>();
    public GUIStyle mainStyle;
    public List<CGUISkin> uISkins = new List<CGUISkin>();
    public Dictionary<string, TaggedColor> colorDictionary = new Dictionary<string, TaggedColor>();
    public Dictionary<string, CGUISkin> uiSkinDictionary = new Dictionary<string, CGUISkin>();


    public static EditorResourceSet CreateInstance()
    {
        var ers = CreateInstance<EditorResourceSet>();
        var pathOfERS = SystemPathes.GetSystemPath(SystemPathes.PathType.EditorResourceSet);
        UnityEditor.AssetDatabase.CreateAsset(ers, pathOfERS);
        UnityEditor.EditorUtility.SetDirty(ers);
        ers.mainStyle = new GUIStyle(EditorStyles.whiteLabel);
        UnityEditor.AssetDatabase.Refresh();
        return ers;
    }
    public void OnEnable()
    {
        uiSkinDictionary = new Dictionary<string, CGUISkin>();
        colorDictionary = new Dictionary<string, TaggedColor>();
        for (int i = 0; i < colors.Count; i++)
        {
            if (!colorDictionary.ContainsKey(colors[i].name))
                colorDictionary.Add(colors[i].name,colors[i]);
        }
        for (int i = 0; i < uISkins.Count; i++)
        {
            if (!uiSkinDictionary.ContainsKey(uISkins[i].Name))
                uiSkinDictionary.Add(uISkins[i].Name, uISkins[i]);
        }
    }
    public static GUIStyle GetMainStyle()
    {
        return Instance.mainStyle;
    }
    public static Color GetColor(string name)
    {
        if (Instance.colorDictionary.ContainsKey(name))
            return Instance.colorDictionary[name].color;
        return Color.white;
    }
    public static CGUISkin GetCSkin(string name)
    {
        return Instance.uiSkinDictionary[name];
    }
    public static GUISkin ApplyCSkin(string name, Rect bgRect)
    {
        try
        {
            var cskin = Instance.uiSkinDictionary[name];
            GUI.skin = cskin.skin;
            GUI.backgroundColor = cskin.contentBG;
            EditorGUI.DrawRect(bgRect, cskin.bgColor);
            if (cskin.bgTexture != null)
                EditorGUI.DrawTextureTransparent(bgRect, cskin.bgTexture, ScaleMode.StretchToFill);
            return GUI.skin;
        }
        catch(System.Exception e)
        {
            Debug.LogWarning("ApplyCSkin : " + e.Message.ToString());
            return GUI.skin;
        }
    }
}
