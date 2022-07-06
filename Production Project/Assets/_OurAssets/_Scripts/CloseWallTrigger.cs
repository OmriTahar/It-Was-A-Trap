using System.Collections.Generic;
using UnityEngine;

public class CloseWallTrigger : MonoBehaviour
{
    [SerializeField] GameObject _wallToClose;
    [SerializeField] GameObject _lineToOpen;
    Collider myCollider;
    bool activeOpen = true;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activeOpen)
            if (other.CompareTag("Player"))
            {
                _wallToClose.SetActive(true);
                if (_lineToOpen)
                    _lineToOpen.SetActive(true);
                myCollider.enabled = false;
            }
    }
}