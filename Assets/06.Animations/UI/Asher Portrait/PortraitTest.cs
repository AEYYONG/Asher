using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitTest : MonoBehaviour
{
    [SerializeField] private Animator _asher;

    public void SetGood()
    {
        _asher.SetTrigger("Good");
    }
    public void SetBad()
    {
        _asher.SetTrigger("Bad");
    }
    public void SetFever()
    {
        _asher.SetTrigger("Fever");
    }
    public void SetGlass()
    {
        _asher.SetTrigger("Glass");
    }
    public void SetTimeBomb()
    {
        _asher.SetTrigger("TimeBomb");
    }
}
