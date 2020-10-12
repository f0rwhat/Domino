using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase//базовый класс для Player и Bot
{
    public const int maxStonesCount = 14;
    const int stonesSpacing = 31;
    protected List<Stone> hand;
    protected Vector2 handPos;
    bool areStonesMovable;
    

    public PlayerBase(bool _areStonesMovable, Vector2 _handPos)
    {
        hand = new List<Stone>();
        areStonesMovable = _areStonesMovable;
        handPos = _handPos;
    }

    public void PickStone()//взять кость в руку
    {
        Stone newStone = GameCore.stonesPile.GetStone();
        Vector3 pos = new Vector3(handPos.x + stonesSpacing * hand.Count, handPos.y, 0);
        newStone.SetNewAnchor(pos);
        newStone.Deploy(areStonesMovable);
        hand.Add(newStone);
    }

    public void DropStone(Stone stone)//удалить кость из руки
    {
        stone.MakeUnmovable();
        hand.Remove(stone);
        for (int i = 0; i < hand.Count; i++)
        {
            Vector3 pos = new Vector3(handPos.x + stonesSpacing * i, handPos.y, 0);
            hand[i].SetNewAnchor(pos);
        }
    }

    public bool HasStoneWithValue(int value)//проверка на наличие костей с таким значением
    {
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].Values().firstValue == value || hand[i].Values().secondValue == value)
            {
                return true;
            }
        }
        return false;
    }

    public int StonesInHandCount()//кол-во костей в руке
    {
        return hand.Count;
    }
} 
