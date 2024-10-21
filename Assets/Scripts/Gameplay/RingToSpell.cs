using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingToSpell : MonoBehaviour
{
    [SerializeField] private DrawRing ring;
    public void Init(float radius, Vector3 position, float duration = 1f)
    {
        DrawRing newRing = Instantiate(ring, new Vector3(position.x, 1, position.z), ring.gameObject.transform.rotation);
        newRing.SetRadius(radius);

        StartCoroutine(DestroyRing(duration, newRing.gameObject));
    }

    private IEnumerator DestroyRing(float duration, GameObject ring)
    {
        yield return new WaitForSeconds(duration);
        Destroy(ring);
    } 
}
