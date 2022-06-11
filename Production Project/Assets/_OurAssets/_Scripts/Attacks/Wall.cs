using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private int _maxHP = 4;
    private int _currentHP;

    private void OnEnable()
    {
        WallsPool.ActiveWallsQueue.Enqueue(gameObject);
        _currentHP = _maxHP;
    }

    private void OnDisable()
    {
        WallsPool.ReadyToFireWallsQueue.Enqueue(gameObject);
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