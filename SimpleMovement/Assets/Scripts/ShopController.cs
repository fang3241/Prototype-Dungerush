using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{
    public GameObject shopPanel;

    //0 = sword
    //1 = shield
    //2 = potion
    public TextMeshProUGUI[] itemLevels;
    public TextMeshProUGUI[] itemCosts;

    public TextMeshProUGUI playerMoney;
    
    public bool isShopOpened;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.levelController.shop = this;
        isShopOpened = false;
        //shopPanel.SetActive(isShopOpened);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        openShopMenu();
    //    }
    //}

    public void openShopMenu()
    {
        if (!isShopOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
        isShopOpened = !isShopOpened;
        updateShopPrice();
        shopPanel.SetActive(isShopOpened);

        GameManager.instance.levelController.player.GetComponent<P_Controller>().isOnShop = isShopOpened;
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().isOnShop = isShopOpened;
    }

    public void updateShopPrice()
    {
        Debug.Log("Updet");
        playerMoney.text = GameManager.instance.money.ToString();
        int i = 0;
        foreach(TextMeshProUGUI itemLevel in itemLevels)
        {
            itemLevel.text = "Lv " + (GameManager.instance.playerItemLevel[i] + 1);
            Debug.Log("Lv " + (GameManager.instance.playerItemLevel[i] + 1));
            i++;
        }

        itemLevels[2].text = GameManager.instance.playerItemLevel[2].ToString();

        i = 0;
        foreach (TextMeshProUGUI itemCost in itemCosts)
        {
            itemCost.text = getNextLevelItemCost(i).ToString();
            i++;
        }

    }

    public void buyUpgrades(int id)
    {
        checkMoney(id);
    }

    public void checkMoney(int id)
    {
        if(GameManager.instance.money >= getNextLevelItemCost(id))
        {
            if(GameManager.instance.playerItemLevel[id] != 4)
            {
                GameManager.instance.money -= getNextLevelItemCost(id);
                GameManager.instance.playerItemLevel[id]++;
                updateShopPrice();
            }
        }
        else
        {
            Debug.Log("Uang Kurang");
        }
    }

    public int getNextLevelItemCost(int id)
    {
        if(id == 2)//kalau potion
        {
            return GameManager.instance.potionCost;
        }
        else
        {
            if (GameManager.instance.playerItemLevel[id] != 4)
            {
                //harga[id_senjata, current_level + 1]
                return GameManager.instance.itemUpgradeCost[id, GameManager.instance.playerItemLevel[id] + 1];
            }
            else
            {
                return 0;
            }
        }
        
    }




    //unused
    public void buyPotion()
    {
        if (GameManager.instance.money >= GameManager.instance.potionCost)
        {
            GameManager.instance.money -= GameManager.instance.potionCost;
            GameManager.instance.playerItemLevel[2]++;

        }
        else
        {
            Debug.Log("Uang buat beli potion Kurang");
        }
    }

}
