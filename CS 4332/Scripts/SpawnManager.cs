using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    //arrays for holding which characters to spawn. Add characters to these in the inspector
    public GameObject[] playerCharacters;
    public GameObject[] enemyCharacters;
    public Vector2Int playerSpawn;
    public Vector2Int enemySpawn;

    public GeneralManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GetComponent<GeneralManager>();
        StartCoroutine(LateStart(.1f));
    }

    //waits specified amount of time before executing functions
    private IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Your Function You Want to Call
        SetupSpawn();
    }


    //spawns characters at specified tiles.
    private void SetupSpawn()
    {
        OverlayTile tile;
        foreach (var character in playerCharacters)
        {
            tile = mapManager.Instance.FindTileInScene(playerSpawn);
            gm.AddCharacterToPlayerList(SpawnCharacterOnTile(character, tile));
            //edit this line below to change how players are placed after the first one
            playerSpawn = new Vector2Int(playerSpawn.x + 1, playerSpawn.y);
        }

        foreach (var character in enemyCharacters)
        {
            tile = mapManager.Instance.FindTileInScene(enemySpawn);
            gm.AddCharacterToEnemyList(SpawnCharacterOnTile(character, tile));
            //edit this line below to change how enemies are placed after the first one
            enemySpawn = new Vector2Int(enemySpawn.x + 1, enemySpawn.y);
        }
    }

    //Spawn character at location in the world and returns that character
    public GameObject SpawnCharacterOnTile(GameObject characterPrefab, OverlayTile tile)
    {
        Debug.Log("Spawning character " + characterPrefab);
        GameObject character = Instantiate(characterPrefab);
        CharacterInfo ci = character.GetComponent<CharacterInfo>();
        ci.PositionCharacterOnTile(tile);
        ci.GetInRangeTiles();
        return character;
    }

    
}
