using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//No MonoBehaviour means that it will not be attached to an object as a component in Unity.
public class PathFinder
{
    //Find the start and end to the path, as well as a list to know the character's bounds.
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> searchTiles)
    {
        //List of tiles we want to get/have.
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closedList = new List<OverlayTile>();

        //Add the starting point to the list.
        openList.Add(start);

        //While the pathfinding has not ended.
        while(openList.Count > 0)
        {
            //Find the tile with the lowest F cost, or, the tile with the least unique tiles to the destination.
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

            //Once the tile has been used, remove from the open list and add it to the current one, as a tile that we have passed.
            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if(currentOverlayTile == end)
            {
                //Finalize our path once the destination has been found.
                return GetFinishedList(start, end);
            }

            //Get the set of adjacent tiles from our current tile the character is on, from the dictionary used in mapManager.
            var neighbourTiles = mapManager.Instance.GetNeighbourTiles(currentOverlayTile, searchTiles);

            //Check if the neighbor tiles are valid, and if so, add them to open list.
            foreach(var neighbour in neighbourTiles)
            {
                //Contine once any one of these conditions is satisfied. (Last one might be responsible for letting character register water tiles still?)
                if (neighbour.isBlocked || closedList.Contains(neighbour) || Mathf.Abs(currentOverlayTile.gridLocation.z - neighbour.gridLocation.z) > 1)
                {
                    continue;
                }

                //Get the distance from both start node -> current node/current node -> end node.
                neighbour.G = GetManhattenDistance(start, neighbour);
                neighbour.H = GetManhattenDistance(end, neighbour);

                //Step through all of our previous tiles.
                neighbour.previous = currentOverlayTile;

                //Add any neighboring tile to the list if not accounted for yet, in case of duplicates.
                if(!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }

        return new List<OverlayTile>();
    }

    //Final List of tiles.
    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();

        OverlayTile currentTile = end;

        //Established the best possible path and looking through all past nodes that has been travelled along to get there.
        while(currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }

        //Put the list back in proper order.
        finishedList.Reverse();

        return finishedList;
    }

    //Calculate distance using tiles only. Does not consider diagonals.
    private int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
    {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }
}
