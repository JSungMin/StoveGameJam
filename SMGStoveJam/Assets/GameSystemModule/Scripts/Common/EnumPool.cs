using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    Frame,
    Spine
}

public enum ActorEmotionType
{
    Normal,
    Smile01,
    Smile02,
    Angry01,
    Angry02,
    Painful01,
    Painful02,
    Surprise01,
    Surprise02,
    ETC
}
public enum StatusAffectMethod
{
    Once,
    Tick,
    Forever
}
public enum SkillType
{
    PhysicalAttack = 0,
    MagicalAttack,
    Buff,
    DeBuff,
    Avoid,
    Parry
}

public enum SkillCoverageType
{
    None,
    High,
    Middle,
    Low,
    All
}
public enum ItemType
{
    장비,
    음식,
    재료,
    조리법
}
public enum FoodCookDiv
{
    구이,
    튀김,
    탕,
    디저트,
    후첨
}
public enum FoodBigDiv
{
    고기,
    생선,
    채소,
    쿠키,
    아이스크림
}
public enum FoodMiddleDiv
{
    꼬치,
    스테이크
}
public enum SFXType
{
    None
}
//  DON'T CHANGE ORDER
public enum TimerState
{
    FINISH = 0,
    PAUSE,
    RUNNING
}
