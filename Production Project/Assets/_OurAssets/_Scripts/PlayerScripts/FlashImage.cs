using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashImage : MonoBehaviour
{

    Image _image = null;
    Coroutine _currentFlashRoutine = null;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void StartFlash(float secondsForOneFlash, float maxAlpha, Color newColor)
    {
        _image.color = newColor;

        // ensure maxAlpha isn't above 1
        maxAlpha = Mathf.Clamp(maxAlpha, 0, 1);

        if (_currentFlashRoutine != null)
            StopCoroutine(_currentFlashRoutine);

        _currentFlashRoutine = StartCoroutine(Flash(secondsForOneFlash, maxAlpha));
    }

    IEnumerator Flash(float secondsForOneFlash, float maxAlpha)
    {
        float flashInDuration = secondsForOneFlash / 2;

        for (float t = 0; t <= flashInDuration; t += Time.deltaTime)
        {
            Color colorThisFrame = _image.color;
            colorThisFrame.a = Mathf.Lerp(0, maxAlpha, t / flashInDuration);
            _image.color = colorThisFrame;
            yield return null;
        }


    }
}
