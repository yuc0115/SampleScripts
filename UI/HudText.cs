using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudText : MonoBehaviour
{
    public int _idx = 0;
    private ActorBase _target = null;
    private Text _damage = null;

    private Coroutine routine = null;
    private void Awake()
    {
        _damage = transform.Find("Text").GetComponent<Text>();
    }

    private Vector3 randomPos = Vector3.zero;
    public void SetTarget(ActorBase actor, int damageValue)
    {
        SetActive(true);

        _target = actor;
        _idx = actor._idx;

        Vector3 v = _target.transform.position + Vector3.up * 2;
        randomPos = Vector3.right * Random.Range(0.0f, 1.0f) + Vector3.up * Random.Range(0.0f, 1.0f) + Vector3.forward * Random.Range(0.0f, 1.0f);
        v += randomPos;
        v -= Vector3.one * 0.5f;

        transform.position = Camera.main.WorldToScreenPoint(v);

        _damage.text = damageValue.ToString("N0");
        //_damage.color = Color.red;
        _damage.canvasRenderer.SetColor(Color.white);
        _damage.rectTransform.localScale = Vector3.one;
        
        StartCoroutine(HudRoutine());
        routine = StartCoroutine(Root());
    }


    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    IEnumerator Root()
    {
        while (true)
        {
            Vector3 v = _target.transform.position + Vector3.up * 2;
            v += randomPos;
            v -= Vector3.one * 0.5f;

            transform.position = Camera.main.WorldToScreenPoint(v);
            yield return null;
        }

    }

    IEnumerator HudRoutine()
    {
        float duration = 2.0f;
        float time = 0.0f;

        _damage.CrossFadeAlpha(0, duration, false);

        float scaleTime = 0.15f;
        while (true)
        {
            time += Time.deltaTime;
            _damage.rectTransform.localScale = Vector3.one + Vector3.one * 1.0f * (time / scaleTime);
            

            if (scaleTime <= time)
            {
                break;
            }
            yield return null;
        }

        while (true)
        {
            time -= Time.deltaTime;
            _damage.rectTransform.localScale = Vector3.one + Vector3.one * 1.0f * (time / scaleTime);

            if (0 >= time)
            {
                break;
            }
            yield return null;
        }

        yield return DeActiveRoutine();
        
        yield break;
    }

    IEnumerator DeActiveRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        StopCoroutine(routine);
        HudManager.Inst.RemoveHudText();
    }
}
