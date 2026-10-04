using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    public int columns;
    public int rows;
    public GameObject bubble;
    public Transform group;
    public Vector3 initialPos;    
    [Range(0, 1)]
    public float gap;

    [Header("Game")]
    public GameObject youWin;
    public TextAsset levelDataAsset;
    public string GridData = "Assets/Data/level1.data";

    [Header("Fall down controller")]
    public Transform compressor;

    private GameObject[,] grid;

    private void Start()
    {
        grid = new GameObject[columns, rows];

        string level = LoadLevel();
        ArragementBubble(level);
    }

    private void Update()
    {
        if (compressor != null)
        {
            compressor.position = new Vector3(compressor.position.x, initialPos.y, compressor.position.z);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(initialPos.x + 2f, initialPos.y, 0), new Vector3(5, .1f, 1));
    }

    public Vector3 Snap(Vector3 position)
    {
        Vector3 objectOffset = position - initialPos;
        Vector3 objectSnap = new Vector3(
            Mathf.Round(objectOffset.x / gap),
            Mathf.Round(objectOffset.y / gap),
            0f
        );

        if ((int)objectSnap.y % 2 != 0)
        {
            if (objectOffset.x > objectSnap.x * gap)			
                objectSnap.x += 0.5f;
            else			
                objectSnap.x -= 0.5f;			
        }
        return initialPos + objectSnap * gap;
    }

    public GameObject Create(Vector2 position, int kind)
    {
        Vector3 snappedPosition = Snap(position);
        int row = (int)Mathf.Round((snappedPosition.y - initialPos.y) / gap);
        int column = 0;
        if (row % 2 != 0)		
            column = (int)Mathf.Round((snappedPosition.x - initialPos.x) / gap - 0.5f);		
        else		
            column = (int)Mathf.Round((snappedPosition.x - initialPos.x) / gap);		

        int gridRow = -row;
        if (column < 0 || column >= columns || gridRow < 0 || gridRow >= rows)
        {
            return null;
        }

        GameObject bubbleClone = (GameObject)Instantiate(bubble, snappedPosition, Quaternion.identity);
        if (group != null)
        {
            bubbleClone.transform.parent = group;
        }

        grid[column, gridRow] = bubbleClone;

        CircleCollider2D collider = bubbleClone.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        GridMember gridMember = bubbleClone.GetComponent<GridMember>();
        if (gridMember != null)
        {
            gridMember.row = row;
            gridMember.column = column;
            if (kind == 6)
            {
                gridMember.kind = Random.Range(1, 6);
            }
            else
            {
                gridMember.kind = kind;
            }

            SpriteRenderer spriteRenderer = bubbleClone.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && gridMember.sp != null && gridMember.kind - 1 >= 0 && gridMember.kind - 1 < gridMember.sp.Length)			
            {
                spriteRenderer.sprite = gridMember.sp[gridMember.kind - 1];
            }
        }
        bubbleClone.SetActive(true);

        return bubbleClone;
    }

    public void Seek(int column, int row, int kind)
    {
        if (column < 0 || column >= columns || row < 0 || row >= rows)
            return;

        int[] pair = new int[2] { column, row };
        bool[,] visited = new bool[columns, rows];
        visited[column, row] = true;

        int[] deltax = { -1, 0, -1, 0, -1, 1 };
        int[] deltaxprime = { 1, 0, 1, 0, -1, 1 };
        int[] deltay = { -1, -1, 1, 1, 0, 0 };

        Queue<int[]> queue = new Queue<int[]>();
        Queue<GameObject> objectQueue = new Queue<GameObject>();

        queue.Enqueue(pair);

        int count = 0;
        while (queue.Count != 0)
        {
            int[] top = queue.Dequeue();
            GameObject gtop = grid[top[0], top[1]];
            if (gtop != null)
            {
                objectQueue.Enqueue(gtop);
            }
            count += 1;
            for (int i = 0; i < 6; i++)
            {
                int nx = (top[1] % 2 == 0) ? top[0] + deltax[i] : top[0] + deltaxprime[i];
                int ny = top[1] + deltay[i];

                if (nx >= 0 && nx < columns && ny >= 0 && ny < rows)
                {
                    GameObject g = grid[nx, ny];
                    if (g != null)
                    {
                        GridMember gridMember = g.GetComponent<GridMember>();
                        if (gridMember != null && gridMember.kind == kind && !visited[nx, ny])
                        {
                            visited[nx, ny] = true;
                            queue.Enqueue(new int[] { nx, ny });
                        }
                    }
                }
            }
        }

        if (count >= 3)
        {
            while (objectQueue.Count != 0)
            {
                GameObject g = objectQueue.Dequeue();
                if (g != null)
                {
                    GridMember gm = g.GetComponent<GridMember>();
                    if (gm != null)
                    {
                        int c = gm.column;
                        int r = -gm.row;
                        if (c >= 0 && c < columns && r >= 0 && r < rows)
                        {
                            grid[c, r] = null;
                        }
                        gm.state = "Pop";
                    }
                }
            }

            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
                audioSource.Play();
        }

        CheckCeiling(0);
    }

    public void CheckCeiling(int ceiling)
    {
        if (ceiling < 0 || ceiling >= rows)
            return;

        bool[,] visited = new bool[columns, rows];
        Queue<int[]> queue = new Queue<int[]>();

        int[] deltax = { -1, 0, -1, 0, -1, 1 };
        int[] deltaxprime = { 1, 0, 1, 0, -1, 1 };
        int[] deltay = { -1, -1, 1, 1, 0, 0 };

        for (int i = 0; i < columns; i++)
        {
            if (grid[i, ceiling] != null)
            {
                visited[i, ceiling] = true;
                queue.Enqueue(new int[] { i, ceiling });
            }
        }

        while (queue.Count != 0)
        {
            int[] top = queue.Dequeue();
            for (int i = 0; i < 6; i++)
            {
                int nx = (top[1] % 2 == 0) ? top[0] + deltax[i] : top[0] + deltaxprime[i];
                int ny = top[1] + deltay[i];

                if (nx >= 0 && nx < columns && ny >= 0 && ny < rows)
                {
                    GameObject g = grid[nx, ny];
                    if (g != null && !visited[nx, ny])
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue(new int[] { nx, ny });
                    }
                }
            }
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (grid[c, r] != null && !visited[c, r])
                {
                    GameObject g = grid[c, r];
                    grid[c, r] = null;
                    if (g != null)
                    {
                        GridMember gm = g.GetComponent<GridMember>();
                        if (gm != null)
                        {
                            gm.state = "Explode";
                        }
                    }
                }
            }
        }

        // Check if all bubbles cleared
        bool hasRemainingBubbles = false;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (grid[c, r] != null)
                {
                    hasRemainingBubbles = true;
                    break;
                }
            }
            if (hasRemainingBubbles) break;
        }

        if (!hasRemainingBubbles && youWin != null)
        {
            youWin.SetActive(true);
        }
    }

    #region private function 
    private string LoadLevel()
    {
        if (levelDataAsset != null)
        {
            return levelDataAsset.text;
        }

        TextAsset loadedText = Resources.Load<TextAsset>("level1");
        if (loadedText != null)
        {
            return loadedText.text;
        }

        if (File.Exists(GridData))
        {
            return File.ReadAllText(GridData);
        }

        string streamingPath = Path.Combine(Application.streamingAssetsPath, Path.GetFileName(GridData));
        if (File.Exists(streamingPath))
        {
            return File.ReadAllText(streamingPath);
        }

        string dataPath = Path.Combine(Application.dataPath, "Data", Path.GetFileName(GridData));
        if (File.Exists(dataPath))
        {
            return File.ReadAllText(dataPath);
        }

        Debug.LogWarning("Level data file could not be loaded from " + GridData);
        return string.Empty;
    }

    private void ArragementBubble(string level)
    {
        if (string.IsNullOrEmpty(level))
            return;

        int levelpos = 0;
        for (int r = 0; r < rows; r++)
        {
            int colsInRow = (r % 2 != 0) ? columns - 1 : columns;
            for (int c = 0; c < colsInRow; c++)
            {
                Vector3 position = new Vector3((float)c * gap, (float)(-r) * gap, 0f) + initialPos;
                if (r % 2 != 0)
                    position.x += 0.5f * gap;

                while (levelpos < level.Length && (level[levelpos] == '\r' || level[levelpos] == '\n'))
                {
                    levelpos++;
                }

                if (levelpos >= level.Length)
                    return;

                char ch = level[levelpos];
                levelpos++;

                if (ch == '0')
                    continue;

                int newKind = ch - '0';
                if (newKind >= 1 && newKind <= 5)
                {
                    Create(position, newKind);
                }
            }
        }
    }
    #endregion
}

