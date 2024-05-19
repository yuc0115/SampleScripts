using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform _target = null;
    public float _smooth = 0.3f;
    public float _height = 15.0f;
    public float _dist = -10.0f;    
    public Vector3 _vAngle = Vector3.right;

    private Vector3 velocity = Vector3.zero;

    public void SetTarget(Transform target)
    {
        _target = target;
    }


    /// <summary>
    /// 카메라 앵글을 변환한다 zero값이라면 변환하지 않는다.
    /// </summary>
    /// <param name="vAngle"></param>
    public void ChangeAngle(Vector3 vAngle)
    {
        if (vAngle == Vector3.zero)
            return;
        _vAngle = vAngle;
        StartCoroutine(ChangeAngleRoutine());
    }

    private bool isUpdate = true;
    IEnumerator ChangeAngleRoutine ()
    {
        isUpdate = false;

        float time = 0;
        
        Vector3 endPos = Vector3.zero;
        endPos.x = _target.transform.position.x;
        endPos.z = _target.transform.position.z + _dist;
        endPos.y = _target.transform.position.y + _height;

        endPos = _target.transform.position;
        Vector3 vTemp = _vAngle * _dist + Vector3.up * _height;

        endPos = endPos + vTemp;


        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(_target.transform.position - endPos);

        Vector3 startPos = transform.position;
        while (time < 2)
        {
            time += Time.deltaTime;   
            
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time);
            transform.localPosition = Vector3.Slerp(startPos, endPos, time);

            yield return null;
        }
        
        isUpdate = true;

    }

    void Update()
    {
        if (isUpdate == false)
            return;

        Vector3 v = Vector3.zero;
        v.x = _target.position.x;
        v.z = _target.position.z + _dist;
        v.y = _target.position.y + _height;

        v = _target.transform.position;
        Vector3 vTemp = _vAngle * _dist + Vector3.up * _height;

        v = v + vTemp;

        transform.rotation = Quaternion.LookRotation(_target.transform.position - v);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, v, ref velocity, _smooth) + _shakePos;


        transform.parent.localPosition = _shakePos;
    }



    
    private float _shakeAmount = 0.2f;
    private float _decreaseFactor = 1.0f;
    private float _shakeTime = 0.15f;
    private float _shakeDelay = 0.05f;
    private Vector3 _shakePos;

    public void StartShake()
    {
        StartCoroutine(ShakeRoutine());
    }

    
    IEnumerator ShakeRoutine()
    {
        float shakeAmount = _shakeAmount;
        float shakeTime = _shakeTime;

        while (shakeTime > 0)
        {
            _shakePos = Random.insideUnitSphere * shakeAmount;
            shakeTime -= Time.deltaTime * _decreaseFactor;
            shakeAmount -= Time.deltaTime * _shakeTime;

            if (shakeAmount <= 0)
            {
                shakeAmount = 0;
            }
            if (shakeTime <= 0)
                shakeTime = 0;
            yield return new WaitForSeconds(_shakeDelay);
        }
        _shakePos = Vector3.zero;

        yield break;
    }
}
