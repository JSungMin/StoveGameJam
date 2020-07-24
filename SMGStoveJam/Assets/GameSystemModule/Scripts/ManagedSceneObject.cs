using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ManagedSceneObject : MonoBehaviour
{
    //  자식에서 정의
    public abstract void LoadWithAwake();

    public abstract void LoadWithStart();

    // 씬 메니저에서 순차 호출 할 것
    public virtual void OnAwake()
    {
        LoadWithAwake();
    }

    public virtual void OnStart()
    {
        LoadWithStart();
    }
    public virtual void OnLoaded()
    {

    }
}
