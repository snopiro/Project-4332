using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class mapManager : MonoBehaviour
{
    //Immutable singleton; (Globally accessible class that exists only once in a scene.) (Benefits accessing the map from the dictionary all the time.)
    private static mapManager _instance;
    public static mapManager Instance { get { return _instance; } }

    //Fields for applying the prefab and empty object.
    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    //Store all the overlay tiles using the grid location as the key. (Essentially the map itself) (Good for searching through lists.)
    public Dictionary<Vector2Int, OverlayTile> map;

    //Instantiate the singleton.
    private void Awake()
    {
        //If it already exists, destroy it. Otherwise create a new one.
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update.
    void Start()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();

        //Instantiate the map.
        map = new Dictionary<Vector2Int, OverlayTile>();

        //Limits of the map.
        BoundsInt bounds = tileMap.cellBounds;

        //Loop through all of our tiles for the bounds of the map, including ones at z position 0.
        for(int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);

                    //In case there is not a tile within the bounds of the map.
                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        //Instantiate the overlay prefab within the container.
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);

                        //Get world position of the tile's vector location.
                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        //Allow selected tile to hover over any tile regardless of elevation.
                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z+1);

                        //Ensure the overlay tile will always be sorted correctly in rendering.
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }
    }

    //Get the location of the adjacent tiles.
    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> searchTiles)
    {
        Dictionary<Vector2Int, OverlayTile> tilesToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if(searchTiles.Count > 0)
        {
            foreach(var item in searchTiles)
            {
                tilesToSearch.Add(item.gridLocation2D, item);
            }
        }
        else
        {
            tilesToSearch = map;
        }

        List<OverlayTile> neighbours = new List<OverlayTile>();

        //Value of 1 would technically be defining our jump height as well, if we wish to do that.
        //Top tile.
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);
        if (tilesToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tilesToSearch[locationToCheck].gridLocation.z) <= 1)
                neighbours.Add(tilesToSearch[locationToCheck]);
        }

        //Bottom tile.
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);
        if (tilesToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tilesToSearch[locationToCheck].gridLocation.z) <= 1)
                neighbours.Add(tilesToSearch[locationToCheck]);
        }

        //Left tile.
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y);
        if (tilesToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tilesToSearch[locationToCheck].gridLocation.z) <= 1)
                neighbours.Add(tilesToSearch[locationToCheck]);
        }

        //Right tile.
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y);
        if (tilesToSearch.ContainsKey(locationToCheck))
        {
            if (Mathf.Abs(currentOverlayTile.gridLocation.z - tilesToSearch[locationToCheck].gridLocation.z) <= 1)
                neighbours.Add(tilesToSearch[locationToCheck]);
        }

        return neighbours;
    }

    //finds and returns overlaytile with specified coordinates
    public OverlayTile FindTileInScene(Vector2Int coordinates)
    {
        OverlayTile tile;
        if (map.TryGetValue(coordinates, out tile))
        {
            return tile;
        }
        return null;
    }
}
