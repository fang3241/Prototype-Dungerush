using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerHP;
    public int playerDamage;

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
