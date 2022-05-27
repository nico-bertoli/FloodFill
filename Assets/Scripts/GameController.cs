using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum ePaintMode
    {
        pencil,
        bucket
    }

    public static Color ActiveColor { get; set; }
    private static ePaintMode paintMode;


    [SerializeField] GameObject pixelPrefab;
    [SerializeField] int xDim;
    [SerializeField] int yDim;
    static GameController instance;

    [SerializeField] float slowTime;

    private static Pixel [,] matrix;

    private void Start()
    {
        matrix = new Pixel[yDim,xDim];

        for (int y = 0; y < yDim; y++)
            for (int x = 0; x < xDim; x++)
            {
                matrix[y, x] = Instantiate(pixelPrefab, new Vector2(x, y), Quaternion.identity).GetComponent<Pixel>();
                matrix[y, x].xPos = x;
                matrix[y, x].yPos = y;
            }
        instance = this;
                
    }

    public void ActivatePencilMode()
    {
        paintMode = ePaintMode.pencil;
        Debug.Log("pencil mode activated");
    }

    public void ActivateBucketlMode()
    {
        paintMode = ePaintMode.bucket;
        Debug.Log("bucket mode activated");
    }

    public static void paintPixel (Pixel pixel)
    {
        switch (paintMode)
        {
            case ePaintMode.pencil:
                pixel.color = ActiveColor;
                break;
            case ePaintMode.bucket:
                //BucketDFS(pixel.xPos, pixel.yPos, pixel.color, ActiveColor);
                instance.StartCoroutine(instance.BucketDFSslow(pixel.xPos, pixel.yPos, pixel.color, ActiveColor));
            break;

        }
    }

    //=============================================================================buket functions

    private static void BucketDFS(int x, int y, Color _oldC, Color _newC)
    {
        if (x < 0 || x >= instance.xDim || y < 0 || y >= instance.yDim || matrix[y,x].color != _oldC)
            return;
        else
        {
            matrix[y, x].color = _newC;
            BucketDFS(x - 1, y,_oldC,_newC);
            BucketDFS(x + 1, y, _oldC, _newC);
            BucketDFS(x, y - 1, _oldC, _newC);
            BucketDFS(x, y + 1, _oldC, _newC);
        }
    }

    private IEnumerator BucketDFSslow(int x, int y, Color _oldC, Color _newC)
    {
        if (x < 0 || x >= instance.xDim || y < 0 || y >= instance.yDim || matrix[y, x].color != _oldC)
            yield return 0;
        else
        {
            matrix[y, x].color = _newC;
            yield return new WaitForSeconds(slowTime);
            yield return StartCoroutine(BucketDFSslow(x - 1, y, _oldC, _newC));
            yield return new WaitForSeconds(slowTime);
            yield return StartCoroutine(BucketDFSslow(x + 1, y, _oldC, _newC));
            yield return new WaitForSeconds(slowTime);
            yield return StartCoroutine(BucketDFSslow(x, y - 1, _oldC, _newC));
            yield return new WaitForSeconds(slowTime);
            yield return StartCoroutine(BucketDFSslow(x, y + 1, _oldC, _newC));
            yield return new WaitForSeconds(slowTime);
        }
    }

}
