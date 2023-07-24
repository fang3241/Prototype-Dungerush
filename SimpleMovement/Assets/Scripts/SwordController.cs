using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
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
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit");
        }
    }
}
