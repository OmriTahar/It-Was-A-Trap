using System.Collections.Generic;
using UnityEngine;

public class CloseWallTrigger : MonoBehaviour
{

    [SerializeField] GameObject _wallToClose;

    private Renderer _renderer;
    private MeshCollider _meshCollider;

    void Awake()
    {
        _renderer = _wallToClose.GetComponent<Renderer>();
        _meshCollider = _wallToClose.GetComponent<MeshCollider>();

        _renderer.enabled = false;
        _meshCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _renderer.enabled = true;
            _meshCollider.enabled = true;
        }
    }
}
