using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Bot : PlayerBase
{
    private struct checkStruct
    {
        public Vector2 place { get; set; }
        public int stoneIndex { get; set; }
        public int connector { get; set; }
        public checkStruct(Vector2 _place, int _stoneIndex,int _connector)
        {
            place = _place;
            stoneIndex = _stoneIndex;
            connector = _connector;
        }
        
    }
    private struct placeStruct
    {
        public int i1 { get; set; }
        public int j1 { get; set; }
        public int i2 { get; set; }
        public int j2 { get; set; }
        public int stoneIndex { get; set; }
        public int connectorPart { get; set; }
        public bool dualPlacement { get; set; }
        public Vector2 anchor { get; set; }
        public placeStruct(int _i1, int _j1, int _i2, int _j2, Vector2 _anchor,int _connectorPart, bool _dualPlacement, int _stoneIndex)
        {
            i1 = _i1;
            i2 = _i2;
            j1 = _j1;
            j2 = _j2;
            anchor = _anchor;
            connectorPart = _connectorPart;
            dualPlacement = _dualPlacement;
            stoneIndex = _stoneIndex;
        }

    }
    private List<placeStruct> correctPlaces;
    private GameCore gameCore;
    public Bot() : base(false, new Vector2(-6.5f, 8f))
    {
        hand = new List<Stone>();
        correctPlaces = new List<placeStruct>();
    }
    new void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
        base.Start();
    }
    (bool canBePlaced, bool dualPlacement, Vector2 anchor, int connectorPart) TryPlace(int i1,int j1, int firstValue, int i2, int j2, int secondValue, int stoneIndex)
    {
        Vector2 anchor = new Vector2();
        if (i1 < 0 || i1 >= gameCore.fieldHeight || i2 < 0 || i2 >= gameCore.fieldHeight || j1 < 0 || j1 >= gameCore.fieldWidth || j2 < 0 || j2 >= gameCore.fieldWidth) 
            return (false, false, anchor, 0);
        if (gameCore.field[i1, j1, 0] != -1 || gameCore.field[i2, j2, 0] != -1)
            return (false, false, anchor, 0);
        var result = gameCore.IfCanBePlaced(i1, j1, firstValue, i2, j2, secondValue);
        if (result.canBePlaced)
        {
            correctPlaces.Add(new placeStruct(i1, j1, i2, j2, result.anchor, result.connectorPart, result.dualPlacement, stoneIndex));
        }
        return result;
    }

    public void MakeTurn()//основная функция бота
    {
        StartCoroutine(_PlaceStone());
    }

    IEnumerator _PlaceStone()
    {
        yield return new WaitForSeconds(2f);
        GameCore.WriteLog("MAKETURN START");
        Vector2 firstAnchor = new Vector2(-1, -1), secondAnchor = new Vector2(-1, -1);
        int[,,] placementChecks;
        List<checkStruct> placebleStones = new List<checkStruct>();
        correctPlaces.Clear();
        for (int i = 0; i < gameCore.fieldHeight; i++)//поиск краев в цепи
        {
            for (int j = 0; j < gameCore.fieldWidth && secondAnchor == new Vector2(-1, -1); j++)
            {
                if (gameCore.field[i, j, 0] > -1 && gameCore.field[i, j, 1] < 2)
                {
                    if (firstAnchor == new Vector2(-1, -1)) firstAnchor = new Vector2(j, i);
                    else secondAnchor = new Vector2(j, i);
                }
            }
        }
        GameCore.WriteLog("FirstAnchor::" + firstAnchor.y + "-" + firstAnchor.x);
        GameCore.WriteLog("SecondAnchor::" + secondAnchor.y + "-" + secondAnchor.x);
        if (firstAnchor == new Vector2(-1, -1) && secondAnchor == new Vector2(-1, -1))
        {
            GameCore.WriteLog("No anchors found");
            GameCore.WriteLog("BOT TURN END");
            gameCore.TurnSwitch();
            yield break;
        }

        for (int c = 0; c < hand.Count; c++)//поиск подходящих костей в руке
        {
            if (firstAnchor != new Vector2(-1, -1))
            {
                if (hand[c].Values().firstValue == gameCore.field[(int)firstAnchor.y, (int)firstAnchor.x, 0])
                    placebleStones.Add(new checkStruct(firstAnchor, c, 1));
                else if (hand[c].Values().secondValue == gameCore.field[(int)firstAnchor.y, (int)firstAnchor.x, 0])
                    placebleStones.Add(new checkStruct(firstAnchor, c, 2));
            }
            if (secondAnchor != new Vector2(-1, -1))
            {
                if (hand[c].Values().firstValue == gameCore.field[(int)secondAnchor.y, (int)secondAnchor.x, 0])
                    placebleStones.Add(new checkStruct(secondAnchor, c, 1));
                else if (hand[c].Values().secondValue == gameCore.field[(int)secondAnchor.y, (int)secondAnchor.x, 0])
                    placebleStones.Add(new checkStruct(secondAnchor, c, 2));
            }
            if (c == hand.Count - 1)
            {
                for (int i = 0; i < placebleStones.Count; i++)//проверка всех возможных расположений для каждой кости
                {
                    switch (placebleStones[i].connector)//верх верх
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y - 2, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y - 2, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//верх влево
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);

                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//верх вправо
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);

                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//влево влево
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y, (int)placebleStones[i].place.x - 2, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);

                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y, (int)placebleStones[i].place.x - 2, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//влево вниз
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);

                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//влево вверх
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);

                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//вниз вниз
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y + 2, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y + 2, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//вниз влево
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//вниз вправо
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//вправо вправо
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y, (int)placebleStones[i].place.x + 2, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y, (int)placebleStones[i].place.x + 2, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//вправо вниз
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y + 1, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                    switch (placebleStones[i].connector)//влево вверх
                    {
                        case 1:
                            TryPlace((int)placebleStones[i].place.y, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x + 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);

                            break;
                        case 2:
                            TryPlace((int)placebleStones[i].place.y - 1, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().firstValue,
                                (int)placebleStones[i].place.y, (int)placebleStones[i].place.x - 1, hand[placebleStones[i].stoneIndex].Values().secondValue, placebleStones[i].stoneIndex);
                            break;
                    }
                }
            }
            if (c == hand.Count - 1 && correctPlaces.Count == 0)
            {
                if (hand.Count < PlayerBase.maxStonesCount && !pile.IsEmpty())
                    PickStone();
                else
                {
                    GameCore.WriteLog("No correct stones");
                    GameCore.WriteLog("BOT TURN END");
                    gameCore.TurnSwitch();
                    yield break;
                }
            }
        }
        var chosenPlacement = correctPlaces[Random.Range(0, correctPlaces.Count - 1)];//выбор одного из доступных вариантов расположения
        var chosenStone = hand[chosenPlacement.stoneIndex];
        DropStone(chosenStone);//расположение кости
        int side = 0;
        if (chosenPlacement.i1 < chosenPlacement.i2) side = 0;
        if (chosenPlacement.j1 < chosenPlacement.j2) side = 1;
        if (chosenPlacement.i1 > chosenPlacement.i2) side = 2;
        if (chosenPlacement.j1 > chosenPlacement.j2) side = 3;
        chosenStone.Rotate(side);
        gameCore.PlaceStone(chosenStone, chosenPlacement.dualPlacement, chosenPlacement.connectorPart, chosenPlacement.anchor, chosenPlacement.i1, chosenPlacement.j1, chosenPlacement.i2, chosenPlacement.j2);
        GameCore.WriteLog("Bot placed stone");
        GameCore.WriteLog("BOT TURN END");
        gameCore.TurnSwitch();
        yield break;
    }    
}
