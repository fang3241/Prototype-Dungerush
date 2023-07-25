using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    public EnemyController enemy;
    public Slider hpSlider;

    public int maxHP;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        maxHP = enemy.hp;
        hpSlider.maxValue = (float)maxHP;
    }

    private void Update()
    {
        transform.LookAt(cam.transform);
        hpSlider.value = enemy.hp;
    }
}
