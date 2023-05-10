using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyMovement : CharacterInfo
{

    private List<OverlayTile> path = new List<OverlayTile>();

    public float minMoveTime;
    public float maxMoveTime;
    OverlayTile tile;

    void Start()
    {
        base.fullHeal();
        tile = base.activeTile;
        pathFinder = new PathFinder();
        tm = GameObject.Find("GameManager").GetComponent<TurnManager>();
        gm = GameObject.Find("GameManager").GetComponent<GeneralManager>();
        
    }


    void LateUpdate()
    {
        //Debug.Log("Enemy Path count: " + path.Count());
        //Allow the character to move along the map.
        if (path.Count > 0 && base.isMoving)
        {
            Debug.Log(gameObject.name + "is moving");
            base.MoveAlongPath(path);
        }
        base.KnockBackWrapper();

    }

    //wrapper for making enemy moves
    public void CallEnemyMovement()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true) 
        {
            tile = base.inRangeTiles[Random.Range(0, base.inRangeTiles.Count())];
            if (!base.gm.TileOccupiedByEnemyCharacter(tile) && !base.gm.TileOccupiedByPlayerCharacter(tile))
                break;
            
        }
        path = pathFinder.FindPath(base.activeTile, tile, base.inRangeTiles);
        Debug.Log("Setting enemy path to: " + path);

        base.isMoving = true;
        Debug.Log("Enemy moving to: " + tile.gridLocation2D);
        yield return new WaitUntil(() => !base.isMoving);
        Debug.Log("Enemy has finished moving.");
        tm.enemyMoved = true;
    }
}