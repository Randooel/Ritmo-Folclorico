using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossCharacter : MonoBehaviour
{
    [SerializeField] public string bossName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void ActionA();

    public abstract void ActionB();
}
