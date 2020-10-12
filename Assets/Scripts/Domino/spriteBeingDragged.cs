using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteBeingDragged : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnMouseDown()
    {
        transform.parent.gameObject.SendMessage("enableDrag", true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
