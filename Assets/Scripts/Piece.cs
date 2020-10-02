using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject[,] grid;
    public int rows, columns;
    public int pieceNum;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GameObject[columns, rows];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                grid[i, j] = Instantiate(tilePrefab, this.gameObject.transform);
                grid[i, j].transform.position = new Vector3(this.gameObject.transform.position.x + i * 1.05f - .5f, this.gameObject.transform.position.y + j * 1.05f - .5f, -1);
            }
        }
    }

    public void setPieces()
    {
        Color c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if(grid[i, j].GetComponent<Tile>().pieceNum == -1)
                {
                    grid[i, j].GetComponent<Tile>().pieceNum = -2;
                    grid[i, j].GetComponent<SpriteRenderer>().color = Color.clear;
                }
                else if (grid[i, j].GetComponent<Tile>().pieceNum == 0)
                {
                    grid[i, j].GetComponent<Tile>().pieceNum = pieceNum;
                    grid[i, j].GetComponent<SpriteRenderer>().color = c;
                }
            }
        }
    }

    public void flipX()
    {
        for(int i = 0; i < columns/2; i++)
        {
            for(int j = 0; j < rows; j++)
            {
                int temp = grid[i, j].GetComponent<Tile>().pieceNum;
                Color tempC = grid[i, j].GetComponent<SpriteRenderer>().color;
                grid[i, j].GetComponent<Tile>().pieceNum = grid[columns - 1 - i, j].GetComponent<Tile>().pieceNum;
                grid[i, j].GetComponent<SpriteRenderer>().color = grid[columns - 1 - i, j].GetComponent<SpriteRenderer>().color;
                grid[columns - 1 - i, j].GetComponent<Tile>().pieceNum = temp;
                grid[columns - 1 - i, j].GetComponent<SpriteRenderer>().color = tempC;
            }
        }
    }

    public void flipY()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows / 2; j++)
            {
                int temp = grid[i, j].GetComponent<Tile>().pieceNum;
                Color tempC = grid[i, j].GetComponent<SpriteRenderer>().color;
                grid[i, j].GetComponent<Tile>().pieceNum = grid[i, rows - 1 - j].GetComponent<Tile>().pieceNum;
                grid[i, j].GetComponent<SpriteRenderer>().color = grid[i, rows - 1 - j].GetComponent<SpriteRenderer>().color;
                grid[i, rows - 1 - j].GetComponent<Tile>().pieceNum = temp;
                grid[i, rows - 1 - j].GetComponent<SpriteRenderer>().color = tempC;
            }
        }
    }
}
