using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{

    private Animator _animator = null;
    private PlayerAnimController _animController = null;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animController.GetComponent<PlayerAnimController>();
        
    }

    public void SetAnimController(PlayerAnimController animController)
    {
        _animController = animController;
    }



    #region PalmStrike
    public void PalmStrike(ActorBase target)
    {
        StartCoroutine(PlamStrikeroutine(target));
    }

    private IEnumerator PlamStrikeroutine(ActorBase target)
    {
        Vector3 v = Vector3.zero;
        Vector3[] v1 = new Vector3[]
        {
            Vector3.zero,
             -transform.right * 1f,
             transform.right * 1f,
        };
        // 세발 발사.
        for (int i = 0; i < v1.Length; i++)
        {
            Projectile projectile = ProjectileManager.Inst.CreateProjectile("Ball");
            v = transform.position + transform.forward + v1[i];
            projectile.InitNormal("Ball", Global.Inst.player, target, v, 30.0f, 20.0f);

            yield return new WaitForSeconds(0.2f);
        }

        yield break;
    }
    #endregion




    /*
    #region blockHole
    public float speed = 3.0f;
    public IEnumerator BlackHole()
    {
        GameObject gosource = ResManager.Inst.LoadFile("Effect", "CFX_Magical_Source");
        gosource = GameObject.Instantiate(gosource);

        gosource.transform.position = transform.position + Vector3.up;

        Vector3 vPos = transform.position;
        Vector3 v = Vector3.zero;
        float duration = 2.5f;

        foreach (var pair in Global.Inst.dicEnemy)
        {
            ((Enemy)pair.Value)._navMeshAgent.speed = 0;
        }

        while (true)
        {
            foreach (var pair in Global.Inst.dicEnemy)
            {
                v = vPos - pair.Value.transform.position;
                pair.Value.transform.position += v * speed * Time.deltaTime;
            }


            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                break;
            }
            yield return null;
        }

        //CFX_MagicPoof
        
        gosource = ResManager.Inst.LoadFile("Effect", "CFX_MagicPoof");
        gosource = GameObject.Instantiate(gosource);
        gosource.transform.position = vPos + Vector3.up;


        float value = 1.5f;
        float dddspeed = 10;
        while (true)
        {
            var d = dddspeed * Time.deltaTime * 0.98f;

            foreach (var pair in Global.Inst.dicEnemy)
            {
                v = pair.Value.transform.position - vPos;
                v = v.normalized;
                pair.Value.transform.position += v * d;
            }

            value -= d;

            if (value <= 0)
            {
                break;
            }

            yield return null;
        }

        foreach (var pair in Global.Inst.dicEnemy)
        {
            ((Enemy)pair.Value)._navMeshAgent.speed = 3.5f;
        }
        yield break;
    }
    #endregion
    */

    #region GroundOfFire
    public void GroundOfFire()
    {
        ActorBase target = Global.Inst.LookAtTarget(Global.Inst.player, ActorTagKind.Enemy);
        if (target == null)
            return;
        Projectile projectile = ProjectileManager.Inst.CreateProjectile("GroundOfFire");
        projectile.InitSpecial(MoveType.Arc, "GroundOfFire", Global.Inst.player, target, transform.position + transform.forward * 0.5f, OnArrivalGroundOfFire);
    }

    /// <summary>
    /// 발사체 도착.
    /// </summary>
    private void OnArrivalGroundOfFire(Vector3 vPos)
    {
        GameObject gosource = ResManager.Inst.LoadFile("Effect", "CFX_Explosion_B_Smoke+Text");
        gosource = GameObject.Instantiate(gosource);
        gosource.transform.position = vPos + Vector3.up * Global.Inst.player._height;

        foreach (var pair in Global.Inst.dicEnemy)
        {
            if (Global.Inst.IsAttackRange(Global.Inst.player, pair.Value, 8))
            {
                int damage = UnityEngine.Random.Range(10, 20);
                pair.Value.OnDamage(damage, SpecialHitEffect.None);
            }
        }

        Debug.LogError("도착");
    }
    #endregion

    #region BreathOfFire
    public void BreathOfFire()
    {
        GameObject gosource = ResManager.Inst.LoadFile("Effect", "CFX2_WWExplosion");
        gosource = GameObject.Instantiate(gosource);
        gosource.transform.position = transform.position;

        foreach (var pair in Global.Inst.dicEnemy)
        {
            if (Global.Inst.IsAttackRange(Global.Inst.player, pair.Value, 8))
            {
                int damage = UnityEngine.Random.Range(10, 20);
                pair.Value.OnDamage(damage, SpecialHitEffect.Neckback);
            }
        }
    }

    #endregion

    #region defend
    public IEnumerator Defend()
    {
        _animController.SetInterger(AnimKey.animKey, (int)PlayerSkillState.Defend);

        yield return new WaitForSeconds(1.0f);
        var key = _animController.GetInteger(AnimKey.animKey);
        if (key == (int)PlayerSkillState.Defend)
        {
            _animController.SetInterger(AnimKey.animKey, (int)PlayerSkillState.None);
        }
        yield break;
    }

   
    #endregion

    #region LeapAttack
    public IEnumerator LeapAttack()
    {
        _animController.SetInterger(AnimKey.animKey, (int)PlayerSkillState.LeapAttack);
        
        yield break;
    }
    #endregion

    #region dash
    public IEnumerator Dash()
    {
        _animController.SetInterger(AnimKey.animKey, (int)PlayerSkillState.Dash);
        yield break;
    }
    #endregion

    #region spin
    public IEnumerator Spin()
    {
        _animController.SetInterger(AnimKey.animKey, (int)PlayerSkillState.Spin);
        yield break;
    }
    #endregion




}
