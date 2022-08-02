using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class OpenCurtain : MonoBehaviour
{

    [Header("Refrences")]
    [SerializeField] PlayerController _playerController;
    [Tooltip("Add every activiton trigger before the curtain to be precise. Example: Curtain_02 needs to recieve Activion Trigger 1+2")]
    [SerializeField] List<ActivateEnemies> _enemyActivationTriggersList;
    [SerializeField] Light _pathLight;

    [Header("Cameras")]
    [SerializeField] CinemachineBrain _cameraBrain;
    [SerializeField] GameObject _playerCamera;
    [SerializeField] GameObject _curtainCamera;

    [Header("Camera Transitions Speed")]
    [SerializeField] float _firstSwitchTransitionSpeed;
    [SerializeField] float _secondSwitchTransitionSpeed;

    [Header("Camera Waiting Durtaions Before Switching")]
    [SerializeField] float _waitAfterFirstSwitchStarted;
    [SerializeField] float _focusOnCurtainWaitDuraion;
    [SerializeField] float _waitAfterSecondSwitchStarted;

    [Header("Status")]
    [SerializeField][ReadOnlyInspector] int _bunnyCountToOpen;
    [SerializeField] bool _isOpen = false;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Unit.OnBunnyKilled += BunnyCountCheck;
    }

    private void Start()
    {
        foreach (var enemyActivisionTrigger in _enemyActivationTriggersList)
        {
            _bunnyCountToOpen += enemyActivisionTrigger.EnemiesToActivate.Count;
        }

        if (_pathLight != null) _pathLight.enabled = false;
    }

    private void BunnyCountCheck()
    {
        if (!_isOpen && PlayerData.Instance.bunnyCount >= _bunnyCountToOpen)
        {
            _isOpen = true;
            StartCoroutine(Open());
            Unit.OnBunnyKilled -= BunnyCountCheck;
        }
    }

    IEnumerator Open()
    {
        GameManager.Instance.IsPlayerActive(false, false);
        FirstCameraSwitch();

        yield return new WaitForSeconds(_waitAfterFirstSwitchStarted);
        if (_pathLight != null) _pathLight.enabled = true;
        _animator.SetTrigger("Open");
        FMODUnity.RuntimeManager.PlayOneShot("event:/Opening Curtains-Sound Effect");

        yield return new WaitForSeconds(_focusOnCurtainWaitDuraion);
        SwitchCameraBack();

        yield return new WaitForSeconds(_waitAfterSecondSwitchStarted);
        GameManager.Instance.IsPlayerActive(true);

        enabled = false;
    }

    void FirstCameraSwitch()
    {
        _cameraBrain.m_DefaultBlend.m_Time = _firstSwitchTransitionSpeed;
        _cameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseIn;

        _curtainCamera.SetActive(true);
        _playerCamera.SetActive(false);
    }

    void SwitchCameraBack()
    {
        _cameraBrain.m_DefaultBlend.m_Time = _secondSwitchTransitionSpeed;
        _cameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;

        _playerCamera.SetActive(true);
        _curtainCamera.SetActive(false);
    }

}
