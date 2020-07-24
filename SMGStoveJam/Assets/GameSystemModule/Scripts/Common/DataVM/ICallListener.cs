using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICallListener
{
    BehaviourJob OnCall(string methodName, List<TaggedData> parameters);
}
