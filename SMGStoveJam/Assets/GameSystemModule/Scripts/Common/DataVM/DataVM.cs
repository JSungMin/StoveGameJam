using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum InstructionType
{
    PUSH,
    POP,
    CALL,
    COMPARE,
    BRANCH,
    DELAY
}
public abstract class DataVM : ScriptableObject
{
    public struct ScriptCallState
    {
        public VMScript script;
        public int index;
        public Queue<TaggedData> operandStack;
        public Stack<TaggedData> outputStack;
        public BehaviourJob job;
    }
    //  Push On AwakeRun, Pop On Complete 
    public Stack<ScriptCallState> callStateStack = new Stack<ScriptCallState>();

    public Queue<TaggedData> operandStack = new Queue<TaggedData>();
    public Stack<TaggedData> outputStack = new Stack<TaggedData>();

    public VMScript curScript;
    public List<VMScript> eventListenScripts = new List<VMScript>();
    public ICallListener callListener;

    public int instIdx = -1;

    public BehaviourJob runJob;

    public virtual void Initialize(List<VMScript> scripts, ICallListener listener)
    {
        callListener = listener;
        AwakeRun(scripts);
    }

    public void RunAndInterrupt(VMScript script)
    {
        if (script.CountOfInstruction == 0)
            return;
        if (runJob != null)
        {
            //  다른 스크립트를 실행하는 것으로 이는 인터럽트와 같다.
            //  따라서 해당 스크립트 행위를 끝내면 기존 스크립트로 돌아와야하는데
            //  이를 위해 기존 데이터를 스택에 남긴다.
            var curState = new ScriptCallState();
            curState.script = curScript;
            curState.index = instIdx;
            curState.operandStack = operandStack;
            curState.outputStack = outputStack;
            runJob.Pause();
            curState.job = runJob;
            callStateStack.Push(curState);
        }
        //  새로운 스크립트를 실행하기 위한 초기화 작업을 한다.
        curScript = script;
        instIdx = 0;
        operandStack = new Queue<TaggedData>();
        outputStack = new Stack<TaggedData>();

        runJob = BehaviourJob.Make(IRun(),false);
        //  스크립트 전체가 다 실행되고 나면 stateStack에서
        //  기존 스테이트를 꺼내다 다시 실행한다.
        runJob.Start();
    }

    private void OnEndScript(bool obj)
    {
        var count = callStateStack.Count;
        if (count <= 0) return;
        var callState = callStateStack.Pop();
        curScript = callState.script;
        instIdx = callState.index;
        operandStack = callState.operandStack;
        outputStack = callState.outputStack;
        runJob = callState.job;
        runJob.UnPause();
    }
    public void AwakeRun(List<VMScript> scripts)
    {
        for (var i = 0; i < scripts.Count; i++)
        {
            var script = scripts[i];
            if (script.runOnAwake)
                RunAndInterrupt(script);
            if (script.isEventScript)
                eventListenScripts.Add(script);
        }
    }
    public void Pause()
    {
        runJob?.Pause();
    }

    public void UnPause()
    {
        runJob?.UnPause();
    }

    private void GoNext(bool isKilled)
    {
        if (++instIdx >= Instructions.Count)
        {
            OnEndScript(isKilled);
            return;
        }
        Interpret(CurInstruction);
    }

    private void GoTo(int idx)
    {
        instIdx = idx;
    }

    protected BehaviourJob Execute(IEnumerator co, bool moveNext = true)
    {
        var job = BehaviourJob.Make(co, false);
        if (moveNext)
            job.OnComplete += GoNext;
        job.Start();
        runJob = job;
        return job;
    }
    //  "CALL" Instruction Interpreting is Defined On Child Of DataVM
    protected virtual BehaviourJob Interpret(Instruction i)
    {
        BehaviourJob output = null;
        if (i.instType == InstructionType.PUSH)
        {
            output = Execute(IPush(i.localContainer));
        }
        else if (i.instType == InstructionType.POP)
        {
            Pop();
        }
        else if (i.instType == InstructionType.COMPARE)
        {
            output = Execute(IPush(i.localContainer),false);
            output = Execute(Compare());
        }
        else if (i.instType == InstructionType.BRANCH)
        {
            output = Execute(IPush(i.localContainer),false);
            output = Execute(Branch());
        }
        else if (i.instType == InstructionType.DELAY)
        {
            output = Execute(IPush(i.localContainer),false);
            output = Execute(Delay());
        }
        return output;
    }
    protected IEnumerator IRun()
    {
        yield return Interpret(CurInstruction);
    }

    protected void Push(TaggedData data)
    {
        operandStack.Enqueue(data);
    }
    protected IEnumerator IPush(DataContainer container)
    {
        for (var i = 0; i < container.DataCount; i++)
        {
            Push(container.Datas[i]);
        }
        yield break;
    }

    protected void Pop()
    {
        var pop = operandStack.Dequeue();
        outputStack.Push(pop);
    }
    protected abstract IEnumerator Call(string methodName, List<TaggedData> parameters);

    //  구조적 배치관계
    //  --Push Operator : Instruction Local에 필수로 존재
    //  --Compare => Pop them all => Push Compare result To OutputStack
    protected IEnumerator Compare()
    {
        Pop();
        var x = outputStack.Pop();
        Pop();
        var y = outputStack.Pop();
        Pop();
        var comp = outputStack.Pop().IntData;
        var result = false;
        switch (comp)
        {
            case 0:
                result = x == y;
                break;
            case 1:
                result = x < y;
                break;
            case 2:
                result = x > y;
                break;
            case 3:
                result = x <= y;
                break;
            case 4:
                result = x >= y;
                break;
            case 5:
                result = x != y;
                break;
        }
        Push(new TaggedData("Comp_Result", TaggedData.DataType.Boolean,result));
        yield break;
    }
    //  구조적 배치 관계
    //  --Call Instruction Compare => Push Compare Result
    //  --Call Instruction Branch => Pop Compare Result => GOTO New InstIdx
    protected IEnumerator Branch()
    {
        //  Push Top Data To OutputStack
        Pop();
        var trueIdx = outputStack.Pop().IntData;
        Pop();
        var falseIdx = outputStack.Pop().IntData;
        Pop();
        var compResult = outputStack.Pop().BoolData;
        GoTo(compResult ? trueIdx : falseIdx);
        yield return null;
    }

    protected IEnumerator Delay()
    {
        Pop();
        var delayType = outputStack.Pop().IntData;
        if (delayType == 0)
        {
            Pop();
            var frameAmount = outputStack.Pop().IntData;
            var frameCounter = 0;
            while (++frameCounter <= frameAmount)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Pop();
            var timeAmount = outputStack.Pop().FloatData;
            yield return new WaitForSeconds(timeAmount);
        }
    }
#if UNITY_EDITOR
    public static T CreateInstance<T>(string path) where T : DataVM
    {
        var data = CreateInstance<T>();
        UnityEditor.AssetDatabase.CreateAsset(
            data,
            path
        );
        UnityEditor.EditorUtility.SetDirty(data);

        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.AssetDatabase.SaveAssets();
        return data;
    }
#endif
    public static T GetClone<T>(T origin) where T : DataVM 
    {
        var clone = (T)Instantiate(origin);
        Type cloneType = origin.GetType();
        FieldInfo[] fields = cloneType.GetFields();
        for (var i = 0; i < fields.Length; i++)
        {
            var fieldType = fields[i].FieldType;
            var fieldVal = fields[i].GetValue(clone);
            bool isUO = TypeHelper.HaveBaseType(fieldType, typeof(UnityEngine.Object));
            if (isUO)
            {
                var cloneField = Instantiate((UnityEngine.Object)fieldVal);
                fields[i].SetValue(clone, cloneField);
            }
        }
        return clone;
    }

    public bool IsRunning => runJob.IsRunning;
    public bool IsPaused => runJob.IsPaused;
    public List<Instruction> Instructions => curScript.instructions;
    public Instruction CurInstruction => (instIdx >= Instructions.Count) ? null : Instructions[instIdx];
}
