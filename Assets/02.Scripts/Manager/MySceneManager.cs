using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance { get; private set; }
    public CanvasGroup Fade_img; // 페이드 이미지
    private float fadeDuration = 2f; // 페이드 시간

    void Start()
    {
        // 싱글톤 패턴 적용
        if (Instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 후 콜백 등록
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 후 콜백 해제
    }

    public void ChangeScene(string sceneName)
    {
        // 페이드 아웃 후 씬 로드
        Fade_img.DOFade(1, fadeDuration)
            .OnStart(() => { Fade_img.blocksRaycasts = true; }) // 클릭 방지
            .OnComplete(() => { SceneManager.LoadScene(sceneName); });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 후 페이드 인
        Fade_img.DOFade(0, fadeDuration)
            .OnStart(() => { Fade_img.blocksRaycasts = true; }) // 클릭 방지
            .OnComplete(() => { Fade_img.blocksRaycasts = false; });
    }
}
