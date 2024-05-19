using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBase : MonoBehaviour
{
    protected virtual string setTag { set { gameObject.tag = value; } }

    protected ActorTagKind _getActorTagKind = ActorTagKind.end;
    public virtual ActorTagKind getActorTagKind
    {
        get
        {
            if (_getActorTagKind == ActorTagKind.end)
            {
                if (System.Enum.TryParse(gameObject.tag, out _getActorTagKind) == false)
                {
                    Debug.LogError(gameObject.tag);
                }
            }
            return _getActorTagKind;
        }
    }

    public int _idx = 0;
    public int _maxHP = 1000;
    public int _currentHp = 1000;
    public float _moveSpeed = 3;
    public int _damage = 5;
    public float _atkRange = 2.0f;
    public float _atkDelay = 3.0f;
    public int _atkAngle = 90;
    public float _height = 3.0f;

    public bool IsDie { get { return _currentHp <= 0; } }

    protected List<SkinnedMeshRenderer> _listSmr = new List<SkinnedMeshRenderer>();

    private GameObject _resHitEffect = null;
    public GameObject resHitEffect
    {
        get
        {
            if (_resHitEffect == null)
            {
                _resHitEffect = ResManager.Inst.LoadFile("Effect", "Hit");
            }
            return _resHitEffect;
        }
    }    


    public virtual void OnDamage(int damage, SpecialHitEffect hitEffect)
    {
        _currentHp -= damage;
        if (_currentHp <= 0)
        {
            _currentHp = 0;
            OnDie();
        }
    }

    public virtual void ChangeState(EnemyState state)
    {

    }

    public virtual void OnDie()
    {

    }

    public virtual void Init(int idx, bool isAppear, System.Action onAppearEnd = null)
    {
        _idx = idx;
        for (int i = 0; i < transform.childCount; i++)
        {
            var smr = transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            if (smr == null)
            {
                continue;
            }
            _listSmr.Add(smr);
        }
        
    }
}
