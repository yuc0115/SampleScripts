using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager
{
    private static HudManager _inst = null;
    public static HudManager Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = new HudManager();
                _inst.Init();
            }

            return _inst;
        }
    }

    private Transform _parent = null;
    private GameObject _resGo = null;
    public void Init()
    {
        _resGo = ResManager.Inst.LoadFile("UI", "HudText");

        GameObject go = GameObject.Find("Canvas/HudPanel");
        _parent = go.transform;
    }

    private List<HudText> _list = new List<HudText>();
    private List<HudText> _listTemp = new List<HudText>();

    public HudText CreateHudText()
    {
        HudText hudText = null;
        GameObject go = null;
        if (_listTemp.Count == 0)
        {
            go = GameObject.Instantiate(_resGo);
            hudText = go.AddComponent<HudText>();
            go.transform.parent = _parent;
        }
        else
        {
            hudText = _listTemp[_listTemp.Count - 1];
            _listTemp.RemoveAt(_listTemp.Count - 1);
        }

        _list.Add(hudText);
        return hudText;
    }
    
    public void RemoveHudText()
    {
        if (_list.Count == 0)
            return;

        HudText text = _list[0];
        _listTemp.Add(text);

        text.SetActive(false);
        _list.RemoveAt(0);
    }

    public void Test()
    {
        Debug.LogErrorFormat("listCnt : {0}, listTempCnt : {1}", _list.Count, _listTemp.Count);
    }
}
