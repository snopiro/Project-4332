using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private CharacterInfo character;
    public OverlayTile activeTile;
    private List<OverlayTile> path = new List<OverlayTile>();
    public float moveSpeed;
    public float range;
    public float minMoveTime;
    public float maxMoveTime;

    private Vector2[] moveDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private bool isMoving = false;

    void Start()
    {
        // Set initial movement direction
        Vector2 direction = moveDirections[Random.Range(0, moveDirections.Length)];
        //StartCoroutine(Move(direction));
    }

        IEnumerator Move(Vector2 direction)
    {
        isMoving = true;

        // Calculate target position based on range
        Vector2 targetPos = (Vector2)transform.position + direction * range;

        // Hardcoded A* pathfinding
        List<Vector2> path = new List<Vector2>();
        path.Add(transform.position);
        path.Add(targetPos);

        // Move towards target position
        float moveTime = Random.Range(minMoveTime, maxMoveTime);
        float elapsed = 0f;
        while (elapsed < moveTime)
        {
            transform.position = Vector2.Lerp(transform.position, targetPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        // Wait until the next turn to move again.
       //**************

        // Set new movement direction and move again.
        //direction = moveDirections[Random.Range(0, moveDirections.Length)];
        //StartCoroutine(Move(direction));
        //***************
    }

    void Update()
    {
        // Prevent movement while already moving
        if (isMoving)
        {
            return;
        }
    }
}