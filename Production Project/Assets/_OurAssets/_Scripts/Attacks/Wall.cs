using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            StartCoroutine(CrushWall());
        }
    }
    IEnumerator CrushWall()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
