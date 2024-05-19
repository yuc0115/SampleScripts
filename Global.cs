using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ActorTagKind
{
    None,
    Player,
    Enemy,
    end
}

public enum EnemyState
{
    Ready,
    TargetChase,
    Attack,
    Down,
    Skill,
    Die,
}

public enum EnemySkillState
{
    ArcStart        = 110,
    ArcEnd          = 111,
    JumpAttack      = 120,
    Phase_2         = 130,
    Phase_2_Idle    = 131,
    Phase_2_End     = 132,

}

public enum PlayerState
{
    Ready,
    Move,
    Skill,
    Atk,
    Die,
    end,
}

public enum PlayerSkillState
{
    None = 0,
    Defend      = 100,
    CountAttack = 101,
    BlackHole,
    LeapAttack  = 110,
    Dash        = 120,
    Spin        = 130,
    PalmStrike  = 140,
    BreathOfFire= 150,
    GroundOfFire= 160,
    end
}


public enum SpecialHitEffect
{
    None,
    BlowAway,
    Neckback, 
    end
}

public enum IndicatorAngle
{
    Angle_60    = 60,
    //Angle_90  = 90,
    Angle_120   = 120,
    //Angle_150 = 150,
    Angle_180   = 180,
    Angle_360   = 360,
    end
}

/// <summary>
/// 애니메이션 이벤트의 파라미터 값과 동일해야함.
/// </summary>
public enum ProjectileState
{
    None    = 0,
    Normal_1 = 1,
    Normal_2 = 2,
    Normal_3 = 3,

    end,
}


public class Global
{
    private static Global _global = new Global();
    public static Global Inst { get { return _global; } }

    public Dictionary<int, ActorBase> dicEnemy = new Dictionary<int, ActorBase>();
    public Player player = null;
    public CameraController camController = null;

    
    /// <summary>
    /// 가장 가까운 적을 찾는다.
    /// </summary>
    /// <returns></returns>
    public ActorBase FindEnemyTarget()
    {
        ActorBase actor = null;

        // 가장 가까운 타겟을 찾는다.
        Vector3 vSource = player.transform.position;
        float dist = float.MaxValue;
        float temp = 0.0f;
        foreach (var pair in Global.Inst.dicEnemy)
        {
            temp = Vector3.Magnitude(pair.Value.transform.position - vSource);
            if (temp < dist)
            {
                dist = temp;
                actor = pair.Value;
            }
        }
        return actor;
    }

    /// <summary>
    /// 적을 바라본다.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public ActorBase LookAtTarget(ActorBase source, ActorTagKind target)
    {
        ActorBase actorTarget = null;

        switch (target)
        {
            case ActorTagKind.Enemy:
                actorTarget = Global.Inst.FindEnemyTarget();
                break;

            case ActorTagKind.Player:
                actorTarget = Global.Inst.player;
                break;

            default:
                Debug.LogErrorFormat("TargetError : {0}", target);
                return actorTarget;
        }

        if (actorTarget == null)
        {
            return actorTarget;
        }


        Vector3 sourcePos = source.transform.position;
        Vector3 vDest = actorTarget.transform.position - sourcePos;
        vDest.y = 0;

        // 회전
        Vector3 vLook = sourcePos + vDest.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(vLook - sourcePos);
        source.transform.rotation = targetRotation;

        return actorTarget;
    }

    /// <summary>
    /// 타격이 가능한지 확인.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="atkAngle">공격 범위 각도, 0일경우 거리 무제한</param>
    /// <param name="atkRange">공격 범위, 0일 경우 360도</param>
    /// <returns></returns>
    public bool IsAttackRange(ActorBase source, ActorBase target, int atkRange = 0, int atkAngle = 0)
    {
        if (target.IsDie)
            return false;
        // 범위 체크
        if (atkRange != 0)
        {
            float dist = Vector3.Magnitude(target.transform.position - source.transform.position);
            if (dist > atkRange)
            {
                //Debug.LogFormat("범위가 벗어남. :{0}", dist);
                return false;
            }
        }

        // 각도 체크
        if (atkAngle != 0)
        {
            Vector3 dir = (target.transform.position - source.transform.position).normalized;
            float angle = Vector3.Angle(source.transform.forward, dir);
            if (atkAngle * 0.5f < angle)
            {
                //Debug.LogFormat("충돌 각도가 벗어남. :{0}", angle);
                return false;
            }
        }
        return true;
    }
}

public class AnimKey
{
    public const string animKey = "AnimKey";

    public const string atk01 = "Atk01";
    public const string atk02 = "Atk02";
    public const string atk03 = "Atk03";

    public const string Die = "Die";

    public const string skillTrigger = "Skill";

    public const string Blend = "Blend";
    public const string Appear = "Appear";
}





