using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy
{

    public enum SkillState
    {
        AcrAttack_120,
        ArcAttack_180,
        ArcATtack_360,
        JumpAttack,
        end,
    }

    public float _skillTime = 5;
    private float _skillDelay = 15.0f;
    /// <summary>
    /// 스킬을 쓸 타이밍이 됬는지 체크한다.
    /// </summary>
    private bool _isSkillReady = false;

    public override void Init(int idx, bool isAppear, System.Action appearEnd = null)
    {
        base.Init(idx, isAppear, appearEnd);

        // 기본 스탯 설정.
        _maxHP = 1000;
        _currentHp = _maxHP;
        _moveSpeed = 5;
        _damage = 20;
        _atkRange = 5.0f;
        _atkDelay = 5.0f;
        _atkAngle = 90;
        _height = 5.0f;

        // 네비매시에이전트 속도 설정.
        _navMeshAgent.speed = _moveSpeed;
    }

    protected override void Update()
    {
        base.Update();

        // 스킬을 쓸 타이밍이 됬는지 확인후 변수 저장.
        if (_isSkillReady == false)
        {
            _skillTime += Time.deltaTime;

            if (_skillDelay < _skillTime)
            {
                _isSkillReady = true;
                _skillTime = 0.0f;
            }
        }

        /*
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ArcAttack(15, IndicatorAngle.Angle_60));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(ArcAttack(15, IndicatorAngle.Angle_120));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ArcAttack(15, IndicatorAngle.Angle_180));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ArcAttack(15, IndicatorAngle.Angle_360));
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(JumpAttack());
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(Phase2Routine());
        }
        */
    }



    /// <summary>
    /// 공격시간일때 공격을 하거나 스킬을 쓰거나 정한다.
    /// </summary>
    protected override void TimeToAttack()
    {
        // 스킬을 쓸 차례가 아니라면 공격.
        if (_isSkillReady == false)
        {
            base.TimeToAttack();
            return;
        }

        _isSkillReady = false;
        FireSkill();
    }

    /// <summary>
    /// 스킬 발사. 가지고 잇는 스킬중 랜덤으로 사용한다.
    /// </summary>
    private void FireSkill()
    {
        // 스킬로 상태 변경.
        ChangeState(EnemyState.Skill);

        int skill = Random.Range(0, (int)SkillState.end);

        switch ((SkillState)skill)
        {
            case SkillState.AcrAttack_120:
                StartCoroutine(ArcAttack(20, IndicatorAngle.Angle_120));
                break;

            case SkillState.ArcAttack_180:
                StartCoroutine(ArcAttack(15, IndicatorAngle.Angle_180));
                break;

            case SkillState.ArcATtack_360:
                StartCoroutine(ArcAttack(10, IndicatorAngle.Angle_360));
                break;

            case SkillState.JumpAttack:
                StartCoroutine(JumpAttack());
                break;

            default:
                Debug.LogErrorFormat("스킬 상태가 정의되지 않았다. {0}", (SkillState)skill);
                break;
        }
    }

    #region phase2
    private float _phase2Delay = 2.5f;
    private int _phase2Range = 20;
    IEnumerator Phase2Routine()
    {
        _animator.SetInteger(AnimKey.animKey, (int)EnemySkillState.Phase_2);

        // phase 2가 끝나고 idle로 넘어 갔는지를 체크한다
        AnimatorStateInfo stateinfo;
        while (true)
        {
            stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateinfo.IsName(EnemySkillState.Phase_2_Idle.ToString()))
                break;
            yield return null;
        }

        // indicator 생성.
        IndicatorBase indicator = IndicatorManager.Inst.CreateIndicator(IndicatorAngle.Angle_360, false);
        indicator.Init(this, 5, _phase2Range, IndicatorAngle.Angle_360);

        yield return new WaitForSeconds(_phase2Delay);
        _animator.SetInteger(AnimKey.animKey, (int)EnemySkillState.Phase_2_End);


        //Debug.LogError(stateinfo.cli);
        yield break;
    }

    private void OnSkillAttackPhase2()
    {
        if (Global.Inst.IsAttackRange(this, Global.Inst.player, _phase2Range))
        {
            Debug.LogError("때려!");
        }
    }
    #endregion

    #region jumpAttack
    private int _jumpAttackRange = 5;
    IEnumerator JumpAttack()
    {
        // 타겟을 바라본다.
        Global.Inst.LookAtTarget(this, ActorTagKind.Player);

        // 인디 게이터 생성 및 초기화.
        float indicatorduration = 1.3f;
        IndicatorBase indicator = IndicatorManager.Inst.CreateIndicator(IndicatorAngle.Angle_360, false);
        indicator.Init(Global.Inst.player, indicatorduration, _jumpAttackRange, IndicatorAngle.Angle_360);

        _animator.SetInteger(AnimKey.animKey, (int)EnemySkillState.JumpAttack);

        yield return new WaitForSeconds(0.5f);

        // 플레이어 쪽으로 이동.
        float speed = 2.0f;
        float time = 0.0f;
        Vector3 vStart = transform.position;
        Vector3 vDest = Global.Inst.player.transform.position;
        while (true)
        {
            time += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(vStart, vDest, time);

            if (time >= 1.0f)
            {
                break;
            }
            yield return null;
        }        
        yield break;
    }

    /// <summary>
    /// 애니메이션 공격타이밍 이벤트.
    /// </summary>
    private void OnSkillAttackJumpAtk()
    {
        if (Global.Inst.IsAttackRange(this, Global.Inst.player, _jumpAttackRange))
        {
            Global.Inst.player.OnDamage(Random.Range(30, 50), SpecialHitEffect.Neckback);
        }

        Global.Inst.camController.StartShake();
    }
    #endregion

    #region arcAttack
    private IndicatorAngle _arcSkillAngle;
    private int _arcSkillRange;
    private IEnumerator ArcAttack(int range, IndicatorAngle angle)
    {
        // 타겟을 바라본다.
        Global.Inst.LookAtTarget(this, ActorTagKind.Player);

        _arcSkillAngle = angle;
        _arcSkillRange = range;
        
        _animator.SetInteger(AnimKey.animKey, (int)EnemySkillState.ArcStart);
        
        float indicatorDuration = 3.0f;

        // indicator 생성 및 초기화.
        IndicatorBase indicator = IndicatorManager.Inst.CreateIndicator(angle, false);
        indicator.Init(this, indicatorDuration, _arcSkillRange, angle);

        yield return new WaitForSeconds(indicatorDuration);

        // 애니메이션 변경.
        _animator.SetInteger(AnimKey.animKey, (int)EnemySkillState.ArcEnd);

        yield break;
        
    }

    /// <summary>
    /// 애니메이션 공격타이밍 이벤트.
    /// </summary>
    private void OnAttackSkillArc()
    {
        Global.Inst.camController.StartShake();

        int nAngle = (int)_arcSkillAngle;

        GameObject goEffect = ResManager.Inst.LoadFile("Effect", "Thunderking_1");
        goEffect = Instantiate(goEffect);
        goEffect.transform.position = transform.position;
        goEffect.transform.eulerAngles = transform.eulerAngles;

        var ps = goEffect.GetComponent<ParticleSystem>();
        var shape = ps.shape;
        shape.radius = _arcSkillRange;
        shape.arc = nAngle;
        shape.rotation = new Vector3(90, (180 - nAngle) * -0.5f, 0);

        
        if (Global.Inst.IsAttackRange(this, Global.Inst.player, _arcSkillRange, nAngle))
        {
            Global.Inst.player.OnDamage(Random.Range(30, 50), SpecialHitEffect.Neckback);
        }
    }
    #endregion
}
