using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [Header("Refrences")]
    [SerializeField] GameObject _playerHUD;
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] PlayerController _playerController;

    [Header("Settings")]
    [SerializeField] bool _playThemeMusic = true;

    [Header("Temporary Bandages")]
    [Tooltip("If the player gets deActivated while hit effect is on - this stops the effect")]
    [SerializeField] Animator _playerHitEffectAnimator;

    private bool _isGamePaused = false;

    private void Awake()
    {
        #region Singleton

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        #endregion
    }

    private void Start()
    {
        TogglePauseMenu(false);

        if (_playThemeMusic)
            FMODUnity.RuntimeManager.PlayOneShot("event:/Music");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu(!_isGamePaused);
        }
    }

    #region Pause Panel

    public void TogglePauseMenu(bool isGamePauesd)
    {
        _isGamePaused = isGamePauesd;
        Cursor.visible = isGamePauesd;

        _pauseMenu.SetActive(isGamePauesd);
        _playerHUD.SetActive(!isGamePauesd);

        IsTimeScaleStopped(isGamePauesd);
        IsPlayerActive(!isGamePauesd, !isGamePauesd);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    #endregion

    public void IsPlayerActive(bool isPlayerActive, bool enablePlayerHUD = true)
    {
        _playerController.CanPlayerMoveAndRotate(isPlayerActive);
        PlayerAim.Instance._canAim = isPlayerActive;
        PlayerData.Instance._isAllowedToShoot = isPlayerActive;

        _playerHUD.SetActive(enablePlayerHUD);

        if (!isPlayerActive && _playerHitEffectAnimator != null)
            _playerHitEffectAnimator.SetTrigger("Stop");
    }

    private void IsTimeScaleStopped(bool isStopped)
    {
        if (isStopped)
        {
            Time.timeScale = 0;
        }
        else
            Time.timeScale = 1;
    }
}
