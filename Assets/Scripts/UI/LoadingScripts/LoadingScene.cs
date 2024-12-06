using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    static string nextScene;

    [SerializeField]
    Image progressBar;
    [SerializeField]
    TMP_Text tipText; // 팁을 표시할 UI 텍스트
    [SerializeField]
    string[] tips;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    void Start()
    {
        // 팁 랜덤 설정
        if (tips.Length > 0)
        {
            tipText.text = GetRandomTip();
        }


        StartCoroutine(LoadSceneProgress());
    }
    IEnumerator LoadSceneProgress()
    {
        // 비동기 방식으로 씬 불러옴
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; // 씬 로딩 끝나고 다음 씬으로 자동으로 넘어가지 않도록

        // 진행바
        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if(progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    string GetRandomTip()
    {
        int randomIndex = Random.Range(0, tips.Length);
        return tips[randomIndex];
    }
}
