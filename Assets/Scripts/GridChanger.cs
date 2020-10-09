using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridChanger : MonoBehaviour
{
    int x, y;
    GridBase gb;
    // Start is called before the first frame update
    void Start()
    {
        x = 0;
        y = 0;
        gb = FindObjectOfType<GridBase>();
    }

    public void ChangeX(string num)
    {
        if(!num.Equals(""))
            x = int.Parse(num);
    }

    public void ChangeY(string num)
    {
        if (!num.Equals(""))
            y = int.Parse(num);
    }

    public void ResizeGrid()
    {
        gb.Resize(x, y);
    }
}
