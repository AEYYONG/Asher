using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersAnim : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void AinmDone()
    {
        anim.SetBool("Trun", false);
    }
}
