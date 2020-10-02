﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    private Vector2 followOffset;
    private GameObject objFollowing;
    private PieceManager pm;
    private GridBase gb;

    void Start()
    {
        pm = FindObjectOfType<PieceManager>();
        gb = FindObjectOfType<GridBase>();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null) {
                Tile t = hit.collider.gameObject.GetComponent<Tile>();
                if (t != null)
                {
                    if (t.pieceNum == 0)
                    {
                        hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                        t.pieceNum = -1;
                    }
                    else if(t.pieceNum == -1)
                    {
                        hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                        t.pieceNum = 0;
                    }
                    else if(t.pieceNum > 0)
                    {
                        if (hit.collider.gameObject.transform.parent.GetComponent<Piece>() != null)
                        {
                            objFollowing = hit.collider.gameObject.transform.parent.gameObject;
                        }
                        else if(hit.collider.gameObject.transform.parent.GetComponent<GridBase>() != null)
                        {
                            objFollowing = pm.activatePiece(t.pieceNum);

                            Piece p = objFollowing.GetComponent<Piece>();
                            for(int i = 0; i < p.columns; i++)
                            {
                                for(int j = 0; j < p.rows; j++)
                                {
                                    GameObject tileObj = p.grid[i, j];
                                    if (tileObj.GetComponent<Tile>().pieceNum > 0)
                                    {
                                        Collider2D[] temp = Physics2D.OverlapBoxAll(tileObj.gameObject.transform.position, new Vector2(1, 1), 0, 1 << 8);
                                        if (temp.Length > 0)
                                        {
                                            Tile t2 = temp[0].GetComponent<Tile>();
                                            float dist = Vector2.Distance(tileObj.gameObject.transform.position, temp[0].gameObject.transform.position);
                                            for (int k = 1; k < temp.Length; k++)
                                            {
                                                float tempD = Vector2.Distance(tileObj.gameObject.transform.position, temp[k].gameObject.transform.position);
                                                if (dist > tempD)
                                                {
                                                    t2 = temp[k].GetComponent<Tile>();
                                                    dist = tempD;
                                                }
                                            }

                                            t2.pieceNum = 0;
                                            t2.GetComponent<SpriteRenderer>().color = Color.white;
                                        }
                                    }
                                }
                            }
                        }
                        followOffset = new Vector2(mousePos2D.x - objFollowing.transform.position.x, mousePos2D.y - objFollowing.transform.position.y);
                    }
                }
            }
        }

        if(objFollowing != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            objFollowing.transform.position = mousePos2D - followOffset;
            if (Input.GetKeyDown("f"))
            {
                objFollowing.GetComponent<Piece>().flipX();
            }
            if(Input.GetKeyDown("r"))
            {
                objFollowing.GetComponent<Piece>().flipY();
            }
            if(Input.GetKeyDown("z"))
            {
                pm.removePiece(objFollowing);
                objFollowing = null;
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            if (objFollowing != null)
            {
                Piece p = objFollowing.GetComponent<Piece>();
                List<Tile> tilesToChange = new List<Tile>();
                bool fits = true;
                Color c = new Color(0, 0, 0);
                for(int i = 0; i < p.columns; i++)
                {
                    for(int j = 0; j < p.rows; j++)
                    {
                        Tile t = p.grid[i, j].GetComponent<Tile>();
                        if (t.pieceNum > 0)
                        {
                            c = t.GetComponent<SpriteRenderer>().color;
                            Collider2D[] temp = Physics2D.OverlapBoxAll(t.gameObject.transform.position, new Vector2(1, 1), 0, 1 << 8);
                            if (temp.Length > 0)
                            {
                                Tile t2 = temp[0].GetComponent<Tile>();
                                float dist = Vector2.Distance(t.gameObject.transform.position, temp[0].gameObject.transform.position);
                                for (int k = 1; k < temp.Length; k++)
                                {
                                    float tempD = Vector2.Distance(t.gameObject.transform.position, temp[k].gameObject.transform.position);
                                    if (dist > tempD)
                                    {
                                        t2 = temp[k].GetComponent<Tile>();
                                        dist = tempD;
                                    }
                                }

                                if (t2.pieceNum == 0)
                                {
                                    tilesToChange.Add(t2);
                                }
                                else
                                    fits = false;
                            }
                            else
                                fits = false;
                        }
                    }
                    
                }
                if (fits)
                {
                    foreach (Tile t in tilesToChange)
                    {
                        t.pieceNum = p.pieceNum;
                        t.GetComponent<SpriteRenderer>().color = c;
                    }
                    p.gameObject.SetActive(false);
                }

                objFollowing = null;
            }
        }
        if(Input.GetKeyUp(KeyCode.S))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<Tile>() != null && hit.collider.gameObject.transform.parent.GetComponent<Piece>() != null)
                {
                    hit.collider.gameObject.transform.parent.GetComponent<Piece>().setPieces();
                }
            }
        }
    }
}
