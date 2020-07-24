using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaggedValue<T>
{
    [SerializeField]
    public string name;
    [SerializeField]
    public T val;
}

[System.Serializable]
public class TFloat : TaggedValue<float> { 
    public TFloat()
    {
        name = "None";
        val = 0f;
    }
}
[System.Serializable]
public class TInt32 : TaggedValue<int> { }
[System.Serializable]
public class TString : TaggedValue<string> { }