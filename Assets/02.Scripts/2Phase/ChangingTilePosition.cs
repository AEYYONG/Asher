using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingTilePosition : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ChangingPosition());


    }
    private IEnumerator ChangingPosition()
    {
        yield return new WaitForSeconds(0.5f);
        transform.position = new Vector3(4, 0, 2);

    }
}