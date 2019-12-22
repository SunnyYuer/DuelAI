﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPosition
{
    public const int deck = 1;
    public const int extra = 2;
    public const int grave = 3;
    public const int except = 4;
    public const int handcard = 5;
    public const int monster = 6;
    public const int magictrap = 7;
    public const int field = 8;
    public const int special = 9;
}

public class CardMean
{
    //怪兽
    public const int faceupatk = 1;
    public const int faceupdef = 2;
    public const int facedowndef = 3;
    //魔陷
    public const int faceupmgt = 1;
    public const int facedownmgt = 3;
    //综合
    public const int faceup = 2;
    public const int facedown = 3;
}

public class GameEvent
{
    public const int drawcard = 1;
    public const int specialsummon = 2;
}

public class BuffType
{
    public const int attnew = 1;  // string 新的怪兽属性
    public const int levelnew = 2;  // int 新的怪兽星级
    public const int levelcha = 3;  // int 怪兽星级改变的数值
    public const int racenew = 4;  // string 新的怪兽种族
    public const int atkcha = 5;  // int 攻击力改变的数值
    public const int atkmul = 6;  // double 攻击力改变为几倍
    public const int defcha = 7;  // int 防御力改变的数值
    public const int defmul = 8;  // double 防御力改变为几倍
}
