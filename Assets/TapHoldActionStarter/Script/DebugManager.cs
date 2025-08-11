using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
    [Header("TESTING KEYS")]
    [SerializeField] private Key testStage1OnlyKey = Key.Q;
    [SerializeField] private Key testStage2OnlyKey = Key.E;
    [SerializeField] private Key resetKey = Key.A;

    [Header("DEBUG")]
    [SerializeField] private bool enableTestingAndDebug = false;

    private Keyboard keyboard;
    private SwordController swordController;

    // Testing state
    private bool isStage2TestingActive = false;
    private Coroutine stage2TestCoroutine = null;

    void Start()
    {
        keyboard = Keyboard.current;
        swordController = GetComponent<SwordController>();
    }

    void Update()
    {
        HandleTestingInput();
    }

    void HandleTestingInput()
    {
        if (keyboard == null || swordController == null) return;

        if (keyboard[resetKey].wasPressedThisFrame)
        {
            ForceReset();
        }

        if (keyboard[testStage1OnlyKey].wasPressedThisFrame)
        {
            TestStage1Only();
        }

        if (keyboard[testStage2OnlyKey].wasPressedThisFrame)
        {
            TestStage2Only();
        }
        else if (keyboard[testStage2OnlyKey].wasReleasedThisFrame)
        {
            StopStage2Testing();
        }
    }

    public void ForceReset()
    {
        StopAllCoroutines();
        PhysicsUtils.ResetPhysics(swordController.rb);
        swordController.ResetToInitialStage();
        if (enableTestingAndDebug)
            Debug.Log("FORCE RESET: All states cleared");
    }

    public void TestStage1Only()
    {
        ForceReset();
        swordController.StartStage1();
        if (enableTestingAndDebug)
            Debug.Log("TESTING: Stage 1 only executed");
    }

    public void TestStage2Only()
    {
        StopAllCoroutines();
        PhysicsUtils.ResetPhysics(swordController.rb);
        isStage2TestingActive = true;
        stage2TestCoroutine = StartCoroutine(ExecuteIndependentStage2Test());
        if (enableTestingAndDebug)
            Debug.Log("TESTING: Stage 2 independent test started");
    }

    void StopStage2Testing()
    {
        if (stage2TestCoroutine != null)
        {
            StopCoroutine(stage2TestCoroutine);
            stage2TestCoroutine = null;
        }
        isStage2TestingActive = false;
        Debug.Log("STAGE 2 TEST: Testing stopped");
    }

    IEnumerator ExecuteIndependentStage2Test()
    {
        Vector3 testStabForce = Vector3.zero;
        bool testIsCharging = true;
        float testChargeTimer = 0f;

        while (testIsCharging && testChargeTimer < 1.2f)
        {
            float delta = Time.deltaTime;
            testChargeTimer += delta;

            float addedForce = 10f * delta;
            float currentMagnitude = Mathf.Abs(testStabForce.y) + addedForce;
            testStabForce.y = -1f * Mathf.Clamp(currentMagnitude, 0f, 50f);

            swordController.transform.Rotate(720f * delta, 0, 0);

            yield return null;
        }

        swordController.rb.AddRelativeForce(testStabForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);
        StopStage2Testing();
    }

    void OnGUI()
    {
        if (!enableTestingAndDebug) return;

        GUI.color = Color.white;
        GUI.skin.label.fontSize = 24;

        int y = 10;
        void DrawLabel(string text, Color color)
        {
            GUI.color = color;
            GUI.Label(new Rect(10, y, 500, 32), text);
            y += 37;
        }

        if (isStage2TestingActive)
        {
            DrawLabel("STAGE 2 TESTING ACTIVE", Color.red);
        }
        else
        {
            var stateManager = GetComponent<StateManager>();
            DrawLabel($"Stage: {stateManager.CurrentStage}", Color.white);
            DrawLabel($"Mode: {(swordController.useContinuousMode ? "Continuous" : "Classic")}", Color.white);
        }

        DrawLabel($"Test Controls: Q-Stage1 | E-Stage2 | A-Reset", Color.yellow);
    }

    public bool IsStage2TestingActive => isStage2TestingActive;
}
