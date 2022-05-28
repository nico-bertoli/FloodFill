using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pixel : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;

    public Color color
    {
        get
        {
            return mySpriteRenderer.color;
        }
        set
        {
            mySpriteRenderer.color = value;
        }
    }

    public int xPos { get; set; }
    public int yPos { get; set; }

    private void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown() {
        GameController.paintPixel(this);
    }
}
