using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public void PressToStart()
    {
        SceneManager.LoadScene(1);
    }
   
}
