using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class YesButtonScript : MonoBehaviour
{
    public Button m_button;
    // Start is called before the first frame update
    void Start()
    {
        m_button = GameObject.Find("YesButton").GetComponent<Button>();
        m_button.onClick.AddListener(TaskOnClick);
    }
    void TaskOnClick()
    {
        Debug.Log("Button::Yes button was pressed");
        transform.parent.gameObject.SetActive(false);
        if (SceneManager.GetActiveScene().name == "FungusMenu")
        {
            Debug.Log("Quit");
            Application.Quit();
        }
        else
        {
            Application.LoadLevel("FungusMenu");
        }
        
        //Сюда переход на сцену в меню
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
