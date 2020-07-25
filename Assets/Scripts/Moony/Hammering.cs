using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammering : MonoBehaviour
{
    private bool HammerKey = false;
    public float HammeringTime = 1.0f;
    private float CoTime;
    // Start is called before the first frame update
    void Start()
    {
        CoTime = HammeringTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HammerKey = true;
            CoTime = HammeringTime;
            StartCoroutine("Hammer");
        }
        
    }
    IEnumerator Hammer()
    {
        Collider[] colliderArray = Physics.OverlapBox(transform.position + new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 0f), transform.rotation);
        while (CoTime > 0)
        {
            

            for (int i = 0; i < colliderArray.Length; i++)
            {
                if (colliderArray[i].tag == "Spring")
                {

                    break;
                }
            }
            CoTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        
    }
}
