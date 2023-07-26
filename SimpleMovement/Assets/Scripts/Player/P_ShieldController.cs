using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_ShieldController : MonoBehaviour
{
    public P_Controller player;

    public bool isHit;

    private void Start()
    {
        isHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(this.gameObject.name + " colldi " + other.name);
        if(other.tag != "Player")//soalnya shieldnya nabrak
        {
            //Debug.Log("colldi no pleky");
            
            if (other.TryGetComponent<SwordController>(out SwordController sw))
            {
                Debug.Log(sw);
                isHit = true;
                sw.isAttackBounced = isHit;
                Debug.Log("dari shield " + other.name);
            }
        }
    }
}
