using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitStopEffect : MonoBehaviour
{
    private bool paused;

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }

    public IEnumerator HitStopCoroutine(float timeToPause)
    {

        //When HitStopCoroutine() is called we pause the game.
        Debug.Log("Pause");
        Pause();

        //yield on a new YieldInstruction that waits for x seconds.
        yield return new WaitForSecondsRealtime(timeToPause);

        //After we have waited x seconds TimeScale is set to 1.
        Debug.Log("Resume");
        Resume();
    }
}
