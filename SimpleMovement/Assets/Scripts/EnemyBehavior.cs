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
    Vector2Int tar;
    // Start is called before the first frame update
    void Start()
    {
        behav = Random.Range(0,1);
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
    }
}
