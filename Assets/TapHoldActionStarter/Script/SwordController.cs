using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    [Header("Stand Settings")]
    [SerializeField] private GameObject currentStand;
    [SerializeField] private GameObject nextStand;
    [SerializeField] private GameObject swordPivotStand;
    [SerializeField] private bool firstCollision = false;

    [Header("Gameplay Mode")]
    [SerializeField] public bool useContinuousMode = true;

    // Component References
    private InputHandler inputHandler;
    private PhysicsEngine physicsEngine;
    private StateManager stateManager;
    private DebugManager debugManager;

    // Public Properties
    public GameObject CurrentStand { get => currentStand; set => currentStand = value; }
    public GameObject NextStand { get => nextStand; set => nextStand = value; }
    public GameObject SwordPivotStand { get => swordPivotStand; set => swordPivotStand = value; }
    public bool FirstCollision { get => firstCollision; set => firstCollision = value; }
    public Rigidbody rb { get; private set; }

    void Awake()
    {
        // Get component references
        inputHandler = GetComponent<InputHandler>();
        physicsEngine = GetComponent<PhysicsEngine>();
        stateManager = GetComponent<StateManager>();
        debugManager = GetComponent<DebugManager>();
        rb = GetComponent<Rigidbody>();

        // Subscribe to events
        if (inputHandler != null)
        {
            inputHandler.OnInputPressed += HandleInputPressed;
            inputHandler.OnInputReleased += HandleInputReleased;
        }

        if (physicsEngine != null)
        {
            physicsEngine.OnStageComplete += HandleStageComplete;
        }

        if (stateManager != null)
        {
            stateManager.OnStateChanged += HandleStateChanged;
        }
    }

    void Start()
    {
        Debug.Log("SwordController initialized - Ready for input");
        stateManager?.LogCurrentStage();
    }

    // PUBLIC API
    public void StartStage1()
    {
        if (stateManager.CanStartStage1())
        {
            stateManager.SetState(GameStage.Stage1InProgress);
            physicsEngine.ExecuteStage1Jump();
        }
    }

    public void StartStage2()
    {
        if (stateManager.CanStartStage2())
        {
            stateManager.SetState(GameStage.Stage2InProgress);
            physicsEngine.ExecuteStage2Charge();
        }
    }

    public void StartContinuousSequence()
    {
        Debug.Log("CONTINUOUS MODE: Starting Stage 1, will auto-transition to Stage 2");
        StartStage1();
    }

    public void StopChargingAndAttack()
    {
        physicsEngine.StopCharging();
    }

    public void ResetToInitialStage()
    {
        stateManager.ResetAllStates();
        physicsEngine.ResetPhysicsState();
        Debug.Log("RESET: Ready for new cycle - Waiting for input");
    }

    // EVENT HANDLERS
    void HandleInputPressed()
    {
        switch (stateManager.CurrentStage)
        {
            case GameStage.WaitingForFirstTap:
                StartStage1();
                break;
            case GameStage.WaitingForSecondTap:
                StartStage2();
                break;
        }
    }

    void HandleInputReleased()
    {
        if (stateManager.CurrentStage == GameStage.Stage2InProgress && physicsEngine.IsCharging)
        {
            StopChargingAndAttack();
        }
    }

    void HandleStageComplete(GameStage completedStage)
    {
        switch (completedStage)
        {
            case GameStage.Stage1InProgress:
                if (useContinuousMode && inputHandler.IsInputHeld)
                {
                    Debug.Log("STAGE 1 Complete - Auto-transitioning to Stage 2 (input still held)");
                    stateManager.SetState(GameStage.WaitingForSecondTap);
                    StartStage2();
                }
                else
                {
                    stateManager.SetState(GameStage.WaitingForSecondTap);
                    Debug.Log("STAGE 1 Complete - Waiting for second input");
                }
                break;

            case GameStage.Stage2InProgress:
                ResetToInitialStage();
                break;
        }
    }

    void HandleStateChanged(GameStage newState)
    {
        Debug.Log($"State changed to: {newState}");
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (inputHandler != null)
        {
            inputHandler.OnInputPressed -= HandleInputPressed;
            inputHandler.OnInputReleased -= HandleInputReleased;
        }

        if (physicsEngine != null)
        {
            physicsEngine.OnStageComplete -= HandleStageComplete;
        }

        if (stateManager != null)
        {
            stateManager.OnStateChanged -= HandleStateChanged;
        }
    }
}
