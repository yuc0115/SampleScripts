using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : ActorBase
{
    private PlayerAnimController _animController = null;
    public PlayerState _currentState = PlayerState.Ready;
    public PlayerSkillState _currentSkillState = PlayerSkillState.None;

    private PlayerController _playerMove = null;
    private PlayerSkill _skill = null;
    private Slider _hpSlider = null;
    private ActorBase _projectileTarget = null;

    private void Start()
    {
        _animController = gameObject.AddComponent<PlayerAnimController>();
        _animController.Init();

        // 캐릭터 무브 붙임.
        _playerMove = gameObject.AddComponent<PlayerController>();
        _playerMove.Init();

        ChangeState(PlayerState.Ready);

        _skill = gameObject.AddComponent<PlayerSkill>();
        _skill.SetAnimController(_animController);

        GameObject go = GameObject.Find("Canvas/Player/HPSlider");
        _hpSlider = go.GetComponent<Slider>();
        SetHPGauge();

        Global.Inst.player = this;
    }

    private void SetHPGauge()
    {
        float value = _currentHp / (float)_maxHP;
        _hpSlider.value = value;
    }

    public void SetEnable(bool isEnable)
    {
        _playerMove.enabled = isEnable;
        _skill.enabled = isEnable;
    }

    public void ChangeState(PlayerState state, PlayerSkillState skillState = PlayerSkillState.None)
    {
        _currentState = state;
        switch (state)
        {
            case PlayerState.Ready:
                _playerMove.SetMove = true;
                _animController.SetNormalState();
                break;

            case PlayerState.Atk:
                // 타겟을 바라본다.
                _projectileTarget = Global.Inst.LookAtTarget(this, ActorTagKind.Enemy);
                _playerMove.SetMove = false;
                break;

            case PlayerState.Skill:
                _animController.SetTrigger(AnimKey.skillTrigger);
                ChangeStateSkill(skillState);
                break;
        }
    }

    private void ChangeStateSkill(PlayerSkillState state)
    {
        _currentSkillState = state;
        _playerMove.SetMove = false;
        switch (state)
        {
            case PlayerSkillState.Defend:
                StartCoroutine(_skill.Defend());
                break;

            case PlayerSkillState.LeapAttack:
                StartCoroutine(_skill.LeapAttack());
                StartCoroutine(_playerMove.Jump());
                break;

            case PlayerSkillState.Spin:
                _playerMove.SetMove = true;
                StartCoroutine(_skill.Spin());
                break;

            case PlayerSkillState.BreathOfFire:
                _animController.SetInterger(AnimKey.animKey, (int)PlayerSkillState.BreathOfFire);
                //StartCoroutine(_skill.BreathOfFire());
                break;

            case PlayerSkillState.GroundOfFire:
                _animController.SetInterger(AnimKey.animKey, (int)PlayerSkillState.GroundOfFire);
                break;

            case PlayerSkillState.Dash:
                StartCoroutine(_skill.Dash());
                StartCoroutine(_playerMove.Dash(OnAttack));
                break;

            case PlayerSkillState.PalmStrike:
                _projectileTarget = Global.Inst.LookAtTarget(this, ActorTagKind.Enemy);
                _animController.SetInterger(AnimKey.animKey, (int)PlayerSkillState.PalmStrike);
                break;

            default:
                Debug.LogErrorFormat("처리 안됨 : {0}", state);
                break;
        }
    }

    public void Attack()
    {
        if (_animController.ComboStart())
        {
            // 상태 변경.
            ChangeState(PlayerState.Atk);
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_animController.ComboStart())
            {
                ChangeState(PlayerState.Atk);
            }
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeState(PlayerState.Skill, PlayerSkillState.Defend);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeState(PlayerState.Skill, PlayerSkillState.LeapAttack);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeState(PlayerState.Skill, PlayerSkillState.Dash);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeState(PlayerState.Skill, PlayerSkillState.Spin);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            ChangeState(PlayerState.Ready);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeState(PlayerState.Skill, PlayerSkillState.PalmStrike);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _projectileTarget = Global.Inst.LookAtTarget(this, ActorTagKind.Enemy);
            Projectile projectile = ProjectileManager.Inst.CreateProjectile("Cube");
            projectile.InitSpecial(MoveType.Arc, "Cube", this, _projectileTarget, transform.position + transform.forward * 0.5f);
        }
    }

    /// <summary>
    /// 데미지 입음.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="hitEffect"></param>
    public override void OnDamage(int damage, SpecialHitEffect hitEffect)
    {
        base.OnDamage(damage, hitEffect);
        Debug.LogFormat("currentHP : {0}", _currentHp);

        GameObject go = GameObject.Instantiate(resHitEffect, transform);

        HudText _hudText = HudManager.Inst.CreateHudText();
        _hudText.SetTarget(this, damage);

        SetHPGauge();
    }

    

    #region animation event
    /// <summary>
    /// 애니메이션 이벤트에서 호출
    /// </summary>
    /// <param name="kind"></param>
    private void OnProjectile(int kind)
    {
        switch ((ProjectileState)kind)
        {
            case ProjectileState.Normal_1:
                break;
        }

        Projectile projectile = ProjectileManager.Inst.CreateProjectile("Ball");
        projectile.InitNormal("Ball", this, _projectileTarget, transform.position + transform.forward * 0.5f, 30.0f, 15.0f);
    }

    /// <summary>
    /// 타격 타이밍.
    /// </summary>
    private void OnAttack()
    {
        Transform t = transform;
        Vector3 v = Vector3.zero;

        SpecialHitEffect hit = SpecialHitEffect.None;

        
        if (_currentState == PlayerState.Skill)
        {
            switch (_currentSkillState)
            {

                case PlayerSkillState.Defend:
                    hit = SpecialHitEffect.BlowAway;

                    GameObject gosource = ResManager.Inst.LoadFile("Effect", "CFX_Explosion_B_Smoke+Text");
                    gosource = GameObject.Instantiate(gosource);
                    gosource.transform.position = transform.position + transform.forward + Vector3.up;
                    break;


                case PlayerSkillState.LeapAttack:
                    gosource = ResManager.Inst.LoadFile("Effect", "CFX_MagicPoof");
                    gosource = GameObject.Instantiate(gosource);
                    gosource.transform.position = transform.position + transform.forward + Vector3.up;
                    break;


                case PlayerSkillState.Dash:
                    hit = SpecialHitEffect.Neckback;
                    break;

                case PlayerSkillState.BreathOfFire:
                    //_skill.BreathOfFire(_projectileTarget);
                    break;

                default:
                    hit = SpecialHitEffect.None;
                    break;
            }
        }

        foreach (var pair in Global.Inst.dicEnemy)
        {
            v = t.position - pair.Value.gameObject.transform.position;
            if (v.magnitude > _atkRange)
                continue;
            pair.Value.OnDamage(10, hit);
        }
    }

    /// <summary>
    /// 스킬 발사 타이밍.
    /// </summary>
    private void OnTimeToSkill()
    {
        if (_currentState != PlayerState.Skill)
            return;
        
        switch (_currentSkillState)
        {
            case PlayerSkillState.PalmStrike:
                _skill.PalmStrike(_projectileTarget);
                return;

            case PlayerSkillState.BreathOfFire:
                _skill.BreathOfFire();
                break;

            case PlayerSkillState.GroundOfFire:
                _skill.GroundOfFire();
                break;

            default:
                Debug.LogErrorFormat("처리안됨 : {0}", _currentSkillState);
                break;
        }
    }
    #endregion
}
