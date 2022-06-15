using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables

    #region General
    [Header("General Refrences")]
    [SerializeField] internal Rigidbody _rb;
    [SerializeField] internal Transform _meshTransform;
    #endregion

    #region Camera
    [Header("Camera")]
    public Camera PlayerMovementCamera;
    public bool IsCursorVisable = true;
    #endregion

    #region Movement
    [Header("Movement")]
    public bool PlayerCanMove = true;
    public float WalkSpeed = 5f;

    private float _maxVelocityChange = 10f;
    //private bool _isWalking = false;
    #endregion

    #region Dash
    [Header("Dash Settings")]
    public bool EnableDash = true;
    public KeyCode DashKey = KeyCode.LeftShift;

    // Dash Settings
    public float DashSpeed = 60f;
    public float DashDuration = 0.15f;
    public float DashCooldownTotalTime = 3f;

    private float _dashCooldownRemainingTime;
    private bool _canDash = true;
    private bool _isDashing = false;
    private bool _isDashCooldown = false;

    [Header("Dash Effect")]
    [SerializeField] bool _EnableDashFlashEffect;
    [SerializeField] FlashImage _dashFlashImage;
    [SerializeField] Color _dashFlashColor;
    [Tooltip("How strong the alpha in the dash flash")]
    [SerializeField][Range(0, 1)] float _dashFlashAlpha;

    [Header("Dash UI")]
    public Image DashBarBG;
    public Image DashBarFill;

    private Color32 _dashBarColorFull;
    private Color32 _dashBarColorCharge;
    #endregion

    #region Animation
    [Header("Animation")]
    Animator _animator;
    #endregion

    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _dashCooldownRemainingTime = DashCooldownTotalTime;
    }

    void Start()
    {
        Cursor.visible = IsCursorVisable;
        DashUISetup();
    }

    private void Update()
    {
        CameraInput();
        HandleDashUI();
    }

    void FixedUpdate()
    {
        HandleMovementAndDash();
    }

    private void HandleDashUI()
    {
        if (EnableDash)
        {
            if (!_canDash)
            {
                DashBarFill.fillAmount = 0;
                DashBarFill.color = _dashBarColorCharge;
                _isDashCooldown = true;
            }

            if (_isDashCooldown)
            {
                _dashCooldownRemainingTime -= Time.deltaTime;

                var dashCooldownPercentage = _dashCooldownRemainingTime / DashCooldownTotalTime;
                DashBarFill.fillAmount = 1 - dashCooldownPercentage;

                if (_dashCooldownRemainingTime <= 0)
                {
                    _dashCooldownRemainingTime = DashCooldownTotalTime;
                    DashBarFill.fillAmount = 1;
                    DashBarFill.color = _dashBarColorFull;

                    _canDash = true;
                    _isDashCooldown = false;
                }
            }
        }
    }

    private void HandleMovementAndDash()
    {
        if (PlayerCanMove)
        {
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            #region Animator

            _animator.SetFloat("Velocity", targetVelocity.magnitude);

            //if (targetVelocity.z > 0)
            //{
            //    _animator.SetBool("IsPressingForward", true);
            //}
            //else if (targetVelocity.z < 0)
            //{
            //    _animator.SetBool("IsPressingForward", false);
            //}

            #endregion


            // Checks if player is walking
            //if (targetVelocity.x != 0 || targetVelocity.z != 0)
            //    _isWalking = true;
            //else
            //    _isWalking = false;


            if (EnableDash && Input.GetKeyDown(DashKey) && _canDash) // Dash Logic
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * DashSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = _rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);
                velocityChange.y = 0;

                // Dash while moving
                if (_rb.velocity.magnitude > 0)
                {
                    StartCoroutine(Dash(velocityChange));
                    _isDashing = true;
                    _canDash = false;
                }
                else // If dashing without moving -> dash to a random location
                {
                    StartCoroutine(Dash(new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f))));
                    _isDashing = true;
                    _canDash = false;
                }
            }
            else if (!_isDashing) // Walk Logic
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * WalkSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = _rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);
                velocityChange.y = 0;

                _rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }
    }

    private void DashUISetup()
    {
        DashBarBG.gameObject.SetActive(true);
        DashBarFill.gameObject.SetActive(true);

        _dashBarColorCharge = new Color32(255, 255, 0, 30);
        _dashBarColorFull = new Color32(255, 255, 0, 255);
    }

    IEnumerator Dash(Vector3 dashVecolity)
    {
        if (_EnableDashFlashEffect)
            _dashFlashImage.StartFlash(DashDuration, _dashFlashAlpha, _dashFlashColor);

        dashVecolity = dashVecolity.normalized;
        _rb.AddForce(dashVecolity * DashSpeed, ForceMode.Impulse);

        yield return new WaitForSeconds(DashDuration);
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