using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public enum MenuFunc { Main, Settings, Credits, Save, Load}
public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] GameObject[] settingCategories;
    [SerializeField] GameObject[] settingUnderlines;
    [SerializeField] Image[] iconArray;
    [SerializeField] Menu[] menus;

    Color full = new Color(1, 1, 1, 1);
    Color transparent = new Color(1, 1, 1, 0);

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

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/20_s music game mix Am");
        SceneManager.LoadScene(1);
    }

    public void SelectSettingCategory(int settingNum)
    {
        if (!settingCategories[settingNum].activeSelf)
        {
            foreach (GameObject category in settingCategories)
                if (category)
                    category.SetActive(false);

            foreach (GameObject underline in settingUnderlines)
                if (underline)
                    underline.SetActive(false);

            if (settingCategories[settingNum])
                settingCategories[settingNum].SetActive(true);

            if (settingUnderlines[settingNum])
                settingUnderlines[settingNum].SetActive(true);

        }
    }

    public void OnHoverEnter(int buttonNum)
    {
        if (iconArray[buttonNum])
            iconArray[buttonNum].color = full;
    }

    public void OnHoverExit(int buttonNum)
    {
        if (iconArray[buttonNum])
            iconArray[buttonNum].color = transparent;
    }
    public void Playsound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound/UI/UI 3");
    }

}