using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreSystem.Game.Dialogue;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/DialogueVM")]
public class DialogueActionVM : DataVM
{
    /*TODO : 콜 스택을 만들어야하며, 이를 위한 명령어 래퍼 필요
    1. 현재 BaseScript.ActionScript를 보다 보편적으로 사용하게 빼내야함
    2. Event와 같은 인터럽트가 걸리면 이전 스크립트와 기존 인덱스를 저장해야함
    때문에 이를 위한 스택 필요
    */
    
    public void OnEventInvoke(object sender, string eventName)
    {
        var script = eventListenScripts.FirstOrDefault(x => x.name == eventName);
        if (script != null) RunAndInterrupt(script);
    }

    protected override BehaviourJob Interpret(Instruction i)
    {
        var output = base.Interpret(i);
        if (i.instType == InstructionType.CALL)
        {
            var methodName = i.Datas[0].StringData;
            var parameters = i.Datas.GetRange(1, i.DataCount - 1);
            output = Execute(Call(methodName, parameters));
        }
        return output;
    }

    protected override IEnumerator Call(string methodName, List<TaggedData> parameters)
    {
        yield return callListener.OnCall(methodName, parameters);
    }

    protected IEnumerator INextScript(int idx)
    {
        yield break;
    }
}
