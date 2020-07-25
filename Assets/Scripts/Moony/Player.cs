﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public Animator animator;
    public MoveController MoveController;
    public Hammering Hammering;
    public Grapping Grapping;
    public Transform GrapObject;
    public UnityEvent moveEvent, stopEvent, jumpEvent;
    public UnityEvent grabEvent, dropEvent;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        MoveController = GetComponent<MoveController>();
        Hammering = GetComponent<Hammering>();
        GrapObject = transform.GetChild(0);
        MoveController.onMove += OnMove;
        MoveController.onStop += OnStop;
        MoveController.onJump += OnJump;
    
        Grapping.onGrab += OnGrab;
        Grapping.onDrop += OnDrop;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMove()
    {
        if(MoveController.GetVelocity.x > 0)
            animator.transform.localScale = new Vector3(1, 1, 1);
        else if(MoveController.GetVelocity.x < 0)
            animator.transform.localScale = new Vector3(-1, 1, 1);
        animator.SetBool("Running", true);
        moveEvent?.Invoke();
    }
    private void OnStop()
    {
        animator.SetBool("Running", false);
        stopEvent?.Invoke();
    }
    private void OnJump()
    {
        jumpEvent?.Invoke();
    }
    private void OnGrab(CanGrabObject obj)
    {
        animator.SetBool("Grabbing", true);
        grabEvent?.Invoke();
    }
    private void OnDrop(CanGrabObject obj)
    {
        animator.SetBool("Grabbing", false);
        dropEvent?.Invoke();
    }
}
