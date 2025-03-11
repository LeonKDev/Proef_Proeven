using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public bool start = false;
    public AnimationCurve curve;
    public float duration = 1.0f;

    /* 
       This script should be attached to Main Camera
       To call Shaking add this line of code where you'd like the screen shake to take effect:
       _screenShake.StartScreenShake(shakeAmount);
       shakeAmount should be a different value for every different type of collision
    */

    private void Update()
    {
        //when bool start = true screenshake effect will start for length of duration
        //set start to true in the script where you want this to take effect
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    public void StartScreenShake(float shakeAmount)
    {
        Keyframe[] keyframes = curve.keys; // this is making a copy of all keys

        Keyframe keyFrameTwo = keyframes[1];
        keyFrameTwo.time = 0.1f;
        keyFrameTwo.value = shakeAmount;
        keyframes[1] = keyFrameTwo;

        curve.keys = keyframes; // This is copying the keys back into the AnimationCurve's array.

        start = true;
    }

    IEnumerator Shaking()
    {
        //Store startPosition to reset the camera to this position after screen shake
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        //while elapsed time is less than duration the screen will shake based on the intensity of the curve
        while(elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            /*curve should be changed based on type of collision*/
            float strength = curve.Evaluate(elapsedTime / duration);

            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = startPosition;
    }
}
