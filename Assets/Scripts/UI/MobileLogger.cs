using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileLogger : MonoBehaviour
{
    public RectTransform rectTransform;
    public Text logText;
    public Text controlButtonText;
  
    public bool isLogActive = true;

    private BehaviourJob openJob, closeJob;
    // Use this for initialization
    void Awake()
    {
        Application.logMessageReceivedThreaded += HandleReceivedLog;
    }

    public void ClearLog()
    {
        logText.text = "";
    }

    public void OnClickControlButton()
    {
        if (isLogActive)
            CloseLogger();
        else
            OpenLogger();
    }
    public void OpenLogger()
    {
        isLogActive = true;
        controlButtonText.text = "CLOSE";
        closeJob?.Kill();
        openJob = BehaviourJob.Make(IOpenLogger(0.3f));
    }
    public void CloseLogger()
    {
        isLogActive = false;
        controlButtonText.text = "OPEN";
        openJob?.Kill();
        closeJob = BehaviourJob.Make(ICloseLogger(0.3f));
    }
    private void HandleReceivedLog(string condition, string stackTrace, LogType type)
    {
        var color = "green";
        if (type == LogType.Error)
            color = "maroon";
        logText.text += "<color="+color+">Condition : " + condition + "</color>\n";
        if (stackTrace != "")
            logText.text += "Trace : " + stackTrace + "\n";
        if (logText.text.Length < 3000) return;
        logText.text = logText.text.Substring(logText.text.Length - 3000, 3000);
        var idx = logText.text.IndexOf("\n");
        logText.text = logText.text.Substring(idx, logText.text.Length - idx);
    }

    private IEnumerator IOpenLogger(float duration)
    {
        var timer = 0f;
        while (timer <= duration)
        {
            rectTransform.anchoredPosition = Vector2.down * Mathf.Lerp(400, 0, timer/duration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private IEnumerator ICloseLogger(float duration)
    {
        var timer = 0f;
        while (timer <= duration)
        {
            rectTransform.anchoredPosition = Vector2.down * Mathf.Lerp(0, 400, timer / duration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = Vector2.down * 400f;
    }
}
