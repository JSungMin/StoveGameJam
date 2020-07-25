using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapping : MonoBehaviour
{
    public Player Player;

    public float distance = 0.5f;
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
        rayPosition = transform.position;
        Debug.DrawRay(rayPosition, transform.right * ScanningDistance, Color.red);
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(rayPosition, transform.right, out hit, ScanningDistance))
            {
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    var grabObj = hit.collider.GetComponent<CanGrabObject>();
                    if (grabObj.isGrab)
                    {
                        grabObj.SendMessage("OnGrab", Player);
                        onGrab?.Invoke(grabObj);
                    }
                    else
                    {
                        grabObj.SendMessage("OnDrop");
                        onDrop?.Invoke(grabObj);
                    }
                }
                
            }
        }

    }
    
}
