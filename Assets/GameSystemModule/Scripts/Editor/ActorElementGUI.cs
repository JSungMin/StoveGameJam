using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.Editor.ProfileViewer;
using CoreSystem.ProfileComponents;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor
{
    public class CGUI
    {
        public static string copyJson;
        public static void SLabelField(string label, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(label, GUI.skin.label, options);
        }
        public static void SLabelField(string label, string skin, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(label, EditorResourceSet.GetCSkin(skin).skin.label, options);
        }
        public static void SLabelField(string label, GUISkin skin, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(label, skin.label, options);
        }
        public static string STextField(string text, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextField(text, GUI.skin.textField, options);
        }
        public static string STextField(string text, string skin, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextField(text, EditorResourceSet.GetCSkin(skin).skin.textField, options);
        }
        public static string STextField(string text, GUISkin skin, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextField(text, skin.textField, options);
        }
        public static string SDelayedTextField(string text, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedTextField(text, GUI.skin.textField, options);
        }
        public static string SDelayedTextField(string text, string skin, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedTextField(text, EditorResourceSet.GetCSkin(skin).skin.textField, options);
        }
        public static string SDelayedTextField(string text, GUISkin skin, params GUILayoutOption[] options)
        {
            return EditorGUILayout.DelayedTextField(text, skin.textField, options);
        }
        public static List<T> ObjectGridField<T>(int rowCount, int colCount, List<T> objs, Action<T, int> DrawFunc = null, Action<T> OnRemove = null, params GUILayoutOption[] options)
            where T : new()
        {
            if (rowCount == 0 && colCount != 0)
                rowCount = objs.Count / colCount + 1;
            else if (colCount == 0 && rowCount != 0)
                colCount = objs.Count / rowCount + 1;
            else if (colCount == 0 && rowCount == 0)
            {
                rowCount = objs.Count;
                colCount = 1;
            }
            EditorGUILayout.BeginVertical();
            for (int row = 0; row < rowCount; row++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int col = 0; col < colCount; col++)
                {
                    var idx = row * colCount + col;
                    if (idx >= objs.Count)
                        break;
                    if (DrawFunc == null)
                    {
                        objs[idx] = VObjectField<T>(objs[idx], true, true, options);
                    }
                    else
                    {
                        DrawFunc.Invoke(objs[idx], idx);
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            return objs;
        }
        //  Return 0 : NONE Event
        //  Return 1 : Move Down
        //  Return 2 : Move Up
        //  Return 3 : Delete
        public static int VDropdownHeader<T>(int curIdx, List<T> objs, Action<int, T> onRemove)
        {
            var obj = objs[curIdx];
            var result = 0;
            if (GUILayout.Button("▼") && curIdx < objs.Count - 1)
            {
                objs[curIdx] = objs[curIdx + 1];
                objs[curIdx + 1] = obj;
                result = 1;
            }
            else if (GUILayout.Button("▲") && curIdx > 0)
            {
                objs[curIdx] = objs[curIdx - 1];
                objs[curIdx - 1] = obj;
                result = 2;
            }
            else if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                objs.Remove(obj);
                onRemove?.Invoke(curIdx, obj);
                objs.RemoveAll(x => x == null);
                result = 3;
            }

            return result;
        }
        public static List<T> VObjectDropdown<T>(string label, List<T> objs,
            ref bool foldout, Action<int, T> drawHeaderAction, Action<int, T> drawContentAction, Action onMake = null, Action<int, T> onRemove = null,
            Action<List<T>> headlineAction = null, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Foldout(foldout, label, EditorStyles.foldout);
            var count = objs.Count;
            if (null == headlineAction)
            {
                if (GUILayout.Button("Add"))
                {
                    count++;
                }
            }
            else
            {
                headlineAction.Invoke(objs);
                count = objs.Count;
            }
            count = EditorGUILayout.DelayedIntField(count, GUILayout.MaxWidth(50));
            if (count != objs.Count)
            {
                var delta = count - objs.Count;
                if (delta > 0)
                {
                    var t = typeof(T);
                    for (var i = 0; i < delta; i++)
                    {
                        if (null != onMake)
                        {
                            onMake.Invoke();
                            continue;
                        }
                        var isScriptable = TypeHelper.HaveBaseType(t, typeof(UnityEngine.ScriptableObject));
                        if (isScriptable)
                        {
                            var temp = ScriptableObject.CreateInstance(t.Name);
                            var dt = Convert.ChangeType(temp, t);
                            objs.Add((T)dt);
                        }
                        else
                        {
                            objs.Add(TypeHelper.CreateInstance<T>());
                        }
                    }
                }
                else if (delta < 0)
                {
                    objs.RemoveRange(objs.Count + delta, -delta);
                }
            }
            EditorGUILayout.EndHorizontal();
            if (foldout)
            {
                for (var i = 0; i < objs.Count; i++)
                {
                    var obj = objs[i];
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    if (null == drawHeaderAction)
                    {
                        SLabelField(typeof(T).Name, GUILayout.MinWidth(80));
                        var r = VDropdownHeader(i, objs, onRemove);
                        if (r == 3)
                            break;
                    }
                    else
                    {
                        drawHeaderAction.Invoke(i, obj);
                    }
                    EditorGUILayout.EndHorizontal();
                    drawContentAction?.Invoke(i, objs[i]);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            return objs;
        }
        public static List<T> VObjectDropdown<T>(string label, List<T> objs, ref bool foldout,
            Action<int, T> drawContentAction, Action<int, T> onRemove = null)
        {
            return VObjectDropdown<T>(label, objs, ref foldout, null, drawContentAction, null, onRemove);
        }
        public static List<T> VObjectDropdown<T>(string label, List<T> objs, ref bool foldout, Action<int, T> onRemove = null, params GUILayoutOption[] options)
        { 
            return VObjectDropdown<T>(label, objs, ref foldout,
                delegate(int idx, T obj) { obj = VObjectField<T>(obj, true, false, options); },
                onRemove);
        }

        public static int HDropdownHeader<T>(int curIdx, List<T> objs, Action<int, T> onRemove)
        {
            var obj = objs[curIdx];
            var result = 0;
            if (GUILayout.Button("◀") && curIdx < objs.Count - 1)
            {
                objs[curIdx] = objs[curIdx + 1];
                objs[curIdx + 1] = obj;
                result = 1;
            }
            else if (GUILayout.Button("▶") && curIdx > 0)
            {
                objs[curIdx] = objs[curIdx - 1];
                objs[curIdx - 1] = obj;
                result = 2;
            }
            else if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                objs.Remove(obj);
                onRemove?.Invoke(curIdx, obj);
                objs.RemoveAll(x => x == null);
                result = 3;
            }

            return result;
        }
        public static List<T> HObjectDropdown<T>(string label, List<T> objs,
            ref bool foldout, Action<int, T> drawHeaderAction, Action<int, T> drawContentAction, Action onMake = null, Action<int, T> onRemove = null,
            Action<List<T>> headlineAction = null, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Foldout(foldout, label, EditorStyles.foldout);
            var count = objs.Count;
            if (null == headlineAction)
            {
                if (GUILayout.Button("Add"))
                {
                    count++;
                }
            }
            else
            {
                headlineAction.Invoke(objs);
                count = objs.Count;
            }
            count = EditorGUILayout.DelayedIntField(count, GUILayout.MaxWidth(50));
            if (count != objs.Count)
            {
                var delta = count - objs.Count;
                if (delta > 0)
                {
                    var t = typeof(T);
                    for (var i = 0; i < delta; i++)
                    {
                        if (null != onMake)
                        {
                            onMake.Invoke();
                            continue;
                        }
                        var isScriptable = TypeHelper.HaveBaseType(t, typeof(UnityEngine.ScriptableObject));
                        if (isScriptable)
                        {
                            var temp = ScriptableObject.CreateInstance(t.Name);
                            var dt = Convert.ChangeType(temp, t);
                            objs.Add((T)dt);
                        }
                        else
                        {
                            objs.Add(TypeHelper.CreateInstance<T>());
                        }
                    }
                }
                else if (delta < 0)
                {
                    objs.RemoveRange(objs.Count + delta, -delta);
                }
            }
            EditorGUILayout.EndHorizontal();
            if (foldout)
            {
                for (var i = 0; i < objs.Count; i++)
                {
                    var obj = objs[i];
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    if (null == drawHeaderAction)
                    {
                        SLabelField(typeof(T).Name, GUILayout.MinWidth(80));
                        var r = HDropdownHeader(i, objs, onRemove);
                        if (r == 3)
                            break;
                    }
                    else
                    {
                        drawHeaderAction.Invoke(i, obj);
                    }
                    EditorGUILayout.EndHorizontal();
                    drawContentAction?.Invoke(i, objs[i]);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
            return objs;
        }
        public static List<T> HObjectDropdown<T>(string label, List<T> objs, ref bool foldout,
            Action<int, T> drawContentAction, Action<int, T> onRemove = null)
        {
            return HObjectDropdown<T>(label, objs, ref foldout, null, drawContentAction, null, onRemove);
        }
        public static List<T> HObjectDropdown<T>(string label, List<T> objs, ref bool foldout, Action<int, T> onRemove = null, params GUILayoutOption[] options)
        {
            return HObjectDropdown<T>(label, objs, ref foldout,
                delegate (int idx, T obj) { obj = VObjectField<T>(obj, true, false, options); },
                onRemove);
        }
        //  Vertical Aligned : V
        public static Enum VLabeledEnumField(string label, Enum e, params GUILayoutOption[] labelOptions)
        {
            EditorGUILayout.BeginVertical();
            SLabelField(label, labelOptions);
            e = EditorGUILayout.EnumPopup(e, labelOptions);
            EditorGUILayout.EndVertical();
            return e;
        }
        public static object VObjectField(object obj, Type t, bool useHelpbox = true, bool useLabel = true, params GUILayoutOption[] options)
        {
            if (useHelpbox)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, options);
                if (useLabel)
                    SLabelField(t.Name);
            }
            
            dynamic d = Convert.ChangeType(obj, t);
            if (t == typeof(float))
            {
                obj = EditorGUILayout.DelayedFloatField(d, options);
            }
            else if (t == typeof(string))
            {
                obj = SDelayedTextField(d, options);
            }
            else if (t == typeof(int))
            {
                obj = EditorGUILayout.DelayedIntField(d, options);
            }
            else if (t.IsEnum)
            {
                obj = EditorGUILayout.EnumPopup(d, options);
            }
            else if (t == typeof(bool))
            {
                obj = EditorGUILayout.Toggle(d, options);
            }
            else if (t == typeof(AnimationCurve))
            {
                var curveOption = new[]
                {
                    GUILayout.Width(100),
                    GUILayout.Height(100)
                };
                obj = EditorGUILayout.CurveField(d, curveOption);
            }
            else if (t.IsArray||(obj is IList && t.IsGenericType))
            {
                var toogle = true;
                if (d == null)
                {
                    d = TypeHelper.CreateInstance(t);
                }
                VObjectDropdown("", d, ref toogle, null, options);
            }
            else if (t == typeof(Sprite))
            {
                var spriteBoxOption = new GUILayoutOption[]
                {
                    GUILayout.Width(225),
                    GUILayout.Height(225)
                };
                obj = EditorGUILayout.ObjectField(d, t);
                GUIContent content;
                if (d == null)
                    content = new GUIContent("No Image");
                else
                    content = new GUIContent(d.texture);
                GUILayout.Box(content, spriteBoxOption);
            }
            else
            {
                var isUnityObject = TypeHelper.HaveBaseType(t, typeof(UnityEngine.Object));
                if (isUnityObject)
                {
                    if (d == null)
                    {
                        var isScriptable = TypeHelper.HaveBaseType(t, typeof(ScriptableObject));
                        if (isScriptable)
                            d = ScriptableObject.CreateInstance(t.Name);
                    }
                    obj = EditorGUILayout.ObjectField(d, t, options);
                }
                else
                {
                    FieldInfo[] fields = t.GetFields();
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < fields.Length; i++)
                    {
                        var fieldType = fields[i].FieldType;
                        var fieldVal = fields[i].GetValue(obj);
                        SLabelField(fields[i].Name);
                        isUnityObject = TypeHelper.HaveBaseType(fieldType, typeof(UnityEngine.Object));
                        if (isUnityObject)
                        {
                            fieldVal = EditorGUILayout.ObjectField((UnityEngine.Object)fieldVal, fieldType, options);
                            fields[i].SetValue(obj, fieldVal);
                        }
                        else
                        {
                            dynamic fd = Convert.ChangeType(fieldVal, fieldType);
                            try
                            {
                                if (fd == null)
                                    fd = TypeHelper.CreateInstance(fieldType);
                                fd = VObjectField(fd, false, true, options);
                                fields[i].SetValue(obj, fd);
                            }
                            catch(Exception e)
                            {
                                Debug.Log(t.ToString() + ", Field Can't Show");
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            if (useHelpbox)
            {
                EditorGUILayout.EndVertical();
            }
            return obj;
        }
        public static T VObjectField<T>(T obj, bool useHelpbox = true, bool useLabel = true, params GUILayoutOption[] options)
        {
            return (T)VObjectField(obj, typeof(T), useHelpbox, useLabel, options);
        }
        public static DynamicGauge DynamicGaugeField(DynamicGauge gauge, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            gauge.CurrentGauge = EditorGUILayout.FloatField(gauge.CurrentGauge, options);
            SLabelField("/", GUILayout.Width(10));
            gauge.DefaultGauge = EditorGUILayout.FloatField(gauge.DefaultGauge, options);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return gauge;
        }
        public static DynamicGauge DynamicGaugeField(string label, DynamicGauge gauge, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical();
            SLabelField(label, GUILayout.Width(170));
            EditorGUILayout.BeginHorizontal();
            gauge.CurrentGauge = EditorGUILayout.FloatField(gauge.CurrentGauge, options);
            SLabelField("/", GUILayout.Width(10));
            gauge.DefaultGauge = EditorGUILayout.FloatField(gauge.DefaultGauge, options);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return gauge;
        }
        public static Sprite SpriteField(string label, Sprite sprite, params GUILayoutOption[] options)
        {
            sprite = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), options);
            return sprite;
        }
        public static Stat StatField(string header, Stat stat, params GUILayoutOption[] options)
        {
            var statAreaRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox, ActorEditor.statFieldAreaOption);
            EditorGUILayout.Foldout(true, header, GUI.skin.label);
            var statElements = stat.elements;
            int colCount = statElements.Count / 3;
        
            for (int c = 0; c <= colCount; c++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int i = c * 3; i < statElements.Count && i < 3 * (c + 1); i++)
                {
                    EditorGUILayout.BeginVertical();
                    var element = statElements[i];
                    var name = CoreDataSet.MetaStatData.Names[i];
                    SLabelField(name, GUILayout.Width(65));
                    statElements[i] = EditorGUILayout.DelayedFloatField(element, options);
                    EditorGUILayout.EndVertical();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            return stat;

        }
        public static void StatFieldUnActive(string header, Stat stat, params GUILayoutOption[] options)
        {
            var statAreaRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox, ActorEditor.statFieldAreaOption);
            EditorGUILayout.Foldout(true, header, GUI.skin.label);
            var statElements = stat.elements;
            EditorGUI.BeginDisabledGroup(true);
            int colCount = statElements.Count / 3;
            for (int c = 0; c <= colCount; c++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int i = c * 3; i < statElements.Count && i < 3 * (c + 1); i++)
                {
                    EditorGUILayout.BeginVertical();
                    var element = statElements[i];
                    var name = CoreDataSet.MetaStatData.Names[i];
                    SLabelField(name, GUILayout.Width(65));
                    statElements[i] = EditorGUILayout.FloatField(element, options);
                    EditorGUILayout.EndVertical();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
}