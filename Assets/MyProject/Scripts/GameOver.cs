using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject gameover;
    public Launcher launcher;

    private void Start()
    {
        if (launcher == null)
        {
            launcher = FindObjectOfType<Launcher>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("bubble"))
        {
            GridMember gm = collision.GetComponent<GridMember>();
            if (gm != null && (gm.state == "Pop" || gm.state == "Fall" || gm.state == "Explode"))
            {
                return;
            }

            if (gameover != null)
            {
                gameover.SetActive(true);
            }

            if (launcher != null)
            {
                launcher.startShooting = false;
            }
        }
    }
}

