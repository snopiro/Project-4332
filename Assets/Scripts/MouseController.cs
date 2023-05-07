using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ArrowTranslator;

public class MouseController : MonoBehaviour
{
    //Fields for variables and constructors.
    public GameObject characterPrefab = null;
    private CharacterInfo character;

    private PathFinder pathFinder;
    private ArrowTranslator arrowTranslator;
    private List<OverlayTile> path = new List<OverlayTile>();
    public GeneralManager gm;
    public TurnManager tm;

    private void Start()
    {
        pathFinder = new PathFinder();
        arrowTranslator = new ArrowTranslator();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Get the list of the Raycast2D function.
        var focusedTileHit = GetFocusedOnTile();

        //Once the Raycast list has its first object.
        if (focusedTileHit.HasValue)
        {
            //Set the overlay tile's position.
            OverlayTile tile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = tile.transform.position;

            //Position mouse where the tile is focused on.
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

            if (character != null && character.inRangeTiles.Contains(tile) && !character.isMoving)
            {
                if (!tm.GetPlayerInputLock() && !character.isAttacking)
                {
                    if (character != null && character.inRangeTiles.Contains(tile) && !character.isMoving)
                    {
                        path = pathFinder.FindPath(character.activeTile, tile, character.inRangeTiles);

                        foreach (var item in character.inRangeTiles)
                        {
                            mapManager.Instance.map[item.gridLocation2D].SetArrowSprite(ArrowDirection.None);
                        }

                        for (int i = 0; i < path.Count; i++)
                        {
                            var previousTile = i > 0 ? path[i - 1] : character.activeTile;
                            var futureTile = i < path.Count - 1 ? path[i + 1] : null;

                            var arrowDirection = arrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
                            path[i].SetArrowSprite(arrowDirection);
                        }
                    }
                }
            }


            if (Input.GetMouseButtonDown(0) && !character.isAttacking)
            {
                Debug.Log("Current Tile: " + tile.gridLocation);
                Debug.Log("Character: " + character);
                if (tm.GetPlayerTurn() && character.inRangeTiles.Contains(tile) && !tm.GetPlayerInputLock() && !gm.TileOccupiedByPlayerCharacter(tile))
                {
                    Debug.Log("Updating Turn...");
                    tile.GetComponent<OverlayTile>().ShowTile(Color.white);
                    character.isMoving = true;
                    StartCoroutine(SendPlayerMovement());
                }
            }
            else if (Input.GetMouseButtonDown(0) && character.isAttacking) //attacking function
            {
                if (tm.GetPlayerTurn() && character.inRangeTiles.Contains(tile) && !tm.GetPlayerInputLock() && !gm.TileOccupiedByPlayerCharacter(tile))
                {
                    gm.EnemyUnitOnTile(tile).receiveDamage(character.attackStat);
                }

            }
            else if (Input.GetMouseButtonDown(1) && tm.turn == TurnManager.Turn.Player)
            {
                tm.SendPlayerAttack(); //skip turn on right click
            }
        }
    

        //Allow the character to move along the map.
        if (path.Count > 0 && character.isMoving)
        {
            character.MoveAlongPath(path);
        }


    }

    IEnumerator SendPlayerMovement()
    {
        yield return new WaitUntil(() => !character.isMoving);
        tm.SendPlayerInput();
    }
    //Position cursor where the mouse is.
    public RaycastHit2D? GetFocusedOnTile()
    {
        //Convert the 3D space of the mouse's world position into a 2D vector.
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        //Draw the imaginary line of the raycast to the mouse position, and returns an array of what the line went across.
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        //Once the line has found its first object.
        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        return null;
    }

    //sets character to be controlled by the mouse.
    public void SetControlledCharacter(CharacterInfo chara)
    {
        if(character != null)
            character.isActivelyControlled = false;
        Debug.Log("Setting character to " + chara);
        character = chara;
        character.isActivelyControlled = true;
    }

    public CharacterInfo GetControlledCharacter()
    {
        return character;
    }
}
