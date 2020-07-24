using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabable
{
    void OnGrabStart(GameObject grabber);
    void OnGrab(GameObject grabber);
    void OnDrop(GameObject grabbger);
}
