using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArrowTranslator;

public class OverlayTile : MonoBehaviour
{
    //A* pathfinding variables.
    public int G;
    public int H;
    public int F { get { return G + H; } }

    public bool isBlocked;

    public OverlayTile previous;

    public Vector3Int gridLocation;
    public Vector2Int gridLocation2D { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    public List<Sprite> arrows;
public void ShowTile()
    {
        //Make visible once rendered.
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void HideTile()
    {
        //Make invisible before render
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        SetArrowSprite(ArrowDirection.None);
    }


    public void SetArrowSprite(ArrowDirection d)
    {
        var arrow = GetComponentsInChildren<SpriteRenderer>()[1];
        if(d == ArrowDirection.None)
        {
            //Make arrow invisible before render of cursor.
            arrow.color = new Color(1, 1, 1, 0);
        }
        else
        {
            //Make visible and show appropriate sprite.
            arrow.color = new Color(1, 1, 1, 1);
            arrow.sprite = arrows[(int)d];
            arrow.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        }
    }
}
