using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [Header("Refrences")]
    [SerializeField] GameObject _playerHUD;
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] PlayerController _playerController;

    [Header("Settings")]
    [SerializeField] bool _playThemeMusic = true;

    [Header("Intro Video")]
    [SerializeField] bool _playIntroCutscene;
    [SerializeField] RawImage _videoRawImage;
    [SerializeField] VideoPlayer _introVideoPlayer;
    private bool _isCutsceneFinished = false;

    [Header("Temporary Bandages")]
    [Tooltip("If the player gets deActivated while hit effect is on - this stops the effect")]
    [SerializeField] Animator _playerHitEffectAnimator;

    private bool _isGamePaused = false;

    // ---------- FMOD ----------
    FMOD.Studio.Bus _masterBus;

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

        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {

        _masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");

        //if (_playThemeMusic)
        //    FMODUnity.RuntimeManager.PlayOneShot("event:/Music");

        // --------- Intro Cutscene ------

        _introVideoPlayer.Stop();
        _videoRawImage.gameObject.SetActive(_playIntroCutscene);

        if (_playIntroCutscene)
        {
            TogglePauseMenu(true);
            IsTimeScaleStopped(false);
            Cursor.visible = false;

            _introVideoPlayer.Play();
            Invoke("CloseCutscene", 26f);
        }
    }

    private void CloseCutscene()
    {
        print("Finished Cutscene!");
        _isCutsceneFinished = true;

        _introVideoPlayer.Stop();
        _videoRawImage.gameObject.SetActive(false);
        TogglePauseMenu(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_playIntroCutscene && !_isCutsceneFinished) // Quit Cutscene
            {
                CloseCutscene();
            }
            else if (_playIntroCutscene && _isCutsceneFinished) // Cutscene is finished
            {
                TogglePauseMenu(!_isGamePaused);
            }
            else if (!_playIntroCutscene) // Start without cutscene
            {
                TogglePauseMenu(!_isGamePaused);
            }
        }
    }

    #region Pause Panel

    public void TogglePauseMenu(bool isGamePauesd)
    {
        _isGamePaused = isGamePauesd;
        Cursor.visible = isGamePauesd;
        FMODUnity.RuntimeManager.PauseAllEvents(isGamePauesd);

        _pauseMenu.SetActive(isGamePauesd);
        _playerHUD.SetActive(!isGamePauesd);

        IsTimeScaleStopped(isGamePauesd);
        IsPlayerActive(!isGamePauesd, !isGamePauesd);
    }

    public void GoToMainMenu()
    {
        _masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

        if (!isPlayerActive && _playerHitEffectAnimator != null)
            _playerHitEffectAnimator.SetTrigger("Stop");

        _playerHUD.SetActive(enablePlayerHUD);
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
