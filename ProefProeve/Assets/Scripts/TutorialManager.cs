using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading;

/// <summary>
/// Manages the tutorial mode functionality and UI
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image swipeMouse;
    [SerializeField] private Image pressE;
    [SerializeField] private Image imageToToggle;

    public bool isBlinking = true;
    private float timer = 0f;
    public float blinkInterval = 0.5f;

    private void Start()
    {
        imageToToggle = swipeMouse;
    }


    private void Update()
    {
        //ToggleImage();
        //StartCoroutine(SwitchImage());

    }

    public void ToggleImage()
    {
        if (isBlinking)
        {
            timer += Time.deltaTime;
            if (timer > blinkInterval)
            {
                imageToToggle.enabled = !imageToToggle.enabled;
                timer = 0f;
            }
        }
        else
        {
            swipeMouse.enabled = false;
            pressE.enabled = false;
        }
    }

    public IEnumerator SwitchImage()
    {
        //in StartGame(); turn bool isBlinking to true;
        //in StartGame(); call ToggleImage(); and SwitchImage();
        //After E is pressed during pressE make sure Tutorial Canvas is off
        yield return new WaitForSeconds(10f);
        imageToToggle = pressE;
    }
}