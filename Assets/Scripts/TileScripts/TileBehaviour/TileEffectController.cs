using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEffectController : MonoBehaviour
{
    public void ChangeTileTexEmpty()
    {
        transform.GetComponentInParent<Tile>().ChangeTileTexEmpty();
    }
}
