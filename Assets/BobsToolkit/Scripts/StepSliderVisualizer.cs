using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSliderVisualizer : MonoBehaviour
{
    public void SetValue(int value)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            //Debug.Log("Set child " + i);
            child.SetActive(value >= i + 1);
        }
    }

    public void SetMax(int value)
    {
        int difference = value - transform.childCount;
        Debug.Log("Difference: " + difference);
        if (difference == 0)
        {
            return;
        }

        GameObject visual = GetVisual();
        if (visual == null)
        {
            Debug.LogError("Visualizer has no children or prefab to get its visual from. Either give it a child, or assign a prefab.", this);
        }

        if (difference > 0)
        {
            Debug.Log("Add children");
            // Er moeten children bij
            for (int i = 0; i < difference; i++)
            {
                Instantiate(visual, transform);
            }

            return;
        }

        if (difference < 0)
        {
            // Er moeten children af
            Debug.Log("Remove children");
            difference *= -1;

            for (int i = 0; i < difference; i++)
            {
                DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
            }
            return;
        }
            

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Debug.Log("Set child " + i);
            child.SetActive(value >= i + 1);
        }
    }

    public GameObject visualPrefab;
    public GameObject GetVisual()
    {
        Debug.Log("Get visual");
        if (visualPrefab != null) return visualPrefab;

        if (transform.childCount == 0)
            return null;

        return transform.GetChild(0).gameObject;
    }
}
