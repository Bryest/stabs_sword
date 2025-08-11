using UnityEngine;

public abstract class BaseDebugComponent : MonoBehaviour
{
    [SerializeField] protected bool enableTestingAndDebug = false;

    protected void LogDebugMessage(string message)
    {
        if (enableTestingAndDebug)
            Debug.Log(message);
    }

    protected void LogCurrentStage(GameStage gameStage)
    {
        if (enableTestingAndDebug)
            Debug.Log($"Current stage: ${gameStage}");
    }
}
