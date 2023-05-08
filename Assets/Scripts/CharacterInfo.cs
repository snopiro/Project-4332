using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{

    public float speed;
    public bool isMoving = false;
    public OverlayTile activeTile;
    private RangeFinder rangeFinder;
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

    private void Start()
    {
        isActivelyControlled = false;
        speed = 3.0f;

        currentHealth = maxHealth;
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
    }
    private void Awake()
    {
        tm = GameObject.Find("GameManager").GetComponent<TurnManager>();
        rangeFinder = new RangeFinder();
    }

    private void Update()
    {
        if (movingAudio)
        {
            if (isMoving)
                movingAudio.SetActive(true);
            else
                movingAudio.SetActive(false);
        }
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

    public void receiveDamage(int damage)
    {
        GameObject.Find("HitSound").GetComponent<AudioSource>().Play();
        currentHealth -= damage;
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
        if(currentHealth <= 0)
        {
            Debug.Log("Starting coroutine: death");
            StartCoroutine(Death());
        }
        else        
            tm.ProcessEnemyDeath();
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(1.0f);
        if (playerControlled)
            GameObject.Find("GameManager").GetComponent<GeneralManager>().RemovePlayerCharacterFromList(this.gameObject);
        
        else
            GameObject.Find("GameManager").GetComponent<GeneralManager>().RemoveEnemyCharacterFromList(this.gameObject);
        gameObject.SetActive(false);
        tm.ProcessEnemyDeath();
    }

    public void fullHeal()
    {
        currentHealth = maxHealth;
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
    }

}
