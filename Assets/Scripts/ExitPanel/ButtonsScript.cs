using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsScript : MonoBehaviour
{
    public Button yesButton, noButton;
    GameCore gameCore;
    CameraRotateAround mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();

        yesButton = GameObject.Find("YesButton").GetComponent<Button>();
        yesButton.onClick.AddListener(YesTaskOnClick);
        noButton = GameObject.Find("NoButton").GetComponent<Button>();
        noButton.onClick.AddListener(NoTaskOnClick);
    }
    //private void Awake()
    //{
    //    Debug.Log("Awake");
    //    //transform.parent.gameObject.SetActive(false);
    //}
    void _ButtonWaskClicked()
    {
        transform.gameObject.SetActive(false);
        gameCore.isGameOnPause = false;
    }
    void YesTaskOnClick()
    {
        Debug.Log("Button::Yes button was pressed");
        _ButtonWaskClicked();
        gameCore.FinishGame();
    }
    void NoTaskOnClick()
    {
        Debug.Log("Button::No button was pressed");
        _ButtonWaskClicked();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
