using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MoveType
{
    Normal,         // 직선.
    Arc,            // 기본 포물선.
    Whirlwind,      // 돌면서 나감.
    End,
}


public class Projectile : MonoBehaviour
{
    private Vector3 _vDir;
    private float _speed;
    private ActorBase _source;
    private ActorBase _target;
    private ActorTagKind _targetTag;
    private float _range;
    private string _name;
    private MoveType _moveType = MoveType.Normal;
    private bool _isRemove = false;
    private Action<Vector3> _onArrival = null;

    private Dictionary<int, int> _dicDamagedActor = new Dictionary<int, int>();


    /// <summary>
    /// 기본 포물선 높이 제한. 3~5정도 
    /// </summary>
    public float _arcPosY = 8.0f;

    private float _moveSpeed = 1.0f;

    /// <summary>
    /// 이동 경로 저장.
    /// </summary>
    private List<Vector3> _listPos = new List<Vector3>();

    private void Init(MoveType moveType, string name, ActorBase source, ActorBase target, Vector3 startPos)
    {
        _moveType = moveType;
        _name = name;
        _source = source;
        _target = target;

        if (target != null)
        {
            _targetTag = target.getActorTagKind;
        }
        else
        {
            _targetTag = ActorTagKind.Enemy;
        }

        // 초기 좌표 셋팅.
        transform.position = startPos + Vector3.up * source._height * 0.5f;

        // 방향.
        _vDir = source.transform.forward;

        // 회전
        Vector3 vLook = transform.position + _vDir;
        Quaternion targetRotation = Quaternion.LookRotation(vLook - transform.position);
        transform.rotation = targetRotation;
    }

    public void InitNormal(string name, ActorBase source, ActorBase target, Vector3 startPos,float speed, float range)
    {
        Init(MoveType.Normal, name, source, target, startPos);

        _speed = speed;
        _range = range;
        
        StartCoroutine(Progress());
    }

    public void InitSpecial(MoveType moveType, string name, ActorBase source, ActorBase target, Vector3 startPos, Action<Vector3> onArrival = null)
    {
        Init(moveType, name, source, target, startPos);

        _target = target;

        _onArrival = onArrival;

        switch (moveType)
        {
            case MoveType.Normal:
                //_listPos = TypeNormal(from, to);
                break;

            case MoveType.Arc:
                _listPos = TypeArc(source, target);
                StartCoroutine(Mover());
                break;

            case MoveType.Whirlwind:
                //_listChildPos = TypeWhirlwind(from, to);
                //_listPos = TypeNormal(from, to);
                break;

            default:
                Debug.LogError(string.Format("_type Error!! {0}", moveType));
                break;
        }
    }

    private const float Speed = 15;     // 기본 이동 속도 15로 맞춤.
    private float GetMoveSpeed(ActorBase from, ActorBase to)
    {
        float speed = 0.0f;
        float dist = Vector3.Distance(from.transform.localPosition, to.transform.localPosition);

        speed = Speed / dist;

        return speed;
    }

    /// <summary>
    /// 이동 관련 코루틴.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Mover()
    {
        Vector3 vPos = Vector3.zero;
        float nowTime = 0.0f;
        while (nowTime < 1.0f)
        {
            nowTime += Time.deltaTime * _moveSpeed;

            if (nowTime >= 1.0f)
            {
                nowTime = 1.0f;
            }

            // 좌표 세팅.
            vPos = GetPosition(_listPos.ToArray(), nowTime);
            transform.localPosition = vPos;
            
            yield return null;
        }

        if (_onArrival != null)
        {
            _onArrival(vPos);
            _onArrival = null;
        }
        StopParticle();
    }

    /// <summary>
    /// 좌표 리턴.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    private Vector3 GetPosition(Vector3[] p, double t)
    {
        if (t == 1)
        {
            return p[p.Length - 1];
        }
        int kn, nn, nkn, n = p.Length - 1;
        double blend, muk, munk;
        Vector3 b = Vector3.zero;
        muk = 1;
        munk = Math.Pow(1 - t, (double)n);
        for (int i = 0; i <= n; i++)
        {
            nn = n;
            kn = i;
            nkn = n - i;
            blend = muk * munk;
            muk *= t;
            munk /= (1 - t);
            while (nn >= 1)
            {
                blend *= nn;
                nn--;
                if (kn > 1)
                {
                    blend /= (double)kn;
                    kn--;
                }
                if (nkn > 1)
                {
                    blend /= (double)nkn;
                    nkn--;
                }
            }
            b.x += (float)(p[i].x * blend);
            b.y += (float)(p[i].y * blend);
            b.z += (float)(p[i].z * blend);
        }
        return (b);
    }



    /// 포물선 타입.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private List<Vector3> TypeArc(ActorBase from, ActorBase to)
    {
        List<Vector3> list = new List<Vector3>();

        Vector3 vCetner = Vector3.zero;
        Vector3 vFrom, vTo;
        float dist = 0;
        vFrom = from.transform.position;
        vTo = to.transform.position;
        dist = Vector3.Distance(vFrom, vTo);

        Vector3 vNormal = vTo - vFrom;
        vNormal = vNormal.normalized;

        vCetner = vFrom + vNormal * (dist / 2.0f);
        vCetner.y += _arcPosY;

        list.Add(from.transform.position);
        list.Add(vCetner);
        list.Add(to.transform.position);
        return list;
    }

    IEnumerator Progress()
    {
        float value = 0;
        while(true)
        {
            value = _speed * Time.deltaTime;
            transform.Translate(_vDir * value, Space.World);

            _range -= value;
            if (_range < 0)
            {
                break;
            }
            yield return null;
        }

        StopParticle();
        yield break;
    }

    private void StopParticle()
    {
        this.GetComponent<ParticleSystem>().Stop(true);

        CFX_Demo_Translate translation = this.gameObject.GetComponent<CFX_Demo_Translate>();
        if (translation != null)
        {
            translation.enabled = false;
        }

        _isRemove = true;

        Invoke("RemoveObj", 1.0f);
    }

    void RemoveObj()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isRemove == true)
            return;
        // 타겟 확인.
        ActorBase actor = other.gameObject.GetComponent<ActorBase>();
        if (actor.getActorTagKind != _targetTag)
        {
            return;
        }

        if (actor.IsDie)
            return;

        if (_dicDamagedActor.ContainsKey(actor._idx) == false)
        {
            int damage = UnityEngine.Random.Range(10, 20);
            actor.OnDamage(damage, SpecialHitEffect.Neckback);
            _dicDamagedActor.Add(actor._idx, actor._idx);
        }
    }
}


