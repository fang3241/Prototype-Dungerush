using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{

    float isAtk = 0.0f;
    [SerializeField]
    float isDefMax = 0.25f;
    float isDef = 0f;
    bool clickAtk = false;
    [SerializeField]
    Transform Chara;
    [SerializeField]
    Transform Shield;
    [SerializeField]
    Transform Sword;

    public CharacterController controller;
    public float dashSpeed;
    public float dashTime;
    public float dashCD;
    public float deamult;
    bool isDash = false;
    // Start is called before the first frame update
    void Start()
    {
        Sword.GetComponentInChildren<Collider>().enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;

        if (Input.GetMouseButtonDown(0)){
            Debug.Log("Pressed left-click.");
            StartCoroutine(combo1());
        }

        if (Input.GetMouseButtonUp(0)){
            Debug.Log("Released left-click.");
        }

        if (Input.GetMouseButton(1)){
            //Debug.Log("Pressed right-click.");
            if(isDef >= isDefMax){
                isDef = isDefMax;
                //baru bisa ngeblock kalo isDef udah penuh
            }
            else if(isDef < isDefMax){
                isDef += 1.0f * Time.deltaTime;
            }
        }
        else{
            //Debug.Log("Released right-click.");
            if(isDef > 0.0f){
                isDef -= 1.0f * Time.deltaTime;
            }
            else{
                isDef = 0f;
            }
        }

        //
        if(Input.GetKeyDown(KeyCode.Space) && isDash == false){
            //Debug.Log("SPACE DOWN");
            isDash = true;
            //cek direction player
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + Chara.eulerAngles.y;
            Vector3 movedir = Quaternion.Euler(0f, targetAngle, 0f) * -Vector3.forward;
            //Debug.Log(Chara.eulerAngles.y);
            //dash ke arah belakang player
            //controller.Move(movedir); //malah tp ke belakang
            StartCoroutine(Dash(movedir));
        }

        Shield.rotation = Quaternion.Euler(0, Chara.eulerAngles.y + ((isDef/isDefMax) * 90), 0);

    }

    IEnumerator Dash(Vector3 movedir){
        float startTime = Time.time;
        float curDashSpeed = dashSpeed;
        //cek lagi attacking ngga, kalo iya gabisa
        while(Time.time < startTime + dashTime){
            //pas disini disable movement lain
            //
            controller.Move(movedir * curDashSpeed * Time.deltaTime);
            curDashSpeed = curDashSpeed * deamult;  
            yield return null;
        }
        while(Time.time < startTime + dashTime + dashCD){
            //pas disini disable movement lain
            //
            controller.Move(Vector3.zero);
            yield return null;
        }
        isDash = false;
    }

    IEnumerator combo1(){
        //Edit dikit
        Sword.GetComponentInChildren<Collider>().enabled = true;
        
        float startTime = Time.time;
        StartCoroutine(Anima(Sword, new Vector3(0f, 0f, 0f), new Vector3(-10f, 40f, 0f), 0.2f));
        //wait
        while(Time.time < startTime + 0.2f){
            yield return null;
        }
        
        StartCoroutine(Anima(Sword, new Vector3(-10f, 40f, 0f), new Vector3(25f, -150f, -115f), 0.3f));
        //wait
        while(Time.time < startTime + 0.2f + 0.5f){
            yield return null;
        }


        //Edit dikit biar hit nya cuman 1x
        Sword.GetComponentInChildren<Collider>().enabled = false;
        

        StartCoroutine(Anima(Sword, new Vector3(25f, -150f, -115f), new Vector3(0f, 0f, 0f), 0.2f));
    }

    IEnumerator Anima(Transform obj, Vector3 from, Vector3 to, float aniTime){
        float startTime = Time.time;
        float curTime;
        float perc;
        //Debug.Log("SZAnima");
        //Debug.Log(from);
        //Debug.Log(to);
        //
        while(Time.time < startTime + aniTime){
            curTime = Time.time;
            perc = (curTime - startTime)/aniTime;
            //
            obj.rotation = Quaternion.Euler(((to.x - from.x) * perc) + from.x, ((to.y - from.y) * perc) + from.y + Chara.eulerAngles.y, ((to.z - from.z) * perc) + from.z);
            //Debug.Log(perc);
            yield return null;
        }
    }
}
