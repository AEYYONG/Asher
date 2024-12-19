using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnclickX : MonoBehaviour
{
    public ButtonsController buttonController;

    public void OnClickX()
    {
        transform.parent.gameObject.SetActive(false);

        buttonController.ComeBack();
    }

}
