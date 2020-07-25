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
    public Collider col;
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
        transform.parent = curPivot;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.EulerAngles(new Vector3(0,0,0));
        rigid.velocity = Vector3.zero;
        rigid.Sleep();
        col.isTrigger = true;
        isGrab = true;
    }
    public void OnDrop(Player player)
    {
        int DropDir = 0;
        if (player.transform.localRotation.y < 0)
        {
            DropDir = -100;
        }
        else if (player.transform.localRotation.y >= 0)
        {
            DropDir = 100;
        }
        
        isGrab = false;
        transform.SetParent(restoreParent);
        curPivot = null;
        rigid.WakeUp();
        rigid.velocity = Vector3.zero;
        Debug.Log("Drop: " + DropDir.ToString());
        col.isTrigger = false;
        rigid.AddForce(new Vector3(DropDir, 100f, 0));
    }
    private void FixedUpdate()
    {
        if (isGrab)
        {
            transform.localPosition = Vector3.zero;
            rigid.Sleep();
        }
    }
}
