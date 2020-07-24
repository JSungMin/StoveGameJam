using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VMScript
{
    public string name;
    public bool runOnAwake = false;
    public bool isEventScript = false;
    public List<Instruction> instructions;
    public int CountOfInstruction => instructions.Count;

    public VMScript()
    {
        name = "Action";
        instructions = new List<Instruction>();
    }
    public VMScript(IEnumerable<Instruction> inst)
    {
        name = "Action";
        instructions = new List<Instruction>(inst);
    }

    public void CopyTo(VMScript script)
    {
        script.name = name;
        script.runOnAwake = runOnAwake;
        script.isEventScript = isEventScript;
        script.instructions = new List<Instruction>(instructions);
    }
}