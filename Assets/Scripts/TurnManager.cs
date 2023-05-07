using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class TurnManager : MonoBehaviour
{

    //enumeration to control player or enemy turn
    enum Turn { Player, Enemy };
    Turn turn;

    public float delayTime = 1.0f;
    bool shouldUpdateTurn;
<<<<<<< Updated upstream
=======
    bool playerHasInputted;
    bool playerHasMoved;
    bool playerHasAttacked;
    bool playerInputLock;
>>>>>>> Stashed changes

    // Start is called before the first frame update
    void Start()
    {
        turn = Turn.Player;
        shouldUpdateTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        //updates turn and turns off the boolean so it doesnt automatically update again next frame...
        if (shouldUpdateTurn) {
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
<<<<<<< Updated upstream
    private IEnumerator PlayerTurn()
    {
        //***player turn code goes here***
=======
    //playerInputLock prevents the player from making inputs outside of their turn.
    //playerHasMoved gets set to true in MouseController.cs after player selects a square to move to
    private IEnumerator PlayerTurn()
    {
        Debug.Log("Executing Player turn!");
        playerHasInputted = false;
        playerHasMoved = false;
        playerHasAttacked = false;
        playerInputLock = false;
        //***player turn code goes here***
        mc.SetControlledCharacter(gm.playerCharacters[playerIndex].GetComponent<CharacterInfo>());
        mc.GetControlledCharacter().ShowInRangeTiles(Color.white);

        Debug.Log("Waiting for player input...");
        yield return new WaitUntil(() => playerHasInputted);
        Debug.Log("Received player input!!!");
        mc.GetControlledCharacter().HideInRangeTiles();
        //--- above this is working as intended ---//

        Debug.Log("Waiting for player to finish moving...");
        yield return new WaitUntil(() => playerHasMoved);
        Debug.Log("Player is done moving.");
        mc.GetControlledCharacter().HideInRangeTiles();

        Debug.Log("Showing player tiles...");
        mc.GetControlledCharacter().GetInRangeTiles();
        mc.GetControlledCharacter().ShowInRangeTiles(Color.red);
        Debug.Log("Waiting for player to attack");
        yield return new WaitUntil(() => playerHasAttacked);

        playerInputLock = true;
        mc.GetControlledCharacter().HideInRangeTiles();
>>>>>>> Stashed changes



        //if (playercharacterarrayindex == last) ***pseudocode for changing to enemy turn when there are more than 1 player controlled characters***
        //(
        turn = Turn.Enemy;

        //)
        //waits certain amount of time before updating next turn
        yield return new WaitForSeconds(delayTime);
        shouldUpdateTurn = true;
    }

    private IEnumerator EnemyTurn()
    {
        //***enemy turn code goes here***



        //if (enemycharacterarrayindex == last) ***pseudocode for changing to enemy turn when there are more than 1 enemy characters***
        //(
        turn = Turn.Player;

        //)
        //waits certain amount of time before updating next turn
        yield return new WaitForSeconds(delayTime);
        shouldUpdateTurn = true;
    }

<<<<<<< Updated upstream
=======

    public void SendPlayerInput()
    {
        //player has input where to move
        playerHasInputted = true;
    }

    public void SendPlayerMovement()
    {
        //"player has moved"
        playerHasMoved = true;
    }

    public void SendPlayerAttack()
    {
        playerHasAttacked = true;
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
>>>>>>> Stashed changes
}
