using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void OnInteractStart(GameObject actor, params object[] objs);
    void OnInteractUpdate(GameObject actor);
    void OnInteractEnd(GameObject actor);
}

[RequireComponent(typeof(InteractController), typeof(Rigidbody), typeof(Collider))]
public class CanGrabObject : MonoBehaviour, IInteractable
{
    protected Rigidbody rigid;
    protected Collider col;
    protected InteractController controller; 
    protected Transform curPivot = null;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        controller = GetComponent<InteractController>();
        controller.onInteractStart += OnInteractStart;
        controller.onInteractUpdate += OnInteractUpdate;
        controller.onInteractEnd += OnInteractEnd;
    }
    public void OnInteractStart(GameObject actor, params object[] objs)
    {
        if(objs.Length > 0)
            curPivot = (Transform)objs[0];
        else
            curPivot = actor.transform;
        transform.SetParent(curPivot.transform);
        rigid.velocity = Vector3.zero;
        rigid.Sleep();
    }
    //  It's meen Actor Hold This Object
    public void OnInteractUpdate(GameObject actor)
    {
    }
    public void OnInteractEnd(GameObject actor)
    {
        rigid.WakeUp();
    }
}
