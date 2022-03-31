// CHANGE LOG
// 
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using System.Net;
#endif

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody _rb;

    #region Camera Movement Variables

    public Camera PlayerCamera;

    public float FOV = 60f;
    public bool InvertCamera = false;
    public bool CameraCanMove = true;
    public float MouseSensitivity = 2f;
    public float MaxLookAngle = 50f;

    // Crosshair
    public bool IsCursorLocked = true;
    public bool CrosshairEnabled = true;
    public Sprite CrosshairImage;
    public Color CrosshairColor = Color.white;

    // Internal Variables
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
    private Image _crosshairObject;

    #region Camera Zoom Variables

    public bool EnableZoom = true;
    public bool HoldToZoom = false;
    public KeyCode ZoomKey = KeyCode.Mouse1;
    public float ZoomFOV = 30f;
    public float ZoomStepTime = 5f;

    // Internal Variables
    private bool _isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public bool PlayerCanMove = true;
    public float WalkSpeed = 5f;
    public float MaxVelocityChange = 10f;

    // Internal Variables
    private bool _isWalking = false;

    #region Sprint

    public bool EnableSprint = true;
    public bool UnlimitedSprint = false;
    public KeyCode SprintKey = KeyCode.LeftShift;
    public float SprintSpeed = 7f;
    public float SprintDuration = 5f;
    public float SprintCooldown = .5f;
    public float SprintFOV = 80f;
    public float SprintFOVStepTime = 10f;

    // Sprint Bar
    public bool UseSprintBar = true;
    public bool HideBarWhenFull = true;
    public Image SprintBarBG;
    public Image SprintBarFill;
    public float SprintBarWidthPercent = .3f;
    public float SprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup _sprintBarCanvasGroup;
    private bool _isSprinting = false;
    private float _sprintRemaining;
    private float _sprintBarWidth;
    private float _sprintBarHeight;
    private bool _isSprintCooldown = false;
    private float _sprintCooldownReset;

    #endregion

    #region Jump

    public bool EnableJump = true;
    public KeyCode JumpKey = KeyCode.Space;
    public float JumpPower = 5f;

    // Internal Variables
    private bool _isGrounded = false;

    #endregion

    #region Crouch

    public bool EnableCrouch = true;
    public bool HoldToCrouch = true;
    public KeyCode CrouchKey = KeyCode.LeftControl;
    public float CrouchHeight = .75f;
    public float CrouchSpeedReduction = .5f;

    // Internal Variables
    private bool _isCrouched = false;
    private Vector3 _standingScale;

    #endregion
    #endregion

    #region Head Bob

    public bool EnableHeadBob = true;
    public Transform Joint;
    public float BobSpeed = 10f;
    public Vector3 BobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 _jointOriginalPos;
    private float _headBobTimer = 0;

    #endregion

    #region NewDash

    private bool _isDashing;
    public float _dashSpeed;
    public float _dashRecharge;              // Currenty Unused
    [SerializeField] GameObject _dashEffect; // Currenty Unused

    #endregion

    // New Mouse Movement
    Vector3 _mousePosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _crosshairObject = GetComponentInChildren<Image>();

        // Set internal variables
        PlayerCamera.fieldOfView = FOV;
        _standingScale = transform.localScale;
        _jointOriginalPos = Joint.localPosition;

        if (!UnlimitedSprint)
        {
            _sprintRemaining = SprintDuration;
            _sprintCooldownReset = SprintCooldown;
        }
    }

    void Start()
    {
        if (IsCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (CrosshairEnabled)
        {
            _crosshairObject.sprite = CrosshairImage;
            _crosshairObject.color = CrosshairColor;
        }
        else
        {
            _crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar

        _sprintBarCanvasGroup = GetComponentInChildren<CanvasGroup>();

        if (UseSprintBar)
        {
            SprintBarBG.gameObject.SetActive(true);
            SprintBarFill.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            _sprintBarWidth = screenWidth * SprintBarWidthPercent;
            _sprintBarHeight = screenHeight * SprintBarHeightPercent;

            SprintBarBG.rectTransform.sizeDelta = new Vector3(_sprintBarWidth, _sprintBarHeight, 0f);
            SprintBarFill.rectTransform.sizeDelta = new Vector3(_sprintBarWidth - 2, _sprintBarHeight - 2, 0f);

            if (HideBarWhenFull)
            {
                _sprintBarCanvasGroup.alpha = 0;
            }
        }
        else
        {
            SprintBarBG.gameObject.SetActive(false);
            SprintBarFill.gameObject.SetActive(false);
        }

        #endregion
    }

    float _camRotation; // Not necessary?

    private void Update()
    {
        #region Camera

        // Control camera movement
        if (CameraCanMove)
        {
            _yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * MouseSensitivity;

            if (!InvertCamera)
            {
                _pitch -= MouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                // Inverted Y
                _pitch += MouseSensitivity * Input.GetAxis("Mouse Y");
            }

            // Clamp pitch between lookAngle
            _pitch = Mathf.Clamp(_pitch, -MaxLookAngle, MaxLookAngle);

            transform.localEulerAngles = new Vector3(0, _yaw, 0);
            PlayerCamera.transform.localEulerAngles = new Vector3(_pitch, 0, 0);
        }

        #region Camera Zoom

        if (EnableZoom)
        {
            // Changes isZoomed when key is pressed
            // Behavior for toogle zoom
            if (Input.GetKeyDown(ZoomKey) && !HoldToZoom && !_isSprinting)
            {
                if (!_isZoomed)
                {
                    _isZoomed = true;
                }
                else
                {
                    _isZoomed = false;
                }
            }

            // Changes isZoomed when key is pressed
            // Behavior for hold to zoom
            if (HoldToZoom && !_isSprinting)
            {
                if (Input.GetKeyDown(ZoomKey))
                {
                    _isZoomed = true;
                }
                else if (Input.GetKeyUp(ZoomKey))
                {
                    _isZoomed = false;
                }
            }

            // Lerps camera.fieldOfView to allow for a smooth transistion
            if (_isZoomed)
            {
                PlayerCamera.fieldOfView = Mathf.Lerp(PlayerCamera.fieldOfView, ZoomFOV, ZoomStepTime * Time.deltaTime);
            }
            else if (!_isZoomed && !_isSprinting)
            {
                PlayerCamera.fieldOfView = Mathf.Lerp(PlayerCamera.fieldOfView, FOV, ZoomStepTime * Time.deltaTime);
            }
        }

        #endregion
        #endregion

        #region Sprint

        if (EnableSprint)
        {
            if (_isSprinting)
            {
                _isZoomed = false;
                PlayerCamera.fieldOfView = Mathf.Lerp(PlayerCamera.fieldOfView, SprintFOV, SprintFOVStepTime * Time.deltaTime);

                // Drain sprint remaining while sprinting
                if (!UnlimitedSprint)
                {
                    _sprintRemaining -= 1 * Time.deltaTime;
                    if (_sprintRemaining <= 0)
                    {
                        _isSprinting = false;
                        _isSprintCooldown = true;
                    }
                }
            }
            else
            {
                // Regain sprint while not sprinting
                _sprintRemaining = Mathf.Clamp(_sprintRemaining += 1 * Time.deltaTime, 0, SprintDuration);
            }

            // Handles sprint cooldown 
            // When sprint remaining == 0 stops sprint ability until hitting cooldown
            if (_isSprintCooldown)
            {
                SprintCooldown -= 1 * Time.deltaTime;
                if (SprintCooldown <= 0)
                {
                    _isSprintCooldown = false;
                }
            }
            else
            {
                SprintCooldown = _sprintCooldownReset;
            }

            // Handles sprintBar 
            if (UseSprintBar && !UnlimitedSprint)
            {
                float sprintRemainingPercent = _sprintRemaining / SprintDuration;
                SprintBarFill.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
            }
        }

        #endregion

        #region Jump

        // Gets input and calls jump method
        if (EnableJump && Input.GetKeyDown(JumpKey) && _isGrounded)
        {
            Jump();
        }

        #endregion

        #region Crouch

        if (EnableCrouch)
        {
            if (Input.GetKeyDown(CrouchKey) && !HoldToCrouch)
            {
                Crouch();
            }

            if (Input.GetKeyDown(CrouchKey) && HoldToCrouch)
            {
                _isCrouched = false;
                Crouch();
            }
            else if (Input.GetKeyUp(CrouchKey) && HoldToCrouch)
            {
                _isCrouched = true;
                Crouch();
            }
        }

        #endregion

        CheckGround();

        if (EnableHeadBob)
        {
            HeadBob();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            _isDashing = true;
        }
    }

    void FixedUpdate()
    {
        #region Movement

        if (PlayerCanMove)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Checks if player is walking and isGrounded
            // Will allow head bob
            if (targetVelocity.x != 0 || targetVelocity.z != 0 && _isGrounded)
            {
                // ------- INSERT WALK ANIMATION? ----------
                _isWalking = true;
            }
            else
            {
                _isWalking = false;
            }

            // All movement calculations shile sprint is active
            if (EnableSprint && Input.GetKey(SprintKey) && _sprintRemaining > 0f && !_isSprintCooldown)
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * SprintSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = _rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -MaxVelocityChange, MaxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -MaxVelocityChange, MaxVelocityChange);
                velocityChange.y = 0;

                // Player is only moving when valocity change != 0
                // Makes sure fov change only happens during movement
                if (velocityChange.x != 0 || velocityChange.z != 0)
                {
                    _isSprinting = true;

                    if (_isCrouched)
                    {
                        Crouch();
                    }

                    if (HideBarWhenFull && !UnlimitedSprint)
                    {
                        _sprintBarCanvasGroup.alpha += 5 * Time.deltaTime;
                    }
                }

                _rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            // All movement calculations while walking
            else
            {
                _isSprinting = false;

                if (HideBarWhenFull && _sprintRemaining == SprintDuration)
                {
                    _sprintBarCanvasGroup.alpha -= 3 * Time.deltaTime;
                }

                targetVelocity = transform.TransformDirection(targetVelocity) * WalkSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = _rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -MaxVelocityChange, MaxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -MaxVelocityChange, MaxVelocityChange);
                velocityChange.y = 0;

                _rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }

            if (_isDashing)
            {
                Dash();
            }
        }

        #endregion
    }

    // Sets isGrounded based on a raycast sent straigth down from the player object
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (_isGrounded)
        {
            _rb.AddForce(0f, JumpPower, 0f, ForceMode.Impulse);
            _isGrounded = false;
        }

        // When crouched and using toggle system, will uncrouch for a jump
        if (_isCrouched && !HoldToCrouch)
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        // Stands player up to full height
        // Brings walkSpeed back up to original speed
        if (_isCrouched)
        {
            transform.localScale = new Vector3(_standingScale.x, _standingScale.y, _standingScale.z);
            WalkSpeed /= CrouchSpeedReduction;

            _isCrouched = false;
        }
        // Crouches player down to set height
        // Reduces walkSpeed
        else
        {
            transform.localScale = new Vector3(_standingScale.x, CrouchHeight, _standingScale.z);
            WalkSpeed *= CrouchSpeedReduction;

            _isCrouched = true;
        }
    }

    private void HeadBob()
    {
        if (_isWalking)
        {
            // Calculates HeadBob speed during sprint
            if (_isSprinting)
            {
                _headBobTimer += Time.deltaTime * (BobSpeed + SprintSpeed);
            }
            // Calculates HeadBob speed during crouched movement
            else if (_isCrouched)
            {
                _headBobTimer += Time.deltaTime * (BobSpeed * CrouchSpeedReduction);
            }
            // Calculates HeadBob speed during walking
            else
            {
                _headBobTimer += Time.deltaTime * BobSpeed;
            }
            // Applies HeadBob movement
            Joint.localPosition = new Vector3(_jointOriginalPos.x + Mathf.Sin(_headBobTimer) * BobAmount.x, _jointOriginalPos.y + Mathf.Sin(_headBobTimer) * BobAmount.y, _jointOriginalPos.z + Mathf.Sin(_headBobTimer) * BobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            _headBobTimer = 0;
            Joint.localPosition = new Vector3(Mathf.Lerp(Joint.localPosition.x, _jointOriginalPos.x, Time.deltaTime * BobSpeed), Mathf.Lerp(Joint.localPosition.y, _jointOriginalPos.y, Time.deltaTime * BobSpeed), Mathf.Lerp(Joint.localPosition.z, _jointOriginalPos.z, Time.deltaTime * BobSpeed));
        }
    }

    private void Dash()
    {
        _rb.AddForce(transform.forward * _dashSpeed, ForceMode.Impulse);
        _isDashing = false;

        //if (_dashRecharge <= 0)
        //{
           
        //    _dashRecharge 
        //}
        

        if (_dashEffect != null) // Placeholder. 
        {
            GameObject dashEffect = Instantiate(_dashEffect, transform.position, _dashEffect.transform.rotation);
            dashEffect.transform.parent = transform;
            dashEffect.transform.LookAt(transform);
        }
    }
}



// Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(FirstPersonController)), InitializeOnLoadAttribute]
public class FirstPersonControllerEditor : Editor
{
    FirstPersonController fpc;
    SerializedObject SerFPC;

    private void OnEnable()
    {
        fpc = (FirstPersonController)target;
        SerFPC = new SerializedObject(fpc);
    }

    public override void OnInspectorGUI()
    {
        SerFPC.Update();

        EditorGUILayout.Space();
        GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Jess Case", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.0.1", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        #region Camera Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.PlayerCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "Camera attached to the controller."), fpc.PlayerCamera, typeof(Camera), true);
        fpc.FOV = EditorGUILayout.Slider(new GUIContent("Field of View", "The camera’s view angle. Changes the player camera directly."), fpc.FOV, fpc.ZoomFOV, 179f);
        fpc.CameraCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Rotation", "Determines if the camera is allowed to move."), fpc.CameraCanMove);

        GUI.enabled = fpc.CameraCanMove;
        fpc.InvertCamera = EditorGUILayout.ToggleLeft(new GUIContent("Invert Camera Rotation", "Inverts the up and down movement of the camera."), fpc.InvertCamera);
        fpc.MouseSensitivity = EditorGUILayout.Slider(new GUIContent("Look Sensitivity", "Determines how sensitive the mouse movement is."), fpc.MouseSensitivity, .1f, 10f);
        fpc.MaxLookAngle = EditorGUILayout.Slider(new GUIContent("Max Look Angle", "Determines the max and min angle the player camera is able to look."), fpc.MaxLookAngle, 40, 90);
        GUI.enabled = true;

        fpc.IsCursorLocked = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off the cursor visibility and locks it to the middle of the screen."), fpc.IsCursorLocked);

        fpc.CrosshairEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if the basic crosshair will be turned on, and sets is to the center of the screen."), fpc.CrosshairEnabled);

        // Only displays crosshair options if crosshair is enabled
        if (fpc.CrosshairEnabled)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Crosshair Image", "Sprite to use as the crosshair."));
            fpc.CrosshairImage = (Sprite)EditorGUILayout.ObjectField(fpc.CrosshairImage, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.CrosshairColor = EditorGUILayout.ColorField(new GUIContent("Crosshair Color", "Determines the color of the crosshair."), fpc.CrosshairColor);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        #region Camera Zoom Setup

        GUILayout.Label("Zoom", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.EnableZoom = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), fpc.EnableZoom);

        GUI.enabled = fpc.EnableZoom;
        fpc.HoldToZoom = EditorGUILayout.ToggleLeft(new GUIContent("Hold to Zoom", "Requires the player to hold the zoom key instead if pressing to zoom and unzoom."), fpc.HoldToZoom);
        fpc.ZoomKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Zoom Key", "Determines what key is used to zoom."), fpc.ZoomKey);
        fpc.ZoomFOV = EditorGUILayout.Slider(new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."), fpc.ZoomFOV, .1f, fpc.FOV);
        fpc.ZoomStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while zooming in."), fpc.ZoomStepTime, .1f, 10f);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Movement Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.PlayerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), fpc.PlayerCanMove);

        GUI.enabled = fpc.PlayerCanMove;
        fpc.WalkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), fpc.WalkSpeed, .1f, fpc.SprintSpeed);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #region Sprint

        GUILayout.Label("Sprint", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.EnableSprint = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprint", "Determines if the player is allowed to sprint."), fpc.EnableSprint);

        GUI.enabled = fpc.EnableSprint;
        fpc.UnlimitedSprint = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Determines if 'Sprint Duration' is enabled. Turning this on will allow for unlimited sprint."), fpc.UnlimitedSprint);
        fpc.SprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sprint Key", "Determines what key is used to sprint."), fpc.SprintKey);
        fpc.SprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), fpc.SprintSpeed, fpc.WalkSpeed, 20f);

        //GUI.enabled = !fpc.unlimitedSprint;
        fpc.SprintDuration = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint while unlimited sprint is disabled."), fpc.SprintDuration, 0.5f, 20f);
        fpc.SprintCooldown = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines how long the recovery time is when the player runs out of sprint."), fpc.SprintCooldown, .1f, fpc.SprintDuration);
        //GUI.enabled = true;

        fpc.SprintFOV = EditorGUILayout.Slider(new GUIContent("Sprint FOV", "Determines the field of view the camera changes to while sprinting."), fpc.SprintFOV, fpc.FOV, 179f);
        fpc.SprintFOVStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while sprinting."), fpc.SprintFOVStepTime, .1f, 20f);

        fpc.UseSprintBar = EditorGUILayout.ToggleLeft(new GUIContent("Use Sprint Bar", "Determines if the default sprint bar will appear on screen."), fpc.UseSprintBar);

        // Only displays sprint bar options if sprint bar is enabled
        if (fpc.UseSprintBar)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            fpc.HideBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides the sprint bar when sprint duration is full, and fades the bar in when sprinting. Disabling this will leave the bar on screen at all times when the sprint bar is enabled."), fpc.HideBarWhenFull);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar BG", "Object to be used as sprint bar background."));
            fpc.SprintBarBG = (Image)EditorGUILayout.ObjectField(fpc.SprintBarBG, typeof(Image), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Object to be used as sprint bar foreground."));
            fpc.SprintBarFill = (Image)EditorGUILayout.ObjectField(fpc.SprintBarFill, typeof(Image), true);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            fpc.SprintBarWidthPercent = EditorGUILayout.Slider(new GUIContent("Bar Width", "Determines the width of the sprint bar."), fpc.SprintBarWidthPercent, .1f, .5f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.SprintBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Height", "Determines the height of the sprint bar."), fpc.SprintBarHeightPercent, .001f, .025f);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Jump

        GUILayout.Label("Jump", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.EnableJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Jump", "Determines if the player is allowed to jump."), fpc.EnableJump);

        GUI.enabled = fpc.EnableJump;
        fpc.JumpKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Jump Key", "Determines what key is used to jump."), fpc.JumpKey);
        fpc.JumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player will jump."), fpc.JumpPower, .1f, 20f);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Crouch

        GUILayout.Label("Crouch", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.EnableCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Crouch", "Determines if the player is allowed to crouch."), fpc.EnableCrouch);

        GUI.enabled = fpc.EnableCrouch;
        fpc.HoldToCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Hold To Crouch", "Requires the player to hold the crouch key instead if pressing to crouch and uncrouch."), fpc.HoldToCrouch);
        fpc.CrouchKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Crouch Key", "Determines what key is used to crouch."), fpc.CrouchKey);
        fpc.CrouchHeight = EditorGUILayout.Slider(new GUIContent("Crouch Height", "Determines the y scale of the player object when crouched."), fpc.CrouchHeight, .1f, 1);
        fpc.CrouchSpeedReduction = EditorGUILayout.Slider(new GUIContent("Speed Reduction", "Determines the percent 'Walk Speed' is reduced by. 1 being no reduction, and .5 being half."), fpc.CrouchSpeedReduction, .1f, 1);
        fpc._dashSpeed = EditorGUILayout.Slider(new GUIContent("Dash Speed", "Description"), fpc._dashSpeed, 50, 300);
        fpc._dashRecharge = EditorGUILayout.Slider(new GUIContent("Dash Recharge Time", "Description"), fpc._dashSpeed, 50, 300);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Head Bob

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Head Bob Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.EnableHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Enable Head Bob", "Determines if the camera will bob while the player is walking."), fpc.EnableHeadBob);


        GUI.enabled = fpc.EnableHeadBob;
        fpc.Joint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Joint", "Joint object position is moved while head bob is active."), fpc.Joint, typeof(Transform), true);
        fpc.BobSpeed = EditorGUILayout.Slider(new GUIContent("Speed", "Determines how often a bob rotation is completed."), fpc.BobSpeed, 1, 20);
        fpc.BobAmount = EditorGUILayout.Vector3Field(new GUIContent("Bob Amount", "Determines the amount the joint moves in both directions on every axes."), fpc.BobAmount);
        GUI.enabled = true;

        #endregion

        //Sets any changes from the prefab
        if (GUI.changed)
        {
            EditorUtility.SetDirty(fpc);
            Undo.RecordObject(fpc, "FPC Change");
            SerFPC.ApplyModifiedProperties();
        }

    }

}

#endif