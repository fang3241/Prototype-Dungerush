using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public Collider colliderSelf;

    public bool isAttackBounced;

    private void Start()
    {
        isAttackBounced = false;
    }
    /*
     * 1. dobel hit karena dia 2x masuk ke musuh, pas hit pertama, sama pas kembali ke posisi semula 
     * 2. kalo misal posisi pedang di depan, dan bisa collide sama musuh, bisa accidental hit
     * 
     * solusi :
     * 1. - mungkin kasih knockback ke musuh biar pas pedang ke posisi semula, senjata gk kena lg
     *    - matiin collider setelah hit pertama, pas klik hit lagi, baru nyalain lg
     * 2. matiin collider pedang, nyalain pas klik hit
     */
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Swooa");
        
        if (other != colliderSelf && other.CompareTag("Player"))
        {
            Debug.Log("Player Hit");
            if (!isAttackBounced)
            {
                int dmg = colliderSelf.GetComponent<EnemyController>().damage;
                other.GetComponent<P_Controller>().Hit(dmg);
                Debug.Log("Player damaged by " + colliderSelf.name);
            }
            else
            {
                Debug.Log("Player Shielded " + transform.parent.parent.gameObject);
                isAttackBounced = false;

                //kasi tau ke enemy tsb bahwa keblock

                //EnemyCombat enemyCombat = transform.parent.parent.gameObject.GetComponent<EnemyCombat>();
                //enemyCombat.blocked = true;

                EnemyCombat enemyCombat = colliderSelf.GetComponent<EnemyCombat>();
                enemyCombat.blocked = true;

                //animasi enemy tsb gagal atk
                //
            }
            
            
            

            
        }

    }
}
