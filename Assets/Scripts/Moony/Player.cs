using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MoveController a;
    public Hammering b;
    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<MoveController>();
        b = GetComponent<Hammering>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
