using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pile
{
    private Stone stonePrefab;
    private List<Stone> pile = new List<Stone>();

    public Pile()
    {
        stonePrefab = Resources.Load<Stone>("Domino/Prefab/Stone");
    }
    public void GeneratePile()//инициализация кучи
    {
        //GameObject canvas = GameObject.Find("Canvas");
        for (byte i = 0; i <= 6; i++)
        {
            for (byte j = i; j <= 6; j++)
            {
                Stone temp = Stone.Instantiate<Stone>(stonePrefab, new Vector3(0, 0, 0), stonePrefab.transform.rotation);
                temp.Init(i, j);
                pile.Insert(Random.Range(0, pile.Count), temp);
            }
        }
    }

    public Stone GetDual()//поиск кости с одинаковыми половинками, необходимо для начала игры
    {
        for (int i = 0; i < pile.Count; i++) 
        {
            if (pile[i].Values().firstValue == pile[i].Values().secondValue)
            {
                Stone returnStone = pile[i];
                pile.RemoveAt(i);
                returnStone.Deploy(false);
                return returnStone;
            }
        }
        return new Stone();
    }

    public bool IsEmpty()//проверка на наличие костей
    {
        return (pile.Count == 0);
    }

    public Stone GetStone()//взять одну кость из кучи
    {
        Stone returnStone = pile[0];
        pile.RemoveAt(0);
        return returnStone;
    }
}
