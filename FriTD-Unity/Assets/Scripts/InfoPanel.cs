using System;
using UnityEngine;
using System.Collections;

public class InfoPanel : MonoBehaviour
{

    private String _text = "";

    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 1000, 1000), _text);
    }

    public void updateInfo(int hp, int gold)
    {
        _text = "HP: " + hp + "\nGold: " + gold;
    }
}
