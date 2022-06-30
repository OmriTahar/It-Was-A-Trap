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
    [SerializeField][ReadOnlyInspector] int _bunnyCountToOpen;

    [Header("Cameras")]
    [SerializeField] CinemachineBrain _cameraBrain;
    [SerializeField] GameObject _playerCamera;
    [SerializeField] GameObject _curtainCamera;

    [Header("Transition Durtaions")]
    [SerializeField] float _firstSwitchTransitionSpeed;
    [SerializeField] float _secondSwitchTransitionSpeed;

    [Header("Waiting Durtaions")]
    [SerializeField] float _firstSwitchWaitDuration;
    [SerializeField] float _focusOnCurtainWaitDuraion;
    [SerializeField] float _switchBackWaitDurtaion;

    private Animator _animator;
    private bool _open = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        foreach (var enemyActivisionTrigger in _enemyActivationTriggersList)
        {
            _bunnyCountToOpen += enemyActivisionTrigger.EnemiesToActivate.Count;
        }
    }

    void Update()
    {
        if (!_open && PlayerData.Instance.bunnyCount >= _bunnyCountToOpen)
        {
            _open = true;
            StartCoroutine(Open());
        }
    }

    IEnumerator Open()
    {
        _playerController.IsAllowedToMove = false;
        FirstSwitch();
        yield return new WaitForSeconds(_firstSwitchWaitDuration);

        _animator.SetTrigger("Open");

        yield return new WaitForSeconds(_focusOnCurtainWaitDuraion);

        SwitchBack();

        yield return new WaitForSeconds(_switchBackWaitDurtaion);
        _playerController.IsAllowedToMove = true;
        enabled = false;
    }

    void FirstSwitch()
    {
        _cameraBrain.m_DefaultBlend.m_Time = _firstSwitchTransitionSpeed;
        _cameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseIn;

        _curtainCamera.SetActive(true);
        _playerCamera.SetActive(false);

    }

    void SwitchBack()
    {
        _cameraBrain.m_DefaultBlend.m_Time = _secondSwitchTransitionSpeed;
        _cameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;

        _playerCamera.SetActive(true);
        _curtainCamera.SetActive(false);
    }

}
