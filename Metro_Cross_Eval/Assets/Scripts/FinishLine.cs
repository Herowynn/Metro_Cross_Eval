using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("j'ai gagn� !!");
        GameManager.Instance.TimeRunning = false;
    }
}
