using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCreator : MonoBehaviour
{
    int x, y;
    PieceManager pm;
    // Start is called before the first frame update
    void Start()
    {
        x = 0;
        y = 0;
        pm = FindObjectOfType<PieceManager>();
    }

    public void ChangeX(string num)
    {
        if (!num.Equals(""))
            x = int.Parse(num);
    }

    public void ChangeY(string num)
    {
        if (!num.Equals(""))
            y = int.Parse(num);
    }

    public void CreatePiece()
    {
        pm.AddPiece(x, y);
    }
}
