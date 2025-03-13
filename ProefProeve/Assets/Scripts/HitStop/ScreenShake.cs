using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public bool start = false;
    public AnimationCurve curve;
    public float duration = 1.0f;

    private Vector3 originalPosition;

    private void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    public void StartScreenShake(float shakeAmount)
    {
        Keyframe[] keyframes = curve.keys;
        Keyframe keyFrameTwo = keyframes[1];
        keyFrameTwo.time = 0.1f;
        keyFrameTwo.value = shakeAmount;
        keyframes[1] = keyFrameTwo;
        curve.keys = keyframes;
        start = true;
    }

    IEnumerator Shaking()
    {
        originalPosition = transform.localPosition;
        Vector3 animatedPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.localPosition = animatedPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
