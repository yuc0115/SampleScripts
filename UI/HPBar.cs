using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public ActorBase _target = null;

    private Image _imgHP = null;
    

    public void SetTarget(ActorBase actor)
    {
        _target = actor;
        _imgHP = transform.Find("HP").GetComponent<Image>();
        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetValue(float value)
    {
        _imgHP.fillAmount = value;
    }

    public void Update()
    {
        Vector3 v = Camera.main.WorldToScreenPoint(_target.transform.position + Vector3.up * _target._height);
        transform.position = v;
    }
}
