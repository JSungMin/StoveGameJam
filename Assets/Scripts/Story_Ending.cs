using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story_Ending : MonoBehaviour
{

    void Start()
    {
        // DefaultUI.instance.OpenScene("Credit");
        SceneManager.LoadScene("Credit");
    }

}
