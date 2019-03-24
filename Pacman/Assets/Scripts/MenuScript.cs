using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{

    public MenuButton[] menuButtons;
    public Transform selector;

    private int index = 0;
    public int normalFontSize = 36;
    public int selectedFontSize = 38;

    private void Start()
    {
        foreach (var item in menuButtons)
        {
            item.GetComponent<Text>().fontSize = normalFontSize;
        }
        ChangeMenuSelection();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (index > 0)
            {
                index--;
                ChangeMenuSelection();
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (index < menuButtons.Length - 1)
            {
                index++;
                ChangeMenuSelection();
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            menuButtons[index].ExucteButton();
        }
    }

    private void ChangeMenuSelection()
    {
        Vector3 selectorPosition = selector.position;
        selectorPosition.y = menuButtons[index].transform.position.y;
        selector.position = selectorPosition;

        for (int i = 0; i < menuButtons.Length; i++)
        {
            Text text = menuButtons[i].GetComponent<Text>();
            if (i != index)
            {
                text.fontSize = normalFontSize;
            }
            else
            {
                text.fontSize = selectedFontSize;
            }
        }
    }
}
