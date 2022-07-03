using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    #region Variables

    #region General
    [Header("General Refrences")]
    [SerializeField] internal Transform _meshTransform;
    private Animator _animator;
    private Rigidbody _rb;
    #endregion

    #region Camera
    [Header("Camera")]
    public Camera PlayerMovementCamera;
    public bool IsCursorVisable = true;
    #endregion

    #region Movement
    [Header("Movement")]
    public bool IsAllowedToMove = true;
    public float WalkSpeed = 5f;
    #endregion

    #region Dash
    [Header("Dash Settings")]
    public KeyCode DashKey = KeyCode.LeftShift;
    public float DashSpeed = 60f;
    public float DashDuration = 0.15f;
    public float DashCooldownTotalTime = 3f;

    private float _dashCooldownRemainingTime;
    private bool _canDash = true;
    private bool _isDashing = false;
    private WaitForSeconds _dashDurationCoroutine;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] private GameObject _activeUpgradesWindow;
    [SerializeField] private Image DashBarFill;
    [SerializeField] private Image DashBarBG;
    [SerializeField] private Color _dashBarColorFull = new Color(1, 1, 0, 1);
    [SerializeField] private Color _dashBarColorCharge = new Color(1, 1, 0, 0.3f);
    #endregion

    #endregion

    private void Awake()
    {
        Cursor.visible = IsCursorVisable;

        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();

        DashBarBG.gameObject.SetActive(true);
        DashBarFill.gameObject.SetActive(true);

        _dashCooldownRemainingTime = DashCooldownTotalTime;
        _dashDurationCoroutine = new WaitForSeconds(DashDuration);
    }

    private void Update()
    {
        CameraInput();
        HandleDashUI();

        if (Input.GetKeyDown(KeyCode.Tab))
            if (_activeUpgradesWindow)
                _activeUpgradesWindow.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Tab))
            if (_activeUpgradesWindow)
                _activeUpgradesWindow.SetActive(false);

    }

    void FixedUpdate()
    {
        HandleMovementAndDash();
    }

    private void HandleDashUI()
    {
        if (!_canDash)
        {
            DashBarFill.fillAmount = 0;
            DashBarFill.color = _dashBarColorCharge;

            _dashCooldownRemainingTime -= Time.deltaTime;
            var dashCooldownPercentage = _dashCooldownRemainingTime / DashCooldownTotalTime;
            DashBarFill.fillAmount = 1 - dashCooldownPercentage;

            if (_dashCooldownRemainingTime <= 0)
            {
                _dashCooldownRemainingTime = DashCooldownTotalTime;
                DashBarFill.fillAmount = 1;
                DashBarFill.color = _dashBarColorFull;

                _canDash = true;
            }
        }
    }

    private void HandleMovementAndDash()
    {
        if (IsAllowedToMove)
        {
            if (!PlayerData.Instance.IsStunned && PlayerData.Instance._stunEffect.isPlaying)
            {
                _animator.SetBool("IsStunned", false);
                PlayerData.Instance._stunEffect.Stop();
            }

            Vector3 playerVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            _animator.SetFloat("Velocity", playerVelocity.magnitude);

            if (Input.GetKeyDown(DashKey) && _canDash) // Dash Logic
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Magician Dash");
                if (_rb.velocity.magnitude > 0) // Dash while moving
                {
                    StartCoroutine(Dash(playerVelocity));

                }
                else // If dashing without moving -> dash to a random location
                {
                    StartCoroutine(Dash(new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f))));
                }
            }

            else if (!_isDashing) // Moving (not dashing) Logic
            {
                playerVelocity = transform.TransformDirection(playerVelocity) * WalkSpeed;
                Vector3 velocity = _rb.velocity;
                Vector3 velocityChange = (playerVelocity - velocity);
                _rb.AddForce(velocityChange, ForceMode.VelocityChange);                
            }
        }
        else
        {
            // Stun effect. Check conditions only if PlayerCanMove = false
            if (PlayerData.Instance.IsStunned && PlayerData.Instance._stunEffect != null && !PlayerData.Instance._stunEffect.isPlaying)
            {
                _animator.SetBool("IsStunned", true);
                _animator.SetTrigger("StartStun");

                _rb.Sleep();
                PlayerData.Instance._stunEffect.Play();
            }
        }
    }

    IEnumerator Dash(Vector3 dashVecolity)
    {
        _canDash = false;
        _isDashing = true;

        dashVecolity = dashVecolity.normalized;
        _rb.AddForce(dashVecolity * DashSpeed, ForceMode.Impulse);

        yield return _dashDurationCoroutine;
        _isDashing = false;
    }

    private void CameraInput()
    {
        RaycastHit hit;
        Ray camRay = PlayerMovementCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(camRay, out hit))
        {
            _meshTransform.LookAt(new Vector3(hit.point.x, _meshTransform.position.y, hit.point.z));
            Debug.DrawLine(camRay.origin, hit.point, Color.yellow);
        }
    }

    #region Jump & Crouch Variables

    #region Jump

    //[Header("Jump")]
    //public bool EnableJump = true;
    //public KeyCode JumpKey = KeyCode.Space;
    //public float JumpPower = 5f;

    //private bool _isGrounded = false;

    #endregion

    #region Crouch

    //[Header("Crouch")]
    //public bool EnableCrouch = true;
    //public bool HoldToCrouch = true;
    //public KeyCode CrouchKey = KeyCode.LeftControl;
    //public float CrouchHeight = .75f;
    //public float CrouchSpeedReduction = .5f;

    //private bool _isCrouched = false;
    //private Vector3 _standingScale;

    #endregion

    #endregion

    #region Jump & Crouch & CheckGround Methods (Not Used)

    //private void HandleJumpAndCrouch()
    //{
    //    #region Jump & Crouch & CheckGround (Not Used)

    //    if (EnableJump && Input.GetKeyDown(JumpKey) && _isGrounded)
    //        Jump();

    //    #region Handle Crouch

    //    if (EnableCrouch)
    //    {
    //        if (Input.GetKeyDown(CrouchKey) && !HoldToCrouch)
    //            Crouch();

    //        if (Input.GetKeyDown(CrouchKey) && HoldToCrouch)
    //        {
    //            _isCrouched = false;
    //            Crouch();
    //        }
    //        else if (Input.GetKeyUp(CrouchKey) && HoldToCrouch)
    //        {
    //            _isCrouched = true;
    //            Crouch();
    //        }
    //    }

    //    #endregion
    //    CheckGround();
    //    #endregion
    //}

    //private void CheckGround() // Sets _isGrounded based on a raycast sent straigth down from the player object
    //{
    //    Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
    //    Vector3 direction = transform.TransformDirection(Vector3.down);
    //    float distance = .75f;

    //    if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
    //    {
    //        Debug.DrawRay(origin, direction * distance, Color.red);
    //        _isGrounded = true;
    //    }
    //    else
    //        _isGrounded = false;
    //}

    //private void Jump()
    //{
    //    if (_isGrounded)
    //    {
    //        _rb.AddForce(0f, JumpPower, 0f, ForceMode.Impulse);
    //        _isGrounded = false;
    //    }

    //    // When crouched and using toggle system, will uncrouch for a jump
    //    if (_isCrouched && !HoldToCrouch)
    //        Crouch();
    //}

    //private void Crouch()
    //{
    //    // Brings walkSpeed back up to original speed
    //    if (_isCrouched)
    //    {
    //        transform.localScale = new Vector3(_standingScale.x, _standingScale.y, _standingScale.z);
    //        WalkSpeed /= CrouchSpeedReduction;

    //        _isCrouched = false;
    //    }
    //    // Crouches player down to set height
    //    // Reduces walkSpeed
    //    else
    //    {
    //        transform.localScale = new Vector3(_standingScale.x, CrouchHeight, _standingScale.z);
    //        WalkSpeed *= CrouchSpeedReduction;

    //        _isCrouched = true;
    //    }
    //}

    #endregion 

}