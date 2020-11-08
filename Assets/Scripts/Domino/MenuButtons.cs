using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public Button startButton, exitButton, yesButton, noButton;
    GameObject exitPanel;
    GameCore gameCore;
    CameraRotateAround mainCamera;
    bool isInMenu = true;
    // Start is called before the first frame update
    void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
        mainCamera = GameObject.Find("3D Camera").GetComponent<CameraRotateAround>();
        exitPanel = GameObject.Find("ExitPanel");

        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(StartTaskOnClick);
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(ExitTaskOnClick);
        yesButton = GameObject.Find("YesButton").GetComponent<Button>();
        yesButton.onClick.AddListener(YesTaskOnClick);
        noButton = GameObject.Find("NoButton").GetComponent<Button>();
        noButton.onClick.AddListener(NoTaskOnClick);

        exitPanel.transform.SetAsLastSibling();
        exitPanel.SetActive(false);
    }
    void StartTaskOnClick()
    {
        GameCore.WriteLog("Button::Start button was pressed");
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        isInMenu = false;
        mainCamera.GoGame();
    }
    void ExitTaskOnClick()
    {
        GameCore.WriteLog("Button::Exit button was pressed");
        Application.Quit();
    }
    void _PauseButtonWaskClicked()
    {
        exitPanel.SetActive(false);
        gameCore.isGameOnPause = false;
    }
    void YesTaskOnClick()
    {
        Debug.Log("Button::Yes button was pressed");
        _PauseButtonWaskClicked();
        gameCore.FinishGame();
        GoMenu();
    }
    void NoTaskOnClick()
    {
        Debug.Log("Button::No button was pressed");
        _PauseButtonWaskClicked();
    }
    public void GoMenu()
    {
        isInMenu = true;
        mainCamera.GoMenu();
        StartCoroutine(_WaitForCamera());
    }
    IEnumerator _WaitForCamera()
    {
        while (!mainCamera.IsInMenu())
        {
            yield return new WaitForEndOfFrame();
        }
        startButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isInMenu)
        {
            exitPanel.SetActive(!exitPanel.activeSelf);
            gameCore.isGameOnPause = exitPanel.activeSelf;
        }
    }
}
