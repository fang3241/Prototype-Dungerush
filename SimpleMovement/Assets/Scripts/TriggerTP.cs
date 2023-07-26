using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TriggerTP : MonoBehaviour
{
    public ButtonNav nav;
    public string triggerTitle;


    public ButtonNav.SceneList scene;

    private TextMeshProUGUI title;
    private Camera cam;
    private void Start()
    {
        nav = GameManager.instance.gameObject.GetComponent<ButtonNav>();
        cam = Camera.main;
        title = GetComponentInChildren<TextMeshProUGUI>();
        title.text = triggerTitle;

    }

    public void setTP(ButtonNav.SceneList s)
    {

        scene = s;
    }

    private void Update()
    {
        title.gameObject.transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        //Debug.Log(title.gameObject.GetComponent<RectTransform>().rotation);
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.tag == "Player")
        {
            if(scene == ButtonNav.SceneList.Dungeon)
            {
                nav.toDungeon();
            }else if(scene == ButtonNav.SceneList.MainMenu)
            {
                nav.toMenu();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }else if(scene == ButtonNav.SceneList.Shop)//sebenernya bukan scene, cuman taro sini aja sementara
            {
                GameManager.instance.levelController.shop.openShopMenu();
            }
        }
    }
}
