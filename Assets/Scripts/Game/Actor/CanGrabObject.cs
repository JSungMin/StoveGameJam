using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class CanGrabObject : MonoBehaviour
{
    public class GrabParam
    {
        public GameObject actor;
        public Transform pivot;
        public GrabParam(GameObject a, Transform p)
        {
            actor = a;
            pivot = p;
        }
    }
    protected Rigidbody rigid;
    protected Collider col;
    protected Transform curPivot = null;
    protected Transform restoreParent;
    public bool isGrab = false;
    // Start is called before the first frame update
    protected void Start()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        restoreParent = transform.parent;
    }
    public static GrabParam GetGrabParam(GameObject actor, Transform pivot = null) => new GrabParam(actor, pivot); 
    public void OnGrab(Player player)
    {
        var grabParam = GetGrabParam(player.gameObject, player.GrapObject);
        var actor = grabParam.actor;
        var pivot = grabParam.pivot;

        if(pivot != null)
            curPivot = pivot;
        else
            curPivot = actor.transform;
        transform.SetParent(curPivot.transform);
        transform.localPosition = Vector3.zero;
        rigid.velocity = Vector3.zero;
        rigid.Sleep();
        col.enabled = false;
        isGrab = true;
    }
    public void OnDrop()
    {
        isGrab = false;
        transform.SetParent(restoreParent);
        curPivot = null;
        rigid.WakeUp();
        rigid.velocity = Vector3.zero;
        col.enabled = true;
    }
    private void Update()
    {
        if (isGrab) rigid.Sleep();
    }
}
