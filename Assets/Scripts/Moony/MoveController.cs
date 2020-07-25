using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MoveController : MonoBehaviour
{
    public float speed = 10.0f;
    public float AerialSlowDown = 0.5f;
    public float jumpPower = 20.0f;
    public float gravity = 20.0f;
    private bool JumpKey = false;
    private float RealSpeed;
    private Vector3 ExtraPower;
    private Vector3 Direction;
    private CharacterController controller;
    // Start is called before the first frame update
    private void Awake()
    {
        RealSpeed = speed;
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
        if (controller.isGrounded)
        {
            RealSpeed = speed;
        }
        else if (RealSpeed == speed)
        {
            RealSpeed *= AerialSlowDown;
        }
        Direction.x = Input.GetAxis("Horizontal") * RealSpeed;
        
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
