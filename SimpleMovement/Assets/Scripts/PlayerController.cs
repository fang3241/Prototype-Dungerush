using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ShieldController shield;
    public int playerHP;
    public int playerDamage;

    //public bool isShieldUp;

    private void Start()
    {
        shield.GetComponent<BoxCollider>().enabled = false;
        //isShieldUp = false;
    }

    public void Hit(int dmg)
    {
        playerHP -= dmg;

        if(playerHP <= 0)
        {
            Dead(); 
        }
    }

    private void Dead()
    {
        Debug.Log("Player Dead");
        Destroy(this.gameObject);
    }

}
