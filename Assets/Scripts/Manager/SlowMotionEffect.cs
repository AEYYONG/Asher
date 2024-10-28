using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SlowMotionEffect : MonoBehaviour
{
    public bool isSlowMotionEnd = false;
    public float slowFactor;
    public float slowLength;

    public void DoSlowMotion(float value)
    {
        slowFactor = value;
        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    
    private void Update()
    {
        if (isSlowMotionEnd)
        {
            Time.timeScale += (1f / slowLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }
}
