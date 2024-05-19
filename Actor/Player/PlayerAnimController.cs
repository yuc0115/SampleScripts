using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    public bool _isCanClick = true;
    private int _noOfClicks = 0;
    private Animator _animator;

    // Start is called before the first frame update
    public void Init()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    public void SetNormalState()
    {
        _isCanClick = true;
        _noOfClicks = 0;
        _animator.SetInteger(AnimKey.animKey, _noOfClicks);
    }

    /// <summary>
    /// 트리거 설정..
    /// </summary>
    /// <param name="value"></param>
    public void SetTrigger(string value)
    {
        _animator.SetTrigger(value);
    }

    /// <summary>
    /// interger 설정.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetInterger(string key, int value)
    {
        _animator.SetInteger(key, value);
    }

    public int GetInteger(string key)
    {
        return _animator.GetInteger(key);
    }
   
    public bool ComboStart()
    {
        bool isAtk = false;

        if (_isCanClick == true)
        {
            _noOfClicks++;
            isAtk = true;
        }
        if (_noOfClicks == 1)
        {
            _animator.SetInteger(AnimKey.animKey, _noOfClicks);
        }

        return isAtk;
    }

    private void ComboCheck()
    {
        _isCanClick = false;
        var stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.IsName(AnimKey.atk01) && _noOfClicks == 1)
        {
            Debug.Log("step_01");
            _noOfClicks = 0;
            _animator.SetInteger(AnimKey.animKey, _noOfClicks);
            _isCanClick = true;
        }
        else if (stateinfo.IsName(AnimKey.atk01) && _noOfClicks >= 2)
        {
            Debug.Log("step_02");
            _animator.SetInteger(AnimKey.animKey, _noOfClicks);
            _isCanClick = true;
        }
        else if (stateinfo.IsName(AnimKey.atk02) && _noOfClicks == 2)
        {
            Debug.Log("step_03");
            _noOfClicks = 0;
            _animator.SetInteger(AnimKey.animKey, _noOfClicks);
            _isCanClick = true;
        }
        else if (stateinfo.IsName(AnimKey.atk02) && _noOfClicks >= 3)
        {
            Debug.Log("step_04");
            _animator.SetInteger(AnimKey.animKey, _noOfClicks);
            _isCanClick = true;
        }
        else if (stateinfo.IsName(AnimKey.atk03))
        {
            Debug.Log("step_05");
            _noOfClicks = 0;
            _animator.SetInteger(AnimKey.animKey, _noOfClicks);
            _isCanClick = true;
        }
    }
}
