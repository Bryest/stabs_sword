using System;
using Unity.VisualScripting;
using UnityEngine;

public class Stand : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        SwordController sword = collision?.gameObject.GetComponentInParent<SwordController>();
        if (sword == null) return;

        if (!sword.FirstCollision && sword.CurrentStand == null)
        {
            sword.FirstCollision = true;
            if (collision.gameObject.CompareTag("Sword_Stand"))
            {

                Debug.Log("Tag: " + collision.gameObject.tag);
                Debug.Log("Tag: " + collision.gameObject.name);

                sword.ResetToInitialStage();
                PhysicsUtils.ResetPhysics(sword.rb);
                sword.StopAllCoroutines();

                sword.CurrentStand = gameObject.GetComponent<Transform>().gameObject;
                sword.SwordPivotStand = sword.CurrentStand.transform.GetChild(0).gameObject;
            }
        }
        else if (sword.FirstCollision && sword.CurrentStand != null)
        {
            sword.ResetToInitialStage();
        }


        // Catching the next Stand and validating the progression
        if (sword.CurrentStand != this.gameObject)
        {
            if (StandProgression.instance.ValidateProgression(sword.CurrentStand, this.gameObject))
            {
                sword.CurrentStand = this.gameObject;
                sword.SwordPivotStand = sword.CurrentStand.transform.GetChild(0).gameObject;
            }
        }
    }
}


