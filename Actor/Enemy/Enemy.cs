using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : ActorBase
{
    [SerializeField]
    private EnemyState _currentState = EnemyState.Ready;

    public NavMeshAgent _navMeshAgent = null;

    protected Animator _animator = null;

    /// <summary>
    /// 어택 딜레이 재기 위한 변수.
    /// </summary>
    private float _atkDelayTime = 0.0f;
    private HPBar _hpBar = null;

    private bool _isInit = false;

    protected virtual void Start()
    {
        Init(0, false);
    }

    public override void Init(int idx, bool isAppear, System.Action onAppearEnd = null)
    {
        if (_isInit == true)
            return;
        _isInit = true;
        base.Init(idx, isAppear, onAppearEnd);

        // 기본 스탯 설정.
        _maxHP = 100;
        _currentHp = _maxHP;
        _moveSpeed = 3;
        _damage = 5;
        _atkRange = 4.0f;
        _atkDelay = 3.0f;
        _atkAngle = 90;

        _animator = GetComponent<Animator>();

        // 네비매쉬 에이전트 추가.
        _navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        _navMeshAgent.radius = 0.1f;
        _navMeshAgent.height = 3.0f;
        _navMeshAgent.speed = _moveSpeed;

        // 충돌 명령어 때문에 추가함.
        CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
        collider.radius = 0.8f;
        collider.height = _height;
        collider.isTrigger = true;
        collider.center = Vector3.up * _height * 0.5f;

        // 충돌 명령어 때문에 추가함.
        var rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;

        if (isAppear == true)
        {
            _animator.Play(AnimKey.Appear);
            StartCoroutine(AppearLengthCheck(onAppearEnd));
        }
        else
        {
            //ChangeState(EnemyState.TargetChase);
        }


        // 체력바 생성.
        _hpBar = HPBarManager.Inst.CreateBar(_idx);
        _hpBar.SetTarget(this);

        // 태그 설정.
        setTag = ActorTagKind.Enemy.ToString();
    }

    IEnumerator AppearLengthCheck(System.Action onAppearEnd)
    {
        AnimatorStateInfo stateinfo;
        while (true)
        {
            stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateinfo.IsName(AnimKey.Appear))
            {
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(stateinfo.length);

        if (onAppearEnd != null)
            onAppearEnd();
        yield break;
    }


    /// <summary>
    /// 데미지 입음.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="hitEffect"></param>
    public override void OnDamage(int damage, SpecialHitEffect hitEffect)
    {
        base.OnDamage(damage, hitEffect);

        _hpBar.SetActive(true);
        _hpBar.SetValue((float)_currentHp / _maxHP);

        GameObject go = GameObject.Instantiate(resHitEffect, transform);

        HudText _hudText = HudManager.Inst.CreateHudText();
        _hudText.SetTarget(this, damage);

        // 스킬 상태라면 특수 상태 먹히지 않도록.
        if (_currentState != EnemyState.Skill)
        {
            switch (hitEffect)
            {
                case SpecialHitEffect.BlowAway:
                    StartCoroutine(BlowAway());
                    break;

                case SpecialHitEffect.Neckback:
                    StartCoroutine(Neckback());
                    break;
            }
        }

        StartCoroutine(RimShaderRoutine(0.1f));
    }

    /// <summary>
    /// 데미지 입었을때 빤짝임 처리.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected IEnumerator RimShaderRoutine(float duration)
    {
        Color color = Color.black;
        float time = 0.0f;
        string shaderNameID = "_RimValue";
        while (time < duration)
        {
            time += Time.deltaTime;
            float value = time / duration;

            for (int i = 0; i < _listSmr.Count; i++)
            {
                for (int j = 0; j < _listSmr[i].materials.Length; j++)
                {
                    _listSmr[i].materials[j].SetFloat(shaderNameID, value);
                }
            }
            yield return null;
        }
        time = duration;
        while (time > 0)
        {
            time -= Time.deltaTime;
            float value = time / duration;
            for (int i = 0; i < _listSmr.Count; i++)
            {
                for (int j = 0; j < _listSmr[i].materials.Length; j++)
                {
                    _listSmr[i].materials[j].SetFloat(shaderNameID, value);
                }

            }
            yield return null;
        }
        yield break;
    }

    public override void OnDie()
    {
        base.OnDie();
        ChangeState(EnemyState.Die);
    }

    /// <summary>
    /// 몬스터 상태 바꿈.
    /// </summary>
    /// <param name="state"></param>
    public override void ChangeState(EnemyState state)
    {
        base.ChangeState(state);
        _currentState = state;

        switch (_currentState)
        {
            case EnemyState.Attack:
                //_navMeshAgent.isStopped = true;
                break;

            case EnemyState.TargetChase:
                _navMeshAgent.enabled = true;
                _navMeshAgent.isStopped = false;
                break;

            case EnemyState.Die:
                GameObject.Destroy(_navMeshAgent);
                _navMeshAgent.enabled = false;
                _animator.SetTrigger("Die");
                StartCoroutine(DeathRoutine());
                break;

            case EnemyState.Skill:
                _navMeshAgent.enabled = false;
                break;
        }
    }

    /// <summary>
    /// 죽고 난 후 삭제 처리.
    /// </summary>
    /// <returns></returns>
    IEnumerator DeathRoutine()
    {
        AnimatorStateInfo info;
        while(true)
        {
            info = _animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName(AnimKey.Die))
            {
                break;
            }
            yield return null;
        }

        // 삭제 애니메이션 타임.
        yield return new WaitForSeconds(info.length + 30.0f);

        //EnemyManager
        EnemyManager.Inst.RemoveEnemy(_idx);
        
        yield break;
    }

    
    public float _neckBackValue = 5.0f;
    public float _neckBackDist = 1.0f;
    /// <summary>
    /// 넉백 처리.
    /// </summary>
    /// <returns></returns>
    IEnumerator Neckback()
    {
      
        _currentState = EnemyState.Ready;
        _navMeshAgent.enabled = false;
        Vector3 v = transform.position - Global.Inst.player.transform.position;
        v = v.normalized;
        float value = _neckBackDist;
        while (true)
        {
            var d = _neckBackValue * Time.deltaTime * 0.98f;
            transform.position += v * d;
            value -= d;

            if (value <= 0 )
            {
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        if (IsDie == false)
        {
            ChangeState(EnemyState.TargetChase);
        }
        yield break;
    }

    IEnumerator BlowAway()
    {
        float gravity = 14.0f;
        float jumpForce = 5.0f;
        float verticalVelocity = jumpForce;

        _navMeshAgent.enabled = false;

        ChangeState(EnemyState.Down);
        _animator.Play("IdleToLayGround");

        Vector3 vDir = transform.position - Global.Inst.player.transform.position;
        vDir.y = 0;
        vDir = vDir.normalized;
        while (true)
        {
            Vector3 v = Vector3.zero;
            verticalVelocity -= gravity * Time.deltaTime;

            v = Vector3.up * verticalVelocity;
            v += vDir * 3.0f;
            transform.Translate(v * Time.deltaTime, Space.World);

            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.LogError(hit.transform.name);
                Transform objectHit = hit.transform;
                if (hit.transform.name.Equals("navi"))
                {
                    float f = Vector3.Distance(transform.position, hit.point);
                    if (f <= 0.1f)
                    {
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("Break;");
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        _navMeshAgent.enabled = true;
        _animator.CrossFade("Lay Ground to Idle", 0.1f);

        yield return new WaitForSeconds(0.2f);
        ChangeState(EnemyState.TargetChase);
    }

    protected virtual void Update()
    {
        switch (_currentState)
        {
            case EnemyState.TargetChase:
                _animator.SetFloat(AnimKey.Blend, 1);
                StateTargetChase();
                break;

            case EnemyState.Attack:
                _animator.SetFloat(AnimKey.Blend, 0);
                StateAttack();
                break;
        }
    }
    
    /// <summary>
    /// 공격
    /// </summary>
    protected virtual void StateAttack()
    {
        _atkDelayTime += Time.deltaTime;
        float dist = (transform.position - Global.Inst.player.transform.position).magnitude;
        if (dist > _atkRange)
        {
            ChangeState(EnemyState.TargetChase);
            return;
        }

        if (_atkDelayTime < _atkDelay)
        {
            return;
        }

        _atkDelayTime = 0;

        TimeToAttack();
    }

    /// <summary>
    /// 공격할 시간.
    /// </summary>
    protected virtual void TimeToAttack()
    {
        // 공격 타이밍!!!!!!!!! 
        NormalAttack();
    }

    /// <summary>
    /// 일반 공격.
    /// </summary>
    protected virtual void NormalAttack()
    {
        // 타겟을 바라본다.
        Global.Inst.LookAtTarget(this, ActorTagKind.Player);
        _animator.SetInteger(AnimKey.animKey, 1);
    }


    /// <summary>
    /// 대상 추격.
    /// </summary>
    private void StateTargetChase()
    {
        if (_navMeshAgent.enabled == false)
            return;

        _navMeshAgent.SetDestination(Global.Inst.player.transform.position);

        float dist = (transform.position - Global.Inst.player.transform.position).magnitude;

        // 사거리 절반 까지 가까이 붙는다.
        if (dist <= _atkRange * 0.8f)
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.enabled = false;
            ChangeState(EnemyState.Attack);
        }
    }

    #region animationEvent
    private void OnAttack()
    {
        Global.Inst.player.OnDamage(Random.Range(5, 10), SpecialHitEffect.None);
    }

    private void ComboCheck()
    {
        var stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.IsName("Atk01"))
        {
            _animator.SetInteger(AnimKey.animKey, 0);
        }
    }
    #endregion
}
