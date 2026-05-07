using UnityEngine;

public class TestMovement : MonoBehaviour
{
    private Stats stats;
    private Vector2 moveInput;

    public Rigidbody2D rb;
    UnityEngine.Vector2 facing = UnityEngine.Vector2.right;

    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    public float moveSpeed = 5f;

    float dashDurationTimer;
    float dashCooldownTimer;
    bool isDashing;
    public float baseMoveSpeed = 5f;

    private UnityEngine.Vector2 endPos;

    // [SerializeField] private MovementSO[] movementArray;
    public MovementSO slideMovementSO;
    public MovementSO chargeMovementSO;
    public MovementSO slideDashMovementSO;

    public MovementStateMachine movementStateMachine;

    public ParticleSystem failureParticle;
    public ParticleSystem dashParticle;
    public ParticleSystem slideDashParticle;
    public ParticleSystem slideParticle;
    public ParticleSystem chargeParticle;
    public ParticleSystem chargeDecayParticle;

    [Header("Audio")]
    [SerializeField] private SoundDefinition slideSound;
    [SerializeField] private SoundDefinition dashSound;
    [SerializeField] private SoundDefinition chargeSound;

    private bool IsSliding => HasMovementState(MovementStateMachine.State.slide);
    private bool IsSlideDecaying => HasMovementState(MovementStateMachine.State.slideDecay);
    private bool IsSlidingOrDecaying => IsSliding || IsSlideDecaying;
    private bool IsSlideDashing => HasMovementState(MovementStateMachine.State.slideDash);
    private bool IsSlideDashDecaying => HasMovementState(MovementStateMachine.State.slideDashDecay);
    private bool IsInSlideDashState => IsSlideDashing || IsSlideDashDecaying;
    private bool IsCharging => HasMovementState(MovementStateMachine.State.charge);
    private bool IsChargeDecaying => HasMovementState(MovementStateMachine.State.chargeDecay);
    private bool IsInChargeState => IsCharging || IsChargeDecaying;

    private bool HasMovementState(MovementStateMachine.State state)
    {
        return movementStateMachine.HasState(state);
    }

    private void Update()
    {
        if (isDashing)
        {
            dashDurationTimer -= Time.deltaTime;
            if (dashDurationTimer <= 0f)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown;
            }
        }
        else if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void Awake()
    {
        stats = GetComponent<Stats>();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = Vector2.ClampMagnitude(input, 1f);

        if (moveInput != Vector2.zero)
        {
            facing = moveInput.normalized;
        }
    }

    public void TryDash()
    {
        if (IsSliding && !IsInSlideDashState)
        {
            Debug.Log("Slide Dash Initiated");
            movementStateMachine.AddComboState(slideDashMovementSO, MovementStateMachine.State.slide, MovementStateMachine.State.dash);
            SoundManager.Play(dashSound);
            return;
        }
        else
        {
            MoveFail();
        }

        if (IsDashing || IsDashDecaying || IsInSlideDashState)
        {
            MoveFail();
            return;
        }


        isDashing = true;
        dashDurationTimer = dashDuration;
        SoundManager.Play(dashSound);
    }

    public void TrySlide()
    {
        if (IsSlidingOrDecaying)
        {
            MoveFail();
            return;
        }

        movementStateMachine.AddState(slideMovementSO);
        SoundManager.Play(slideSound);
    }

    public void TryCharge()
    {
        if (IsSlidingOrDecaying || IsInChargeState)
        {
            MoveFail();
            return;
        }

        movementStateMachine.AddState(chargeMovementSO);
        SoundManager.Play(chargeSound);
    }
    //__________________________________________________________________________________________________
    void FixedUpdate()
    {
        UnityEngine.Vector2 endPos = new UnityEngine.Vector2(0, 0);
        float currentMoveSpeed = (stats != null) ? stats.GetSpeed(baseMoveSpeed) : moveSpeed;
        endPos += rb.position;
        // rb.MovePosition(rb.position + (moveInput * moveSpeed) * Time.fixedDeltaTime);
        endPos += moveInput * (currentMoveSpeed * Time.fixedDeltaTime);

        if (isDashing)
        {
            // rb.MovePosition(rb.position + facing * dashSpeed * Time.fixedDeltaTime);
            endPos += Dash();
        }
        if (IsSlideDashing)
        {
            endPos += SlideDash();
        }

        if (IsSliding)
        {
            transform.localScale = new Vector3(.25f, .25f, .25f);
            endPos += Slide();
        }
        if (IsSlideDecaying)
        {
            endPos += SlideDecay();
        }
        if (IsCharging && !IsSlidingOrDecaying)
        {
            endPos += Charge();
        }
        if (IsChargeDecaying && !IsSlidingOrDecaying)
        {
            endPos += ChargeDecay();
        }
        
        
        // print(endPos);
        rb.MovePosition(endPos);
    }

    private Vector2 Dash()
    {
        // failureParticle.startColor = Color.blue;
        failureParticle.Stop();
        dashParticle.transform.right = facing;
        dashParticle.Play();
        
        Debug.Log("DashAmount");
        Debug.Log(dashSpeed * (slideDashMovementSO.agilityScale * Time.fixedDeltaTime));
        return facing * (dashSpeed + slideDashMovementSO.agilityScale) * Time.fixedDeltaTime;
    }

    private Vector2 Slide()
    {
        //Shrink the Player
        // Debug.Log("In Slide");
        failureParticle.Stop();
        slideParticle.Play();
        transform.localScale = new Vector3(.25f, .25f, .25f);
        // rb.MovePosition(rb.position + facing*slideMovementSO.movePower*Time.fixedDeltaTime);
        return facing.normalized * (slideMovementSO.movePower * (slideMovementSO.agilityScale) * Time.fixedDeltaTime);
    }

    private Vector2 SlideDecay()
    {
        //Unshrink the player
        // Debug.Log("In Slide Decay");
        transform.localScale = new Vector3(.5f, .5f, .5f);
        rb.MovePosition(rb.position + facing * (slideMovementSO.movePower / 2 * Time.fixedDeltaTime));
        return facing * (-slideMovementSO.movePower / 4 * Time.fixedDeltaTime);
    }

    private Vector2 Charge()
    {
        //Bulk the Player
        transform.localScale = new UnityEngine.Vector3(.75f, .75f, .75f);

        //Just for testing play the failure particle
        failureParticle.Stop();
        chargeParticle.Play();
        
        rb.MovePosition(rb.position + facing * chargeMovementSO.movePower / 2 * Time.fixedDeltaTime);
        //Moves you backwards a bit which can be used to do chargeswitch tech! EEEE!
        return facing * (-slideMovementSO.movePower / 1.5f * Time.fixedDeltaTime);
    }

    private Vector2 ChargeDecay()
    {
        //Shrink the player
        failureParticle.Stop();
        chargeParticle.Stop();
        chargeDecayParticle.Play();

        //Make the player a bit green for a bit.

        transform.localScale = new UnityEngine.Vector3(.5f,.5f,.5f);
        // Debug.Log("In Slide Decay");
        // rb.MovePosition(rb.position + facing*(slideMovementSO.movePower)*Time.fixedDeltaTime);
        return facing.normalized * (slideMovementSO.movePower * (chargeMovementSO.strengthScale) * Time.fixedDeltaTime);
    }

    private Vector2 SlideDash()
    {
        // failureParticle.startColor = Color.purple;
        // failureParticle.Play();
        failureParticle.Stop();

        slideDashParticle.transform.right = facing;
        slideDashParticle.Play();

        Debug.Log("In Slide Dash");
        return facing.normalized * (slideDashMovementSO.movePower * (slideDashMovementSO.agilityScale) * Time.fixedDeltaTime);
    }

    // Public now so we can see when abilities fail outside class
    public void MoveFail()
    {
        //Add a bad sound in here
        failureParticle.Play();
    }
    //__________________________________________________________________________________________________

    void OnCollisionStay2D(Collision2D collision)
    {
        if (movementStateMachine.HasState(MovementStateMachine.State.chargeDecay))
        {
            int damage = (stats != null) ? stats.GetDamage(1) : 1;
            float pierce = stats != null ? stats.GetPierce() : 0f;
            if (collision.gameObject != null && collision.gameObject.CompareTag("Enemy"))
            {
                Enemy player = collision.gameObject.GetComponent<Enemy>();
                if (player != null)
                {
                    player.TakeDamage(damage, pierce);
                    Debug.Log("Charging Into Enemy");
                }
            }
        }
    }

    // New stuff
    public MovementSO dashMovementSO;

    private bool IsDashing => HasMovementState(MovementStateMachine.State.dash);
    private bool IsDashDecaying => HasMovementState(MovementStateMachine.State.dashDecay);
    private bool IsGrappleWhipping => HasMovementState(MovementStateMachine.State.grappleWhipping);

    public Vector2 GetFacing() => facing;
    public Vector2 GetMoveInput() => moveInput;
}
