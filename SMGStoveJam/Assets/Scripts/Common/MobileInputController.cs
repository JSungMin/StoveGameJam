using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileInputController : SingletonGameObject<MobileInputController> 
{
    public Vector3[] beganPos = new Vector3[3];
    public Vector3[] curTouchPos = new Vector3[3];
    public Vector3[] lastPos = new Vector3[3];

    public List<Touch> curTouches = new List<Touch>();

    public Action<int, Touch> onTouchBegan;
    public Action<int, Touch> onTouchStationary;
    public Action<int, Touch> onTouchMoved;
    public Action<int, Touch> onTouchEnded;

	// Update is called once per frame
	void Update ()
    {
        ProcessTouchInput();
	}
    
    public void ProcessTouchInput()
    {
        curTouches.Clear();
        for (var i = 0; i < Input.touchCount && i < 3; i++)
        {
            var isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject(i);
            if (isPointerOverGameObject)
                continue;
            var touch = Input.GetTouch(i);
            curTouches.Add(touch);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    curTouchPos[i] = touch.position;
                    beganPos[i] = curTouchPos[i];
                    onTouchBegan?.Invoke(i, touch);
                    break;
                case TouchPhase.Stationary:
                    onTouchStationary?.Invoke(i, touch);
                    break;
                case TouchPhase.Moved:
                    curTouchPos[i] = touch.position;
                    onTouchMoved?.Invoke(i, touch);
                    break;
                case TouchPhase.Ended:
                    curTouchPos[i] = Vector3.zero;
                    beganPos[i] = Vector3.zero;
                    lastPos[i] = touch.position;
                    onTouchEnded?.Invoke(i, touch);
                    break;
                case TouchPhase.Canceled:
                    curTouchPos[i] = Vector3.zero;
                    beganPos[i] = Vector3.zero;
                    lastPos[i] = touch.position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
