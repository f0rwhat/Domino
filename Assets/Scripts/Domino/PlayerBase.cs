using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour//базовый класс для Player и Bot
{
    public const int maxStonesCount = 16;
    private const int stonesSpacing = 1;
    private bool areStonesMovable;
    protected Pile pile;
    protected List<Stone> hand;
    protected Vector2 handPos;


    public PlayerBase(bool _areStonesMovable, Vector2 _handPos)
    {
        hand = new List<Stone>();
        areStonesMovable = _areStonesMovable;
        handPos = _handPos;
    }
    protected void Start()
    {
        pile = GameObject.Find("Pile").GetComponent<Pile>();
    }
    void ShiftStones()
    {

        Vector3 pos = new Vector3(-(float)hand.Count / 2 + 0.5f, handPos.y);
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].SetNewAnchor(pos);
            pos.x += stonesSpacing;
        }
    }
    public void PickStone()//взять кость в руку
    {
        Stone newStone = pile.GetStone();
        newStone.Deploy(areStonesMovable);
        hand.Add(newStone);
        ShiftStones();
    }

    public Stone ShowBiggestStone()
    {
        int sum = hand[0].Values().firstValue + hand[0].Values().secondValue,
            index = 0;
        for (int i = 1; i < hand.Count; i++)
        {
            int newSum = hand[i].Values().firstValue + hand[i].Values().secondValue;
            if (newSum > sum)
            {
                sum = newSum;
                index = i;
            }
        }
        return hand[index];
    }
    public void DropStone(Stone stone)//удалить кость из руки
    {
        stone.MakeUnmovable();
        hand.Remove(stone);
        ShiftStones();
    }

    public void ClearHand()
    {
        hand.Clear();
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
