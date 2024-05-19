using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarManager 
{
    private static HPBarManager _inst = null;
    public static HPBarManager Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = new HPBarManager();
                _inst.Init();
            }

            return _inst;
        }
    }

    private Transform _parent = null;
    private GameObject _resGo = null;

    private Dictionary<int, HPBar> _dic = new Dictionary<int, HPBar>();
    private List<HPBar> _listTemp = new List<HPBar>();

    public void Init()
    {
        _resGo = ResManager.Inst.LoadFile("UI", "EnemyPanel");

        GameObject go = GameObject.Find("Canvas/HPPanel");
        _parent = go.transform;
    }

    public HPBar CreateBar(int actorIdx)
    {
        HPBar hpBar = null;
        if (_listTemp.Count != 0)
        {
            hpBar = _listTemp[_listTemp.Count - 1];
            _listTemp.RemoveAt(_listTemp.Count - 1);
        }
        else
        {
            GameObject go = GameObject.Instantiate(_resGo);
            hpBar = go.AddComponent<HPBar>();
            go.transform.SetParent(_parent);
        }

        hpBar.SetActive(true);
        _dic.Add(actorIdx, hpBar);
        return hpBar;
    }

    public void RemoveBar(int actoridx)
    {
        HPBar hpbar = null;
        if (_dic.TryGetValue(actoridx, out hpbar) == false)
        {
            Debug.LogError("key error!");
            return;
        }
        _dic.Remove(actoridx);
        hpbar.SetActive(false);
        _listTemp.Add(hpbar);
    }
}
