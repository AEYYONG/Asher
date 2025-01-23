using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2Manager : MonoBehaviour
{
    public GameObject Furnitures;
    public GameObject Characters;


    void Start()
    {
        Furnitures.SetActive(false);
        Characters.SetActive(false);
    }

    void Update()
    {
        
    }
}
