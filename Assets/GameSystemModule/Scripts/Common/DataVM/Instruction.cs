using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Instruction
{
	public InstructionType instType = 0;
	public DataContainer localContainer;
    public bool foldContainer = true;
	[SerializeField]
	private Queue<Instruction> childQ;

    public static Instruction Make(InstructionType t, DataContainer container)
	{
		var newInst = new Instruction();
		newInst.instType = t;
        if (null != container)
			newInst.localContainer = container;
        else
            newInst.localContainer = new DataContainer();
        newInst.childQ = new Queue<Instruction>();
        return newInst;
	}
	
	public Instruction CreateAndAddChild(InstructionType t, DataContainer container = null)
    {
        var i = Make(t, container);
		AddChild(i);
        return i;
    }
    public void AddChild(Instruction i)
    {
        childQ.Enqueue(i);
    }
    public void RemoveChild(Instruction i)
    {
        if (childQ.Contains(i))
        {
            var q = new Queue<Instruction>(this.childQ.Count - 1);
            var allCurChildren = this.childQ.ToArray();
            for (int j = 0; j < allCurChildren.Length; j++)
            {
                var c = allCurChildren[j];
                if (c != i)
                    q.Enqueue(c);
            }
            childQ = q;
        }
    }
	public List<TaggedData> Datas => localContainer.Datas;
	public int ChildCount => childQ.Count;
	public int DataCount => localContainer.DataCount;
}