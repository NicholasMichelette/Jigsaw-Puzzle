using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    private Vector2 followOffset;
    private GameObject objFollowing;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null) {
                if (hit.collider.gameObject.GetComponent<Tile>() != null && hit.collider.gameObject.GetComponent<Tile>().pieceNum == 0)
                {
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                    hit.collider.gameObject.GetComponent<Tile>().pieceNum = -1;
                }
                else if (hit.collider.gameObject.GetComponent<Tile>() != null && hit.collider.gameObject.GetComponent<Tile>().pieceNum == -1)
                {
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    hit.collider.gameObject.GetComponent<Tile>().pieceNum = 0;
                }
                else if (hit.collider.gameObject.GetComponent<Tile>() != null && hit.collider.gameObject.transform.parent.GetComponent<Piece>() != null)
                {
                    objFollowing = hit.collider.gameObject.transform.parent.gameObject;
                    followOffset = new Vector2(mousePos2D.x - hit.collider.gameObject.transform.parent.position.x, mousePos2D.y - hit.collider.gameObject.transform.parent.position.y);
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
        }
        if(Input.GetMouseButtonUp(0))
        {
            objFollowing = null;
            //tbd
        }
        if(Input.GetMouseButtonDown(2))
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
