using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _inst = null;
    public static EnemyManager Inst
    {
        get
        {
            if (_inst == null)
            {
                GameObject go = GameObject.Find("EnemyManager");
                _inst = go.GetComponent<EnemyManager>();
            }
            return _inst;
        }
    }
    private Transform _parent = null;
    private GameObject _goAppear = null;

    private DungeonInfo _dicEnemyData = new DungeonInfo();

    private System.Action _onAppearEnd = null;

    private void Start()
    {
        Init();
    }

    public void SetAppearEnd(System.Action action)
    {
        _onAppearEnd = action;
    }

    private void Init()
    {
        GameObject go = GameObject.Find("Enemy");
        if (go == null)
        {
            go = new GameObject("Enemy");
        }
        _parent = go.transform;

        var ta = Resources.Load("dungeon") as TextAsset;
        _dicEnemyData = Newtonsoft.Json.JsonConvert.DeserializeObject<DungeonInfo>(ta.text);
        _goAppear = ResManager.Inst.LoadFile("Effect", "CFX3_Vortex_Ground_Appear");
        
        // qodemfhd
        //CreateEmeny();
    }

    public void RemoveEnemy(int idx)
    {
        ActorBase actor = null;
        if (Global.Inst.dicEnemy.TryGetValue(idx, out actor) == false)
        {
            Debug.LogError("없는 아이디를 삭제 하려고합니다");
            return;
        }

        actor.gameObject.SetActive(false);
        Global.Inst.dicEnemy.Remove(idx);
        HPBarManager.Inst.RemoveBar(idx);
    }


    public void CreateEmeny(int lv)
    {
        StartCoroutine(CreateEnemyRoutine(lv));
    }

    /// <summary>
    /// 몬스터 생성 및 생성 효과.
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    IEnumerator CreateEnemyRoutine(int lv)
    {
        yield return new WaitForSeconds(1.0f);

        List<EnemyInfo> info = null;
        if (_dicEnemyData.dicInfos.TryGetValue(lv, out info) == false)
        {
            Debug.LogErrorFormat("createEnemyRoutine lv error!! lv : {0}", lv);
            yield break;
        }

        int appearEndCnt = 0;
        for (int i = 0; i < info.Count; i++)
        {
            GameObject goSource = ResManager.Inst.LoadFile("Enemy", info[i].resName);
            GameObject go = GameObject.Instantiate(goSource);
            go.transform.parent = _parent;
            go.transform.position = info[i].pos;
            go.transform.eulerAngles = info[i].angle;

            GameObject appear = GameObject.Instantiate(_goAppear);
            appear.transform.position = info[i].pos + Vector3.up * 0.2f;

            ActorBase actor = go.GetComponent<ActorBase>();
            if (actor == null)
            {
                actor = go.AddComponent<Enemy>();
            }
            // 초기화를 하고 등장이 끝났는지 체크한다.
            actor.Init(i, true, () => 
            {
                appearEndCnt++;
                if (appearEndCnt >= info.Count - 1)
                {
                    StartCoroutine(AppearEnd());
                }
            });
            Global.Inst.dicEnemy.Add(i, actor);

            yield return new WaitForSeconds(0.2f);
        }

        
        yield break;
    }

    IEnumerator AppearEnd()
    {
        yield return new WaitForSeconds(0.3f);

        if (_onAppearEnd != null)
            _onAppearEnd();

        foreach (var pair in Global.Inst.dicEnemy)
        {
            pair.Value.ChangeState(EnemyState.TargetChase);
        }
    }
}
