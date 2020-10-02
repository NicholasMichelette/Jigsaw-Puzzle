using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    List<GameObject> pieces;
    public int[] initialX;
    public int[] initialY;
    public GameObject piecePrefab;

    // Start is called before the first frame update
    void Start()
    {
        pieces = new List<GameObject>();
        float offset = 0;
        for(int i = 0; i < initialX.Length; i++)
        {
            GameObject o = Instantiate(piecePrefab, this.transform);
            Piece p = o.GetComponent<Piece>();
            p.rows = initialX[i];
            p.columns = initialY[i];
            p.pieceNum = i+1;
            o.transform.position = new Vector3(offset + this.transform.position.x, this.transform.position.y, 1);
            offset += p.columns + 2;
            pieces.Add(o);
        }
    }

    public void addPiece(int rows, int cols)
    {
        int pn = 1;
        if(pieces.Count > 0)
        {
            pn = pieces[pieces.Count - 1].GetComponent<Piece>().pieceNum + 1;
        }

        GameObject o = Instantiate(piecePrefab, this.transform);
        Piece p = o.GetComponent<Piece>();
        p.rows = rows;
        p.columns = cols;
        p.pieceNum = pn;
        pieces.Add(o);
    }

    public GameObject activatePiece(int pn)
    {
        GameObject p = null;
        foreach(GameObject o in pieces)
        {
            if(o.GetComponent<Piece>().pieceNum == pn)
            {
                o.SetActive(true);
                p = o;
            }
        }
        return p;
    }
}
