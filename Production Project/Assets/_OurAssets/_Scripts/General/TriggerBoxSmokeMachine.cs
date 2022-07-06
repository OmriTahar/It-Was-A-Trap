using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoxSmokeMachine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" )
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Other/Smoke Machine");
        }
    }
    
}