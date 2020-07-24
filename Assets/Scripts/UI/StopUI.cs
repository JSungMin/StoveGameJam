using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StopUI : MonoBehaviour
{
    public static StopUI instance;
    public bool active = false;
    public GameObject child_background;

    private void Awake()
    {
        instance = this;
        if (child_background.activeSelf != active)
            child_background.SetActive(active);
    }

    private void Update()
    {
        bool esc = Input.GetKeyDown(KeyCode.Escape);

        if (esc)
            StopUIActive(!active);

    }

    public void StopUIActive(bool _true = true)
    {
        active = _true;
        if (_true)
        {
            GameStop();
            child_background.SetActive(true);
        }
        else if (!_true){
            GameStop(false);
            child_background.SetActive(false);
        }
    }
    
    public void GameStop(bool _stop = false) 
    {
        if(!_stop)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameGoToTitle()
    {
        SceneManager.LoadScene(0);
    }

    public void GameEnd()
    {
        Application.Quit();
        Debug.Log("게임 종료 실행됨");
    }

}
