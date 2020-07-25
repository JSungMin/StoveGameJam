using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultUI : MonoBehaviour
{
    public static DefaultUI instance;
    public bool active = false;
    public GameObject child_background;

    private void Awake()
    {
        instance = this;
        if (child_background != null && child_background.activeSelf != active)
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
        GameStop(active);
        child_background.SetActive(active);
    }

    public void StopUINone()
    {
        Debug.Log("으아아");
        StopUIActive(false);
    }

    public void OpenScene(string _sceneName)
    {
        OpenScene(SceneManager.GetSceneByName(_sceneName).buildIndex);
    }

    public void OpenScene(int _sceneIdx)
    {
        SceneManager.LoadScene(_sceneIdx);
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
        OpenScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameGoToTitle()
    {
        OpenScene(0);
    }

    public void GameEnd()
    {
        Application.Quit();
        Debug.Log("게임 종료 실행됨");
    }

}
