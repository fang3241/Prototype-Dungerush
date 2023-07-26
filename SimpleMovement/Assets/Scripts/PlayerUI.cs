using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerUI : MonoBehaviour
{
    public P_Controller player;
    public Slider hpSlider;

    public TextMeshProUGUI potion;
    public TextMeshProUGUI gold;

    public int maxHP;
    
    private void Start()
    {
        player = GameObject.FindObjectOfType<P_Controller>();
    }

    private void Update()
    {
        hpSlider.value = ((float)player.playerHP / (float)maxHP);
        potion.text = GameManager.instance.playerItemLevel[2].ToString();
        gold.text = GameManager.instance.money.ToString();
        
    }

}
