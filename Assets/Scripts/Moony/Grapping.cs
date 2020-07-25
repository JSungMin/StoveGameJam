using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapping : MonoBehaviour
{
    public Player Player;

    public float HammeringTime = 0.5f;
    public float ScanningDistance = 0.5f;
    private float CoTime;
    // Start is called before the first frame update
    void Start()
    {
        CoTime = HammeringTime;
        Player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hit;
            Vector3 Scale = new Vector3((transform.localScale.x / 2) + 0.1f, 0, 0);
            Vector3 rayPosition;
            rayPosition = transform.position + Scale;
            Debug.DrawRay(rayPosition, transform.forward * ScanningDistance, Color.red);
            if (Physics.Raycast(rayPosition, transform.forward, out hit, ScanningDistance))
            {
                if (hit.collider != null)
                {
                    var grabObj = hit.collider.GetComponent<CanGrabObject>();
                    if (grabObj.isGrab)
                        grabObj.SendMessage("OnGrab", Player);
                    else
                        grabObj.SendMessage("OnDrop");
                }
                
            }
        }

    }
    
}
