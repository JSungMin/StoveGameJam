using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(Collider),typeof(Animator))]
public class SpringObject : MonoBehaviour
{
    public Collider col;
    protected Animator animator;
    public int bendLimit = 1, bendCount = 0;
    public bool isBending = false;
    public Vector3 springDir;
    public float springPower = 15f;
    private Player playerOb;



    protected void Start()
    {
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        
    }
    public void Bending(Player player)
    {
        playerOb = player;
        isBending = true;
        bendCount = 1;
        animator.SetInteger("BendCount", bendCount);
        col.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.transform.CompareTag("Player") && isBending)
        {
            Debug.Log("Enter!!!!!");
            isBending = false;
            bendCount = 0;
            springDir = new Vector3(springPower, springPower * 0.5f, 0);
            playerOb.MoveController.SpringJump(springDir);
            animator.SetTrigger("IsOverBending");
            col.isTrigger = false;

        }
        animator.SetInteger("BendCount", bendCount);
    }
   
    //public void OnHammering(Player player)
    //{
    //    if(bendCount < bendLimit)
    //    {
    //        bendCount++;
    //        isBending = true;
    //
    //    }
    //    else
    //    {
    //        
    //        animator.SetTrigger("IsOverBending");
    //        isBending = false;
    //        bendCount = 0;
    //        //  TODO : AddForce To Player
    //        player.MoveController.SpringJump(springDir);
    //        //  moveCon.AddForce(sprintDir*sprintPower);
    //    }
    //    animator.SetInteger("BendCount", bendCount);
    //    //parentAnimator.SetInteger("parentbendCount",bendCount);
    //
    //}

    
}
