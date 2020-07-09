using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSide
{
    public const int own = 0;
    public const int ops = 1;
    public const int both = 2;
}

public class CardPosition
{
    public const int deck = 10;
    public const int extra = 11;
    public const int grave = 12;
    public const int except = 13;
    public const int handcard = 14;
    public const int area = 15; // 场上
    public const int monster = 16;
    public const int magictrap = 17;
    public const int field = 18;
    public const int special = 19;
}

public class GameCard
{
    public const int name = 30;
    public const int type = 31;
    public const int series = 32;
    public const int attribute = 33;
    public const int level = 34;
    public const int race = 35;
    public const int atk = 36;
    public const int def = 37;
    public const int mean = 38;
}

public class CardType
{ // 临时，应该配置在规则文件里
    public const string monster = "怪兽";
    public const string magic = "魔法";
    public const string trap = "陷阱";
}

public class MonsterType
{
    public const string normal = "通常";
    public const string effect = "效果";
    public const string flip = "反转";
}

public class MagicType
{
    public const string normal = "通常";
    public const string quick = "速攻";
    public const string continuous = "永续";
    public const string equip = "装备";
    public const string field = "场地";
}

public class TrapType
{
    public const string normal = "通常";
    public const string continuous = "永续";
    public const string counter = "反击";
}

public class EffectType
{
    public const int startup = 1; // 启动效果
    public const int continuous = 2; // 永续效果
    public const int trigger = 3; // 诱发效果
    public const int cantrigger = 4; // 选发诱发效果
    public const int musttrigger = 5; // 必发诱发效果
    public const int triggerinstant = 6; // 诱发即时效果
    public const int cantriggerinstant = 7; // 选发诱发即时效果
    public const int musttriggerinstant = 8; // 必发诱发即时效果
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

public class GamePhase
{
    public const int turnstart = 0;
    public const int draw = 10;
    public const int standby = 20;
    public const int main1 = 30;
    public const int battle = 40;
    public const int battlestep = 41;
    public const int damageStepStart = 42;
    public const int damageCalBefore = 43;
    public const int damageCalculate = 44;
    public const int damageCalAfter = 45;
    public const int damageStepEnd = 46;
    public const int main2 = 50;
    public const int end = 60;
}

public class GameEvent
{
    public const int drawcard = 1;
    public const int discard = 2;
    public const int selectcard = 3;
    public const int showcard = 4;
    public const int normalsummon = 5;
    public const int specialsummon = 6;
    public const int setmagictrap = 7;
    public const int changemean = 8;
    public const int activateeffect = 9;
    public const int battle = 10;
    public const int battledestroy = 11;
    public const int effectdestroy = 12;
}

public class PlayerAction : GameEvent
{
    public const int enterbattle = 50;
    public const int endbattle = 51;
    public const int endturn = 52;
}

public class BuffType
{
    public const int atknew = 1;  // 攻击力
    public const int defnew = 2;  // 防御力
    public const int attnew = 3;  // 怪兽属性
    public const int levelnew = 4;  // 怪兽星级
    public const int racenew = 5;  // 怪兽种族
}
