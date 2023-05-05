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
    private IEnumerator PlayerTurn()
    {
        //***player turn code goes here***



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

}
