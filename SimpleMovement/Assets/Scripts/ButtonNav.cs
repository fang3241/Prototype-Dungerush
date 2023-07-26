using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonNav : MonoBehaviour
{
    public enum SceneList
    {
        MainMenu,
        City,
        Dungeon,
        Shop
    }

    public void toMenu()
    {
        SceneManager.LoadScene(SceneList.MainMenu.ToString());
    }

    public void toCity()
    {
        SceneManager.LoadScene(SceneList.City.ToString());
    }

    public void toDungeon()
    {
        SceneManager.LoadScene(SceneList.Dungeon.ToString());
    }
}
