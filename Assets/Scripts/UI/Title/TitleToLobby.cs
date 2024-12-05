using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleToLobby : MonoBehaviour
{

    private bool isSceneChanging = false;


    void Update()
    {
        // 아무 키나 입력하거나 마우스 클릭/터치하면 씬 전환
        if (!isSceneChanging && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            StartCoroutine(LoadNextScene(1f));
        }
    }

    IEnumerator LoadNextScene(float delay)
    {
        isSceneChanging = true;
        yield return null;
        MySceneManager.Instance.ChangeScene("Lobby");
    }
}
