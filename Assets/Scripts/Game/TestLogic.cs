using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
    public InteractController box;
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
            box.Interact(gameObject);
        }        
    }
}
