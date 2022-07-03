using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private int _maxHP = 4;
    private int _currentHP;

    private void OnEnable()
    {
        CoverPool.ActiveCoversQueue.Enqueue(gameObject);
        _currentHP = _maxHP;  
    }

    private void OnDisable()
    {
        CoverPool.ReadyToFireCoversQueue.Enqueue(gameObject);
    }

    private void Update()
    {
        if (_currentHP <= 0)
            gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            _currentHP--;
    }

}