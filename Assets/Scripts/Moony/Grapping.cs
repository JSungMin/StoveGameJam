using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapping : MonoBehaviour
{
    public Player Player;
    public CanGrabObject grabbedObject;
    public float distance = 0.5f;
    public float ScanningHeight = 0.5f;
    public Action<CanGrabObject> onGrab, onDrop;
    
    // Start is called before the first frame update
    void Start()
    {
        Player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        float ScanningDistance = (transform.localScale.x / 2) + distance;
        Vector3 rayPosition;
        rayPosition = new Vector3(transform.position.x, transform.position.y + ScanningHeight, 0);
        Debug.DrawRay(rayPosition, transform.right * ScanningDistance, Color.red);
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(rayPosition, transform.right, out hit, ScanningDistance))
            {
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    grabbedObject = hit.collider.GetComponent<CanGrabObject>();
                    if (!grabbedObject.isGrab)
                    {
                        grabbedObject.SendMessage("OnGrab", Player);
                        onGrab?.Invoke(grabbedObject);
                        return;
                    }
                }
            }
            if(null != grabbedObject && grabbedObject.isGrab)
            {
                grabbedObject.SendMessage("OnDrop", Player);
                onDrop?.Invoke(grabbedObject);
                grabbedObject = null;
            }
        }

    }
    
}
