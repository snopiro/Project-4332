using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

 
public class TurnManager : MonoBehaviour
{

    private GeneralManager gm;
    public MouseController mc;
    private int playerIndex;
    private int enemyIndex;

    //enumeration to control player or enemy turn
    public enum Turn { Player, Enemy };
    public Turn turn;

    public float delayTime = 1.0f;
    bool shouldUpdateTurn;
    bool playerInputLock;

    // Start is called before the first frame update
    void Start()
    {
        gm = GetComponent<GeneralManager>();
        turn = Turn.Player;
        playerIndex = 0;
        enemyIndex = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //updates turn and turns off the boolean so it doesnt automatically update again next frame...
        if (shouldUpdateTurn) {
            Debug.Log("Current turn: " + turn);
            shouldUpdateTurn = false;
            if (turn == Turn.Player)
            {
                StartCoroutine(PlayerTurn());
            }
            else if (turn == Turn.Enemy)
            {
                StartCoroutine(EnemyTurn());
            }
        }

    }

    //Coroutines for player and enemy turn
    private IEnumerator PlayerTurn()
    {
        //***player turn code goes here***
        Debug.Log("Executing Player turn!");
        mc.SetControlledCharacter(gm.playerCharacters[playerIndex].GetComponent<CharacterInfo>());

        playerInputLock = true;
        //sets player index to next in line
        if (playerIndex == gm.playerCharacters.Count() - 1)
        {
            playerIndex = 0; 
        }
        else
        {
            playerIndex++;
        }

        Debug.Log("Setting player index to: " + playerIndex);

        //waits certain amount of time before updating next turn
        yield return new WaitForSeconds(delayTime);
        playerInputLock = false;
        //shouldUpdateTurn = true;
    }

    private IEnumerator EnemyTurn()
    {
        if(enemyIndex == 0)
        {
            yield return new WaitForSeconds(delayTime);
        }

        //***enemy turn code goes here***
        Debug.Log("Executing Enemy Turn!");
        gm.enemyCharacters[enemyIndex].GetComponent<EnemyMovement>().CallEnemyMovement();

        //sets enemy index to next in line
        if (enemyIndex == gm.enemyCharacters.Count() - 1)
        {
            Debug.Log("Changing to player turn...");
            enemyIndex = 0;
            turn = Turn.Player;
        }
        else
        {
            enemyIndex++;
        }
        Debug.Log("Setting enemy index to: " + enemyIndex);


        //waits certain amount of time before updating next turn
        yield return new WaitForSeconds(delayTime);
        shouldUpdateTurn = true;
    }

    public void UpdateTurn()
    {
        if(playerIndex == gm.playerCharacters.Count() - 1)
        {
            Debug.Log("Changing to enemy turn...");
            turn = Turn.Enemy;
        }

        shouldUpdateTurn = true;
    }

    public bool GetPlayerTurn()
    {
        if (turn == Turn.Player)
        {
            //Debug.Log("returning player turn as true...");
            return true;

        }
        else 
        {
            //Debug.Log("returning player turn as false...");
            return false;
        }
    }

    public bool GetPlayerInputLock()
    {
        return playerInputLock;
    }
}
