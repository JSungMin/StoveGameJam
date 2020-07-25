using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider),typeof(Animator))]
public class SpringObject : MonoBehaviour
{
    protected Collider col;
    protected Animator animator;
    public int bendLimit = 1, bendCount = 0;
    private bool isBending = false;
    public Vector3 springDir = Vector3.up;
    public float springPower = 15f;
    protected void Start()
    {
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }
    public void OnHammering(Player player)
    {
        if(bendCount < bendLimit)
        {
            bendCount++;
            isBending = true;
            animator.SetInteger("BendCount", bendCount);
        }
        else
        {
            isBending = false;
            //  TODO : AddForce To Player
            player.MoveController.SpringJump(springDir*springPower);
            //  moveCon.AddForce(sprintDir*sprintPower);
        }
        animator.SetBool("IsOverBending", isBending);
    }
}
