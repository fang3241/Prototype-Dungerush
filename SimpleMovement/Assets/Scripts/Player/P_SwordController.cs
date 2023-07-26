using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_SwordController : MonoBehaviour
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
        if(other != colliderSelf && other.CompareTag("Enemy"))
        {
            int dmg = colliderSelf.GetComponent<P_Controller>().playerDamage;
            other.GetComponent<EnemyController>().Hit(dmg);//nanti bikin weapon stats buat masukkin damage nya, sementara 1 dulu
            Debug.Log("Enemy hit");
           
        }
        


        if (other != colliderSelf && other.CompareTag("Player"))
        {
            Debug.Log("Player Hit");

            
            
            

            
        }

    }
}
