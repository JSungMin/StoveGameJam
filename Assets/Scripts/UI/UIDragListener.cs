using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragListener : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<PointerEventData> onBeginDrag, onDrag, onEndDrag;
	
    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(eventData);
    }
}
