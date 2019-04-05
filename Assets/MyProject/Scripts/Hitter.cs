﻿using UnityEngine;
using System.Collections;

public class Hitter : MonoBehaviour
{
    public Sprite[] sp;
	public int kind;
    public GameObject parent;

    private bool m_hitflag = false;
	//public Sprite specialBubble;

	void Start()
	{
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer != null)
		{
			Color[] colorArray = new Color[] { Color.red, Color.cyan, Color.yellow, Color.green, Color.magenta };

			kind = (int)Random.Range(1f, 6f);
            spriteRenderer.sprite = sp[kind - 1];

   //         if (kind == 6)
			//{
			//	spriteRenderer.sprite = specialBubble;
			//}
			//else
			//{
   //             spriteRenderer.color = colorArray[kind - 1];
   //         }
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider != null)
		{
            if (!m_hitflag && collider.tag == "bubble")
            {
                m_hitflag = true;

                CircleCollider2D selfcollider = GetComponent<CircleCollider2D>();
                if (selfcollider != null)
                    selfcollider.enabled = false;

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
                Launcher launcher = parent.GetComponent<Launcher>();
                if (launcher != null)
                {
                    launcher.nextBubble = null;
                    launcher.Load();
                }

                Destroy(gameObject);
            }

		}
	}
}
