using UnityEngine;
using UnityEngine.Rendering;

public class Floor : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        SwordController sword = collision?.gameObject.GetComponent<SwordController>();
        if (sword == null) return;

        if (collision.gameObject.CompareTag("Sword_Stand"))
        {
            sword.StopAllCoroutines();
            PhysicsUtils.ResetPhysics(sword.rb);
            TransformUtils.ResetRotation(sword.transform);
            // sword.ResetAllStates();
            sword.FirstCollision = false;
            PhysicsUtils.RecoverRigidbodyMass(sword.rb);
            sword.transform.position = sword.SwordPivotStand.transform.position;
        }
    }

}