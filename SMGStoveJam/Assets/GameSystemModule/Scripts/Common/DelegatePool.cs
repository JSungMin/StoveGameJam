using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  DelegatePool : 델리게이트 풀은 DelegatePool.cs로 추후 옮겨질 것임
public class CDelegates
{
    public delegate void VoidFunc();
    public delegate void VoidParamFunc(object param);
    public delegate void VoidParamsFunc(object[] paramArray);

    public delegate object ValueFunc();
    public delegate object ValueParamFunc(object param);
    public delegate object ValueParamsFunc(object[] paramArray);

    public delegate object[] ValuesFunc();
    public delegate object[] ValuesParamFunc(object param);
    public delegate object[] ValuesParamsFunc(object[] paramArray);

    public delegate T1 SpecificValueParamFunc<T1,T2>(T2 param);
}