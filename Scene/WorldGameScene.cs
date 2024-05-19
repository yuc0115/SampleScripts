using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGameScene : MonoBehaviour
{
    private Player _player;

    private StageBase _currentStage = null;

    private CameraController _cameraController = null;
    private EnemyManager _enemyManager = null;
    void Start()
    {
        Init();
        InitStage();
    }

    private void InitStage()
    {
        Transform t = Camera.main.transform;
        _cameraController = t.GetComponent<CameraController>();
        if (_cameraController == null)
        {
            _cameraController = t.gameObject.AddComponent<CameraController>();
        }
        Global.Inst.camController = _cameraController;
        
        _currentStage = new Dungeon();
        _currentStage.Init(_cameraController, _enemyManager);
    }


    private void Init()
    {
        // 플레이어 생성.
        GameObject go = GameObject.Find("Player");
        if (go == null)
        {
            go = ResManager.Inst.LoadFile("Actor", "Player");
            go = GameObject.Instantiate(go);
        }
        _player = go.GetComponent<Player>();

        // 적 매니저 생성.
        go = new GameObject("EnemyManager");
        _enemyManager = go.AddComponent<EnemyManager>();
    }
}
