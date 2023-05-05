using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder
{
    public List<OverlayTile> GetTilesInRange(OverlayTile startTile, int range)
    {
        //Make a list for the specified number of adjacent tiles the character can move.
        var inRangeTiles = new List<OverlayTile>();
        //Step count to know how many searches we've done.
        int stepCount = 0;

        //Add starting tiles to the in range tiles.
        inRangeTiles.Add(startTile);

        //List that contains the previous steps.
        var prevSteps = new List<OverlayTile>();
        prevSteps.Add(startTile);

        //Loop through the map to find all the tiles within range.
        while(stepCount < range)
        {
            //List for adjacent/neighboring tiles.
            var surroundTiles = new List<OverlayTile>();

            //Iterate through our step count in the previous step(s).
            foreach(var item in prevSteps)
            {
                //Add all neighboring tiles to the surrounding set of tiles.
                surroundTiles.AddRange(mapManager.Instance.GetNeighbourTiles(item, new List<OverlayTile>()));
            }

            //Add the new amount of surrounding tiles from the foreach loop to the in range tiles.
            inRangeTiles.AddRange(surroundTiles);
            //Only add unique values, in case of doubles from rendering issues.
            prevSteps = surroundTiles.Distinct().ToList();
            stepCount++;
        }

        return inRangeTiles.Distinct().ToList();
    }
}
