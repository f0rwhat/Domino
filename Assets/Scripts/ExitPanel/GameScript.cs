using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public GameObject m_exitPanel;
    // Start is called before the first frame update
    void Start()
    {
        m_exitPanel = GameObject.Find("ExitPanel");
        m_exitPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_exitPanel.SetActive(!m_exitPanel.active);
        }
    }
}
