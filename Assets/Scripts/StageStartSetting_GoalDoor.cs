using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStartSetting_GoalDoor : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ClearUI.instance.ClearUIOpen();
            StageStartSetting.instance.NextStage();
        }
    }
}
