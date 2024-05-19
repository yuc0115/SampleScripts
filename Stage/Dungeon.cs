using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dungeon : StageBase
{
    
    public override void Init(CameraController camController, EnemyManager _enemyManager)
    {
        base.Init(camController, _enemyManager);
        InitTrigger();
    }


    /// <summary>
    /// 던전에 있는 트리거 찾아서 이벤트 등록.
    /// </summary>
    private void InitTrigger()
    {
        GameObject go = GameObject.Find("Trigger");
        if (go == null)
            return;

        Transform tItem = null;
        DungeonTrigger trigger = null;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            tItem = go.transform.GetChild(i);
            trigger = tItem.GetComponent<DungeonTrigger>();
            if (trigger == null)
            {
                trigger = tItem.gameObject.AddComponent<DungeonTrigger>();
            }

            trigger.triggerEnter = (lv) =>
            {
                OnTriggerEnter(lv);
            };
        }
    }

    /// <summary>
    /// 던전에 설치해둔 트리거 동작.
    /// </summary>
    /// <param name="lv"></param>
    private void OnTriggerEnter(int lv)
    {
        if (_currentLv == lv)
            return;

        _currentLv = lv;

        // 카메라 임시 타겟 설정.

        GameObject goFind = null;
        string objname = string.Format("background/Trigger/{0}/LookTarget", lv);
        goFind = GameObject.Find(objname);
        if (goFind != null)
        {
            _cameraController.SetTarget(goFind.transform);
        }

        Vector3 angle = Vector3.zero;
        switch (lv)
        {
            case 1:
                angle = Vector3.right;
                break;

            case 2:
                angle = Vector3.forward;
                break;

            case 3:
                angle = Vector3.forward;
                break;

            default:
                Debug.LogErrorFormat("lv 처리가 안되어있다 :{0}", lv);
                return;
        }
        
        _cameraController.ChangeAngle(angle);
        _enemyManager.CreateEmeny(lv);

        Global.Inst.player.SetEnable(false);
    }

    public override void OnAppearEnd()
    {
        base.OnAppearEnd();
        Global.Inst.player.SetEnable(true);
        _cameraController.SetTarget(Global.Inst.player.transform);
    }
}

public class DungeonTrigger : MonoBehaviour
{

    public delegate void onTriggerEnter(int idx);
    public onTriggerEnter triggerEnter;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == false)
        {
            return;
        }

        string idx = gameObject.name;
        int lv = 0;
        if (int.TryParse(idx, out lv) == false)
        {
            Debug.LogErrorFormat("trigger Name Error!! {0}", idx);
            return;
        }

        if (triggerEnter != null)
        {
            triggerEnter(lv);
        }
    }
}
