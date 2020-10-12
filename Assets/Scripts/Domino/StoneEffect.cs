using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StoneEffect:MonoBehaviour
{
    static Sprite[] spriteSheet;
    byte rotationState;
    SpriteRenderer firstSprite, secondSprite;
    Color greenColor, transparentColor;
    public enum EffectType
    {
        ET_Transparent = 0,
        ET_Green = 1,
    }

    private void Start()
    {
        spriteSheet = Resources.LoadAll<Sprite>("Domino/Sprites/txt_bones");
        greenColor = new Color(255, 255, 0, 0.7f);
        transparentColor = new Color(0, 0, 0, 0);
    }


    public void SetRotationState(int state)//поворот эффекта
    {
        if (rotationState != (byte)state)
        {
            transform.RotateAround(transform.position, Vector3.forward, - 90 * rotationState);
            rotationState = (byte)state;
            transform.RotateAround(transform.position, Vector3.forward, 90 * rotationState);
        }
    }

    public void Deploy()//инициализация спрайтов
    {
        gameObject.SetActive(true);
        firstSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        secondSprite = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
        firstSprite.sprite = spriteSheet[0];
        secondSprite.sprite = spriteSheet[0];
    }

    public void SetEffect(EffectType effectType)//выбор цвета
    {
        switch (effectType)
        {
            case EffectType.ET_Transparent:
                firstSprite.color = transparentColor;
                secondSprite.color = transparentColor;
                break;
            case EffectType.ET_Green:
                firstSprite.color = greenColor;
                secondSprite.color = greenColor;
                break;
        }
    }

    void Update()
    {

    }

}
