using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isSelected;

    void Awake()
    {
        isSelected = false;
    }

    void OnMouseDown()
    {
        if (!isSelected)
        {
            Debug.Log("Clicked");
            isSelected = true;
            transform.rotation = Quaternion.Euler(0,0,180);
        }
    }
}
