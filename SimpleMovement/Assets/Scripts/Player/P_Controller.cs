using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Controller : MonoBehaviour
{
    public P_ShieldController shield;
    public int playerHP;
    public int playerDamage;


    public bool isOnShop;
    //public bool isShieldUp;

    private void Start()
    {
        shield.GetComponent<BoxCollider>().enabled = false;
        //isShieldUp = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(GameManager.instance.CheckPotion() > 0)
            {
                Heal(2);
                GameManager.instance.playerItemLevel[2]--;//ngurangin potionnya 1
            }
        }
    }

    public void Heal(int hp)
    {
        playerHP = Mathf.Clamp(playerHP + hp, 1, 10);
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
