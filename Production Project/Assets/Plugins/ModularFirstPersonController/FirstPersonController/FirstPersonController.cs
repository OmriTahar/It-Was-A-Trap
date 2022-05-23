// CHANGE LOG
// 
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using System.Net;
#endif

public class FirstPersonController : MonoBehaviour
{

    #region Variables

    #region General

    [SerializeField] internal Rigidbody _rb;
    [SerializeField] internal Transform _GFXBody;

    #endregion

    #region Camera

    public Camera PlayerMovementCamera;
    public bool CameraCanMove = true;
    public bool IsCursorLocked = true;

    #endregion

    #region Movement

    public bool PlayerCanMove = true;
    public bool SharonsMovement = false;
    public float WalkSpeed = 5f;
    public float MaxVelocityChange = 10f;

    private bool _isWalking = false;

    #endregion

    #region Sprint

    public bool EnableSprint = true;
    public bool UnlimitedSprint = false;
    public KeyCode SprintKey = KeyCode.LeftShift;
    public float SprintSpeed = 50f;
    public float SprintDuration = 0.1f;
    public float SprintCooldown = 3f;

    // Sprint Bar
    public bool UseSprintBar = true;
    public bool HideBarWhenFull = false;
    public Image SprintBarBG;
    public Image SprintBarFill;

    private CanvasGroup _sprintBarCanvasGroup;
    private bool _isSprinting = false;
    private float _sprintRemaining;
    private bool _isSprintCooldown = false;
    private float _sprintCooldownReset;

    #endregion

    #region NewSprint

    public bool NewDash;
    private bool _canDash = true;
    private bool _hasDashed = false;

    #endregion

    #region Jump

    public bool EnableJump = true;
    public KeyCode JumpKey = KeyCode.Space;
    public float JumpPower = 5f;

    private bool _isGrounded = false;

    #endregion

    #region Crouch

    public bool EnableCrouch = true;
    public bool HoldToCrouch = true;
    public KeyCode CrouchKey = KeyCode.LeftControl;
    public float CrouchHeight = .75f;
    public float CrouchSpeedReduction = .5f;

    private bool _isCrouched = false;
    private Vector3 _standingScale;

    #endregion

    #endregion

    private void Awake()
    {
        _standingScale = transform.localScale;

        if (!UnlimitedSprint)
        {
            _sprintRemaining = SprintDuration;
            _sprintCooldownReset = SprintCooldown;
        }
    }

    void Start()
    {
        if (IsCursorLocked)
            Cursor.lockState = CursorLockMode.Locked;

        #region Sprint Bar Setup

        _sprintBarCanvasGroup = GetComponentInChildren<CanvasGroup>();

        if (UseSprintBar)
        {
            SprintBarBG.gameObject.SetActive(true);
            SprintBarFill.gameObject.SetActive(true);

            if (HideBarWhenFull)
                _sprintBarCanvasGroup.alpha = 0;
        }
        else
        {
            SprintBarBG.gameObject.SetActive(false);
            SprintBarFill.gameObject.SetActive(false);
        }

        #endregion
    }

    private void Update()
    {
        if (CameraCanMove)
            CameraInput();

        #region Handle Sprint

        if (EnableSprint)
        {
            if (_isSprinting)
            {
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

                // Fill Sprint bar while on cooldown
                float sprintCooldownPercent = _sprintRemaining / SprintCooldown;
                SprintBarFill.fillAmount = sprintCooldownPercent;

                if (SprintCooldown <= 0)
                {
                    _isSprintCooldown = false;
                    //_hasDashed = false;
                }
            }
            else
            {
                // Drain Sprint bar while sprinting

                float sprintRemainingPercent = _sprintRemaining / SprintDuration;
                SprintBarFill.fillAmount = sprintRemainingPercent;

                SprintCooldown = _sprintCooldownReset;
            }

            // --------- OLD METHOD: Handles sprintBar -------------
            //if (UseSprintBar && !UnlimitedSprint)
            //{
            //    float sprintRemainingPercent = _sprintRemaining / SprintDuration;
            //    SprintBarFill.fillAmount = sprintRemainingPercent;
            //}
        }

        #endregion

        if (EnableJump && Input.GetKeyDown(JumpKey) && _isGrounded)
            Jump();

        #region Handle Crouch

        if (EnableCrouch)
        {
            if (Input.GetKeyDown(CrouchKey) && !HoldToCrouch)
                Crouch();

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
    }

    void FixedUpdate()
    {
        #region Movement

        if (PlayerCanMove)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Checks if player is walking and isGrounded
            if (targetVelocity.x != 0 || targetVelocity.z != 0 && _isGrounded)
            {
                // ------- INSERT WALK ANIMATION? ----------
                _isWalking = true;
            }
            else
            {
                _isWalking = false;
            }

            if (!NewDash)
            {
                // All movement calculations while sprint is active
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
                    if (velocityChange.x != 0 || velocityChange.z != 0)
                    {
                        _isSprinting = true;
                        #region NotUsed

                        if (_isCrouched)
                            Crouch();

                        if (HideBarWhenFull && !UnlimitedSprint)
                            _sprintBarCanvasGroup.alpha += 5 * Time.deltaTime;

                        #endregion
                    }

                    _rb.AddForce(velocityChange, ForceMode.Impulse);
                }
                // All movement calculations while walking
                else
                {
                    _isSprinting = false;

                    if (HideBarWhenFull && _sprintRemaining == SprintDuration)
                        _sprintBarCanvasGroup.alpha -= 3 * Time.deltaTime;

                    targetVelocity = transform.TransformDirection(targetVelocity) * WalkSpeed;

                    // Apply a force that attempts to reach our target velocity
                    Vector3 velocity = _rb.velocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -MaxVelocityChange, MaxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -MaxVelocityChange, MaxVelocityChange);
                    velocityChange.y = 0;

                    _rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
            }
            else
            {
                if (EnableSprint && Input.GetKey(SprintKey) && _canDash)
                {

                }
                // All movement calculations while walking
                else
                {

                }
            }
        }

        #endregion
    }

    private void CameraInput()
    {

        if (!SharonsMovement)
        {
            Ray cameraRay = PlayerMovementCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;

            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));

                Debug.DrawLine(cameraRay.origin, pointToLook, Color.cyan);
            }
        }
        else
        {           
            RaycastHit hit;
            Ray camRay = PlayerMovementCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(camRay, out hit))
            {
                _GFXBody.LookAt(new Vector3(hit.point.x, _GFXBody.position.y, hit.point.z));
                Debug.DrawLine(camRay.origin, hit.point, Color.yellow);
            }
        }

    }

    private void CheckGround() // Sets _isGrounded based on a raycast sent straigth down from the player object
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
}


// Custom Editor - Script Inspector Setup
#if UNITY_EDITOR
[CustomEditor(typeof(FirstPersonController)), InitializeOnLoadAttribute]
public class FirstPersonControllerEditor : Editor
{
    FirstPersonController _firstPersonController_EditorRef;
    SerializedObject _serializedObject;

    public float SprintMaxSpeed = 100f;
    public float SprintMaxDuration = 10f;

    private void OnEnable()
    {
        _firstPersonController_EditorRef = (FirstPersonController)target;
        _serializedObject = new SerializedObject(_firstPersonController_EditorRef);
    }

    public override void OnInspectorGUI() // All Inspector Shit - Highly Customizable
    {
        _serializedObject.Update();

        #region Script Title In Inspector

        EditorGUILayout.Space();
        GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Jess Case - Modded by OmriT", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.0.1", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        #endregion

        #region Camera Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Camera", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        _firstPersonController_EditorRef.PlayerMovementCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Player Movement Camera", "The camera that shoots rays for the player to look at."), _firstPersonController_EditorRef.PlayerMovementCamera, typeof(Camera), true);
        _firstPersonController_EditorRef.CameraCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Rotation", "Determines if the camera is allowed to move."), _firstPersonController_EditorRef.CameraCanMove);

        GUI.enabled = _firstPersonController_EditorRef.CameraCanMove;
        GUI.enabled = true;

        _firstPersonController_EditorRef.IsCursorLocked = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off the cursor visibility and locks it to the middle of the screen."), _firstPersonController_EditorRef.IsCursorLocked);
        EditorGUILayout.Space();

        #endregion

        #region Movement Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        _firstPersonController_EditorRef.SharonsMovement = EditorGUILayout.ToggleLeft(new GUIContent("Sharons Movement", "Movement which i think fits better"), _firstPersonController_EditorRef.SharonsMovement);
        _firstPersonController_EditorRef.PlayerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), _firstPersonController_EditorRef.PlayerCanMove);
        _firstPersonController_EditorRef._rb = (Rigidbody)EditorGUILayout.ObjectField(new GUIContent("RigidBody", "Place Moving RigidBody"), _firstPersonController_EditorRef._rb, typeof(Rigidbody), true);
        _firstPersonController_EditorRef._GFXBody = (Transform)EditorGUILayout.ObjectField(new GUIContent("Transform", "Place Players' Graphic Transform"), _firstPersonController_EditorRef._GFXBody, typeof(Transform), true);

        GUI.enabled = _firstPersonController_EditorRef.SharonsMovement;
        GUI.enabled = _firstPersonController_EditorRef.PlayerCanMove;
        _firstPersonController_EditorRef.WalkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), _firstPersonController_EditorRef.WalkSpeed, .1f, _firstPersonController_EditorRef.SprintSpeed);
        GUI.enabled = true;
        EditorGUILayout.Space();

        #region Sprint

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Sprint", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        _firstPersonController_EditorRef.NewDash = EditorGUILayout.ToggleLeft(new GUIContent("New Dash", "more dashy than sprinty"), _firstPersonController_EditorRef.NewDash);
        _firstPersonController_EditorRef.EnableSprint = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprint", "Determines if the player is allowed to sprint."), _firstPersonController_EditorRef.EnableSprint);

        GUI.enabled = _firstPersonController_EditorRef.NewDash;
        GUI.enabled = _firstPersonController_EditorRef.EnableSprint;
        _firstPersonController_EditorRef.UnlimitedSprint = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Determines if 'Sprint Duration' is enabled. Turning this on will allow for unlimited sprint."), _firstPersonController_EditorRef.UnlimitedSprint);
        _firstPersonController_EditorRef.SprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sprint Key", "Determines what key is used to sprint."), _firstPersonController_EditorRef.SprintKey);
        _firstPersonController_EditorRef.SprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), _firstPersonController_EditorRef.SprintSpeed, _firstPersonController_EditorRef.WalkSpeed, SprintMaxSpeed);

        GUI.enabled = !_firstPersonController_EditorRef.UnlimitedSprint;
        _firstPersonController_EditorRef.SprintDuration = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint while unlimited sprint is disabled."), _firstPersonController_EditorRef.SprintDuration, 0.01f, 30f);
        _firstPersonController_EditorRef.SprintCooldown = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines how long the recovery time is when the player runs out of sprint."), _firstPersonController_EditorRef.SprintCooldown, .1f, 10f);
        GUI.enabled = true;

        _firstPersonController_EditorRef.UseSprintBar = EditorGUILayout.ToggleLeft(new GUIContent("Use Sprint Bar", "Determines if the default sprint bar will appear on screen."), _firstPersonController_EditorRef.UseSprintBar);

        // Only displays sprint bar options if sprint bar is enabled
        if (_firstPersonController_EditorRef.UseSprintBar)
        {
            EditorGUI.indentLevel++;

            //EditorGUILayout.BeginHorizontal();
            //_firstPersonController_EditorRef.HideBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides the sprint bar when sprint duration is full, and fades the bar in when sprinting. Disabling this will leave the bar on screen at all times when the sprint bar is enabled."), _firstPersonController_EditorRef.HideBarWhenFull);
            //EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar BG", "Object to be used as sprint bar background."));
            _firstPersonController_EditorRef.SprintBarBG = (Image)EditorGUILayout.ObjectField(_firstPersonController_EditorRef.SprintBarBG, typeof(Image), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Object to be used as sprint bar foreground."));
            _firstPersonController_EditorRef.SprintBarFill = (Image)EditorGUILayout.ObjectField(_firstPersonController_EditorRef.SprintBarFill, typeof(Image), true);
            EditorGUILayout.EndHorizontal();


            //EditorGUILayout.BeginHorizontal();
            //_firstPersonController_EditorRef.SprintBarWidthPercent = EditorGUILayout.Slider(new GUIContent("Bar Width", "Determines the width of the sprint bar."), _firstPersonController_EditorRef.SprintBarWidthPercent, .1f, .5f);
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //_firstPersonController_EditorRef.SprintBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Height", "Determines the height of the sprint bar."), _firstPersonController_EditorRef.SprintBarHeightPercent, .001f, .025f);
            //EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        GUI.enabled = true;
        EditorGUILayout.Space();

        #endregion

        #region Jump

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Jump", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        _firstPersonController_EditorRef.EnableJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Jump", "Determines if the player is allowed to jump."), _firstPersonController_EditorRef.EnableJump);

        GUI.enabled = _firstPersonController_EditorRef.EnableJump;
        _firstPersonController_EditorRef.JumpKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Jump Key", "Determines what key is used to jump."), _firstPersonController_EditorRef.JumpKey);
        _firstPersonController_EditorRef.JumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player will jump."), _firstPersonController_EditorRef.JumpPower, .1f, 20f);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Crouch

        //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //GUILayout.Label("Crouch", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        //EditorGUILayout.Space();

        //_firstPersonController_EditorRef.EnableCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Crouch", "Determines if the player is allowed to crouch."), _firstPersonController_EditorRef.EnableCrouch);

        //GUI.enabled = _firstPersonController_EditorRef.EnableCrouch;
        //_firstPersonController_EditorRef.HoldToCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Hold To Crouch", "Requires the player to hold the crouch key instead if pressing to crouch and uncrouch."), _firstPersonController_EditorRef.HoldToCrouch);
        //_firstPersonController_EditorRef.CrouchKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Crouch Key", "Determines what key is used to crouch."), _firstPersonController_EditorRef.CrouchKey);
        //_firstPersonController_EditorRef.CrouchHeight = EditorGUILayout.Slider(new GUIContent("Crouch Height", "Determines the y scale of the player object when crouched."), _firstPersonController_EditorRef.CrouchHeight, .1f, 1);
        //_firstPersonController_EditorRef.CrouchSpeedReduction = EditorGUILayout.Slider(new GUIContent("Speed Reduction", "Determines the percent 'Walk Speed' is reduced by. 1 being no reduction, and .5 being half."), _firstPersonController_EditorRef.CrouchSpeedReduction, .1f, 1);
        //GUI.enabled = true;

        #endregion

        #endregion

        //Sets any changes from the prefab
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_firstPersonController_EditorRef);
            Undo.RecordObject(_firstPersonController_EditorRef, "FPC Change");
            _serializedObject.ApplyModifiedProperties();
        }
    }

}

#endif