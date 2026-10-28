using UnityEngine;
using System.Collections;

public class Hitter : MonoBehaviour
{
    public SpriteRenderer spHint;
    public Sprite[] sp;
    public int kind;
    public GameObject parent;

    private bool m_hitflag = false;
    private SpriteRenderer thissprenderer;
    private float m_lifeTimer = 0f;
    private const float MAX_FLIGHT_TIME = 8.0f;

    private void Awake()
    {
        thissprenderer = GetComponent<SpriteRenderer>();
        if (spHint == null)
        {
            GameObject hintObj = GameObject.Find("Next-hint");
            if (hintObj != null)
            {
                spHint = hintObj.GetComponent<SpriteRenderer>();
            }
        }
        kind = Random.Range(1, 6);
    }

    void Start()
    {
        if (spHint != null && sp != null && kind - 1 >= 0 && kind - 1 < sp.Length)
        {
            spHint.sprite = sp[kind - 1];
        }

        if (thissprenderer != null)
        {
            thissprenderer.enabled = false;
            if (sp != null && kind - 1 >= 0 && kind - 1 < sp.Length)
            {
                thissprenderer.sprite = sp[kind - 1];
            }
        }

        if (parent == null)
        {
            Launcher foundLauncher = FindObjectOfType<Launcher>();
            if (foundLauncher != null)
            {
                parent = foundLauncher.gameObject;
            }
        }
    }

    private void Update()
    {
        // Failsafe: if projectile is moving in flight for too long without collision, reload launcher
        if (thissprenderer != null && thissprenderer.enabled && !m_hitflag)
        {
            m_lifeTimer += Time.deltaTime;
            if (m_lifeTimer > MAX_FLIGHT_TIME || transform.position.y > 20f || transform.position.y < -15f)
            {
                RecoverLauncher();
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null && !m_hitflag && collider.CompareTag("bubble"))
        {
            m_hitflag = true;

            CircleCollider2D selfcollider = GetComponent<CircleCollider2D>();
            if (selfcollider != null)
                selfcollider.enabled = false;

            if (parent != null)
            {
                GridManager gridManager = parent.GetComponent<GridManager>();
                if (gridManager != null)
                {
                    GameObject newBubble = gridManager.Create(transform.position, kind);
                    if (newBubble != null)
                    {
                        GridMember gridMember = newBubble.GetComponent<GridMember>();
                        if (gridMember != null)
                            gridManager.Seek(gridMember.column, -gridMember.row, gridMember.kind);
                    }
                }
            }

            RecoverLauncher();
            Destroy(gameObject);
        }
    }

    private void RecoverLauncher()
    {
        if (parent != null)
        {
            Launcher launcher = parent.GetComponent<Launcher>();
            if (launcher != null)
            {
                launcher.nextBubble = null;
                launcher.Load();
            }
        }
    }
}

