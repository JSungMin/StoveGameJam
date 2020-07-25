using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StaticTrap : BaseTrap
{
    protected override void OnTrapIn(Collider other)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnTrapOut(Collider other)
    {
        throw new System.NotImplementedException();
    }
}
