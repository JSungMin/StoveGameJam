using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider),typeof(Animator))]
public class SpringObject : MonoBehaviour
{
    public Collider col;
    protected Animator animator;
    public int bendLimit = 1, bendCount = 0;
    public bool isBending = false;
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

        }
        else
        {
            animator.SetTrigger("IsOverBending");
            isBending = false;
            bendCount = 0;
            //  TODO : AddForce To Player
            player.MoveController.SpringJump(springDir*springPower);
            //  moveCon.AddForce(sprintDir*sprintPower);
        }
        animator.SetInteger("BendCount", bendCount);
        //parentAnimator.SetInteger("parentbendCount",bendCount);

    }

    
}
