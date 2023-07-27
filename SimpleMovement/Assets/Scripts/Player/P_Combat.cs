using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Combat : MonoBehaviour
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

    public P_Controller player;
    public CharacterController controller;
    public float dashSpeed;
    public float dashTime;
    public float dashCD;
    public float deamult;
    public bool isDash = false;

    private Animator anim;

    public float attInterval;
    public bool isAttCD;

    public bool isShieldUp;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<P_Controller>();
        Sword.GetComponent<BoxCollider>().enabled = false;
        anim = GetComponentInChildren<Animator>();

        isAttCD = false;
        isShieldUp = false;
        
    }

    // Update is called once per frame
    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;

        if (Input.GetMouseButtonDown(0) && !player.isOnShop && isDef == 0){
            Debug.Log("Pressed left-click.");

            if (!isAttCD && !isShieldUp)
            {
                StartCoroutine(CooldownForAttack());
            }
        }

        if (Input.GetMouseButtonUp(0) && !player.isOnShop)
        {
            Debug.Log("Released left-click.");
        }

        if (Input.GetMouseButton(1) && !player.isOnShop)
        {
            //Debug.Log("Pressed right-click.");
            if(isDef == isDefMax){
                //Debug.Log("fullDef");
                isShieldUp = true;
                Shield.GetComponent<BoxCollider>().enabled = true;
                anim.Play("defLoop");//
            }
            if(isDef >= isDefMax){
                isDef = isDefMax;
                //baru bisa ngeblock kalo isDef udah penuh
            }
            else if(isDef < isDefMax){
                if (isAttCD == false)
                {
                    isDef += 1.0f * Time.deltaTime;
                    anim.Play("defUp");//
                }
            }
        }
        else{
            //Debug.Log("Released right-click.");
            //Debug.Log(isDef);
            if(isDef > 0.0f){
                isDef -= 1.0f * Time.deltaTime;
                anim.CrossFade("Movement", 0.15f);//
            }
            else{
                isDef = 0f;
            }
            isShieldUp = false;
            Shield.GetComponent<BoxCollider>().enabled = false;
            //player.isShieldUp = false;
            //player.shield.isHit = false;
        }

        //
        if(Input.GetKeyDown(KeyCode.Space) && isDash == false && !isShieldUp && !isAttCD){
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

        //Shield.rotation = Quaternion.Euler(0, Chara.eulerAngles.y + ((isDef/isDefMax) * 90), 0);

    }

    //0 = att
    //1 = shield
    public IEnumerator CooldownForAttack()
    {
        isAttCD = true;
        StartCoroutine(combo1());
        yield return new WaitForSeconds(attInterval);
        isAttCD = false;
    }

    IEnumerator Dash(Vector3 movedir){
        //dash back anim
        //
        anim.CrossFade("dashBack", 0.05f);
        yield return new WaitForSeconds(0.05f);
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
        //animasi atk 1
        Debug.Log("atk1");
        anim.CrossFade("atk1", 0.15f);
        yield return new WaitForSeconds(0.3f);
        Sword.GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(0.4f);
        //Edit dikit biar hit nya cuman 1x
        Sword.GetComponent<BoxCollider>().enabled = false;

        yield return null;
        
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
