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
    private Vector3 Direction;
    private CharacterController controller;
    // Start is called before the first frame update
    private void Awake()
    {
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
            Direction = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            Direction = transform.TransformDirection(Direction);
            Direction *= speed;

        }
        if (JumpKey)
        {
            Direction.y = jumpPower;
            JumpKey = false;
        }


        Direction.y -= gravity * Time.deltaTime;
        controller.Move(Direction * Time.deltaTime);
    }

    public void SpringJump(Vector3 SpringPower)
    {
        Direction += SpringPower;
    }
}
