using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void OnInteractStart(GameObject actor, params object[] objs);
    void OnInteractUpdate(GameObject actor);
    void OnInteractEnd(GameObject actor);
}
public abstract class InteractObject : MonoBehaviour, IInteractable
{
    public int interactState = -1;
    [SerializeField]
    private GameObject _actor;
    public Action<GameObject, object[]> onInteractStart;
    public Action<GameObject> onInteractUpdate;
    public Action<GameObject> onInteractEnd;

    public void Interact(GameObject actor, params object[] objs)
    {
        if(interactState == -1)
        {
            interactState = 0;
            _actor = actor;
            onInteractStart?.Invoke(actor, objs);
        }
    }
    public void StopInteract(GameObject actor)
    {
        if(interactState >= 0) onInteractEnd?.Invoke(actor);
        actor = null;
        interactState = -1;
    }
    protected virtual void Start() 
    {
        onInteractStart += OnInteractStart;
        onInteractUpdate += OnInteractUpdate;
        onInteractEnd += OnInteractEnd;
    }
    private void Update()
    {
        if(interactState >= 0)
        {
            interactState = 1;
            onInteractUpdate?.Invoke(_actor);
        }
    }

    public abstract void OnInteractStart(GameObject actor, params object[] objs);
    public abstract void OnInteractUpdate(GameObject actor);
    public abstract void OnInteractEnd(GameObject actor);
}
