using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum MenuFunc { Main, Settings, Credits, Save, Load}
public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] Menu[] menus;

    private void Awake()
    {
        if (instance && instance != this)
            Destroy(gameObject);

        instance = this;
    }

    public void OpenMenu(MenuFunc func)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].myFunction == func)
                menus[i].Open();
            else if (menus[i].isActiveAndEnabled)
                menus[i].Close();
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
            if (menus[i].isActiveAndEnabled)
                menus[i].Close();

        menu.Open();
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

}