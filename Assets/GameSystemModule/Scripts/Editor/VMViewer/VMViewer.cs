using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreSystem.Editor
{
    
    public class VMViewer : EditorWindow
    {
        public static string copyData;
        public static VMViewer window;
        public static VMMacroSet macroSet;
        public static Rect windowRect = new Rect(300, 300, 600, 800);
        public VMScript mScript = new VMScript();
        public Vector2 instScroll;

        private bool foldInst = true;
        private int focusedInstIdx = 0;
        private InstructionType targetInstType;

        public List<Instruction> Instructions => mScript.instructions;

        public enum VMType
        {
            DialogueActionVM
        }
        public enum CompType
        {
            Equal = 0,
            Less,
            Greater,
            LessEqual,
            GreaterEqual,
            NotEqual
        }

        public static void CopyScript(VMScript script)
        {
            copyData = JsonUtility.ToJson(script, true);
        }

        public static void PasteScript(VMScript target)
        {
            JsonUtility.FromJsonOverwrite(copyData, target);
        }

        private void DrawCompare(Instruction i)
        {
            var instIdx = GetInstructionIndex(i);
            if (i.DataCount != 1)
            {
                var oper = new TaggedData("Operation", TaggedData.DataType.Int, 0);
                i.Datas.Add(oper);
            }
            EditorGUILayout.BeginHorizontal();

            var operand01 = (instIdx - 2 >= 0) ? Instructions[instIdx - 2] : null;
            var operand02 = (operand01 != null) ? Instructions[instIdx - 1] : null;
            var operandText01 = "";
            var operandText02 = "";
            if (operand01 != null && operand02 != null)
            {
                if (operand01.instType == InstructionType.PUSH)
                {
                    operandText01 = "#" + (instIdx - 2)+":"+operand01.Datas[0].PrintData();
                }
                else if (operand01.instType == InstructionType.CALL)
                {
                    operandText01 = "#" + (instIdx - 2) + ":{" + operand01.Datas[0].PrintData() + "}";
                }
                if (operand02.instType == InstructionType.PUSH)
                {
                    operandText02 = "#" + (instIdx - 1) + ":" + operand02.Datas[0].PrintData();
                }
                else if (operand02.instType == InstructionType.CALL)
                {
                    operandText02 = "#" + (instIdx - 1) + ":{" + operand02.Datas[0].PrintData() + "}";
                }
            }
            EditorGUILayout.LabelField(operandText01);
            GUILayout.FlexibleSpace();
            var operType = i.Datas[0].IntData;
            i.Datas[0].IntData = Convert.ToInt32(EditorGUILayout.EnumPopup((CompType)i.Datas[0].IntData));
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(operandText02);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        private void DrawBranch(Instruction i)
        {
            var instIdx = GetInstructionIndex(i);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("IF ");
            if (instIdx == 0)
            {
                var compInst = Instruction.Make(InstructionType.COMPARE, new DataContainer());
                Instructions.Insert(instIdx, compInst);
                instIdx++;
            }
            else if (Instructions[instIdx - 1].instType != InstructionType.COMPARE)
            {
                var compInst = Instruction.Make(InstructionType.COMPARE, new DataContainer());
                Instructions.Insert(instIdx - 1, compInst);
                instIdx++;
            }
            EditorGUILayout.LabelField("Index Of #" + (instIdx - 1) + " Is True");
            EditorGUILayout.EndHorizontal();

            //  TODO : Define True IDX, False IDX
            if (i.DataCount != 2)
            {
                var first = new TaggedData("T", TaggedData.DataType.Int, instIdx);
                var second = new TaggedData("F", TaggedData.DataType.Int, instIdx + 1);
                i.Datas.Add(first);
                i.Datas.Add(second);
            }
            EditorGUI.BeginChangeCheck();
            i.Datas[0].IntData = EditorGUILayout.IntField("TRUE ", i.Datas[0].IntData);
            i.Datas[1].IntData = EditorGUILayout.IntField("FALSE ", i.Datas[1].IntData);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateFocusIdx(i);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCall(Instruction i)
        {
            if (i.DataCount == 0)
                i.Datas.Add(new TaggedData("MethodName", TaggedData.DataType.String, ""));
            EditorGUILayout.LabelField("Name " + i.Datas[0].StringData);
            var prevName = i.Datas[0].StringData;
            CGUI.VObjectDropdown("Local Container", i.Datas, ref i.foldContainer, DrawTaggedValue);
            if (macroSet != null)
            {
                if (prevName != i.Datas[0].StringData)
                {
                    var matchedData = macroSet.GetMatchedParameters(i.Datas[0].StringData);
                    if (null != matchedData)
                        i.localContainer.Datas = matchedData.ToList();
                }
            }
        }

        private void DrawPush(Instruction i)
        {
            if (i.DataCount == 0)
                i.Datas.Add(new TaggedData("Operand", TaggedData.DataType.Float, 0f));
            CGUI.VObjectDropdown("Local Container", i.Datas, ref i.foldContainer, DrawTaggedValue);
        }
        private void DrawDelay(Instruction i)
        {
            if (i.DataCount == 0)
            {
                i.Datas.Add(new TaggedData("Type(0:Frame,1:Sec)", TaggedData.DataType.Int, 0));
                i.Datas.Add(new TaggedData("Amount", TaggedData.DataType.Int, 0));
            }
            CGUI.VObjectDropdown("Local Container", i.Datas, ref i.foldContainer, DrawTaggedValue);
        }
        private void DrawTaggedValue(int idx, TaggedData data)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();
            data.dataType = (TaggedData.DataType)EditorGUILayout.EnumPopup(data.dataType);
            EditorGUILayout.BeginHorizontal();
            data.dataName = EditorGUILayout.DelayedTextField(data.dataName);
            if (data.dataType == TaggedData.DataType.Int)
            {
                data.IntData = EditorGUILayout.DelayedIntField(data.IntData);
            }
            else if (data.dataType == TaggedData.DataType.Float)
            {
                data.FloatData = EditorGUILayout.DelayedFloatField(data.FloatData);
            }
            else if (data.dataType == TaggedData.DataType.String)
            {
                data.StringData = EditorGUILayout.DelayedTextField(data.StringData);
            }
            else if (data.dataType == TaggedData.DataType.Boolean)
            {
                data.BoolData = EditorGUILayout.Toggle(data.BoolData);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                
            }
        }

        public void DrawInstructionContent(int idx, Instruction i)
        {
            if (i.localContainer == null)
                i.localContainer = new DataContainer();
            EditorGUI.BeginChangeCheck();
            i.instType = (InstructionType)EditorGUILayout.EnumPopup(i.instType);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateFocusIdx(i);
            }

            if (i.instType == InstructionType.PUSH)
            {
                DrawPush(i);
            }
            else if (i.instType == InstructionType.CALL)
            {
                DrawCall(i);
            }
            else if (i.instType == InstructionType.COMPARE)
            {
                DrawCompare(i);
            }
            else if (i.instType == InstructionType.BRANCH)
            {
                DrawBranch(i);
            }
            else if (i.instType == InstructionType.DELAY)
            {
                DrawDelay(i);
            }
        }
        public void DrawInstructionHeader(int idx, Instruction i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("#"+idx);
            CGUI.VDropdownHeader(idx, Instructions, null);
            EditorGUILayout.EndHorizontal();
        }

        private void OnMakeInstruction()
        {
            var newOwn = Instruction.Make(InstructionType.PUSH, new DataContainer());
            Instructions.Add(newOwn);
        }

        private void OnRemoveInstruction(int idx, Instruction i)
        {
            Instructions.Remove(i);
            Instructions.RemoveAll(x => x == null);
        }

        public void OnSaveInstructions()
        {
            var absPath = EditorUtility.SaveFilePanel("Save VMScript", "", "Action", "json");
            var relPath = absPath.Substring(absPath.IndexOf("Asset") + 7);
            JsonHelper.DataObjectToJsonFile(mScript, relPath);
        }

        public void OnLoadInstructions()
        {
            var absPath = EditorUtility.OpenFilePanel("Load VMScript", "", "json");
            var relPath = absPath.Substring(absPath.IndexOf("Asset") + 7);
            var loaded = new VMScript();
            JsonHelper.DataJsonFileToObject(relPath, loaded);
            loaded.CopyTo(mScript);
        }
        public void DrawVM()
        {
            macroSet = EditorGUILayout.ObjectField("Macro", macroSet, typeof(VMMacroSet)) as VMMacroSet;
            EditorGUILayout.BeginHorizontal();
            mScript.name = EditorGUILayout.TextField("Name ", mScript.name);
            targetInstType = (InstructionType)EditorGUILayout.EnumPopup(targetInstType);
            focusedInstIdx = EditorGUILayout.IntField("Insert Index", focusedInstIdx);
            focusedInstIdx = Mathf.Clamp(focusedInstIdx, 0, Instructions.Count);
            if (GUILayout.Button("Insert"))
            {
                var newInst = Instruction.Make(targetInstType, new DataContainer());
                Instructions.Insert(focusedInstIdx, newInst);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                OnSaveInstructions();
            }
            if (GUILayout.Button("Load"))
            {
                OnLoadInstructions();
            }
            EditorGUILayout.EndHorizontal();
            instScroll = EditorGUILayout.BeginScrollView(instScroll);
            CGUI.VObjectDropdown("Instructions", Instructions, ref foldInst,DrawInstructionHeader, DrawInstructionContent, OnMakeInstruction, OnRemoveInstruction);
            EditorGUILayout.EndScrollView();
        }
        private int GetInstructionIndex(Instruction i)
        {
            for (var c = 0; c < Instructions.Count; c++)
            {
                if (Instructions[c] == i)
                    return c;
            }
            return -1;
        }
        private void UpdateFocusIdx(Instruction i)
        {
            focusedInstIdx = GetInstructionIndex(i);
        }
        [MenuItem("Window/CoreSystem/VMViewer")]
        public static void ShowWindow()
        {
            window = GetWindowWithRect<VMViewer>(windowRect);
            window.titleContent.text = "VM Editor";
        }

        public static void ShowWindow(VMScript script)
        {
            window = GetWindowWithRect<VMViewer>(windowRect);
            window.titleContent.text = "VM Editor";
            window.mScript = script;
        }
        private void OnGUI()
        {
            DrawVM();
        }
    }
}
