using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public bool start = false;
    public AnimationCurve curve;
    public float duration = 1.0f;

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

    IEnumerator Shaking()
    {
        //Store startPosition to reset the camera to this position after screen shake
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        //while elapsed time is less than duration the screen will shake based on the intensity of the curve
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            /*curve should be changed based on type of collision*/
            float strength = curve.Evaluate(elapsedTime / duration);

            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = startPosition;
    }
}
