﻿using UnityEngine;

public class FieldObject : MonoBehaviour
{
    //현재 체력에 대한 변수
    public float _curHp;
    
    //체력의 최대치 변수
    public float _maxHp;

    //아군인지 적군인지에 대한 변수
    public eTeam _team = eTeam.PLAYER;

    //죽었는지 살았는지에 대한 변수
    public bool _isdead;

    public virtual void DamageReceive(float damage)
    {
    }

    public bool IsDead
    {
        get => _isdead;
        set {  }
    }
}
