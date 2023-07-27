using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    int behav;
    //xpos ypos dikasih dari enemy generator biar tau dia ada di posisi mana
    [SerializeField]
    int xpos;
    [SerializeField]
    int ypos;
    [SerializeField]
    Transform player;
    [SerializeField]
    Transform headd;
    Vector2Int tar;

    GameObject GOPlayer;

    [SerializeField]
    EnemyController enemycontroller;

    float detectRadius = 30f;
    float viewSpread = 40f;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        behav = Random.Range(0,1);
        GOPlayer = GameObject.Find("ShieldWarrior");
        player = GOPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(behav == 0){
            //idle
            Debug.Log("idle");
        }
        else if(behav == 1){
            //roaming
            Debug.Log("roaming");
            //pick random coordinat     Generator2D.grid[pos]
            tar = new Vector2Int(Random.Range(0,Generator2D.size.x), Random.Range(0,Generator2D.size.y));
            Debug.Log(Generator2D.grid[tar]);
        }

        //detect in radius
        if(Vector3.Distance(player.position, transform.position) > detectRadius){
            Debug.Log("The Enemy cannot see you...");
            enemycontroller.isPlayerDetected = false;
        }
        if(Vector3.Distance(player.position, transform.position) < detectRadius){
            Debug.Log("Be careful! You're close to an enemy!");
            if(Vector3.Angle(player.position - transform.position, transform.forward) < viewSpread){
                Debug.Log("You've been spotted! RUN!");
                if(Physics.Raycast(headd.position, headd.TransformDirection(Vector3.forward), out hit, detectRadius)){
                    Debug.DrawRay(headd.position, headd.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    if(hit.transform == player){
                        // In Range and i can see you!
                        Debug.Log("Not Hindered By Object");
                        //jalan ke depan
                        enemycontroller.isPlayerDetected = true;
                    }
                }
                else{
                    Debug.DrawRay(headd.position, headd.TransformDirection(Vector3.forward) * 1000, Color.white);
                    Debug.Log("Did not Hit");
                }
            }
        }
    }
}
