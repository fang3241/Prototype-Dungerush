using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int hp;
    public GameObject player;

    public float attackRange;//attack range diisi manual, ngepasiin sendiri sama hitbox pedangnya => max 3.25

    public float rotationSpeed;
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
        if(distance < attackRange)
        {
            //Debug.Log("Attack");
        }
    }

    public void RotateTowardsPlayer()
    {
        //sementara gini, bisa diganti sekalian nggerakkin enemy nya
        transform.LookAt(player.transform);

        Debug.Log("looking at player");
        //Vector3 dir = player.transform.position - transform.position;
        //Quaternion toRotation = Quaternion.LookRotation(transform.forward, dir);
        //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == player.GetComponent<Collider>())
        {
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

}
