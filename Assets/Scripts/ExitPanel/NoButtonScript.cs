using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoButtonScript : MonoBehaviour
{
    public Button m_button;
    // Start is called before the first frame update
    void Start()
    {
        m_button = GameObject.Find("NoButton").GetComponent<Button>();
        m_button.onClick.AddListener(TaskOnClick);
    }
    void TaskOnClick()
    {
        Debug.Log("Button::No button was pressed");
        transform.parent.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
