using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InteractController : MonoBehaviour
{
    public int state = -1;
    [SerializeField]
    private GameObject _actor;
    public Action<GameObject, object[]> onInteractStart;
    public Action<GameObject> onInteractUpdate;
    public Action<GameObject> onInteractEnd;

    public void Interact(GameObject actor, params object[] objs)
    {
        if(state == -1)
        {
            state = 0;
            _actor = actor;
            onInteractStart?.Invoke(actor, objs);
        }
    }
    public void StopInteract(GameObject actor)
    {
        if(state >= 0) onInteractEnd?.Invoke(actor);
        actor = null;
        state = -1;
    }
    private void Update()
    {
        if(state >= 0)
        {
            state = 1;
            onInteractUpdate?.Invoke(_actor);
        }
    }
}
