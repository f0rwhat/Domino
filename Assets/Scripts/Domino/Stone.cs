using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stone : MonoBehaviour
{
    public float speed;
    bool isBeingDragged, isMovable;
    byte firstValue, secondValue, rotationState;
    Vector3 anchorPoint = new Vector3(700,0,0);
    SpriteRenderer firstSprite, secondSprite;
    static Sprite[] spriteSheet;
    bool initialised = false;

    private void Start()
    {
        if (spriteSheet == null)
        {
            spriteSheet = Resources.LoadAll<Sprite>("Domino/Sprites/txt_bones");
        }
        
    }

    public void Init(byte _firstValue, byte _secondValue)//инициализация кости
    {
        firstValue = _firstValue;
        secondValue = _secondValue;
        rotationState = 0;
        isBeingDragged = false;
        firstSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        secondSprite = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
        firstSprite.sprite = spriteSheet[firstValue];
        secondSprite.sprite = spriteSheet[secondValue];
        gameObject.SetActive(false);
        initialised = true;
    }

    public void Deploy(bool _isMovable)//подготовка кости к появлению в игре
    {
        gameObject.SetActive(true);
        isMovable = _isMovable;
    }

    public void Place(Vector3 pos)//расположение
    {
        transform.localPosition = pos;
        isMovable = false;
    }

    public void SetNewAnchor(Vector3 newAnchor)//установка позиции, к которой будет стремится кость после взаимодействия
    {
        //Debug.Log("Anchor set:"+newAnchor.x+" " + newAnchor.y + " " + newAnchor.z);
        newAnchor.z = 0.77f;
        anchorPoint = newAnchor;
    }

    public (int firstValue, int secondValue) Values()//получить значения
    {
        return ((int)firstValue, (int)secondValue);
    }

    public int GetRotationState()//получить вращение
    {
        return rotationState;
    }

    public bool IsBeingDragged()//проверка на взаимодействие с костью
    {
        return isBeingDragged;
    }

    public void Rotate(int count)//повернуть кость, count - кол-во поворотовн а 90 градусов
    {
        transform.RotateAround(transform.position, Vector3.forward, count * 90);
        rotationState += (byte)count;
        if (rotationState >= 4)
        {
            rotationState = (byte)(rotationState % 4);
        }
    }
    public void MakeUnmovable()//сделать кость неинтерактивной
    {
        isMovable = false;
    }
    public (Vector3 firstHalfPos, Vector3 secondHalfPos)  HalfsPositions()//получить позиции каждой половинки
    {
        //Debug.Log(firstSprite.transform.position +"  "+ secondSprite.transform.position);
        return (firstSprite.transform.position, secondSprite.transform.position);
    }
    void Move()//функция, осуществляющая движение кости
    {
        {   if (isBeingDragged)
            {
                var camera = GameCore.CurrentCamera();
                //transform.position = mp;

                var distance_to_screen = camera.WorldToScreenPoint(gameObject.transform.position).z;
                var pos_move = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
                transform.position = new Vector3(pos_move.x, pos_move.y, 0.5f);
            }
            else
            {
                if (rotationState % 2 == 1 && isMovable)
                {
                    Rotate(1);
                }
                if (Vector3.Distance(anchorPoint, transform.localPosition) > 0.01)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, anchorPoint, speed * Time.deltaTime);
                }
                else
                {
                    transform.localPosition = anchorPoint;
                    firstSprite.sortingOrder = 0;
                    secondSprite.sortingOrder = 0;
                }
            }
        }
    }

    //bool IfCotainsMouse()//проверка на наличие курсора внутри кости
    //{
    //    var mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    return (GetComponent<BoxCollider2D>().bounds.Contains(mp));
    //}

    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && isMovable && !GameCore.stoneBeingDragged)
        {
            isBeingDragged = true;
            GameCore.stoneBeingDragged = true;
            GameCore.draggedStone = this;
            firstSprite.sortingOrder = 10;
            secondSprite.sortingOrder = 10;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && isMovable)
        {
            isBeingDragged = false;
            firstSprite.sortingOrder = 5;
            secondSprite.sortingOrder = 5;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && isBeingDragged && isMovable)
        {
            Rotate(1);
        }
    }

    void Update()
    {
        if (!initialised)
            return;
        Move();
        if (GameCore.isGameOnPause)
        {
            isBeingDragged = false;
            return;
        }
       

    }
}

