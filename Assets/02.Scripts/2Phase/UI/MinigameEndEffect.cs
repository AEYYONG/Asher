using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameEndEffect : MonoBehaviour
{
    [SerializeField] private StealGauge stealGauge;
    public void EndEvent()
    {
        stealGauge.GameWin();
    }
}
