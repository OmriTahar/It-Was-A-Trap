using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class OpenCurtain : MonoBehaviour
{

    [Header("Refrences")]
    [SerializeField] ActivateEnemies _enemyActivationInStage;
    [SerializeField][ReadOnlyInspector] int _bunniesInStage;

    [Header("Cameras")]
    [SerializeField] CinemachineBrain _cameraBrain;
    [SerializeField] GameObject _playerCamera;
    [SerializeField] GameObject _curtainCamera;

    [Header("Transition Durtaions")]
    [SerializeField] float _firstSwitchTransitionSpeed;
    [SerializeField] float _secondSwitchTransitionSpeed;

    [Header("Waiting Durtaions")]
    [SerializeField] int _firstSwitchWaitDuration;
    [SerializeField] int _focusOnCurtainWaitDuraion;
    [SerializeField] int _switchBackWaitDurtaion;

    private Animator _animator;
    private bool _open = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _bunniesInStage = _enemyActivationInStage.EnemiesToActivate.Count;
    }

    void Update()
    {
        if (PlayerData.Instance.bunnyCount >= _bunniesInStage && !_open)
        {
            _open = true;
            StartCoroutine(Open());
        }
    }

    IEnumerator Open()
    {
        FirstSwitch();
        yield return new WaitForSeconds(_firstSwitchWaitDuration);

        _animator.SetTrigger("Open");

        yield return new WaitForSeconds(_focusOnCurtainWaitDuraion);

        SwitchBack();

        yield return new WaitForSeconds(_switchBackWaitDurtaion);
        print("Finished Sequence.");
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
