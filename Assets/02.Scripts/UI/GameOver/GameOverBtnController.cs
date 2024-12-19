using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverBtnController : MonoBehaviour
{
    public List<Sprite> yesBtnList = new List<Sprite>();
    public List<Sprite> noBtnList = new List<Sprite>();

    public Image yesBtn;
    public Image noBtn;

    public void ClickYesBtn()
    {
        yesBtn.sprite = yesBtnList[0];
        noBtn.sprite = noBtnList[1];
        MySceneManager.Instance.ChangeScene("StageNoaP");
    }

    public void ClickNoBtn()
    {
        yesBtn.sprite = yesBtnList[1];
        noBtn.sprite = noBtnList[0];
        MySceneManager.Instance.ChangeScene("Lobby");
    }
    
    public void HoverYesBtn()
    {
        yesBtn.sprite = yesBtnList[0];
        noBtn.sprite = noBtnList[1];
    }
    
    public void HoverNoBtn()
    {
        yesBtn.sprite = yesBtnList[1];
        noBtn.sprite = noBtnList[0];
    }
    
    public void InitBtn()
    {
        yesBtn.sprite = yesBtnList[1];
        noBtn.sprite = noBtnList[1];
    }
}
