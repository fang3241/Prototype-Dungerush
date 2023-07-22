using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //harga barang
    //0 = sword
    //1 = shield
    public int[,] itemUpgradeCost;

    //2
    public int potionCost;
    
    //level pedang/shield dan banyak potion
    //0 = pedang
    //1 = shield
    //2 = potion
    public int[] playerItemLevel;

    public int money;

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        itemUpgradeCost = new int[2, 5]
        {
            { 0, 10, 20, 30, 50 },
            { 0, 5, 15, 30, 60 },
        };

        potionCost = 60;

        playerItemLevel = new int[3] { 1, 1, 0 };

        money = 500;
    }

    public void AddMoney()
    {
        money += 100;
        FindAnyObjectByType<ShopController>().updateShopPrice();
    }
}
