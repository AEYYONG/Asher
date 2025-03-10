using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FlashEffect : MonoBehaviour
{
    private Image image;
    private Color originalColor;

    void Start()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        Flash();
    }

    public void Flash()
    {
        image.DOColor(Color.white, 0.3f).OnComplete(() =>
        {
            image.DOColor(originalColor, 0.3f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        });
    }
}
