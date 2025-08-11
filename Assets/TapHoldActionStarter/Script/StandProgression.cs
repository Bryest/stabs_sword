using System.Collections.Generic;
using UnityEngine;

public class StandProgression : MonoBehaviour
{
    public static StandProgression instance { get; private set; }
    [SerializeField] private List<GameObject> standList;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private List<GameObject> StandList
    {
        get { return standList; }
    }

    public bool ValidateProgression(GameObject currentStand, GameObject nextStand)
    {
        int current = standList.IndexOf(currentStand);
        int next = standList.IndexOf(nextStand);

        if (current == -1 || next == -1)
        {
            Debug.LogWarning("One or both stands are not in the list");
            return false;
        }
        return next > current;
    }
}
