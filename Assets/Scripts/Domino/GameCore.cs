using System.Collections;
using System.IO;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    enum TurnType
    {
        TT_Player = 0,
        TT_Bot = 1
    }
    const float verticalFieldX = -7.5f,
                verticalFieldY = 5f,
                horizontalFieldX = -7f,
                horizontalFieldY = 5.5f,
                stoneSize = 1f;
    bool isEndGameCondition = false, isGameGoing = false;
    Pile stonesPile;
    Player player;
    Bot bot;
    Pile pile;
    GameObject winText, looseText, helpText;
    MenuButtons menu;
    TurnType currentTurn = TurnType.TT_Player;

    public int fieldHeight = 12,
               fieldWidth = 16;
    public StoneEffect stoneEffect;
    public Stone draggedStone;
    public bool stoneBeingDragged = false;
    public bool isGameOnPause;
    public SpriteRenderer[,] fieldBlocks;
    public int[,,] field;
    public GameObject mainCamera;
    public bool is3DModeEnabled = true, cameraRotationEnabled = true;

    void Start()
    {
        winText = GameObject.Find("WinText");
        looseText = GameObject.Find("LooseText");
        helpText = GameObject.Find("HelpText");
        mainCamera = GameObject.Find("3D Camera");
        pile = GameObject.Find("Pile").GetComponent<Pile>();
        menu = GameObject.Find("MenuPanel").GetComponent<MenuButtons>();
        mainCamera.SetActive(true);
        winText.SetActive(false);
        looseText.SetActive(false);
        helpText.SetActive(false);
        isGameOnPause = false;
        StoneEffect stonePrefab = Resources.Load<StoneEffect>("Domino/Prefab/StoneEffect");
        stoneEffect = StoneEffect.Instantiate(stonePrefab);
        stoneEffect.transform.localScale = new Vector3(1, 1, 0);
        stoneEffect.SetRotationState(3);

        GameObject slotPrefab = Resources.Load<GameObject>("Domino/Prefab/Slot");
        GameObject panel = GameObject.Find("FirstStoneEffectPanel");
        var stoneSprites = Resources.LoadAll<Sprite>("Domino/Sprites/txt_bones");
        fieldBlocks = new SpriteRenderer[fieldHeight, fieldWidth];
        field = new int[fieldHeight, fieldWidth, 2];
        for (int i = 0; i < fieldHeight; i++)
        {
            for (int j = 0; j < fieldWidth; j++)
            {
                GameObject slot = GameObject.Instantiate(slotPrefab, panel.transform);
                fieldBlocks[i, j] = slot.GetComponent<SpriteRenderer>();
                fieldBlocks[i, j].sortingOrder = 1;
                fieldBlocks[i, j].sprite = stoneSprites[0];
                fieldBlocks[i, j].transform.localScale = new Vector3(1, 1, 0);
                fieldBlocks[i, j].gameObject.SetActive(true);
                fieldBlocks[i, j].color = new Color(1, 0, 0, 0);
                field[i, j, 0] = -1;
                field[i, j, 1] = 0;
            }
        }

        stonesPile = GameObject.Find("Pile").GetComponent<Pile>();
        player = GameObject.Find("Player").GetComponent<Player>();
        bot = GameObject.Find("Bot").GetComponent<Bot>();

        
        isGameOnPause = false;
    }
    public void StartGame()
    {
        isEndGameCondition = false;
        for (int i = 0; i < fieldHeight; i++)
        {
            for (int j = 0; j < fieldWidth; j++)
            {
                field[i, j, 0] = -1;
                field[i, j, 1] = 0;
            }
        }
        stonesPile.GeneratePile();
        for (int i = 0; i < 7; i++)
        {
            player.PickStone();
            bot.PickStone();
        }
        helpText.SetActive(true);
        StartCoroutine(MakeFirstMove());
    }
    IEnumerator MakeFirstMove()
    {
        yield return new WaitForSeconds(3f);
        Stone playerBiggest = player.ShowBiggestStone(),
              botBiggest = bot.ShowBiggestStone(),
              chosenStone = playerBiggest;

        bool playerIsBigger = (playerBiggest.Values().firstValue + playerBiggest.Values().secondValue > botBiggest.Values().firstValue + botBiggest.Values().secondValue),
             playerIsDual = (playerBiggest.Values().firstValue == playerBiggest.Values().secondValue),
             botIsDual = (botBiggest.Values().firstValue == botBiggest.Values().secondValue),
             chosenIsPlayer = false;

        if (botIsDual && playerIsDual)
        {
            if (playerIsBigger)
            {
                chosenStone = playerBiggest;
                player.DropStone(chosenStone);
                chosenIsPlayer = true;
            }
            else
            {
                chosenStone = botBiggest;
                bot.DropStone(chosenStone);
            }
        }
        else if (!botIsDual && playerIsDual)
        {
            chosenStone = playerBiggest;
            player.DropStone(chosenStone);
            chosenIsPlayer = true;
        }
        else if (botIsDual && !playerIsDual)
        {
            chosenStone = botBiggest;
            bot.DropStone(chosenStone);
        }
        else if (!botIsDual && !playerIsDual)
        {
            if (playerIsBigger)
            {
                chosenStone = playerBiggest;
                player.DropStone(chosenStone);
                chosenIsPlayer = true;
            }
            else
            {
                chosenStone = botBiggest;
                bot.DropStone(chosenStone);
            }
        }

        if (chosenStone.Values().firstValue == chosenStone.Values().secondValue)
        {
            WriteLog("FirstMove:Chosen is Dual");
            field[4, 7, 0] = -2;
            field[5, 7, 0] = chosenStone.Values().firstValue;
            field[6, 7, 0] = -2;
            field[5, 7, 1] = 0;
            chosenStone.SetNewAnchor(new Vector3(-0.5f, 0.5f, 0));
        }
        else
        {
            WriteLog("FirstMove:Chosen isnt Dual");
            field[5, 7, 0] = chosenStone.Values().firstValue;
            field[5, 7, 1] = 1;
            field[6, 7, 0] = chosenStone.Values().secondValue;
            field[6, 7, 1] = 1;
            chosenStone.SetNewAnchor(new Vector3(-0.5f, 0f, 0));
        }
        isGameGoing = true;
        if (chosenIsPlayer)
        {
            WriteLog("FirstMove: first move was from player, switching to bot");
            TurnSwitch();
        }
        else 
        {
            WriteLog("FirstMove: first move was from bot");
        }
        yield break;
    }

    public static void WriteLog(string log) //функция для записи в лог файл.
    {
        if (Directory.Exists("Logs/") == false)
        {
            Directory.CreateDirectory("Logs/");
        }
        StreamWriter sw = new StreamWriter("Logs/Domnino.logs", true);
        Debug.Log(log);
        sw.WriteLine(log);
        sw.Close();
    }
    (int direction, Vector2 anchor) OneCorrectAround(int i, int j, int value) //проверка на наличие кости для состыковки вокруг новой кости
    {
        //Debug.Log(i + "---" + j);
        if (i < 0 || i >= fieldHeight || j < 0 || j >= fieldWidth)
            return (0, new Vector2());
        if (i - 1 >= 0 && field[i - 1, j, 0] == value && field[i - 1, j, 1] < 2) //кость сверху
        {
            if (j - 1 >= 0 && field[i, j - 1, 0] > -1) return (0, new Vector2());
            if (j + 1 < fieldWidth && field[i, j + 1, 0] > -1) return (0, new Vector2());
            if (i + 1 < fieldHeight && field[i + 1, j, 0] > -1) return (0, new Vector2());
            return (1, new Vector2(j, i - 1));
        }
        if (j - 1 >= 0 && field[i, j - 1, 0] == value && field[i, j - 1, 1] < 2) //кость слева
        {
            if (i - 1 >= 0 && field[i - 1, j, 0] > -1) return (0, new Vector2());
            if (i + 1 < fieldHeight && field[i + 1, j, 0] > -1) return (0, new Vector2());
            if (j + 1 < fieldWidth && field[i, j + 1, 0] > -1) return (0, new Vector2());
            return (2, new Vector2(j - 1, i));
        }
        if (i + 1 < fieldHeight && field[i + 1, j, 0] == value && field[i + 1, j, 1] < 2) //кость внизу
        {
            if (i - 1 >= 0 && field[i - 1, j, 0] > -1) return (0, new Vector2());
            if (j - 1 >= 0 && field[i, j - 1, 0] > -1) return (0, new Vector2());
            if (j + 1 < fieldWidth && field[i, j + 1, 0] > -1) return (0, new Vector2());
            return (3, new Vector2(j, i + 1));
        }
        if (j + 1 < fieldWidth && field[i, j + 1, 0] == value && field[i, j + 1, 1] < 2) //кость справа
        {
            if (i - 1 >= 0 && field[i - 1, j, 0] > -1) return (0, new Vector2());
            if (j - 1 >= 0 && field[i, j - 1, 0] > -1) return (0, new Vector2());
            if (i + 1 < fieldHeight && field[i + 1, j, 0] > -1) return (0, new Vector2());
            return (4, new Vector2(j + 1, i));
        }
        return (0, new Vector2());
    }
    bool AnyOneAround(int i, int j) //проверка на наличие костей вокруг новой кости
    {
        if (i < 0 || i >= fieldHeight || j < 0 || j >= fieldWidth)
            return true;
        if (i + 1 < fieldHeight && field[i + 1, j, 0] > -1) return true;
        if (i - 1 >= 0 && field[i - 1, j, 0] > -1) return true;
        if (j - 1 >= 0 && field[i, j - 1, 0] > -1) return true;
        if (j + 1 < fieldWidth && field[i, j + 1, 0] > -1) return true;
        return false;
    }
    bool anyOneAroundforDual(int i, int j, int side, int dir) //проверка на наличие подходящей кости для "поперечной" состыковки
    {
        switch (side)
        {
            case 1:
            case 3:
                switch (dir)
                {
                    case 2:
                        return ((i - 1 >= 0 && i + 1 < fieldHeight && j - 1 > 0) && !(field[i - 1, j - 1, 0] <= -1 && field[i + 1, j - 1, 0] <= -1));
                        break;
                    case 4:
                        return ((i - 1 >= 0 && i + 1 < fieldHeight && j + 1 < fieldWidth) && !(field[i - 1, j + 1, 0] <= -1 && field[i + 1, j + 1, 0] <= -1));
                        break;
                }
                break;
            case 2:
            case 4:
                switch (dir)
                {
                    case 1:
                        return ((i - 1 >= 0 && j - 1 > 0 && j + 1 < fieldWidth) && !(field[i - 1, j - 1, 0] <= -1 && field[i - 1, j + 1, 0] <= -1));
                        break;
                    case 3:
                        return ((i + 1 < fieldHeight && j - 1 > 0 && j + 1 < fieldWidth) && !(field[i + 1, j - 1, 0] <= -1 && field[i + 1, j + 1, 0] <= -1));
                        break;
                }
                break;
        }
        return true;
    }
    public (bool canBePlaced, bool dualPlacement, Vector2 anchor, int connectorPart) IfCanBePlaced(int i1, int j1, int firstValue, int i2, int j2, int secondValue) //проверка на возможность расположения кости
    {
        if (i1 < 0 || i1 >= fieldHeight || i2 < 0 || i2 >= fieldHeight
            || j1 < 0 || j1 >= fieldWidth || j2 < 0 || j2 >= fieldWidth)
            return (false, false, new Vector2(), 0);
        if (field[i1, j1, 0] != -1 || field[i2, j2, 0] != -1)
            return (false, false, new Vector2(), 0);
        int side = 0;
        // определяем как повернута кость
        if (i1 < i2 && j1 == j2) side = 1; //вверх
        if (i1 == i2 && j1 < j2) side = 2; //влево
        if (i1 > i2 && j1 == j2) side = 3; //вниз
        if (i1 == i2 && j1 > j2) side = 4; //вправо
        var firstState = OneCorrectAround(i1, j1, firstValue); //проверка на наличие кости для состыковки вокруг новой кости для каждой половинки
        var secondState = OneCorrectAround(i2, j2, secondValue);
        if (firstValue == secondValue) // если половинки одинаковы, то проверяем можно ли поставить кость поперек
        {
            switch (side)
            {
                case 1:
                case 3:
                    if ((firstState.direction == 2 || firstState.direction == 4) && !anyOneAroundforDual(i1, j1, side, firstState.direction))
                        return (true, true, firstState.anchor, 1);
                    if ((secondState.direction == 2 || secondState.direction == 4) && !anyOneAroundforDual(i2, j2, side, secondState.direction))
                        return (true, true, secondState.anchor, 2);
                    break;
                case 2:
                case 4:
                    if ((firstState.direction == 1 || firstState.direction == 3) && !anyOneAroundforDual(i1, j1, side, firstState.direction))
                        return (true, true, firstState.anchor, 1);
                    if ((secondState.direction == 1 || secondState.direction == 3) && !anyOneAroundforDual(i2, j2, side, secondState.direction))
                        return (true, true, secondState.anchor, 2);
                    break;
            }
        }
        switch (side) // проверяем все возможные варианты расположения кости
        {
            case 1:
                switch (firstState.direction)
                {
                    case 1:
                        if (!AnyOneAround(i2, j2))
                            return (true, false, firstState.anchor, 1);
                        break;
                    case 2:
                    case 4:
                        if ((int)firstState.anchor.y - 1 >= 0)
                        {
                            if (field[(int)firstState.anchor.y - 1, (int)firstState.anchor.x, 0] <= -1 && !AnyOneAround(i2, j2))
                                return (true, false, firstState.anchor, 1);
                        }
                        else if (!AnyOneAround(i2, j2))
                            return (true, false, firstState.anchor, 1);
                        break;
                }
                switch (secondState.direction)
                {
                    case 3:
                        if (!AnyOneAround(i1, j1))
                            return (true, false, secondState.anchor, 2);
                        break;
                    case 2:
                    case 4:
                        if ((int)secondState.anchor.y + 1 < fieldHeight)
                        {
                            if (field[(int)secondState.anchor.y + 1, (int)secondState.anchor.x, 0] <= -1 && !AnyOneAround(i1, j1))
                                return (true, false, secondState.anchor, 2);
                        }
                        else if (!AnyOneAround(i1, j1))
                            return (true, false, secondState.anchor, 2);
                        break;
                }
                break;
            case 2:
                switch (firstState.direction)
                {
                    case 2:
                        if (!AnyOneAround(i2, j2))
                            return (true, false, firstState.anchor, 1);
                        break;
                    case 1:
                    case 3:
                        if ((int)firstState.anchor.x - 1 >= 0)
                        {
                            if (field[(int)firstState.anchor.y, (int)firstState.anchor.x - 1, 0] <= -1 && !AnyOneAround(i2, j2))
                                return (true, false, firstState.anchor, 1);
                        }
                        break;
                }
                switch (secondState.direction)
                {
                    case 4:
                        if (!AnyOneAround(i1, j1))
                            return (true, false, secondState.anchor, 2);
                        break;
                    case 1:
                    case 3:
                        if ((int)secondState.anchor.x + 1 < fieldWidth)
                        {
                            if (field[(int)secondState.anchor.y, (int)secondState.anchor.x + 1, 0] <= -1 && !AnyOneAround(i1, j1))
                                return (true, false, secondState.anchor, 2);
                        }
                        else if (!AnyOneAround(i1, j1))
                            return (true, false, secondState.anchor, 2);
                        break;
                }
                break;
            case 3:
                switch (firstState.direction)
                {
                    case 3:
                        if (!AnyOneAround(i2, j2))
                            return (true, false, firstState.anchor, 1);
                        break;
                    case 2:
                    case 4:
                        if ((int)firstState.anchor.y + 1 < fieldHeight)
                        {
                            if (field[(int)firstState.anchor.y + 1, (int)firstState.anchor.x, 0] <= -1 && !AnyOneAround(i2, j2))
                                return (true, false, firstState.anchor, 1);
                        }
                        else if (!AnyOneAround(i2, j2))
                            return (true, false, firstState.anchor, 1);
                        break;
                }
                switch (secondState.direction)
                {
                    case 1:
                        if (!AnyOneAround(i1, j1))
                            return (true, false, secondState.anchor, 2);
                        break;
                    case 2:
                    case 4:
                        if ((int)secondState.anchor.y - 1 >= 0)
                        {
                            if (field[(int)secondState.anchor.y - 1, (int)secondState.anchor.x, 0] <= -1 && !AnyOneAround(i1, j1))
                                return (true, false, secondState.anchor, 2);
                        }
                        else if (!AnyOneAround(i1, j1))
                            return (true, false, secondState.anchor, 2);
                        break;
                }
                break;
            case 4:
                switch (firstState.direction)
                {
                    case 4:
                        if (!AnyOneAround(i2, j2))
                            return (true, false, firstState.anchor, 1);
                        break;
                    case 1:
                    case 3:
                        if ((int)firstState.anchor.x + 1 < fieldWidth)
                        {
                            if (field[(int)firstState.anchor.y, (int)firstState.anchor.x + 1, 0] <= -1 && !AnyOneAround(i2, j2))
                                return (true, false, firstState.anchor, 1);
                        }
                        else if (!AnyOneAround(i2, j2))
                            return (true, false, firstState.anchor, 1);
                        break;
                }
                switch (secondState.direction)
                {
                    case 2:
                        if (!AnyOneAround(i1, j1))
                            return (true, false, secondState.anchor, 2);
                        break;
                    case 1:
                    case 3:
                        if ((int)secondState.anchor.x - 1 >= 0)
                        {
                            if (field[(int)secondState.anchor.y, (int)secondState.anchor.x - 1, 0] <= -1 && !AnyOneAround(i1, j1))
                                return (true, false, secondState.anchor, 2);
                        }
                        else if (!AnyOneAround(i1, j1))
                            return (true, false, secondState.anchor, 2);
                        break;
                }
                break;
        }
        return (false, false, new Vector2(), 0);
    }
    bool AnchorCheck(int i, int j) //проверка края цепи на возможность расположения кости
    {
        if (i == -1 || j == -1)
            return false;
        int value = field[i, j, 0];
        WriteLog("Anchor check:");
        if (IfCanBePlaced(i - 1, j, value, i - 2, j, 0).canBePlaced) //верх верх
        {
            WriteLog("Top Top confirmed");
            return true;
        }
        if (IfCanBePlaced(i - 1, j, value, i - 1, j - 1, 0).canBePlaced) //верх влево
        {
            WriteLog("Top Left confirmed");
            return true;
        }
        if (IfCanBePlaced(i - 1, j, value, i - 1, j + 1, 0).canBePlaced) //верх вправо 
        {
            WriteLog("Top Right confirmed");
            return true;
        }
        if (IfCanBePlaced(i, j - 1, value, i, j - 2, 0).canBePlaced) //влево влево
        {
            WriteLog("Left Left confirmed");
            return true;
        }
        if (IfCanBePlaced(i, j - 1, value, i + 1, j - 1, 0).canBePlaced)  //влево вниз
        {
            WriteLog("Left Bot confirmed");
            return true;
        }
        if (IfCanBePlaced(i, j - 1, value, i - 1, j - 1, 0).canBePlaced) //влево вверх
        {
            WriteLog("Left Top confirmed");
            return true;
        }
        if (IfCanBePlaced(i + 1, j, value, i + 2, j, 0).canBePlaced) //вниз вниз
        {
            WriteLog("Bot Bot confirmed");
            return true;
        }
        if (IfCanBePlaced(i + 1, j, value, i + 1, j - 1, 0).canBePlaced) //вниз влево
        {
            WriteLog("Bot Left confirmed");
            return true;
        }
        if (IfCanBePlaced(i + 1, j, value, i + 1, j + 1, 0).canBePlaced) //вниз вправо
        {
            WriteLog("Bot Right confirmed");
            return true;
        }
        if (IfCanBePlaced(i, j + 1, value, i, j + 2, 0).canBePlaced) //вправо вправо
        {
            WriteLog("Right Right confirmed");
            return true;
        }
        if (IfCanBePlaced(i, j + 1, value, i + 1, j + 1, 0).canBePlaced) //вправо вниз
        {
            WriteLog("Right Bot confirmed");
            return true;
        }
        if (IfCanBePlaced(i, j + 1, value, i - 1, j + 1, 0).canBePlaced) //вправо вверх
        {
            WriteLog("Right Top confirmed");
            return true;
        }
        return false;
    }
    (bool playerCanMove, bool botCanMove) AnyMovesAvailable() //проверка на возможность продолжения игры
    {
        WriteLog("AnyMovesAvailable check");
        Vector2 firstAnchor = new Vector2(-1, -1), secondAnchor = new Vector2(-1, -1);
        for (int i = 0; i < fieldHeight; i++)
        {
            for (int j = 0; j < fieldWidth && secondAnchor == new Vector2(-1, -1); j++)
            {
                if (field[i, j, 0] > -1 && field[i, j, 1] < 2)
                {
                    if (firstAnchor == new Vector2(-1, -1)) firstAnchor = new Vector2(j, i);
                    else secondAnchor = new Vector2(j, i);
                }
            }
        }
        bool firstAnchorCheck = AnchorCheck((int)firstAnchor.y, (int)firstAnchor.x),
            secondAnchorCheck = AnchorCheck((int)secondAnchor.y, (int)secondAnchor.x);
        if (!firstAnchorCheck && !secondAnchorCheck)
        {
            WriteLog("No space left for placement");
            return (false, false);
        }

        bool playerCanMove = ((firstAnchor != new Vector2(-1, -1) && player.HasStoneWithValue(field[(int)firstAnchor.y, (int)firstAnchor.x, 0]) && firstAnchorCheck)
                             || (secondAnchor != new Vector2(-1, -1) && player.HasStoneWithValue(field[(int)secondAnchor.y, (int)secondAnchor.x, 0]) && secondAnchorCheck)
                             || (!stonesPile.IsEmpty() && player.StonesInHandCount() < PlayerBase.maxStonesCount)),
             botCanMove = ((firstAnchor != new Vector2(-1, -1) && bot.HasStoneWithValue(field[(int)firstAnchor.y, (int)firstAnchor.x, 0]) && firstAnchorCheck)
                             || (secondAnchor != new Vector2(-1, -1) && bot.HasStoneWithValue(field[(int)secondAnchor.y, (int)secondAnchor.x, 0]) && secondAnchorCheck)
                             || (!stonesPile.IsEmpty() && bot.StonesInHandCount() < PlayerBase.maxStonesCount));

        if (!playerCanMove && !botCanMove && stonesPile.IsEmpty())
        {
            WriteLog("Player and bot dont have stones for placement");
            return (false, false);
        }
        WriteLog("Bot has stone to move:" + botCanMove);
        WriteLog("Player has stone to move:" + playerCanMove);

        WriteLog("AnyMovesAvailable check end:" + (playerCanMove || botCanMove));
        return (playerCanMove, botCanMove);
    }
    void EndGame(int winner) //конец игры
    {
        switch (winner)
        {
            case 0:
                WriteLog("NOONE WINS");
                winText.SetActive(true);
                break;
            case 1:
                WriteLog("PLAYER WINS");
                winText.SetActive(true);
                break;
            case 2:
                WriteLog("BOT WINS");
                looseText.SetActive(true);
                break;

        }
        isEndGameCondition = true;
    }
    public void FinishGame()
    {
        StopCoroutine(MakeFirstMove());
        player.ClearHand();
        bot.ClearHand();
        pile.ClearPile();
        stoneEffect.SetEffect(StoneEffect.EffectType.ET_Transparent);
        Stone[] stones = Resources.FindObjectsOfTypeAll<Stone>(); //GameObject.FindGameObjectsWithTag("Stone");
        foreach (Stone stone in stones)
        {
            if (stone.gameObject.name!="Stone")
                Destroy(stone.gameObject);
        }
        winText.SetActive(false);
        looseText.SetActive(false);
        helpText.SetActive(false);
        isGameGoing = false;
        menu.GoMenu();
    }
    void CheckForWin()
    {
        if (player.StonesInHandCount() == bot.StonesInHandCount())
        {
            EndGame(0);
        }
        if (player.StonesInHandCount() < bot.StonesInHandCount())
        {
            EndGame(1);
        }
        if (player.StonesInHandCount() > bot.StonesInHandCount())
        {
            EndGame(2);
        }
    }
    public void TurnSwitch() //смена хода
    {
        var movesCheckResult = AnyMovesAvailable();
        if ((!movesCheckResult.playerCanMove && !movesCheckResult.botCanMove)
            || (player.StonesInHandCount() == 0 || bot.StonesInHandCount() == 0))
        {
            CheckForWin();
            return;
        }
        switch (currentTurn)
        {
            case TurnType.TT_Bot:
                if (movesCheckResult.playerCanMove)
                {
                    currentTurn = TurnType.TT_Player;
                    WriteLog("Switching turn to Player");
                }
                else
                {
                    WriteLog("Not switching turn to Player");
                    bot.MakeTurn();
                    TurnSwitch();
                    return;
                }
                break;
            case TurnType.TT_Player:
                if (movesCheckResult.botCanMove)
                {
                    WriteLog("Switching turn to Bot");
                    currentTurn = TurnType.TT_Bot;
                    bot.MakeTurn();
                    TurnSwitch();
                    return;
                }
                else
                {
                    WriteLog("Not switching turn to Bot");
                }
                break;
        }
    }
    Vector3 CalculateScreenPosition(bool dualPlacement, int rotationState, int connectorPart, int i1, int j1, int i2, int j2) //перевод из координат сетки в экранные координаты
    {
        if (dualPlacement)
        {
            switch (rotationState)
            {
                case 0:
                    switch (connectorPart)
                    {
                        case 1:
                            return new Vector3(verticalFieldX + j1 * stoneSize, verticalFieldY - i1 * stoneSize + stoneSize / 2, 0.77f);
                            break;
                        case 2:
                            return new Vector3(verticalFieldX + j1 * stoneSize, verticalFieldY - i1 * stoneSize - stoneSize / 2, 0.77f);
                            break;
                    }

                    break;
                case 1:
                    switch (connectorPart)
                    {
                        case 1:
                            return new Vector3(horizontalFieldX + j1 * stoneSize - stoneSize / 2, horizontalFieldY - i1 * stoneSize, 0.77f);
                            break;
                        case 2:
                            return new Vector3(horizontalFieldX + j1 * stoneSize + stoneSize / 2, horizontalFieldY - i1 * stoneSize, 0.77f);
                            break;
                    }

                    break;
                case 2:
                    switch (connectorPart)
                    {
                        case 1:
                            return new Vector3(verticalFieldX + j2 * stoneSize, verticalFieldY - i2 * stoneSize - stoneSize / 2, 0.77f);
                            break;
                        case 2:
                            return new Vector3(verticalFieldX + j2 * stoneSize, verticalFieldY - i2 * stoneSize + stoneSize / 2, 0.77f);
                            break;
                    }
                    break;
                case 3:
                    switch (connectorPart)
                    {
                        case 1:
                            return new Vector3(horizontalFieldX + j2 * stoneSize + stoneSize / 2, horizontalFieldY - i2 * stoneSize, 0.77f);
                            break;
                        case 2:
                            return new Vector3(horizontalFieldX + j2 * stoneSize - stoneSize / 2, horizontalFieldY - i2 * stoneSize, 0.77f);
                            break;
                    }

                    break;
            }
        }
        else
        {
            switch (rotationState)
            {
                case 0:
                    return new Vector3(verticalFieldX + j1 * stoneSize, verticalFieldY - i1 * stoneSize, 0.77f);
                    break;
                case 1:
                    return new Vector3(horizontalFieldX + j1 * stoneSize, horizontalFieldY - i1 * stoneSize, 0.77f);
                    break;
                case 2:
                    return new Vector3(verticalFieldX + j2 * stoneSize, verticalFieldY - i2 * stoneSize, 0.77f);
                    break;
                case 3:
                    return new Vector3(horizontalFieldX + j2 * stoneSize, horizontalFieldY - i2 * stoneSize, 0.77f);
                    break;
            }
        }
        return new Vector3();
    }
    public void PlaceStone(Stone stone, bool dualPlacement, int connectorPart, Vector2 anchor, int i1, int j1, int i2, int j2) //закрепление камня на поле
    {
        Vector3 placementPosition = CalculateScreenPosition(dualPlacement, stone.GetRotationState(), connectorPart, i1, j1, i2, j2);
        stone.SetNewAnchor(placementPosition);
        field[(int)anchor.y, (int)anchor.x, 1]++;
        if (dualPlacement)
        {
            switch (connectorPart)
            {
                case 1:
                    field[i1, j1, 1] = 1;
                    field[i1, j1, 0] = stone.Values().firstValue;
                    switch (stone.GetRotationState())
                    {
                        case 0:
                        case 2:
                            if (i1 - 1 >= 0)
                                field[i1 - 1, j1, 0] = -2;
                            if (i1 + 1 < fieldHeight)
                                field[i1 + 1, j1, 0] = -2;
                            break;
                        case 1:
                        case 3:
                            if (j1 - 1 >= 0)
                                field[i1, j1 - 1, 0] = -2;
                            if (j1 + 1 < fieldWidth)
                                field[i1, j1 + 1, 0] = -2;
                            break;
                    }
                    break;
                case 2:
                    field[i2, j2, 1] = 1;
                    field[i2, j2, 0] = stone.Values().secondValue;
                    switch (stone.GetRotationState())
                    {
                        case 0:
                        case 2:
                            if (i2 - 1 >= 0)
                                field[i2 - 1, j2, 0] = -2;
                            if (i2 + 1 < fieldHeight)
                                field[i2 + 1, j2, 0] = -2;
                            break;
                        case 1:
                        case 3:
                            if (j2 - 1 >= 0)
                                field[i2, j2 - 1, 0] = -2;
                            if (j2 + 1 <= fieldWidth)
                                field[i2, j2 + 1, 0] = -2;
                            break;
                    }
                    break;
            }
        }
        else
        {
            switch (connectorPart)
            {
                case 1:
                    field[i1, j1, 1] = 2;
                    field[i2, j2, 1] = 1;
                    break;
                case 2:
                    field[i1, j1, 1] = 1;
                    field[i2, j2, 1] = 2;
                    break;
            }
            field[i1, j1, 0] = stone.Values().firstValue;
            field[i2, j2, 0] = stone.Values().secondValue;
        }
    }
    
    void _HandleDraggesStoneCheck()
    {
        int i1 = -1, i2 = -1, j1 = -1, j2 = -1;
        var halfsPositions = draggedStone.HalfsPositions(); //определение позиции кости относительно сетки
        for (int i = 0; i < fieldHeight; i++)
        {
            for (int j = 0; j < fieldWidth; j++)
            {
                if (fieldBlocks[i, j].bounds.Contains(halfsPositions.firstHalfPos) && field[i, j, 0] == -1)
                {
                    i1 = i;
                    j1 = j;
                }
                if (fieldBlocks[i, j].bounds.Contains(halfsPositions.secondHalfPos) && field[i, j, 0] == -1)
                {
                    i2 = i;
                    j2 = j;
                }
            }
        }
        var checkResult = IfCanBePlaced(i1, j1, draggedStone.Values().firstValue, i2, j2, draggedStone.Values().secondValue); //проверка на возможность расположения кости
        if (checkResult.canBePlaced) //если можно, то отображаем подсветку
        {
            Vector3 placementPosition = CalculateScreenPosition(checkResult.dualPlacement, draggedStone.GetRotationState(), checkResult.connectorPart, i1, j1, i2, j2);
            stoneEffect.SetRotationState(draggedStone.GetRotationState());
            stoneEffect.transform.localPosition = placementPosition;
            stoneEffect.SetEffect(StoneEffect.EffectType.ET_Green);
        }
        else
        {
            stoneEffect.SetEffect(StoneEffect.EffectType.ET_Transparent); // в противном случае глушим подсветку
        }
        if (!draggedStone.IsBeingDragged()) //если кость перестали тащить и его можно расположить, то ставим его на поле
        {
            if (checkResult.canBePlaced && currentTurn == TurnType.TT_Player)
            {
                player.DropStone(draggedStone);
                PlaceStone(draggedStone, checkResult.dualPlacement, checkResult.connectorPart, checkResult.anchor, i1, j1, i2, j2);
                TurnSwitch();
            }
            draggedStone = null;
            stoneBeingDragged = false;
        }
    }
    void Update()
    {
        if (isGameOnPause)
            return;
        if (isEndGameCondition && Input.anyKeyDown)
        {
            FinishGame();
            return;
        }
        if (!isGameGoing)
            return;
        if (Input.GetKeyDown(KeyCode.F5))
        {
            EndGame(0);
        }
        if (Input.GetKeyDown(KeyCode.Space) && player.StonesInHandCount() < PlayerBase.maxStonesCount && !stonesPile.IsEmpty() && !isGameOnPause && currentTurn == TurnType.TT_Player)
        {
            player.PickStone();
            if (!AnyMovesAvailable().playerCanMove)
            {
                TurnSwitch();
                return;
            }
        }
        if ((Input.GetKeyDown(KeyCode.Mouse1) && !stoneBeingDragged && is3DModeEnabled))
        {
            cameraRotationEnabled = true;
        }
        if (stoneBeingDragged)
        {
            _HandleDraggesStoneCheck();
        }
    }
}
