using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Animator))]
public class CanCutObject : MonoBehaviour
{
    public GameObject refHidedObj;
    public GameObject[] refDebriObjs;
    public Animator animator;
    public Collider trigger;
    public bool isCut = false;
    public float explosionPower;
    public string cutAnim;

    // Start is called before the first frame update
    private void Start() 
    {
        animator = GetComponent<Animator>();
        trigger = GetComponent<Collider>();
        trigger.isTrigger = true;
        refHidedObj.SetActive(false);    
        for (var i = 0; i < refDebriObjs.Length; i++)
        {
            refDebriObjs[i].SetActive(false);
        }
    }
    public void OnCut(Player player)
    {
        if(isCut) return;
        animator.Play(cutAnim);
        refHidedObj.SetActive(true);
        for(var i = 0; i < refDebriObjs.Length; i++)
        {
            var target = refDebriObjs[i];
            target.SetActive(true);
            var tRigid = target.GetComponent<Rigidbody>();
            if(null == tRigid) continue;
            var dir = (target.transform.position - player.gameObject.transform.position).normalized;
            tRigid.AddForce(explosionPower * dir, ForceMode.Impulse);
        }
    }
    
}
