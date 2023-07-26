using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public Collider colliderSelf;

    public bool isHit;

    private void Start()
    {
        isHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("colldi " + other.name);
        if (colliderSelf != null && other != colliderSelf )//soalnya shieldnya nabrak
        {
            Debug.Log("colldi no pleky");
            if (other.TryGetComponent<SwordController>(out SwordController sw))
            {
                isHit = true;
                sw.isAttackBounced = isHit;
                Debug.Log("dari shield " + other.name);
            }
        }
    }
}
