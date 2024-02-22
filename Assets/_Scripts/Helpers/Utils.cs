using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void DestroyAllChildren(Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
