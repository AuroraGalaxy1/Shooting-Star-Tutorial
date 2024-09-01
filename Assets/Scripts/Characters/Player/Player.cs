using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [SerializeField] StatsBar_HUD statsBar_HUD;
    [SerializeField] bool regenerateHealth = true;
    [SerializeField] float healthRegenerateTime;
    [SerializeField, Range(0f, 1f)] float healthRegeneratePercent;
    [Header("---- INPUT ----")]
    [SerializeField] PlayerInput input;
    [Header("---- MOVE ----")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float accelerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;
    [SerializeField] float moveRotationAngle = 50f;
    WaitForSeconds waitBeamCooldownTime;
    [Header("---- FIRE ----")]
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;
    [SerializeField] GameObject projectileOverdrive;
    [SerializeField] ParticleSystem muzzleVFX;
    [SerializeField] Transform muzzleMiddle;
    [SerializeField] Transform muzzleTop;
    [SerializeField] Transform muzzleBottom;
    [SerializeField] AudioData projectileLaunchSFX;
    [SerializeField, Range(0, 2)] int weaponPower = 0;
    [SerializeField] float fireInterval = 0.2f;
    [Header("---- DODGE ----")]
    [SerializeField] AudioData dodgeSFX;
    [SerializeField, Range(0, 100)] int dodgeEnergyCost = 25;
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("---- OVERDRIVE ----")]
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;
    [SerializeField] float overdriveFireFactor = 1.2f;

    MissileSystem missile;
    readonly float slowMotionDuration = 1f;
    readonly float InvincibleTime = 1f;
    bool isDodging = false;
    bool isOverdriving = false;
    float dodgeDuration;
    float currentRoll;
    float paddingX;
    float paddingY;
    float t;
    new Rigidbody2D rigidbody;
    new Collider2D collider;
    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitForOverdriveFireInterval;
    WaitForSeconds waitHealthRegenerateTime;
    WaitForSeconds waitDecelerationTime;
    WaitForSeconds waitInvincibleTime;
    Coroutine moveCoroutine;
    Coroutine healthRegenerateCoroutine;

    Vector2 moveDirection;
    Vector2 previousVelocity;
    Quaternion previousRotation;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    protected override void OnEnable()
    {
        base.OnEnable();
        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;
        input.onLaunchMissile += LaunchMissile;
        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;
    }

    void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverdrive -= Overdrive;
        input.onLaunchMissile -= LaunchMissile;
        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdriveOff;
    }
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
        //dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;
        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriveFireFactor);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitDecelerationTime = new WaitForSeconds(decelerationTime);
        waitInvincibleTime = new WaitForSeconds(InvincibleTime);
    }
    void Start()
    {
        statsBar_HUD.Initialize(health, maxHealth);
        input.EnableGameplayInput();
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        PowerDown();
        statsBar_HUD.UpdateStats(health, maxHealth);
        TimeController.Instance.BulletTime(slowMotionDuration);
        if (gameObject.activeSelf)
        {
            Move(moveDirection);
            StartCoroutine(InvincibleCoroutine());
            if (regenerateHealth)
            {
                if (healthRegenerateCoroutine != null)
                {
                    StopCoroutine(healthRegenerateCoroutine);
                }
                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }
    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        statsBar_HUD.UpdateStats(health, maxHealth);
    }
    public override void Die()
    {
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        statsBar_HUD.UpdateStats(0f, maxHealth);
        base.Die();
    }

    IEnumerator InvincibleCoroutine()
    {
        collider.isTrigger = true;
        yield return waitInvincibleTime;
        collider.isTrigger = false;

    }


    #region  PROPERTIES

    public bool IsFullHealth => health == maxHealth;
    public bool IsFullPower => weaponPower == 2;

    #endregion

    #region MOVE
    void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveDirection = moveInput.normalized;
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed, Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right)));
        StopCoroutine(nameof(DecelerationCoroutine));
        StartCoroutine(nameof(MoveRangeLimatationCoroutine));
    }

    void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveDirection = Vector2.zero;
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, moveDirection, Quaternion.identity));
        //moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero, Quaternion.identity));
        StartCoroutine(nameof(DecelerationCoroutine));
    }

    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
    {
        t = 0f;
        previousVelocity = rigidbody.velocity;
        previousRotation = transform.rotation;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / time;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t);
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t);

            yield return waitForFixedUpdate;
        }
    }

    IEnumerator MoveRangeLimatationCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position, paddingX, paddingY);

            yield return null;
        }
    }

    IEnumerator DecelerationCoroutine()
    {
        yield return waitDecelerationTime;

        StopCoroutine(nameof(MoveRangeLimatationCoroutine));
    }
    #endregion
    #region FIRE
    void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }
    void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }
    IEnumerator FireCoroutine()
    {
        while (true)
        {
            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleBottom.position);
                    break;
                case 2:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile3, muzzleBottom.position);
                    break;
                default:
                    break;
            }
            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;

        }
    }

    #endregion
    #region Dodge
    void Dodge()
    {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;
        StartCoroutine(nameof(DodgeCoroutine));
    }

    IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);
        PlayerEnergy.Instance.Use(dodgeEnergyCost);
        collider.isTrigger = true;
        currentRoll = 0f;
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
        var scale = transform.localScale;
        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);
            transform.localScale = BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);

            yield return null;
        }
        collider.isTrigger = false;
        isDodging = false;
    }
    #endregion
    #region OverDrive

    void Overdrive()
    {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;

        PlayerOverdrive.on.Invoke();
    }

    void OverdriveOn()
    {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
    }

    void OverdriveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }
    #endregion
    #region MISSILE
    void LaunchMissile()
    {
        missile.Lauch(muzzleMiddle);
    }
    public void PickUpMissile()
    {
        missile.PickUp();
    }

    #endregion

    #region WEAPON POWER
    public void PowerUp()
    {
        weaponPower = Mathf.Min(weaponPower + 1, 2);
    }
    public void PowerDown()
    {
        weaponPower--;
        if (weaponPower <= 0)
        {
            weaponPower = 0;
        }
    }
    #endregion
}
