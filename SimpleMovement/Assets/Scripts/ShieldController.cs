using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public PlayerController player;

    public bool isHit;

    private void Start()
    {
        isHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")//soalnya shieldnya nabrak
        {
            if (other.TryGetComponent<SwordController>(out SwordController sw))
            {
                isHit = true;
                sw.isAttackBounced = isHit;
                Debug.Log("dari shield " + other.name);
            }
        }
    }
}
