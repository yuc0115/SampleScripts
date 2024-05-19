using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager
{
    private static IndicatorManager _inst = new IndicatorManager();
    public static IndicatorManager Inst { get { return _inst; } }

    private Dictionary<IndicatorAngle, GameObject> _dicIndicator = new Dictionary<IndicatorAngle, GameObject>();
    private Dictionary<IndicatorAngle, GameObject> _dicIndicatorTemp = new Dictionary<IndicatorAngle, GameObject>();

    private Dictionary<IndicatorAngle, GameObject> _dicIndicatorMask = new Dictionary<IndicatorAngle, GameObject>();
    private Dictionary<IndicatorAngle, GameObject> _dicIndicatorMaskTemp = new Dictionary<IndicatorAngle, GameObject>();

    public IndicatorBase CreateIndicator(IndicatorAngle angle, bool isMask)
    {
        var dicTemp = isMask ? _dicIndicatorMaskTemp : _dicIndicatorTemp;
        var dic = isMask ? _dicIndicatorMask : _dicIndicator;

        GameObject go = null;
        if (dicTemp.TryGetValue(angle, out go))
        {
            dicTemp.Remove(angle);
        }
        else
        {
            string fileName = isMask ? "Indicator_{0}_Mask" : "Indicator_{0}";
            go = ResManager.Inst.LoadFile("Indicator", string.Format(fileName, (int)angle));
            go = GameObject.Instantiate(go);
        }

        dic.Add(angle, go);
        go.SetActive(true);

        IndicatorBase indicator = null;

        if (isMask)
        {
            indicator = go.GetComponent<IndicatorMask>();
        }
        else
        {
            indicator = go.GetComponent<Indicator>();
        }

        if (indicator == null)
        {
            if (isMask)
            {
                indicator = go.AddComponent<IndicatorMask>();
            }
            else
            {
                indicator = go.AddComponent<Indicator>();
            }
        }
       
        return indicator;
    }

    /// <summary>
    /// 마스크와 가이드 둘다 제거
    /// </summary>
    /// <param name="angle"></param>
    public void RemoveIndicator(IndicatorAngle angle)
    {
        GameObject go = null;
        if (_dicIndicatorMask.TryGetValue(angle, out go) == false)
        {
            Debug.LogErrorFormat("키값이 없습니다. key : {0}", angle);
            return;
        }

        go.SetActive(false);
        _dicIndicatorMaskTemp.Add(angle, go);
        if (_dicIndicatorMask.Remove(angle) == false)
        {
            Debug.LogErrorFormat(" 삭제 실패 {0}", angle);
        }

        if (_dicIndicator.TryGetValue(angle, out go) == false)
        {
            Debug.LogErrorFormat("키값이 없습니다. key : {0}", angle);
            return;
        }

        go.SetActive(false);
        _dicIndicatorTemp.Add(angle, go);
        if (_dicIndicator.Remove(angle) == false)
        {
            Debug.LogErrorFormat(" 삭제 실패 {0}", angle);
        }
    }
}

public class IndicatorBase :MonoBehaviour
{
    public virtual void Init(ActorBase target, float duration, int range, IndicatorAngle angle, bool autoRemove = true)
    {

    }
}

public class Indicator : IndicatorBase
{
    private Projector _projector;

    public override void Init(ActorBase target, float duration, int range, IndicatorAngle angle, bool autoRemove = true)
    {
        base.Init(target, duration, range, angle, autoRemove);

        IndicatorBase indicator = IndicatorManager.Inst.CreateIndicator(angle, true);
        indicator.Init(target, duration, range, angle);

        transform.position = target.transform.position;
        transform.rotation = target.transform.rotation;

        if (_projector == null)
        {
            _projector = gameObject.GetComponentInChildren<Projector>();
        }
        _projector.orthographicSize = range;
        _projector.material.SetColor("_Color", Color.red);

        if (autoRemove)
        {
            StartCoroutine(RemoveIndicator(angle, duration));
        }
    }

    /// <summary>
    /// 인디게이터 삭제.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator RemoveIndicator(IndicatorAngle angle, float duration)
    {
        yield return new WaitForSeconds(duration + 0.3f);
        IndicatorManager.Inst.RemoveIndicator(angle);

        yield break;
    }
}

public class IndicatorMask : IndicatorBase
{
    private Projector _projector = null;

    public override void Init(ActorBase target, float duration, int range, IndicatorAngle angle, bool autoRemove = true)
    {
        base.Init(target, duration, range, angle, autoRemove);
        transform.position = target.transform.position;
        transform.rotation = target.transform.rotation;

        if (_projector == null)
        {
            _projector = gameObject.GetComponentInChildren<Projector>();
        }
        _projector.orthographicSize = 0;
        _projector.material.SetColor("_Color", Color.red);
        StartCoroutine(Sizeroutine(duration, range));
    }

    IEnumerator Sizeroutine(float duration, int projectorSize)
    {
        _projector.orthographicSize = 0;
        float time = 0;

        while (true)
        {
            time += Time.deltaTime / duration;
            _projector.orthographicSize = Mathf.Lerp(0, projectorSize, time);

            if (time >= duration)
            {
                break;
            }
            yield return null;
        }
    }
}

