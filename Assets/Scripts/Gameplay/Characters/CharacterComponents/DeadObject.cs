using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class DeadObject : MonoBehaviour
{
    public void Die()
    {
        StartCoroutine(WaitTimeByDestroy());
    }

    private IEnumerator WaitTimeByDestroy()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
