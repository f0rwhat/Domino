using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase//базовый класс для Player и Bot
{
    public const int maxStonesCount = 16;
    const int stonesSpacing = 1;
    protected List<Stone> hand;
    protected Vector2 handPos;
    bool areStonesMovable;
    

    public PlayerBase(bool _areStonesMovable, Vector2 _handPos)
    {
        hand = new List<Stone>();
        areStonesMovable = _areStonesMovable;
        handPos = _handPos;
    }
    void ShiftStones()
    {
        
        Vector3 pos = new Vector3(-(float)hand.Count / 2 + 0.5f,handPos.y);
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].SetNewAnchor(pos);
            pos.x += stonesSpacing;
        }
    }
    public void PickStone()//взять кость в руку
    {
        Stone newStone = GameCore.stonesPile.GetStone();
        newStone.Deploy(areStonesMovable);
        hand.Add(newStone);
        ShiftStones();
    }

    public void DropStone(Stone stone)//удалить кость из руки
    {
        stone.MakeUnmovable();
        hand.Remove(stone);
        ShiftStones();
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
