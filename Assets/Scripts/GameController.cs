using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum ePaintMode
    {
        pencil,
        baseRecursiveBucket,
        baseStackBucket,
        scanlineBucket
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
    }

    public void ActivateBaseRecursiveBucket()
    {
        paintMode = ePaintMode.baseRecursiveBucket;
    }
    public void ActivateBaseStackBucket() {
        paintMode = ePaintMode.baseStackBucket;
    }
    public void ActivateScanlineBucket() {
        paintMode = ePaintMode.scanlineBucket;
    }

    public static void paintPixel (Pixel pixel)
    {
        switch (paintMode)
        {
            case ePaintMode.pencil:
                pixel.color = ActiveColor;
                break;
            case ePaintMode.baseRecursiveBucket:
                instance.StartCoroutine(instance.BaseAlgStack(pixel.xPos, pixel.yPos, pixel.color, ActiveColor));
                break;
            case ePaintMode.baseStackBucket:
                instance.StartCoroutine(instance.BaseAlgStack(pixel.xPos, pixel.yPos, pixel.color, ActiveColor));
                break;
            case ePaintMode.scanlineBucket:
                instance.StartCoroutine(instance.ScanlineAlg(pixel.xPos, pixel.yPos, pixel.color, ActiveColor));
                break;
        }
    }

    //=============================================================================buket functions

    private IEnumerator BaseAlgRecursive(int x, int y, Color _oldC, Color _newC)
    {
        if (x < 0 || x >= instance.xDim || y < 0 || y >= instance.yDim || matrix[y, x].color != _oldC)
            yield return 0;
        else
        {
            matrix[y, x].color = _newC;
            yield return new WaitForSeconds(slowTime);
            yield return StartCoroutine(BaseAlgRecursive(x - 1, y, _oldC, _newC));
            yield return new WaitForSeconds(slowTime);
            yield return StartCoroutine(BaseAlgRecursive(x + 1, y, _oldC, _newC));
            yield return new WaitForSeconds(slowTime);
            yield return StartCoroutine(BaseAlgRecursive(x, y - 1, _oldC, _newC));
            yield return new WaitForSeconds(slowTime);
            yield return StartCoroutine(BaseAlgRecursive(x, y + 1, _oldC, _newC));
            yield return new WaitForSeconds(slowTime);
        }
    }

    // original C code for the next 2 methods:
    // https://lodev.org/cgtutor/floodfill.html#:~:text=Recursive%20Scanline%20Floodfill%20Algorithm%20(floodFillScanline),-This%20algorithm%20is&text=Start%20by%20filling%20the%20current,there%20are%20no%20more%20seeds

    // non recursive version that uses a stack
    private IEnumerator BaseAlgStack(int x, int y, Color _oldC, Color _newC) {
        if (_oldC == _newC) yield return 0; //avoid infinite loop

        int [] dx = new int [4] { 0, 1, 0, -1 }; // relative neighbor x coordinates
        int [] dy = new int [4] { -1, 0, 1, 0 }; // relative neighbor y coordinates

        Stack<int> stack = new Stack<int> ();

        stack.Push(x);
        stack.Push(y);
        
        while (stack.Count>0) {

            y = stack.Pop();
            x = stack.Pop();

            matrix[y, x].color = _newC;
            yield return new WaitForSeconds(slowTime);
           
            for (int i = 0; i < 4; i++) {
                int neighboursXs = x + dx[i];
                int neighboursYs = y + dy[i];
                if (neighboursXs >= 0 && neighboursXs < xDim && neighboursYs >= 0 && neighboursYs < yDim && matrix[neighboursYs,neighboursXs].color == _oldC) {
                    stack.Push(neighboursXs);
                    stack.Push(neighboursYs);
                }
            }
        }
    }

    // scanline algorithm
    IEnumerator ScanlineAlg(int x, int y, Color _oldC, Color _newC) {
        if (_oldC == _newC) yield return 0;

        int x1;
        bool spanAbove, spanBelow;

        Stack<int> stack = new Stack<int> ();

        stack.Push(x);
        stack.Push(y);

        while (stack.Count>0) {

            y = stack.Pop();
            x = stack.Pop();

            x1 = x;

            while (x1 >= 0 && matrix[y,x1].color == _oldC) x1--;

            x1++;

            spanAbove = spanBelow = false;

            while (x1 < xDim && matrix[y,x1].color == _oldC) {
                matrix[y,x1].color = _newC;
                
                if (!spanAbove && y > 0 && matrix[y - 1, x1].color == _oldC) {
                    stack.Push(x1);
                    stack.Push(y - 1);
                    spanAbove = true;
                }
                else if (spanAbove && y > 0 && matrix[y - 1, x1].color != _oldC) {
                    spanAbove = true;
                }
                // check h 
                if (!spanBelow && y < yDim-1 && matrix[y + 1, x1].color == _oldC) {
                    stack.Push(x1);
                    stack.Push(y + 1);
                    Debug.Log(y + 1 + "added");
                    spanBelow = true;
                }
                else if (spanBelow && y < yDim-1 && matrix[y + 1,x1].color != _oldC) {
                    spanBelow = false;
                }
                x1++;
                yield return new WaitForSeconds(slowTime);
            }
        }
    }

}
