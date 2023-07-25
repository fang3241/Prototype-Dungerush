using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.levelController = this;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    
}
