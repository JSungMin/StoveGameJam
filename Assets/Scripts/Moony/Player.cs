using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MoveController MoveController;
    public Hammering Hammering;
    public Transform GrapObject;
    // Start is called before the first frame update
    void Start()
    {
        MoveController = GetComponent<MoveController>();
        Hammering = GetComponent<Hammering>();
        GrapObject = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
