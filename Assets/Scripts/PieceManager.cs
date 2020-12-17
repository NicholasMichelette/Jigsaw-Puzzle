using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
            o.transform.position = new Vector3(o.transform.position.x, o.transform.position.y, o.transform.position.z + 1);
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
        int[] rowNums = new int[tempPieces.Count];
        int[] colNums = new int[tempPieces.Count];

        //add each possible orientation (flips)
        List<int[,]>[] l = new List<int[,]>[tempPieces.Count];
        List<int>[] offset = new List<int>[tempPieces.Count];

        for (int i = 0; i < tempPieces.Count; i++)
        {
            Piece p = tempPieces[i].GetComponent<Piece>();
            l[i] = new List<int[,]>();
            offset[i] = new List<int>();
            rowNums[i] = p.rows;
            colNums[i] = p.columns;

            int[,] rep = p.GetArrayRepresentation();
            l[i].Add(rep);
            offset[i].Add(FindOffset(rep, p.rows, p.columns));

            int[,] tempRep = FlipX(rep, p.rows, p.columns);
            if (!AlreadyExists(l[i], tempRep, p.rows, p.columns))
            {
                l[i].Add(tempRep);
                offset[i].Add(FindOffset(tempRep, p.rows, p.columns));
            }

            tempRep = FlipY(rep, p.rows, p.columns);
            if (!AlreadyExists(l[i], tempRep, p.rows, p.columns))
            {
                l[i].Add(tempRep);
                offset[i].Add(FindOffset(tempRep, p.rows, p.columns));
            }

            tempRep = FlipX(FlipY(rep, p.rows, p.columns), p.rows, p.columns);
            if (!AlreadyExists(l[i], tempRep, p.rows, p.columns))
            {
                l[i].Add(tempRep);
                offset[i].Add(FindOffset(tempRep, p.rows, p.columns));
            }
        }

        //have array keep track of which iteration of each piece is being used and the location
        int[] whichFlip = new int[l.Length];
        int[] placedX = new int[l.Length];
        int[] placedY = new int[l.Length];
        for(int i = 0; i < l.Length; i++)
        {
            placedX[i] = -2;
            placedY[i] = 0;
        }
        int[,] gi = gb.GetIntArr();

        

        // END SETUP


        // ALGORITHM
        //loop goes through the pieces

        int a = 0;
        bool stop = false;
        while(a < l.Length && !stop)
        {
            bool placed = false;
            //l[k][whichFlip[k]];
            //loop that goes through each iteration of piece
            int b = whichFlip[a];
            while (b < l[a].Count && !placed)
            {
                //loop that tries to place the piece
                //for each tile in grid, attempt to place piece
                int d = placedX[a] + 2;
                int c = placedY[a];
                while (c < gb.rows && !placed)
                {
                    while(d < gb.columns && !placed)
                    {
                        if(gi[d, c] == 0)
                        {
                            bool canPlace = true;
                            for(int i = offset[a][b]; i < rowNums[a] && canPlace; i++)
                            {
                                for(int j = 0; j < colNums[a] && canPlace; j++)
                                {
                                    if (l[a][b][j, i] > 0)
                                    {
                                        if (gb.rows - 1 < c + i - offset[a][b] || gb.columns - 1 < d + j)
                                            canPlace = false;
                                        else if (gi[d + j, c + i - offset[a][b]] != 0)
                                            canPlace = false;
                                    }
                                }
                            }

                            for (int i = offset[a][b] - 1; i >= 0 && canPlace; i--)
                            {
                                for (int j = 0; j < colNums[a] && canPlace; j++)
                                {
                                    if (l[a][b][j, i] > 0)
                                    {
                                        if ((0 > c + i - offset[a][b] || gb.columns - 1 < d + j))
                                            canPlace = false;
                                        else if (gi[d + j, c + i - offset[a][b]] != 0)
                                            canPlace = false;
                                    }
                                }
                            }

                            if(canPlace)
                            {
                                placed = true;
                                placedX[a] = d;
                                placedY[a] = c;
                                whichFlip[a] = b;

                                for (int i = offset[a][b]; i < rowNums[a] && canPlace; i++)
                                {
                                    for (int j = 0; j < colNums[a]; j++)
                                    {
                                        if (l[a][b][j, i] > 0)
                                        {
                                            int tempNum = tempPieces[a].GetComponent<Piece>().pieceNum;
                                            gi[d + j, c + i - offset[a][b]] = tempNum;
                                            gb.grid[d + j, c + i - offset[a][b]].GetComponent<Tile>().pieceNum = tempNum;
                                            gb.grid[d + j, c + i - offset[a][b]].GetComponent<SpriteRenderer>().color = tempPieces[a].GetComponent<Piece>().pieceColor;
                                        }
                                    }
                                }

                                for (int i = offset[a][b] - 1; i >= 0 && canPlace; i--)
                                {
                                    for (int j = 0; j < colNums[a] && canPlace; j++)
                                    {
                                        if (l[a][b][j, i] > 0)
                                        {
                                            int tempNum = tempPieces[a].GetComponent<Piece>().pieceNum;
                                            gi[d + j, c + i - offset[a][b]] = tempNum;
                                            gb.grid[d + j, c + i - offset[a][b]].GetComponent<Tile>().pieceNum = tempNum;
                                            gb.grid[d + j, c + i - offset[a][b]].GetComponent<SpriteRenderer>().color = tempPieces[a].GetComponent<Piece>().pieceColor;
                                        }
                                    }
                                }
                            }


                        }
                        d += 2;
                    }
                    c++;
                    if (c % 2 == 0)
                        d = 0;
                    else
                        d = 1;
                    if(!placed)
                    {
                        placedX[a] = -2;
                        placedY[a] = 0;
                    }
                }
                b++;
            }
            if (placed)
                a++;
            else if (a > 0)
            {
                whichFlip[a] = 0;
                placedX[a] = -2;
                placedY[a] = 0;
                a--;
                //unplaces a piece, looks for any tile with the piece number through the entire grid
                for (int i = 0; i < gb.rows; i++)
                {
                    for (int j = 0; j < gb.columns; j++)
                    {
                        Tile t = gb.grid[j, i].GetComponent<Tile>();
                        if (t.pieceNum == tempPieces[a].GetComponent<Piece>().pieceNum)
                        {
                            t.pieceNum = 0;
                            gb.grid[j, i].GetComponent<SpriteRenderer>().color = Color.white;
                            gi[j, i] = 0;
                        }
                    }
                }
            }
            else
                stop = true;
        }
        
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

    static int FindOffset(int[,] arr, int rows, int cols)
    {
        int result = 0;
        bool cont = true;

        for(int i = 0; i < rows && cont; i++)
        {
            if(arr[0, i] > 0)
            {
                cont = false;
                result = i;
            }
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

    public void Reset()
    {
        SceneManager.LoadScene("Puzzle_Solver");
    }
}
