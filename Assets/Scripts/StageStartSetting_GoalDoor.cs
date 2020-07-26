using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStartSetting_GoalDoor : MonoBehaviour
{
    bool active = false;

    private void OnTriggerStay(Collider other)
    {
        if ( !active && other.gameObject.tag == "Player")
        {
            active = true;
            StartCoroutine(ClearCoroutine());
        }
    }

    IEnumerator ClearCoroutine()
    {
        ClearUI.instance.ClearUIOpen();
        yield return new WaitForSeconds(2);
        ClearUI.instance.ClearUIOpen(false);
        StageStartSetting.instance.NextStage();
    }
}
