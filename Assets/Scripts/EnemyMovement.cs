using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyMovement : CharacterInfo
{

    private List<OverlayTile> path = new List<OverlayTile>();
    private PathFinder pathFinder;

    public float minMoveTime;
    public float maxMoveTime;
    OverlayTile tile;
    private GeneralManager gm;

    private Vector2[] moveDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

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
            Debug.Log("Moving enemy");
            base.MoveAlongPath(path);
        }


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
            if (!gm.TileOccupiedByEnemyCharacter(tile) && !gm.TileOccupiedByPlayerCharacter(tile))
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