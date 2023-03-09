using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("j'ai gagné !!");
        GameManager.Instance.TimeRunning = false;
    }
}
