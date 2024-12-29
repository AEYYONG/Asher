using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTypingEffect : MonoBehaviour
{
    public float typingSpeed;
    private TextMeshProUGUI _tmp;
    void Start()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
        string text = _tmp.text;
        
        StartCoroutine(TypingText(text));
    }

    IEnumerator TypingText(string text)
    {
        _tmp.text = "";
        for (int i = 0; i < 10; i++)
        {
            foreach (char letter in text.ToCharArray())
            {
                _tmp.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            _tmp.text = "";
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
