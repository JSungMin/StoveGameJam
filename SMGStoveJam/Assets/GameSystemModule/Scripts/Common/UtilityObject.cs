using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DynamicGauge
{
    //  cur : current, def : default
    [SerializeField]
    private float defGauge;
    [SerializeField]
    private float curGauge;
    public Action<float> onCurrentGaugeChanged;
    public Action<float> onDefaultGaugeChanged;
    public DynamicGauge()
    {
        defGauge = 0f;
        curGauge = 0f;
    }
    public DynamicGauge(float def, float cur)
    {
        defGauge = def;
        curGauge = cur;
    }
    public DynamicGauge(DynamicGauge origin)
    {
        defGauge = origin.defGauge;
        curGauge = origin.curGauge;
        onDefaultGaugeChanged = origin.onDefaultGaugeChanged;
        onCurrentGaugeChanged = origin.onCurrentGaugeChanged;
    }
    public static DynamicGauge operator -(DynamicGauge a, float b)
    {
        a.CurrentGauge -= b;
        return a;
    }
    public static DynamicGauge operator +(DynamicGauge a, float b)
    {
        a.CurrentGauge += b;
        return a;
    }
    public float CurrentGauge
    {
        get
        {
            return curGauge;
        }
        set
        {
            onCurrentGaugeChanged?.Invoke(value);
            curGauge = value;
        }
    }
    public float DefaultGauge
    {
        get
        {
            return defGauge;
        }
        set
        {
            onDefaultGaugeChanged?.Invoke(value);
            defGauge = value;
        }
    }
    public float GetPercentage()
    {
        return curGauge / defGauge;
    }
}
[System.Serializable]
public class Timer
{
    public object owner;
    public string timerName;
    public TimerState timerState = TimerState.FINISH;
    public float duration = 0f;
    [SerializeField]
    private float timer = 0f;
   
    //  Start Func -> Start From Stop | Start From Pause | Start From Running
    private readonly Action[] startFunc = new Action[EnumHelper.CountOfElement<TimerState>()];
    public Timer()
    {
        startFunc[0] = StartFromStop;
        startFunc[1] = StartFromPause;
        startFunc[2] = StartFromRunning;
    }
    public Timer (object owner, string name = "timer", float duration = 0f)
    {
        this.owner = owner;
        this.timerName = name;
        this.duration = duration;
        startFunc[0] = StartFromStop;
        startFunc[1] = StartFromPause;
        startFunc[2] = StartFromRunning;
    }
    #region inner logic functions
    //  if you want, then add some call back func
    private void StartFromStop()
    {
        //  TODO : START
        timerState = TimerState.RUNNING;
        //  Already timer = 0
    }
    private void StartFromPause()

    {
        //  TODO : RESUME
        timerState = TimerState.RUNNING;
        //  Sustain timer value
    }
    private void StartFromRunning()
    {
        //  TODO : RESTART
        //  Already Running State
        timer = 0f;
    }
    
    private void UpdateInRunning<T>(float delta, Action<T> OnUpdate, Action<T> OnStopped)
    {
        if (timer >= duration)
        {
            OnStopped?.Invoke((T)owner);
            return;
        }
        else
        {
            OnUpdate?.Invoke((T)owner);
            timer += delta;
        }
    }
    #endregion
    public void StopTimer()
    {
        if (timerState == TimerState.FINISH)
            return;
        timer = 0f;
        timerState = TimerState.FINISH;
    }
    public void PauseTimer()
    {
        if (timerState == TimerState.PAUSE)
            return;
        timerState = TimerState.PAUSE;
    }
    public void StartTimer()
    {
        if (timerState == TimerState.RUNNING)
            return;
        startFunc[(int)timerState].Invoke();
    }

    public void ResetTimer(TimerState state)
    {
        timer = 0f;
        timerState = state;
    }

    public bool IsOver()
    {
        return timer >= duration;
    }
    public void UpdateTimer<T>(float delta, Action<T> OnUpdate = null, Action<T> onFinish = null, Action OnPaused = null)
    {
        if(timerState == TimerState.RUNNING)
            UpdateInRunning(delta, OnUpdate, onFinish);
    }
}
