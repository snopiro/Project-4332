using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{

    public float speed;
    public bool isMoving = false;
    public OverlayTile activeTile;
    protected RangeFinder rangeFinder;
    protected PathFinder pathFinder;
    protected GeneralManager gm;
    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    public bool playerControlled;
    public bool isActivelyControlled;
    public TurnManager tm;
    public bool isAttacking = false;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private Healthbar healthbar;

    public GameObject movingAudio;

    public int attackStat;
    public int range;
    public int attackRange;

    List<OverlayTile> knockbackPath = new List<OverlayTile>();
    Vector2Int knockBackDirection;
    bool enableKnockback;
    bool sendFlying = false;
    Vector3 worldSpaceKnockback;
    int kbMultiplier;



    private void Start()
    {
        isActivelyControlled = false;
        speed = 3.0f;

        currentHealth = maxHealth;
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
    }
    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GeneralManager>();
        tm = GameObject.Find("GameManager").GetComponent<TurnManager>();
        rangeFinder = new RangeFinder();
        pathFinder = new PathFinder();
        enableKnockback = false;
    }

    private void Update()
    {
        if (movingAudio)
        {
            if (isMoving && !enableKnockback)
                movingAudio.SetActive(true);
            else
                movingAudio.SetActive(false);
        }

        if (sendFlying)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + translateVector(knockBackDirection), 5 * Time.deltaTime);
        }

        
    }
    private void LateUpdate()
    {

        KnockBackWrapper();
        
    }

    public void MoveAlongPath(List<OverlayTile> path)
    {
        var step = speed * Time.deltaTime;

        //Reference to the node's z position. Otherwise Vector3 would set z to 0. 
        var zIndex = path[0].transform.position.z;
        //Identify x and y values.
        Vector2 destination = new Vector2(path[0].transform.position.x, path[0].transform.position.y + 0.5f);
        transform.position = Vector2.MoveTowards(transform.position, destination, step);
        //character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);
        //Add zIndex along with the Vector2 into a new Vector3.
        transform.position = new Vector3(transform.position.x, transform.position.y, zIndex);

        //0.0001f is for rendering the character correctly, say when a block is in front of them.
        if (Vector2.Distance(transform.position, destination) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {
            HideInRangeTiles();
            tm.SendPlayerInput();
            tm.SendPlayerMovement();

            //Set to false, otherwise would only work for one movement.
            isMoving = false;
            //currentHealth -= 1;
            healthbar.UpdateHealthBar(maxHealth, currentHealth);
        }

    }

    public void KnockBackWrapper()
    {
        Debug.Log(gameObject.name + "enableKnockback: " + enableKnockback);
        Debug.Log(gameObject.name + " knockback path count: " + knockbackPath.Count);
        if (knockbackPath.Count > 0 && enableKnockback)
        {
            Debug.Log("Knocking back " + gameObject.name);
            KnockBackCharacter(knockbackPath);
        }
    }
    public void KnockBackCharacter(List<OverlayTile> path)
    {
        Debug.Log("Executing knockback");
        var step = 10 * Time.deltaTime;

        //Reference to the node's z position. Otherwise Vector3 would set z to 0. 
        var zIndex = path[0].transform.position.z;
        //Identify x and y values.
        Vector2 destination = new Vector2(path[0].transform.position.x, path[0].transform.position.y + 0.5f);
        transform.position = Vector2.MoveTowards(transform.position, destination, step);
        //character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);
        //Add zIndex along with the Vector2 into a new Vector3.
        transform.position = new Vector3(transform.position.x, transform.position.y, zIndex);

        //0.0001f is for rendering the character correctly, say when a block is in front of them.
        if (Vector2.Distance(transform.position, destination) < 0.0001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {
            //Set to false, otherwise would only work for one movement.
            enableKnockback = false;
            healthbar.UpdateHealthBar(maxHealth, currentHealth);
        }
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f + 0.5f, tile.transform.position.z);
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        activeTile = tile;
    }
    public void GetInRangeTiles()
    {
        if (isAttacking)
        {
            inRangeTiles = rangeFinder.GetTilesInRange(activeTile, attackRange);
        }
        else
        {
            inRangeTiles = rangeFinder.GetTilesInRange(activeTile, range);
        }
    }

    public void ShowInRangeTiles(Color c)
    {
            foreach (var item in inRangeTiles)
            {
            item.ShowTile(c);
            }
        }

    public void HideInRangeTiles()
        {
            foreach (var item in inRangeTiles)
            {
            item.HideTile();
        }

        if (isAttacking)
        {
            GetInRangeTiles();
            }
        }

    public void receiveDamage(int damage, CharacterInfo source)
    {
        GameObject.Find("HitSound").GetComponent<AudioSource>().Play();
        //knock back multiplier equation
        kbMultiplier = (int)(4 * (1.0f - currentHealth/maxHealth)) + 1;
        currentHealth -= damage;
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Starting coroutine: death");
            StartCoroutine(Death(1.0f));
        }
        else
        {
            StartCoroutine(KnockBack(source));
        }
    }

    IEnumerator Death(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (playerControlled)
            GameObject.Find("GameManager").GetComponent<GeneralManager>().RemovePlayerCharacterFromList(this.gameObject);
        
        else
            GameObject.Find("GameManager").GetComponent<GeneralManager>().RemoveEnemyCharacterFromList(this.gameObject);
        gameObject.SetActive(false);
        tm.ProcessEnemyDeath();
    }

    //knocks back 
    IEnumerator KnockBack(CharacterInfo source)
    {
        knockBackDirection = new Vector2Int(source.activeTile.gridLocation2D.x - activeTile.gridLocation2D.x, source.activeTile.gridLocation2D.y - activeTile.gridLocation2D.y);
        OverlayTile tileToKnockbackTo = mapManager.Instance.FindTileInScene(activeTile.gridLocation2D - knockBackDirection * kbMultiplier);
        Debug.Log("knockback Direction: " + knockBackDirection);
        //if tile to Knockback to exists and there isn't a unit on it
        if (tileToKnockbackTo && !gm.TileOccupiedByPlayerCharacter(tileToKnockbackTo) && !gm.TileOccupiedByEnemyCharacter(tileToKnockbackTo))
        {
            Debug.Log("tileToKnockbackTo: " + tileToKnockbackTo.gridLocation2D);
            Debug.Log("Knocking back");
            knockbackPath = pathFinder.FindPath(activeTile, tileToKnockbackTo, mapManager.Instance.GetEntireMap());
            enableKnockback = true;
            yield return new WaitUntil(() => !enableKnockback);
            GetInRangeTiles();
            tm.ProcessEnemyDeath();
        }
        //if it doesnt exist in the map then it must be outside of the map;
        else if (!tileToKnockbackTo)
        {
            sendFlying = true;
            Debug.Log("Ring Out!!!");
            StartCoroutine(Death(0.6f));
        }
        //last condition is that there is a unit, in which case there is no knockback for simplicity
        else
        {
            tm.ProcessEnemyDeath();
        }
    }
    public void fullHeal()
    {
        currentHealth = maxHealth;
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
    }

    //takes the vector direction and rotates it 135 degrees for correct knockback off map
    Vector3 translateVector(Vector2Int isometricVector)
    {
        float rotationAngle = 140.0f;

        // Convert from degrees to radians
        float radians = rotationAngle * Mathf.Deg2Rad;

        // Compute the sin and cosine of the rotation angle
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        // Compute the rotation matrix
        Matrix4x4 rotationMatrix = new Matrix4x4();
        rotationMatrix.SetRow(0, new Vector4(cos, 0, sin, 0));
        rotationMatrix.SetRow(1, new Vector4(0, 1, 0, 0));
        rotationMatrix.SetRow(2, new Vector4(-sin, 0, cos, 0));
        rotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

        // Apply the rotation to the isometric vector
        Vector4 worldSpaceVector = rotationMatrix * new Vector4(isometricVector.x, 0, isometricVector.y, 1);
        Vector2 finalVector = new Vector2(worldSpaceVector.x, worldSpaceVector.z);

        // Output the final vector
        Debug.Log("Isometric Vector: " + isometricVector + ", World Space Vector: " + finalVector);
        return finalVector;
    }

}
