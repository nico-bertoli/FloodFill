using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyButton : MonoBehaviour
{
    Color myColor;
    private void Start()
    {
        myColor = GetComponent<Image>().color;
    }

    public void SetActiveColor()
    {
        GameController.ActiveColor = myColor;
    }
}
