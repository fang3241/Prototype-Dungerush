using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{//INI TEST COMBAT
    
    public Transform Chara;
    public Transform Shield;
    public Transform Sword;

    public bool isAttack;

    // Start is called before the first frame update
    void Start()
    {
        Sword.GetComponentInChildren<Collider>().enabled = false;
        isAttack = false;
    }

    public void Attack()
    {
        //Debug.Log("Enemy Attacking");
        StartCoroutine(combo1());
    }


    public IEnumerator combo1()
    {
        isAttack = true;
        //Edit dikit
        Sword.GetComponentInChildren<Collider>().enabled = true;

        float startTime = Time.time;
        StartCoroutine(Anima(Sword, new Vector3(0f, 0f, 0f), new Vector3(-10f, 40f, 0f), 0.2f));
        //wait
        while (Time.time < startTime + 0.2f)
        {
            yield return null;
        }

        StartCoroutine(Anima(Sword, new Vector3(-10f, 40f, 0f), new Vector3(25f, -150f, -115f), 0.3f));
        //wait
        while (Time.time < startTime + 0.2f + 0.5f)
        {
            yield return null;
        }


        


        StartCoroutine(Anima(Sword, new Vector3(25f, -150f, -115f), new Vector3(0f, 0f, 0f), 0.2f));
        //Edit dikit biar hit nya cuman 1x
        yield return new WaitForSeconds(0.5f);
        Sword.GetComponentInChildren<Collider>().enabled = false;
        yield return new WaitForSeconds(1f);
        isAttack = false;
    }

    public IEnumerator Anima(Transform obj, Vector3 from, Vector3 to, float aniTime)
    {
        float startTime = Time.time;
        float curTime;
        float perc;
        //Debug.Log("SZAnima");
        //Debug.Log(from);
        //Debug.Log(to);
        //
        while (Time.time < startTime + aniTime)
        {
            curTime = Time.time;
            perc = (curTime - startTime) / aniTime;
            //
            obj.rotation = Quaternion.Euler(((to.x - from.x) * perc) + from.x, ((to.y - from.y) * perc) + from.y + Chara.eulerAngles.y, ((to.z - from.z) * perc) + from.z);
            //Debug.Log(perc);
            yield return null;
        }
    }

}
