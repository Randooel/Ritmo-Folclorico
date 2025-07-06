using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DOAnimDebug : MonoBehaviour
{
    [SerializeField] Transform visual;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            DOOno1();
        }
    }

    void DOOno1()
    {
        visual.DOLocalMoveY(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo);
    }
}
