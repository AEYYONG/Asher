using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameResultController : MonoBehaviour
{
    public StageInfoSO stageSO;
    public TextMeshProUGUI scoreUI;
    public GameObject lobbyToggle;
    public GameObject retryToggle;

    public void Start()
    {
        scoreUI.text = $"{stageSO.score}";
    }

    public void ClickLobbyBtn()
    {
        lobbyToggle.SetActive(true);   
        retryToggle.SetActive(false);
        MySceneManager.Instance.ChangeScene("Lobby");
    }

    public void ClickRetryBtn()
    {
        retryToggle.SetActive(true);
        lobbyToggle.SetActive(false);
        MySceneManager.Instance.ChangeScene("StageNoaP");
        
    }

    public void InitBtn()
    {
        retryToggle.SetActive(false);
        lobbyToggle.SetActive(false);
    }

    public void HoverLobbyBtn()
    {
        retryToggle.SetActive(false);
        lobbyToggle.SetActive(true);
    }

    public void HoverRetryBtn()
    {
        retryToggle.SetActive(true);
        lobbyToggle.SetActive(false);
    }
}
