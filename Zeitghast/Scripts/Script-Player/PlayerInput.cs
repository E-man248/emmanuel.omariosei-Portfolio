using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInput : Movement
{
    //Input system
    private InputAction movementInputAction;
    private InputAction jumpInputAction;
    private InputAction dashInputAction;
    private InputAction crouchInputAction;

    public enum AimInput
    {
        Mouse,
        Joystick
    }

    public bool controlsDisabled { get; private set; }
    public AimInput currentAimInputDevice { get; private set; }

    [Header("States")]
    internal bool isCrouching;
    internal bool isDashing;

    public bool stunned;

    [Header("Crouch")]
    public float crouchSpeed;
    private float toggledDefaultSpeed;
    public Vector2 crouchBoxcolliderSize;
    public Vector2 crouchBoxcolliderOffset;

    public Vector2 orignalBoxcolliderSize;
    public Vector2 orignalBoxcolliderOffset;
    public Transform OverHeadCheck;
    public float OverHeadRadius;

    [Header("Jump")]
    public float hangtime = 0.2f;
    public float shortJumpHeightMultiplier;
    private float hangtimeCounter;
    private bool hasShortJumped;
    public int lookDirection = 1;
    public float jumpMovementDrag = 1;
    private const float BOOSTED_HANGTIME_CONSTANT = 0.1f;

    [Header("Dash")]
    public DashInfo defaultDash;
    private DashInfo currentDash;
    internal Vector2 dashDirection;
    private float dashTimer;
    private float dashCoolDownTimer;
    private bool resetDashWhenCurrentDashComplete;
    public UnityEvent dashResetEvent;
    private WeaponManager weaponManager;
    private PlayerKnockback playerKnockback;

    [Header("Wall Interaction")]
    public bool WallJumpEnabled = true;
    public LayerMaskObject wallTouchLayers;
    public Vector2 wallJumpForce;
    public float wallJumpInputDuration;
    public float wallJumpInputTimer;
    public float wallJumpDuration;
    public float wallJumpTimer;
    public Vector2 WallClingForce;
    [SerializeField] private bool wallDetectionDebugEnabled = true;
    private int currentWallJumpDirection = 0;

    [Header("Fall Settings")]
    [SerializeField] private float MaxFallSpeed = 30f;
    [SerializeField] protected Vector2 fastFallSpeedMultiplier = Vector2.one;
    [SerializeField] protected Vector2 fastFallVariableSpeedIntensity = Vector2.one;
    [SerializeField] private Vector2 FastFallSpeedCap;
    internal bool isFastFalling;

    // Useful References
    private PlayerHealth playerHealth;


    private void Start()
    {
        if (defaultDash == null) Debug.LogError("[PlayerInput] No 'Default Dash' DashInfo for " + name);
        
        currentDash = defaultDash;

        dashTimer = currentDash.startDashTime;
        dashCoolDownTimer = currentDash.dashCoolDown;
        resetDashWhenCurrentDashComplete = false;

        wallJumpTimer = wallJumpDuration;
        lookDirection = 1;
        variableSpeed = 0f;

        //Getting useful refrences
        weaponManager = GetComponentInChildren<WeaponManager>();
        playerKnockback = GetComponent<PlayerKnockback>();
        if (weaponManager == null) Debug.LogError("[PlayerInput] No 'weaponManager' WeaponManager for " + name);
        if (playerKnockback == null) Debug.LogError("[PlayerInput] No 'playerKnockback' PlayerKnockback for " + name);
        playerHealth = GetComponent<PlayerHealth>();

        orignalBoxcolliderSize = ((BoxCollider2D)entityCollider).size;
        orignalBoxcolliderOffset = ((BoxCollider2D)entityCollider).offset;

        //Input system initalization
        movementInputAction = InputActionManager.Instance.playerAction.Move;
        jumpInputAction = InputActionManager.Instance.playerAction.Jump;
        dashInputAction = InputActionManager.Instance.playerAction.Dash;
        crouchInputAction = InputActionManager.Instance.playerAction.Crouch;

        //Enabling each  instance of the input system
        movementInputAction.Enable();
        jumpInputAction.Enable();
        dashInputAction.Enable();
        crouchInputAction.Enable();
        
        subscribeToEvents();
    }

    protected void OnEnable()
    {
        subscribeToEvents();
    }

    protected void OnDisable()
    {
        unsubscribeToEvents();
    }

    protected void OnDestroy()
    {
        unsubscribeToEvents();
    }

    void FixedUpdate()
    {
        if (Timer.gamePaused)
        {
            return;
        }
        if (!isDashing) // Not Dashing
        {
            if (!isWallJumping())  // Not Wall Jumping
            {
                float leftRightInput = getLeftRightInput();
                if (!controlsDisabled)
                {
                    if (isGrounded())
                    {
                        moveX(leftRightInput);
                    }
                    else
                    {
                        if (leftRightInput == 0 && !isFastFalling)
                        {
                            entityRigidbody.velocity = new Vector2(entityRigidbody.velocity.x / (jumpMovementDrag+Time.deltaTime), entityRigidbody.velocity.y);
                        }
                        else
                        {
                            moveX(leftRightInput);
                        }
                    }
                    if (leftRightInput != 0) lookDirection = Mathf.RoundToInt(leftRightInput);
                }
            }
        }

        fall();
    }

    protected override void Update()
    {
        if (Timer.gamePaused)
        {
            return;
        }

        updateAimInputDevice();

        // HangTime Counter
        if (isGrounded())
        {
            //Setting the 
            if(toggledDefaultSpeed / currentSpeed != 1)
            {
                hangtimeCounter = hangtime - (hangtime * ((toggledDefaultSpeed / currentSpeed) + BOOSTED_HANGTIME_CONSTANT));
                hangtimeCounter = hangtime;
            }
            else
            {
                hangtimeCounter = hangtime;
            }
        }
        else
        {
            hangtimeCounter -= Time.deltaTime;
        }
        
        if (!isCrouching)
        {
            // Movement Actions:
            if ((isWallSliding() && !isGrounded()) || wallJumpTimer < wallJumpDuration) // Inputing a WallJump?
            {
                wallJumpInput();
            }
            else // Otherwise just jump
            {
                performJumpInput();
            }
        }

        dashInput();
        crouchInput();

        if(isCrouching)
        {
            toggledDefaultSpeed = crouchSpeed;
        }
        else
        {
            toggledDefaultSpeed = speed;
        }
        currentSpeed = toggledDefaultSpeed + variableSpeed;
    }

    #region Event Functions

    private void subscribeToEvents()
    {
        PlayerInfo.PlayerDeathEvent.AddListener(OnPlayerDeath);
    } 

    private void unsubscribeToEvents()
    {
        PlayerInfo.PlayerDeathEvent.RemoveListener(OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        // Disable Player Controls:
        setControlsDisable(true, true);

        // Lock Player Position:
        LockPlayerPosition();
    }

    #endregion

    float getLeftRightInput()
    {
        movementInputAction.ToString();
        return movementInputAction.ReadValue<Vector2>().x;
    }

    float getUpDownInput()
    {
        return movementInputAction.ReadValue<Vector2>().y;
    }

    void performJumpInput()
    {
        if(!controlsDisabled)
        {
            if (jumpInputAction.WasPressedThisFrame() && hangtimeCounter > 0f)
            {
                jump(jumpForce);
            }
            else if (!hasShortJumped && jumpInputAction.WasReleasedThisFrame() && entityRigidbody.velocity.y > 0)
            {
                jump(entityRigidbody.velocity.y * 0.5f);
                hasShortJumped = true;
            }
        }

        if (isGrounded())
        {
            hasShortJumped = false;
        }
    }

    void crouchInput()
    {
        if (crouchInputAction.IsPressed() && !controlsDisabled && isGrounded())
        {
            ((BoxCollider2D)entityCollider).size = crouchBoxcolliderSize;
            ((BoxCollider2D)entityCollider).offset = crouchBoxcolliderOffset;

            isCrouching = true;
        }
        else 
        {
            //Checks if they is something above the player when crouching 
            if(Physics2D.OverlapCircle(OverHeadCheck.position, OverHeadRadius, groundLayers.layerMask) && isGrounded())
            {
                ((BoxCollider2D)entityCollider).size = crouchBoxcolliderSize;
                ((BoxCollider2D)entityCollider).offset = crouchBoxcolliderOffset;

                isCrouching = true; 
                
            }
            else
            {
                ((BoxCollider2D)entityCollider).size = orignalBoxcolliderSize;
                ((BoxCollider2D)entityCollider).offset = orignalBoxcolliderOffset;

                isCrouching = false;
            }
        }    
    }

    public void crouchReset()
    {
        ((BoxCollider2D)entityCollider).size = orignalBoxcolliderSize;
        ((BoxCollider2D)entityCollider).offset = orignalBoxcolliderOffset;

        isCrouching = false;
    }

    Vector2 getDashDirectionInput()
    {
        Vector2 dashInput = new Vector2(getLeftRightInput(), getUpDownInput());

        dashInput = (dashInput * 100000).normalized;

        return dashInput;
    }

    public float getStartDashTime()
    {
        return currentDash.startDashTime;
    }

    private void updateAimInputDevice()
    {
        if ((new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))).magnitude > 0.05f)
        {
            currentAimInputDevice = AimInput.Mouse;
        }

        Vector3 joystickAimInput = getJoystickAimInput();
        if (joystickAimInput.magnitude > 0.05f)
        {
            currentAimInputDevice = AimInput.Joystick;
        }
    }

    public Vector3 getMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public Vector3 getJoystickAimInput()
    {
        return new Vector3(Input.GetAxisRaw("Horizontal Aim"), Input.GetAxisRaw("Vertical Aim"));
    }

    void wallJumpInput()
    {
        //Checking to perform wall jumnp if it is enabled
        if(!WallJumpEnabled)
        {
            return;
        }

        if (wallJumpTimer >= wallJumpDuration)
        {
            lookDirection = -GetWallJumpDirection();

            if (jumpInputAction.WasPressedThisFrame()) // Wall Jump Launch
            {
                setControlsDisable(true);

                currentWallJumpDirection = GetWallJumpDirection();

                entityRigidbody.velocity = new Vector2(wallJumpForce.x * currentWallJumpDirection, wallJumpForce.y);

                lookDirection = currentWallJumpDirection;
                wallJumpTimer = wallJumpDuration;
                wallJumpTimer -= Time.deltaTime;
            }
        }
        else
        {
            lookDirection = currentWallJumpDirection;

            if (wallJumpTimer > 0) // Wall Jump Animation
            {
                wallJumpTimer -= Time.deltaTime;

                int NewWallJumpDirection = GetWallJumpDirection();
                bool NewWallDetected = NewWallJumpDirection != 0 && NewWallJumpDirection != currentWallJumpDirection;

                if (NewWallDetected || isGrounded())
                {
                    wallJumpTimer = wallJumpDuration;
                    setControlsDisable(false);
                }
            }
            else // Wall Jump Animation Release
            {
                wallJumpTimer = wallJumpDuration;
                setControlsDisable(false);
            }
        }
    }

    private int GetWallJumpDirection()
    {
        if (!isWallSliding()) return 0; // No wall detected...

        RaycastHit2D wallHitRight = Physics2D.BoxCast(entityCollider.bounds.center, entityCollider.bounds.size, 0, Vector2.right, wallDetectionDistance, wallTouchLayers.layerMask);
        RaycastHit2D wallHitLeft = Physics2D.BoxCast(entityCollider.bounds.center, entityCollider.bounds.size, 0, (-1) * Vector2.right, wallDetectionDistance, wallTouchLayers.layerMask);

        if (wallHitRight)
        {
            return -1; // If a wall is to the player's right, wall jump to the left
        }
        else if (wallHitLeft)
        {
            return 1; // If a wall is to the player's left, wall jump to the right
        }
        else
        {
            return 0; // No wall detected...
        }
    }

    private void fall()
    {
        // Check for Fall Speed Input from Player:
        fastFallInput();

        // Balance the Fall Speed Using the Maximum Fall Speed: (Only When Not Fast Falling)
        if (!isFastFalling) capFallSpeed();
    }

    private void capFallSpeed()
    {
        float fallVelocity = entityRigidbody.velocity.y;

        if (fallVelocity < -MaxFallSpeed)
        {
            fallVelocity = -MaxFallSpeed;
        }

        entityRigidbody.velocity = new Vector2(entityRigidbody.velocity.x, fallVelocity);
    }

    void fastFallInput()
    {
        if((getUpDownInput() < 0) && !controlsDisabled && !isGrounded())
        {
            if (!isWallSliding())
            {
                fastFall();
            }
            else
            {
                wallCling();
            }
        }
        else
        {
            isFastFalling = false;
        }
    }

    private void fastFall()
    {
        // Fast Fall:

        float newVelocityX = entityRigidbody.velocity.x * (fastFallSpeedMultiplier.x + variableSpeed * fastFallVariableSpeedIntensity.x);
        float newVelocityY = -(fastFallSpeedMultiplier.y + variableSpeed * fastFallVariableSpeedIntensity.y);

        // Apply Fast Fall Speed Cap:
        if (FastFallSpeedCap.x > 0)
        {
            int dir = (int) Mathf.Sign(newVelocityX);
            newVelocityX = dir * Mathf.Clamp(Mathf.Abs(newVelocityX), 0, FastFallSpeedCap.x);
        }
        if (FastFallSpeedCap.y > 0)
        {
            int dir = (int) Mathf.Sign(newVelocityY);
            newVelocityY = dir * Mathf.Clamp(Mathf.Abs(newVelocityY), 0, FastFallSpeedCap.y);
        }

        entityRigidbody.velocity += new Vector2(newVelocityX, newVelocityY) / Time.timeScale;
        isFastFalling = true;
    }

    private void wallCling()
    {
        // Wall Cling:
        entityRigidbody.velocity += new Vector2(entityRigidbody.velocity.x * WallClingForce.x, -WallClingForce.y) / Time.timeScale;
        isFastFalling = false;
    }

    void dashInput()
    {
        if (dashDirection == Vector2.zero)
        {
            if (dashCoolDownTimer < 0) 
            {
                //Giving back the player control when the dash is over
                if (controlsDisabled)
                {
                    setControlsDisable(false);
                }

                if (dashInputAction.IsPressed())
                {
                    if(!controlsDisabled)
                    {
                        setControlsDisable(true);

                        Vector2 dashInput = getDashDirectionInput();
                        if (dashInput != Vector2.zero) dashDirection = dashInput;
                        else dashDirection = Vector2.right * lookDirection;

                        // Start of Dash Spawn Bullet:
                        if (currentDash.spawnBulletStartOfDash.bulletToSpawn != null)
                        {
                            Vector3 spawnRotation = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(dashDirection.normalized.y, dashDirection.normalized.normalized.x));

                            // Dash Bullet Spawned at the Start of Dash:
                            GameObject spawn = Instantiate(currentDash.spawnBulletStartOfDash.bulletToSpawn, transform.position, Quaternion.Euler(spawnRotation));
                            if (currentDash.spawnBulletStartOfDash.parentOfSpawn == DashInfo.parentOfSpawnInfo.Player)
                            {
                                spawn.transform.parent = transform;
                            }
                            else if (currentDash.spawnBulletStartOfDash.parentOfSpawn == DashInfo.parentOfSpawnInfo.WeaponFiringPoint)
                            {
                                spawn.transform.position = weaponManager.currentWeapon.FiringPoint.position;
                                spawn.transform.parent = weaponManager.currentWeapon.FiringPoint;
                            }

                            // Check if Spawn was a Bullet:
                            Bullet spawnBulletScript = spawn.GetComponent<Bullet>();
                            if (spawnBulletScript != null) spawnBulletScript.destroyCollisions = weaponManager.bulletDestroyCollisions;
                        }

                        dashCoolDownTimer = currentDash.dashCoolDown;
                        //Debug.Log("Dash Aim: " + dashDirection + " | Potential Dash Displacement: " + potentialDashDisplacement());
                    }
                }
            }
            else
            {
                if (isGrounded()) dashCoolDownTimer -= Time.deltaTime * currentDash.groundCoolDownMultiplier;
                else dashCoolDownTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (dashTimer <= 0)
            {
                dashReset();
                dashCoolDownTimer = currentDash.dashCoolDown;
                // Allow Extra Mid-Air Dash if Enabled:
                if (resetDashWhenCurrentDashComplete)
                {
                    dashCoolDownTimer = 0f;
                    resetDashWhenCurrentDashComplete = false;
                }

                // End of Dash Spawn:
                if (currentDash.spawnBulletEndOfDash.bulletToSpawn != null)
                {
                    Vector3 spawnRotation = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(dashDirection.normalized.y, dashDirection.normalized.normalized.x));

                    // Dash Bullet Spawned at the End of Dash:
                    GameObject spawn = Instantiate(currentDash.spawnBulletEndOfDash.bulletToSpawn, transform.position, Quaternion.Euler(spawnRotation));
                    if (currentDash.spawnBulletEndOfDash.parentOfSpawn == DashInfo.parentOfSpawnInfo.Player)
                    {
                        spawn.transform.parent = transform;
                    }
                    else if (currentDash.spawnBulletEndOfDash.parentOfSpawn == DashInfo.parentOfSpawnInfo.WeaponFiringPoint)
                    {
                        spawn.transform.position = weaponManager.currentWeapon.FiringPoint.position;
                        spawn.transform.parent = weaponManager.currentWeapon.FiringPoint;
                    }
                    
                    // Check if Spawn was a Bullet:
                    Bullet spawnBulletScript = spawn.GetComponent<Bullet>();
                    if (spawnBulletScript != null) spawnBulletScript.destroyCollisions = weaponManager.bulletDestroyCollisions;
                }             

                entityRigidbody.velocity = Vector2.zero;
            }
            else
            {
                // Potential code dash "wall-bump":
                /*if (isWallSliding())
                {
                    dashReset();
                    playerKnockback.applyKnockBackDirection((int)(-dashWallKnockbackForce*lookDirection));
                    playerKnockback.knockbackFrames(dashWallKnockbackTime);
                    Debug.Log(dashWallKnockbackForce*lookDirection);
                    return;
                }*/

                // Continue Dash:
                isDashing = true;
                dashTimer -= Time.deltaTime;

                // Extra Dash Aim:
                if (dashTimer > currentDash.startDashTime - currentDash.extraDashAimTime && new Vector2(getLeftRightInput(), getUpDownInput()) != Vector2.zero) dashDirection = getDashDirectionInput();
                if (dashDirection.x != 0) lookDirection = dashDirection.x > 0 ? 1 : -1;

                // Dash Speed Equation:
                float calculatedDashSpeed = currentDash.dashSpeedMultiplier * toggledDefaultSpeed + variableSpeed * currentDash.dashVariableSpeedMultiplier;
                entityRigidbody.velocity = calculatedDashSpeed * dashDirection.normalized + getDashDirectionInput() * currentDash.afterDashControl;
            }
        }
    }

    public void setPlayerDashInfo(DashInfo dashInfo)
    {
        if (dashInfo != null)
        {
            currentDash = dashInfo;
        }
        else
        {
            currentDash = defaultDash;
        }
    }

    public bool canDash()
    {
        return !isDashing && dashDirection == Vector2.zero && dashTimer >= currentDash?.startDashTime && dashCoolDownTimer < 0;
    }

    public void setControlsDisable(bool controlDisableValue, bool forceValue = false)
    {
        //Check if we want to forcfuly enable and disable the controls
        if (!forceValue)
        {
            //Preventing you from renabling your controls if you are stunned 
            if (stunned && !controlDisableValue)
            {
                return;
            }

            //Preventing you from renabling your controls if you are dead 
            if (playerHealth.isDead && !controlDisableValue)
            {
                return;
            }
        }

        controlsDisabled = controlDisableValue;
    }

    public void LockPlayerPosition()
    {
        LockPlayerPositionX();
        LockPlayerPositionY();
    }

    public void LockPlayerPositionX()
    {
        entityRigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX;
    }

    public void LockPlayerPositionY()
    {
        entityRigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;
    }

    public void UnlockPlayerPosition()
    {
        bool rotationWasFrozen = entityRigidbody.freezeRotation;

        entityRigidbody.constraints = RigidbodyConstraints2D.None;
        
        if (rotationWasFrozen)
        {
            entityRigidbody.constraints |= RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void DeactivateStun()
    {
        stunned = false;
    }

    /// <summary>
    /// Reset all applied Movement states on the Player to their initial state.
    /// </summary>
    public void Reset()
    {
        // Reset Player Velocity:
        ResetPlayerVelocity();

        // Deactivate any Player Stun:
        DeactivateStun();

        // Unlock Player Velocity:
        UnlockPlayerPosition();

        // Enable Player Control:
        setControlsDisable(false, true);
    }

    public void ResetPlayerVelocity()
    {
        // Reset Player Velocity:
        entityRigidbody.velocity = Vector2.zero;
    }

    /** <summary>
       Stops any current player dash and resets it.
       </summary>
    */
    public void dashReset()
    {
        isDashing = false;
        dashDirection = Vector2.zero;
        dashTimer = currentDash.startDashTime;
        setControlsDisable(false);
        dashResetEvent.Invoke();
    }

    /** <summary>
       Adds a time of startDashTime to the dashTimer to extend current player dash.
       </summary>
    */
    public void extendDashTime()
    {
        dashTimer += currentDash.startDashTime;
    }

    /** <summary>
       Adds a time of time to the dashTimer to extend current player dash.
       </summary>
    */
    public void extendDashTime(float time)
    {
        dashTimer += time;
    }

    public void triggerResetDashWhenCurrentDashComplete()
    {
        resetDashWhenCurrentDashComplete = true;
    }

    public bool isFalling()
    {
        return !isGrounded() && entityRigidbody.velocity.y <= 0;
    }

    public bool isJumping()
    {
        return !isGrounded() && entityRigidbody.velocity.y > 0;
    }

    public bool isWallSliding()
    {
        RaycastHit2D wallHitRight = Physics2D.BoxCast(entityCollider.bounds.center, entityCollider.bounds.size, 0, Vector2.right, wallDetectionDistance, wallTouchLayers.layerMask);
        RaycastHit2D wallHitLeft = Physics2D.BoxCast(entityCollider.bounds.center, entityCollider.bounds.size, 0, (-1) * Vector2.right, wallDetectionDistance, wallTouchLayers.layerMask);
        
        return wallHitRight.collider != null || wallHitLeft.collider != null;
    }

    public bool isWallJumping()
    {
        return wallJumpTimer < wallJumpDuration && wallJumpTimer >= 0;
    }

    // Distance = Velocity * Time
    private Vector2 potentialDashDisplacement()
    {
        return (entityRigidbody.velocity - (currentDash.dashSpeedMultiplier * dashDirection)) * -currentDash.startDashTime;
    }

    private Vector2 potentialDashDisplacement(Vector2 aim)
    {
        return (entityRigidbody.velocity - (currentDash.dashSpeedMultiplier * aim)) * -currentDash.startDashTime;
    }

    public override void setVariableSpeed(float newSpeed)
    {
        float calculatedSpeed = toggledDefaultSpeed + newSpeed;

        if (calculatedSpeed > maxSpeed)
        {
            variableSpeed = maxSpeed - speed;
        }
        else if (calculatedSpeed <= minSpeed)
        {
            variableSpeed = minSpeed - crouchSpeed;
        }
        else
        {
            variableSpeed = newSpeed;
        }
    }

    public override void moveX(float xDirection)
    {
        entityRigidbody.velocity = new Vector2(xDirection * Time.deltaTime* currentSpeed * 100, entityRigidbody.velocity.y);
    }

    public override void jump(float appliedJumpForce)
    {
        Vector2 movment = new Vector2(entityRigidbody.velocity.x, appliedJumpForce);
        entityRigidbody.velocity = movment;
    }

    private void OnDrawGizmos()
    {
        // Debug Player Wall Detection:
        if (wallDetectionDebugEnabled)
        {
            DebugWallDetection();
        }
    }

    private void DebugWallDetection()
    {
        if (entityCollider == null) return;

        Gizmos.color = Color.cyan;

        Vector3 sizeXVector = new Vector3(entityCollider.bounds.size.x, 0f, 0f) * lookDirection;
        Vector3 sizeYVector = new Vector3(0f, entityCollider.bounds.size.y, 0f);
        Vector3 wallCheckDistanceVector = Vector2.right * lookDirection * wallDetectionDistance;

        Vector3 origin = entityCollider.bounds.center - wallCheckDistanceVector - sizeXVector / 2f;

        Vector3 end = origin + sizeXVector + 2 * wallCheckDistanceVector;

        Gizmos.DrawLine(origin + sizeYVector / 2, end + sizeYVector / 2);
        Gizmos.DrawLine(origin - sizeYVector / 2, end - sizeYVector / 2);
    }
}
