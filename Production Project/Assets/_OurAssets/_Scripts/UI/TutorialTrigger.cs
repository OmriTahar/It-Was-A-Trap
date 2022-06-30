using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textObject;
    [SerializeField] string _textToShow;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _textObject.text = _textToShow;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _textObject.text = "";
            Destroy(gameObject);
        }
    }
}
