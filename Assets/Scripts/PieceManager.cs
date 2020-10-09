using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    List<GameObject> pieces;
    public int[] initialX;
    public int[] initialY;
    public GameObject piecePrefab;
    GridBase gb;

    // Start is called before the first frame update
    void Start()
    {
        gb = FindObjectOfType<GridBase>();
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

    public void AddPiece(int cols, int rows)
    {
        if (rows > 0 && cols > 0)
        {
            int pn = 1;
            if (pieces.Count > 0)
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
    }

    public GameObject ActivatePiece(int pn)
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

    public void RemovePiece(GameObject obj)
    {
        pieces.Remove(obj);
        Destroy(obj);
    }

    public void SolvePuzzle()
    {
        // SETUP

        List<GameObject> tempPieces = new List<GameObject>(pieces);

        //sort list by length (col length)
        tempPieces.Sort(SortByLength);

        //add each possible orientation (flips)
        List<int[,]>[] l = new List<int[,]>[tempPieces.Count];
        for(int i = 0; i < tempPieces.Count; i++)
        {
            Piece p = tempPieces[i].GetComponent<Piece>();
            l[i] = new List<int[,]>();

            int[,] rep = p.GetArrayRepresentation();
            //ArrToString(rep, p.rows, p.columns);
            l[i].Add(rep);
            int[,] tempRep = FlipX(rep, p.rows, p.columns);
            //ArrToString(tempRep, p.rows, p.columns);
            if (!AlreadyExists(l[i], tempRep, p.rows, p.columns))
                l[i].Add(tempRep);
            tempRep = FlipY(rep, p.rows, p.columns);
            //ArrToString(tempRep, p.rows, p.columns);
            if (!AlreadyExists(l[i], tempRep, p.rows, p.columns))
                l[i].Add(tempRep);
            tempRep = FlipX(FlipY(rep, p.rows, p.columns), p.rows, p.columns);
            //ArrToString(tempRep, p.rows, p.columns);
            if (!AlreadyExists(l[i], tempRep, p.rows, p.columns))
                l[i].Add(tempRep);
            Debug.Log(l[i].Count);


        }

        // END SETUP



        // ALGORITHM
    }

    static int SortByLength(GameObject g1, GameObject g2)
    {
        Piece p1 = g1.GetComponent<Piece>();
        Piece p2 = g2.GetComponent<Piece>();

        int result = p2.columns.CompareTo(p1.columns);

        if(result == 0)
        {
            result = p2.rows.CompareTo(p1.rows);
        }

        return result;
    }

    static int[,] FlipX(int[,] arr, int rows, int cols)
    {
        int[,] result = new int[cols, rows];

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                result[i, j] = arr[cols - 1 - i, j];
            }
        }

        return result;
    }

    static int[,] FlipY(int[,] arr, int rows, int cols)
    {
        int[,] result = new int[cols, rows];

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                result[i, rows - 1 - j] = arr[i, j];
            }
        }

        return result;
    }

    static bool ArrayCheckEqual(int[,] arr1, int[,] arr2, int rows, int cols)
    {
        bool result = true;
        if(arr1.Length == arr2.Length)
        {
            for(int i = 0; i < cols && result; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (arr1[i, j] != arr2[i, j])
                        result = false;
                }
            }
        }
        

        return result;
    }

    static bool AlreadyExists(List<int[,]> l, int[,] arr, int rows, int cols)
    {
        bool result = false;

        for(int i = 0; i < l.Count && !result; i++)
        {
            if(ArrayCheckEqual(l[i], arr, rows, cols))
                result = true;
        }

        return result;
    }

    // FOR DEBUGGING PURPOSES ONLY  
    static void ArrToString(int[,] arr, int rows, int cols)
    {
        string str = "";
        for(int i = rows-1; i >= 0; i--)
        {
            for(int j = 0; j < cols; j++)
            {
                str = str + arr[j, i] + ", ";
            }
            str = str + "\n\r";
        }
        Debug.Log(str);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
