﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pile: MonoBehaviour
{
    private Stone stonePrefab;
    private List<Stone> pile = new List<Stone>();

    void Start()
    {
        //stonePrefab = GameObject.Find("Stone").GetComponent<Stone>();
    }
    public void GeneratePile()//инициализация кучи
    {
        stonePrefab = GameObject.Find("Stone").GetComponent<Stone>();
        //GameObject canvas = GameObject.Find("Canvas");
        for (byte i = 0; i <= 6; i++)
        {
            for (byte j = i; j <= 6; j++)
            {
                Stone temp = Stone.Instantiate<Stone>(stonePrefab,new Vector3 (0,0,0),stonePrefab.transform.rotation);
                temp.Init(i, j);
                pile.Insert(Random.Range(0, pile.Count), temp);
            }
        }
    }

    public bool IsEmpty()//проверка на наличие костей
    {
        return (pile.Count == 0);
    }
    public void ClearPile()
    {
        pile.Clear();
    }
    public Stone GetStone()//взять одну кость из кучи
    {
        Stone returnStone = pile[0];
        pile.RemoveAt(0);
        return returnStone;
    }
}
