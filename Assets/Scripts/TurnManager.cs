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
    bool playerHasInputted;
    bool playerInputLock;

    // Start is called before the first frame update
    void Awake()
    {
        gm = GetComponent<GeneralManager>();
        turn = Turn.Player;
        playerIndex = 0;
        enemyIndex = 0;
        shouldUpdateTurn = false;
    }

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1.1f);
        shouldUpdateTurn = true;
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
    //playerInputLock prevents the player from making inputs outside of their turn.
    //playerHasInputted gets set to true in MouseController.cs after player selects a square to move to
    private IEnumerator PlayerTurn()
    {
        Debug.Log("Executing Player turn!");
        playerHasInputted = false;
        playerInputLock = false;
        //***player turn code goes here***
        mc.SetControlledCharacter(gm.playerCharacters[playerIndex].GetComponent<CharacterInfo>());

        Debug.Log("Waiting for player input...");
        yield return new WaitUntil(() => playerHasInputted);
        Debug.Log("Received player input!!!");
        playerInputLock = true;


        //*** player turn code goes above here***
        //waits certain amount of time before updating next turn
        yield return new WaitForSeconds(delayTime);
        //sets player index to next in line
        if (playerIndex == gm.playerCharacters.Count() - 1)
        {
            playerIndex = 0;
            Debug.Log("Changing to enemy turn...");
            turn = Turn.Enemy;
        }
        else
        {
            playerIndex++;
        }
        Debug.Log("Setting player index to: " + playerIndex);

        shouldUpdateTurn = true;
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

        


        //***enemy turn code goes above here***
        //waits certain amount of time before updating next turn
        yield return new WaitForSeconds(delayTime);
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

        shouldUpdateTurn = true;
    }

    public void SendPlayerInput()
    {
        playerHasInputted = true;
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
