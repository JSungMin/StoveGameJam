using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreSystem;

public class GrabableObject : MonoBehaviour, IGrabable
{
    public ActorProfile profile;
    public Rigidbody rigid;
    public bool isGrabbed = false;
    
    public virtual void OnGrabStart(GameObject grabber)
    {
        
    }
    public virtual void OnGrab(GameObject grabber)
    {
        
    }
    public virtual void OnDrop(GameObject grabber)
    {
        
    }
    protected void OnUpdate()
    {
        if(isGrabbed)
        {

        }
    }

}
