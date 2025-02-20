using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingTilePosition : MonoBehaviour
{
    public static ChangingTilePosition Instance { get; private set; }
    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void ChangingPosition()
    {
        transform.position = new Vector3(4, 0, 2);
    }

}