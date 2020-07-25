using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
    public GameObject box;
    public SpringObject spring;
    public bool isGrab = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var inputSpace = Input.GetKeyDown(KeyCode.Space);
        if(inputSpace)
        {
            Debug.Log("Space");
            if(!isGrab)
                box.SendMessage("OnGrab", CanGrabObject.GetGrabParam(gameObject));
            else
                box.SendMessage("OnDrop");
            isGrab = !isGrab;
        }        
    }
}
