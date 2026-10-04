using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Launcher : MonoBehaviour
{
    public GameObject Bubble_shootball;

    [ReadOnly]
    public GameObject nextBubble;
    public float LAUNCH_SPEED = 10f;
    public float fireRate = 0.5f;

    [Header("Falling")]
    public bool atuoFalling = true;
    public float falldownDistance = 0.25f;

    [Header("Aim Trajectory")]
    public bool showTrajectory = true;
    public LineRenderer trajectoryLine;
    public int maxTrajectoryReflections = 2;
    public float maxTrajectoryDistance = 25f;
    public LayerMask trajectoryLayerMask = ~0;

    [HideInInspector]
    public bool startShooting = true;

    private GridManager grid;
    private Camera mainCamera;
    private float nextFire = 0.0f;
    private int count = 0;
    private GameObject gotemp;

    private void Awake()
    {
        mainCamera = Camera.main;
        InitTrajectoryLine();
    }

    private void Start()
    {
        grid = GetComponent<GridManager>();
        Load();
    }

    private void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 delta = mousePos - new Vector2(transform.position.x, transform.position.y);

            float clampValue = Mathf.Clamp(-Mathf.Rad2Deg * Mathf.Atan2(delta.x, delta.y), -60, 60);
            transform.rotation = Quaternion.Euler(0f, 0f, clampValue);
        }

        UpdateTrajectory();

        // Check if pointer is over UI buttons
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0) && Time.time > nextFire && startShooting)
        {
            nextFire = Time.time + fireRate;
            Fire();

            if (gotemp != null)
            {
                SpriteRenderer sr = gotemp.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.enabled = true;
            }

            if (atuoFalling && grid != null)
            {
                count++;
                if (count >= 3)
                {
                    count = 0;
                    FallingDown(grid.initialPos.y, grid.initialPos.y - falldownDistance);
                }
            }
        }
    }

    public void Load()
    {
        if (nextBubble == null && Bubble_shootball != null)
        {
            nextBubble = (GameObject)Instantiate(Bubble_shootball, transform.position, transform.rotation);
            nextBubble.SetActive(true);
            gotemp = nextBubble;

            CircleCollider2D collider = nextBubble.GetComponent<CircleCollider2D>();
            if (collider != null)
                collider.enabled = false;

            Hitter hitter = nextBubble.GetComponent<Hitter>();
            if (hitter != null)
                hitter.parent = gameObject;
        }
    }

    #region Trajectory
    private void InitTrajectoryLine()
    {
        if (trajectoryLine == null)
            trajectoryLine = GetComponent<LineRenderer>();

        if (trajectoryLine == null)
        {
            GameObject lineObj = new GameObject("TrajectoryLine");
            lineObj.transform.parent = transform;
            lineObj.transform.localPosition = Vector3.zero;
            trajectoryLine = lineObj.AddComponent<LineRenderer>();
            trajectoryLine.startWidth = 0.05f;
            trajectoryLine.endWidth = 0.05f;
            trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
            trajectoryLine.startColor = new Color(1f, 1f, 0.4f, 0.75f);
            trajectoryLine.endColor = new Color(1f, 1f, 0.4f, 0.2f);
            trajectoryLine.sortingOrder = 10;
        }
    }

    private void UpdateTrajectory()
    {
        if (trajectoryLine == null)
            return;

        if (!showTrajectory || !startShooting)
        {
            trajectoryLine.enabled = false;
            return;
        }

        trajectoryLine.enabled = true;
        List<Vector3> points = CalculateTrajectoryPoints();

        trajectoryLine.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            trajectoryLine.SetPosition(i, points[i]);
        }
    }

    private List<Vector3> CalculateTrajectoryPoints()
    {
        List<Vector3> points = new List<Vector3>();
        Vector2 currentPos = transform.position;
        Vector2 currentDir = transform.up;
        points.Add(currentPos);

        float remainingDistance = maxTrajectoryDistance;

        for (int i = 0; i <= maxTrajectoryReflections; i++)
        {
            RaycastHit2D hit = Physics2D.CircleCast(currentPos + currentDir * 0.1f, 0.15f, currentDir, remainingDistance, trajectoryLayerMask);

            if (hit.collider != null)
            {
                points.Add(hit.point);
                remainingDistance -= hit.distance;

                if (hit.collider.CompareTag("SideWall") && remainingDistance > 0.5f)
                {
                    currentPos = hit.point + hit.normal * 0.05f;
                    currentDir = Vector2.Reflect(currentDir, hit.normal);
                }
                else
                {
                    break;
                }
            }
            else
            {
                points.Add(currentPos + currentDir * remainingDistance);
                break;
            }
        }

        return points;
    }

    private void OnDrawGizmos()
    {
        if (!showTrajectory)
            return;

        Gizmos.color = Color.cyan;
        List<Vector3> points = CalculateTrajectoryPoints();
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
    #endregion

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
        if (grid != null)
        {
            grid.initialPos = new Vector3(grid.initialPos.x, value, grid.initialPos.z);
        }
    }
    #endregion
}

