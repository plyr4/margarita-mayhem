using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnStart : MonoBehaviour
{
    public List<GameObject> _objects;

    void Start()
    {
        foreach (var obj in _objects)
        {
            obj.SetActive(true);
        }
    }
}