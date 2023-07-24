using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyCombat enemyCombat;
    public GameObject player;
    public int hp;
    public int damage;
    public float attInterval;

    public float attackRange;//attack range diisi manual, ngepasiin sendiri sama hitbox pedangnya => max 2.88
    
    private Collider EnemyRange;

    public bool isPlayerDetected;

    public int goldDrop;

    private void Start()
    {
        isPlayerDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerDetected)
        {
            RotateTowardsPlayer();
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if(distance < attackRange && !enemyCombat.isAttack)
        {
            StartCoroutine(AttackTimer(attInterval));
            //Debug.Log("Attack");
        }
    }

    public void RotateTowardsPlayer()
    {
        //sementara gini, bisa diganti sekalian nggerakkin enemy nya
        transform.LookAt(player.transform);
        
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == player.GetComponent<Collider>())
        {
            isPlayerDetected = true;
            Debug.Log(other);
        }
    }

    public void Hit(int dmg)
    {
        hp -= dmg;

        if(hp <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        //add gold
        GameManager.instance.AddMoney(goldDrop);
        //remove GO
        Destroy(this.gameObject);
    }

    public IEnumerator AttackTimer(float time)
    {
        //yield return new WaitForSeconds(time);
        yield return new WaitForSeconds(time);
        enemyCombat.Attack();
    }
}
