using UnityEngine;

/// <summary>
/// Game progression stages - represents the current state of sword behavior
/// </summary>
public enum GameStage
{
    WaitingForFirstTap,     // Ready to start - waiting for initial input
    Stage1InProgress,       // Currently executing jump
    WaitingForSecondTap,    // Between stages - waiting for charge input (classic mode only)
    Stage2InProgress,       // Currently charging and rotating
    StabAttack,             // Executing final stab attack
    Reset                   // Resetting back to initial state
}

public static class PhysicsUtils
{
    public static void ResetPhysics(Rigidbody rb)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


    public static void ReduceRigidbodyMass(Rigidbody rb)
    {
        rb.mass = 0.01f;
        rb.linearDamping = 10f;
    }

    public static void RecoverRigidbodyMass(Rigidbody rb)
    {
        rb.mass = 1f;
        rb.linearDamping = 2f;
    }
}

public static class TransformUtils
{
    public static void ResetRotation(Transform transform)
    {
        transform.eulerAngles = new Vector3(0f, 90f, 0f);
    }
}

public static class DebugUtils
{
    /// <summary>
    /// Debug helper - logs current stage to console
    /// </summary>
    public static void LogCurrentStage(GameStage currentStage, bool enableTestingAndDebug)
    {
        if (enableTestingAndDebug)
            Debug.Log($"Current Stage: {currentStage}");
    }

}