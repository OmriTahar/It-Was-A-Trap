using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public virtual void OnInteraction()
    {
        Debug.Log("Interaction!");
    }
}
