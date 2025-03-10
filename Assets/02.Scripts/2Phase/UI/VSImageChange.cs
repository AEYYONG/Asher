using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VSImageChange : MonoBehaviour
{
    [SerializeField] private StealGauge StealGauge;

    public float ImgChange = 0.5f;

    private Image Image;
    [SerializeField] private Sprite changeSprite; // 변경될 이미지
    private Sprite normalSprite;

    // Start is called before the first frame update
    void Start()
    {

        Image = GetComponent<Image>();
        normalSprite = Image.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("fillvalue의 값: " + StealGauge.fillValue);
        if(gameObject.name == "AsherVs")
        {
            if (StealGauge.fillValue <= ImgChange)
            {

                Debug.Log("***** if문 들어옴 ");
                Image.sprite = changeSprite;
            }

            else
            {
                Image.sprite = normalSprite;
            }
        }

        else
        {
            if(StealGauge.fillValue >= ImgChange)
            {
                Image.sprite = changeSprite;
            }

            else
            {
                Image.sprite = normalSprite;
            }
        }
        
        
        
    }
}
