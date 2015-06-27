using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TNObject))]
public class Entity : TNBehaviour  {
    public readonly int maxLive = 100;
    public int live = 100;

    protected virtual void Start()
    {
    }

    public bool IsAlive(){
        return live > 0;
    }


    public virtual bool IsPlayer() { return false; }
}
