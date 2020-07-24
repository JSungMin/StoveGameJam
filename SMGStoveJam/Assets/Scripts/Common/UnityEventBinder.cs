using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventBinder : MonoBehaviour
{
    [System.Serializable]
    public class EventData
    {
        public string name;
        public UnityEvent bindEvent;
    }
    public List<EventData> bindEvents = new List<EventData>();
    private Dictionary<string, EventData> bindMap = new Dictionary<string, EventData>();

    void Awake()
    {
        for (var i = 0; i < bindEvents.Count;i++)
        {
            bindMap[bindEvents[i].name] = bindEvents[i];
        }
    }

    public void InvokeEvent(int index)
    {
        bindEvents[index]?.bindEvent.Invoke();
    }

    public void InvokeEvent(string n)
    {
        bindMap[n]?.bindEvent.Invoke();
    }
    public void InvokeAllEvent()
    {
        foreach (var e in bindEvents)
        {
            e?.bindEvent.Invoke();
        }
    }

}
