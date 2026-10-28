using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shinn;

public class Score : MonoBehaviour
{
    [SerializeField] private GameObject success;
    [SerializeField] private GameObject fall;

    private SimpleItween m_myitween;
    private Text m_mytext;
    private bool m_pause = false;
    private bool m_flag = false;
    private float m_startTime = 0f;

    private void Start()
    {
        m_startTime = Time.time;
        m_mytext = GetComponent<Text>();
        m_myitween = GetComponent<SimpleItween>();
    }

    private void Update()
    {
        if (!m_pause && m_mytext != null)
        {
            float elapsed = Time.time - m_startTime;
            m_mytext.text = elapsed.ToString("0.0");
        }

        bool isEndState = (success != null && success.activeSelf) || (fall != null && fall.activeSelf);
        if (isEndState)
        {
            m_pause = true;
            if (!m_flag)
            {
                m_flag = true;
                if (m_myitween != null)
                {
                    m_myitween.CallStart();
                }
            }
        }
    }
}

