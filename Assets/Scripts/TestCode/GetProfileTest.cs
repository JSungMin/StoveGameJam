using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreSystem;

public class GetProfileTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //  Get ActorProfile 
        var playerProfile = ActorProfileSet.GetElement("Player");
        //  Reference Stat
        var playerStr = playerProfile.basicStat.elements[0];
        var playerSpeed = playerProfile.basicStat.elements[1];
        //  How to Find Item In ActorProfile
        playerProfile.inventory.Find(x => {
            return x.Profile.itemName == "Test";
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
