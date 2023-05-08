using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        //mc.SetControlledCharacter(playerCharacters[0].GetComponent<CharacterInfo>());
        //turnManager.UpdateTurn();
        //turnManager.ExecutePlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCharacters.Count() == 0)
        {
            EndGameDefeat();
        }
        
        if(enemyCharacters.Count() == 0)
        {
            EndGameVictory();
    }
    }

    public void AddCharacterToPlayerList(GameObject character)
    {
        playerCharacters.Add(character);
    }

    public void AddCharacterToEnemyList(GameObject character)
    {
        enemyCharacters.Add(character);
    }

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

    public bool TileOccupiedByEnemyCharacter(OverlayTile tile)
    {
        foreach (var item in enemyCharacters)
        {
            if (item.GetComponent<CharacterInfo>().activeTile == tile)
            {
                return true;
            }
        }

        return false;
    }
    public CharacterInfo EnemyUnitOnTile(OverlayTile tile)
    {
        foreach (var item in enemyCharacters)
        {
            if (item.GetComponent<CharacterInfo>().activeTile == tile)
            {
                Debug.Log("Returning enemy: " + item);
                return item.GetComponent<CharacterInfo>();
            }
        }
        return null;
    }

    //functionality for player victory
    void EndGameVictory()
    {

    }

    //functionality for player defeat
    void EndGameDefeat()
    {

    }
}
