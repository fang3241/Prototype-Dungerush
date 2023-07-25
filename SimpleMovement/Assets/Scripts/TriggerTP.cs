using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTP : MonoBehaviour
{
    public ButtonNav nav;

    public ButtonNav.SceneList scene;

    private void Start()
    {
        nav = GameManager.instance.gameObject.GetComponent<ButtonNav>();
    }

    public void setTP(ButtonNav.SceneList s)
    {
        scene = s;
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
            }
        }
    }
}
