using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class CanGrabObject : InteractObject
{
    protected Rigidbody rigid;
    protected Collider col;
    protected Transform curPivot = null;

    // Start is called before the first frame update
    protected override void Start()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    public override void OnInteractStart(GameObject actor, params object[] objs)
    {
        if(objs.Length > 0)
            curPivot = (Transform)objs[0];
        else
            curPivot = actor.transform;
        transform.SetParent(curPivot.transform);
        transform.localPosition = Vector3.zero;
        rigid.velocity = Vector3.zero;
        rigid.Sleep();
    }
    //  It's meen Actor Hold This Object
    public override void OnInteractUpdate(GameObject actor)
    {
    }
    public override void OnInteractEnd(GameObject actor)
    {
        rigid.WakeUp();
    }
}
