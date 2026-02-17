using System.Collections;
using TMPro;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    public TMP_Text text;
    public float blinkSpeed = 0.3f;
    Coroutine blinkRoutine;

    void OnEnable()
    {
        blinkRoutine = StartCoroutine(Blink());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (text) text.enabled = true;
    }

    IEnumerator Blink()
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    public void DisableBlinkAndHide()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        gameObject.SetActive(false);   // hides permanently
    }
}
