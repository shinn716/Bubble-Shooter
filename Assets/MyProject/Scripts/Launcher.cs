using UnityEngine;
using System.Collections;

public class Launcher : MonoBehaviour
{
	public GameObject Bubble_shootball;

    [ReadOnly]
	public GameObject nextBubble;
    public float LAUNCH_SPEED = 10f;
    public float fireRate = 0.5F;

    [Header("Falling")]
    public bool atuoFalling = true;

    [HideInInspector]
    public bool startShooting = true;

    private GridManager grid;
    private float nextFire = 0.0F;
    private int count = 0;

    private void Start()
	{
        grid = GetComponent<GridManager>();
        Load();
	}

    private void Update()
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 delta = mousePos - new Vector2(transform.position.x, transform.position.y);
		transform.rotation = Quaternion.Euler(0f, 0f, -Mathf.Rad2Deg * Mathf.Atan2(delta.x, delta.y));

        if (Input.GetMouseButtonDown(0) && Time.time > nextFire && startShooting)
        {
            nextFire = Time.time + fireRate;
            Fire();
            count++;

            if (count==3)
            {
                count = 0;
                FallingDown(grid.initialPos.y, grid.initialPos.y-.5f);
            }
        }
    }


    public void Load()
	{
		if (nextBubble == null)
		{
            nextBubble = (GameObject)Instantiate(Bubble_shootball, transform.position, transform.rotation);
            nextBubble.SetActive(true);

			CircleCollider2D collider = nextBubble.GetComponent<CircleCollider2D>();
			if (collider != null)
				collider.enabled = false;

			Hitter hitter = nextBubble.GetComponent<Hitter>();
            
            if (hitter != null)
				hitter.parent = gameObject;
		}
	}


    #region Private function
    private void Fire()
	{
		if (nextBubble != null)
		{
			CircleCollider2D collider = nextBubble.GetComponent<CircleCollider2D>();
			if (collider != null)
				collider.enabled = true;

			Rigidbody2D rb = nextBubble.GetComponent<Rigidbody2D>();
			if (rb != null)			
				rb.velocity = transform.up * LAUNCH_SPEED;
			
		}
	}

    private void FallingDown(float from, float to)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", from, "to", to, "time", 2, "easetype", iTween.EaseType.easeOutBounce, "onupdate", "valueProcess"));
    }

    private void valueProcess(float value)
    {
        grid.initialPos = new Vector3(grid.initialPos.x, value, grid.initialPos.z);
    }
    #endregion
}
