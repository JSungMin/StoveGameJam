using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MoveController : MonoBehaviour
{
    public float speed = 10.0f;
    public float jumpPower = 20.0f;
    public float gravity = 20.0f;
    private bool JumpKey = false;
    private Vector3 ExtraPower;
    private Vector3 Direction;
    private CharacterController controller;
    private float InAir;
    // Start is called before the first frame update
    private void Awake()
    {
        InAir = 0f;
        ExtraPower = Vector3.zero;
        controller = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                JumpKey = true;
            }
        }
    }
    // Update is called once per frame
    private void FixedUpdate()
    {

        Direction.x = Input.GetAxis("Horizontal") * speed;
        
        if (JumpKey)
        {
            Direction.y = jumpPower;
            JumpKey = false;
        }
        if(ExtraPower!=Vector3.zero)
        {
            Direction += ExtraPower;
            ExtraPower = Vector3.zero;
        }
        
        Direction.y -= gravity * Time.deltaTime;
        Direction = transform.TransformDirection(Direction);
        controller.Move(Direction * Time.deltaTime);
    }

    public void SpringJump(Vector3 SpringPower)
    {
        Direction = Vector3.zero;
        ExtraPower = SpringPower;
    }

   
}
