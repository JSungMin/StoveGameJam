﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MoveController MoveController;
    public Hammering Hammering;
    // Start is called before the first frame update
    void Start()
    {
        MoveController = GetComponent<MoveController>();
        Hammering = GetComponent<Hammering>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
