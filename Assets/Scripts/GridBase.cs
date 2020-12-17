using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBase : MonoBehaviour
{
    public GameObject tilePrefab;
    public float x_Start, y_Start;
    public int rows, columns;
    public GameObject[,] grid;

    void Start()
    {
        grid = new GameObject[columns, rows];
        for(int i = 0; i < columns; i++)
        {
            for(int j = 0; j < rows; j++)
            {
                grid[i, j] = Instantiate(tilePrefab, this.gameObject.transform);
                grid[i, j].transform.position = new Vector3(x_Start + i * 1.05f - .5f, y_Start + j * 1.05f - .5f, 0);
                grid[i, j].layer = 8;
            }
        }
    }

    public void Resize(int x, int y)
    {
        foreach(GameObject g in grid)
        {
            Destroy(g);
        }

        columns = x;
        rows = y;

        grid = new GameObject[columns, rows];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                grid[i, j] = Instantiate(tilePrefab, this.gameObject.transform);
                grid[i, j].transform.position = new Vector3(x_Start + i * 1.05f - .5f, y_Start + j * 1.05f - .5f, 0);
                grid[i, j].layer = 8;
            }
        }
    }

    public int[,] GetIntArr()
    {
        int[,] result = new int[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                result[i, j] = grid[i, j].GetComponent<Tile>().pieceNum;
            }
        }

        return result;
    }
}
