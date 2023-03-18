using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ArrowTranslator;

public class MouseController : MonoBehaviour
{
    //Fields for variables and constructors.
    public float speed;
    public GameObject characterPrefab = null;
    private CharacterInfo character;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private ArrowTranslator arrowTranslator;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        arrowTranslator = new ArrowTranslator();
    }

    private bool isMoving = false;

    // Update is called once per frame
    void LateUpdate()
    {
        //Get the list of the Raycast2D function.
        var focusedTileHit = GetFocusedOnTile();

        //Once the Raycast list has its first object.
        if(focusedTileHit.HasValue)
        {
            //Set the overlay tile's position.
            OverlayTile tile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = tile.transform.position;

            //Position mouse where the tile is focused on.
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

            if(inRangeTiles.Contains(tile) && !isMoving)
            {
                path = pathFinder.FindPath(character.activeTile, tile, inRangeTiles);

                foreach(var item in inRangeTiles)
                {
                    mapManager.Instance.map[item.gridLocation2D].SetArrowSprite(ArrowDirection.None);
                }

                for(int i = 0; i < path.Count; i++)
                {
                    var previousTile = i > 0 ? path[i - 1] : character.activeTile;
                    var futureTile = i < path.Count - 1 ? path[i + 1] : null;

                    var arrowDirection = arrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
                    path[i].SetArrowSprite(arrowDirection);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                tile.GetComponent<OverlayTile>().ShowTile();

                if(character == null)
                {
                    character = Instantiate(characterPrefab).GetComponentInChildren<CharacterInfo>();
                    PositionCharacterOnTile(tile);
                    //Assign the movement range to the character.
                    GetInRangeTiles();
                }
                else
                {
                    isMoving = true;
                }
            }
        }

        //Allow the character to move along the map.
        if(path.Count > 0 && isMoving)
        {
            MoveAlongPath();
        }
    }

    private void GetInRangeTiles()
    {
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }

        //3 represents the movement range of the character.
        inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, 4);

        foreach(var item in inRangeTiles)
        {
            item.ShowTile();
        }
    }

    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;

        //Reference to the node's z position. Otherwise Vector3 would set z to 0. 
        var zIndex = path[0].transform.position.z;
        //Identify x and y values.
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);
        //Add zIndex along with the Vector2 into a new Vector3.
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        //0.0001f is for rendering the character correctly, say when a block is in front of them.
        if(Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

        if(path.Count == 0)
        {
            GetInRangeTiles();

            //Set to false, otherwise would only work for one movement.
            isMoving = false;
        }
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
        if(hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        return null;
    }

    //Position and render the character on the overlay tile.
    private void PositionCharacterOnTile(OverlayTile tile)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        character.activeTile = tile;
    }
}
