using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBase
{
    protected CameraController _cameraController;
    protected EnemyManager _enemyManager;

    protected int _currentLv = 0;
    public int GetCurrentLv { get { return _currentLv; } }

    public virtual void Init(CameraController camController, EnemyManager enemyManager)
    {
        _cameraController = camController;
        _enemyManager = enemyManager;
        _enemyManager.SetAppearEnd(OnAppearEnd);
    }

    public virtual void StartAppear()
    {

    }

    public virtual void EndAppear()
    {

    }

    public virtual void OnAppearEnd()
    {

    }
}
