using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    private SpawnManager spawnManager;
    private TurnManager turnManager;
    public MouseController mc;
    public List<GameObject> playerCharacters;
    public List<GameObject> enemyCharacters;

    // Start is called before the first frame update
    void Start()
    {
        turnManager = GetComponent<TurnManager>();
        spawnManager = GetComponent<SpawnManager>();

        StartCoroutine(LateStart(1.0f));
    }

    //waits specified amount of time before executing functions
    private IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Your Function You Want to Call
        //sets player character to first in spawn manager list
        mc.SetControlledCharacter(playerCharacters[0].GetComponent<CharacterInfo>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCharacterToPlayerList(GameObject character)
    {
        playerCharacters.Add(character);
    }

    public void AddCharacterToEnemyList(GameObject character)
    {
        enemyCharacters.Add(character);
    }
<<<<<<< Updated upstream
=======

    //returns true if there is a player character whose activeTile is asked tile
    //i.e. if there's a player on said tile
    public bool TileOccupiedByPlayerCharacter(OverlayTile tile)
    {
        foreach(var item in playerCharacters)
        {
            if(item.GetComponent<CharacterInfo>().activeTile == tile)
            {
                return true;
            }
        }
        return false;
    }

    public CharacterInfo UnitOnTile(OverlayTile tile)
    {
        foreach (var item in enemyCharacters) {
            if(item.GetComponent<CharacterInfo>().activeTile == tile)
            {
                return item.GetComponent<CharacterInfo>();
            }
        }
        return null;
    }
    
>>>>>>> Stashed changes
}
