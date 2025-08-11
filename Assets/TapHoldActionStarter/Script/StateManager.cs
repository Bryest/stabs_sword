using UnityEngine;

public class StateManager : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] private bool enableTestingAndDebug = false;

    // Events
    public event System.Action<GameStage> OnStateChanged;

    // State
    [SerializeField] private GameStage currentStage = GameStage.WaitingForFirstTap;

    public GameStage CurrentStage { get => currentStage; private set => currentStage = value; }

    public void SetState(GameStage newState)
    {
        if (CurrentStage != newState)
        {
            CurrentStage = newState;
            OnStateChanged?.Invoke(newState);
            LogCurrentStage();
        }
    }

    public void ResetAllStates()
    {
        SetState(GameStage.WaitingForFirstTap);
    }

    public bool CanStartStage1()
    {
        return CurrentStage == GameStage.WaitingForFirstTap;
    }

    public bool CanStartStage2()
    {
        return CurrentStage == GameStage.WaitingForSecondTap;
    }

    public void LogCurrentStage()
    {
        if (enableTestingAndDebug)
            Debug.Log($"Current stage: {CurrentStage}");
    }
}
