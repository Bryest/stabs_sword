using System.Collections;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    [Header("FORCE ACCUMULATION SYSTEM")]
    [SerializeField] private float maxChargeForce = 50f;
    [SerializeField] private float chargeRate = 10f;

    [Header("STAGE 1: INITIAL JUMP PHYSICS")]
    [SerializeField] private float jumpVerticalForce = 12f;
    [SerializeField] private float jumpHorizontalForce = 8f;
    [SerializeField] private float jumpDuration = 0.8f;

    [Header("STAGE 2: ROTATION & CHARGE PHYSICS")]
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float chargeDuration = 1.2f;
    [SerializeField] private float stabDirection = -1f;

    [Header("DEBUG")]
    [SerializeField] private bool enableTestingAndDebug = false;

    // Events
    public event System.Action<GameStage> OnStageComplete;

    // Physics state
    public bool IsCoroutineRunning { get; private set; } = false;
    public bool IsCharging { get; private set; } = false;
    public Vector3 StabForce { get; private set; } = Vector3.zero;
    public float ChargeTimer { get; private set; } = 0f;
    public float CurrentRotationSpeed { get; private set; } = 0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ExecuteStage1Jump()
    {
        if (!IsCoroutineRunning)
            StartCoroutine(Stage1JumpCoroutine());
    }

    public void ExecuteStage2Charge()
    {
        if (!IsCoroutineRunning)
            StartCoroutine(Stage2ChargeCoroutine());
    }

    public void StopCharging()
    {
        IsCharging = false;
        if (enableTestingAndDebug)
            Debug.Log($"Input released! Stopping charge and attacking with accumulated force: {StabForce.y}");
    }

    public void ResetPhysicsState()
    {
        IsCoroutineRunning = false;
        CurrentRotationSpeed = 0f;
        IsCharging = false;
        ChargeTimer = 0f;
        StabForce = Vector3.zero;
        PhysicsUtils.ResetPhysics(rb);
    }

    IEnumerator Stage1JumpCoroutine()
    {
        IsCoroutineRunning = true;

        Vector3 jumpForce = new Vector3(jumpHorizontalForce, jumpVerticalForce, 0f);
        rb.AddForce(jumpForce, ForceMode.Impulse);

        if (enableTestingAndDebug)
            Debug.Log($"Applied jump force: {jumpForce}");

        yield return new WaitForSeconds(jumpDuration);

        IsCoroutineRunning = false;
        OnStageComplete?.Invoke(GameStage.Stage1InProgress);
    }

    IEnumerator Stage2ChargeCoroutine()
    {
        IsCoroutineRunning = true;
        PhysicsUtils.ReduceRigidbodyMass(rb);
        CurrentRotationSpeed = rotationSpeed;
        IsCharging = true;
        ChargeTimer = 0f;
        StabForce = Vector3.zero;

        if (enableTestingAndDebug)
            Debug.Log("Charging... RELEASE INPUT to attack with accumulated force");

        while (IsCharging && ChargeTimer < chargeDuration)
        {
            float delta = Time.deltaTime;
            ChargeTimer += delta;

            float addedForce = chargeRate * delta;
            float currentMagnitude = Mathf.Abs(StabForce.y) + addedForce;
            StabForce = new Vector3(0, stabDirection * Mathf.Clamp(currentMagnitude, 0f, maxChargeForce), 0);

            float rotationThisFrame = CurrentRotationSpeed * delta;
            transform.Rotate(rotationThisFrame, 0, 0);

            yield return null;
        }

        if (IsCharging)
        {
            IsCharging = false;
            if (enableTestingAndDebug)
                Debug.Log("Max charge duration reached! Auto-attacking with maximum force.");
        }

        ExecuteStabAttack();
        yield return new WaitForSeconds(0.5f);

        IsCoroutineRunning = false;
        OnStageComplete?.Invoke(GameStage.Stage2InProgress);
    }

    void ExecuteStabAttack()
    {
        PhysicsUtils.RecoverRigidbodyMass(rb);
        CurrentRotationSpeed = 0f;

        Debug.Log($"stabForce applied: {StabForce}");
        rb.AddRelativeForce(StabForce, ForceMode.Impulse);

        if (enableTestingAndDebug)
            Debug.Log($"STAB ATTACK executed with accumulated force: {StabForce}");
    }
}
