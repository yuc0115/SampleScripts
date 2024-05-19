using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillInfo
{
    /// <summary>
    /// 스킬이 준비 되었는지.
    /// </summary>
    public bool isReady = true;

    /// <summary>
    /// 스킬 쿨타임
    /// </summary>
    public float coolTime = 0;
    /// <summary>
    /// 스킬 시간 체크용..
    /// </summary>
    public float elapsedTime = 0;
    public Image coolTimeImg = null;
    public Text coolTimeText = null;

    public void SetFire()
    {
        isReady = false;
        coolTimeText.enabled = true;
        elapsedTime = 0;
    }
}


public class PlayerController : PlayerStateBase
{
    public VariableJoystick _joystick;
    public float _moveSpeed = 6.0f;

    private CharacterController _controller = null;
    private Animator _animator = null;

    /// <summary>
    /// 이동이 가능한 상태인지를 확인한다.
    /// </summary>
    private bool _isMove = true;
    public bool SetMove { set { _isMove = value; } }

    private Dictionary<PlayerSkillState, PlayerSkillInfo> _skillCoolTimeInfo = new Dictionary<PlayerSkillState, PlayerSkillInfo>();

    public override void Init()
    {
        base.Init();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        GameObject go = GameObject.Find("Canvas/Controller/Joystick");
        _joystick = go.GetComponent<VariableJoystick>();

        Button btn = null;
        go = GameObject.Find("Canvas/Controller/BtnAttack");
        btn = go.GetComponent<Button>();
        btn.onClick.AddListener(() => { OnClickAttack(); });


        go = GameObject.Find("Canvas/Controller/BtnSkill_1");
        btn = go.GetComponent<Button>();
        btn.onClick.AddListener(() => { OnClickSkill(PlayerSkillState.PalmStrike); });

        PlayerSkillInfo info = new PlayerSkillInfo();
        info.coolTime = 5;
        info.coolTimeImg = GameObject.Find("Canvas/Controller/BtnSkill_1/CoolTime").GetComponent<Image>();
        info.coolTimeText = GameObject.Find("Canvas/Controller/BtnSkill_1/CoolTime/Text").GetComponent<Text>();
        info.coolTimeText.enabled = false;
        _skillCoolTimeInfo.Add(PlayerSkillState.PalmStrike ,info);

        

        go = GameObject.Find("Canvas/Controller/BtnSkill_2");
        btn = go.GetComponent<Button>();
        btn.onClick.AddListener(() => { OnClickSkill(PlayerSkillState.BreathOfFire); });

        info = new PlayerSkillInfo();
        info.coolTime = 1;
        info.coolTimeImg = GameObject.Find("Canvas/Controller/BtnSkill_2/CoolTime").GetComponent<Image>();
        info.coolTimeText = GameObject.Find("Canvas/Controller/BtnSkill_2/CoolTime/Text").GetComponent<Text>();
        info.coolTimeText.enabled = false;
        _skillCoolTimeInfo.Add(PlayerSkillState.BreathOfFire, info);


        go = GameObject.Find("Canvas/Controller/BtnSkill_3");
        btn = go.GetComponent<Button>();
        btn.onClick.AddListener(() => { OnClickSkill(PlayerSkillState.Dash); });

        info = new PlayerSkillInfo();
        info.coolTime = 7;
        info.coolTimeImg = GameObject.Find("Canvas/Controller/BtnSkill_3/CoolTime").GetComponent<Image>();
        info.coolTimeText = GameObject.Find("Canvas/Controller/BtnSkill_3/CoolTime/Text").GetComponent<Text>();
        info.coolTimeText.enabled = false;
        _skillCoolTimeInfo.Add(PlayerSkillState.Dash, info);



        go = GameObject.Find("Canvas/Controller/BtnSkill_4");
        btn = go.GetComponent<Button>();
        btn.onClick.AddListener(() => { OnClickSkill(PlayerSkillState.GroundOfFire); });

        info = new PlayerSkillInfo();
        info.coolTime = 1;
        info.coolTimeImg = GameObject.Find("Canvas/Controller/BtnSkill_4/CoolTime").GetComponent<Image>();
        info.coolTimeText = GameObject.Find("Canvas/Controller/BtnSkill_4/CoolTime/Text").GetComponent<Text>();
        info.coolTimeText.enabled = false;
        _skillCoolTimeInfo.Add(PlayerSkillState.GroundOfFire, info);
    }

    public void OnClickAttack()
    {
        Global.Inst.player.Attack();
    }

    /// <summary>
    /// 스킬 클릭. 스킬을 쓸 수 있는지를 검사하고 쓸수 있는 상태라면 상태 변경.
    /// </summary>
    /// <param name="skill"></param>
    public void OnClickSkill(PlayerSkillState skill)
    {
        // 해당 스킬을 쓸 수 있는지를 검사한다.
        PlayerSkillInfo coolTimeInfo = null;
        if (_skillCoolTimeInfo.TryGetValue(skill, out coolTimeInfo) == false)
        {
            Debug.LogErrorFormat("스킬 처리 안됨 : {0}", skill);
            return;
        }

        if (coolTimeInfo.isReady == false)
        {
            Debug.Log("쿨타임 중인 스킬이다");
            return;
        }

        coolTimeInfo.SetFire();

        Global.Inst.player.ChangeState(PlayerState.Skill, skill);
    }

    public override void Update()
    {
        UpdateSkillCooltime();
        if (_isMove == false)
            return;

        base.Update();
        // 조이패드
        //Vector3 v = Vector3.right * _joystick.Direction.x + Vector3.forward * _joystick.Direction.y;

        Vector3 right = Camera.main.transform.right;
        Quaternion vRot = Quaternion.Euler(0f, -90f, 0f);  // 회전각
        Vector3 forward = vRot * right;

        Vector3 v = right * _joystick.Direction.x + forward * _joystick.Direction.y;

        // 키보드 움직임
        if (v.magnitude == 0)
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            v = right * horizontal + forward * vertical;
        }
        
        if (_controller.isGrounded)
        {
            _verticalVelocity = 0;
            if (Input.GetKeyDown(KeyCode.G))
            {
                _verticalVelocity = _jumpForce;
            }
        }
        else
        {
            _verticalVelocity -= _gravity * Time.deltaTime;
        }

        Vector3 vdir = Vector3.up * _verticalVelocity;
        _controller.Move((v + vdir) * Time.deltaTime * _moveSpeed);

        transform.LookAt(transform.position + v, Vector3.up);
        _animator.SetFloat("Blend", Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.z)));
    }

    /// <summary>
    /// 쿨타임중인 스킬 처리.
    /// </summary>
    private void UpdateSkillCooltime()
    {
        PlayerSkillInfo info = null;
        foreach(var pair in _skillCoolTimeInfo)
        {
            info = pair.Value;

            if (info.isReady == true)
                continue;

            info.elapsedTime += Time.deltaTime;
            if (info.elapsedTime >= info.coolTime)
            {
                info.isReady = true;
                info.coolTimeText.enabled = false;
            }

            info.coolTimeImg.fillAmount = 1 - (info.elapsedTime / info.coolTime);

            float time = info.coolTime - info.elapsedTime;
            info.coolTimeText.text = time > 1 ? time.ToString("N0") : string.Format("{0:0.0}", time);
        }
    }

    private float _verticalVelocity;
    private float _gravity = 14.0f;
    private float _jumpForce = 4;

    public IEnumerator Jump()
    {
        Vector3 v = Vector3.zero;

        v = transform.forward * 7.0f;

        _verticalVelocity = _jumpForce;
        while (true)
        {
            _verticalVelocity -= _gravity * Time.deltaTime;

            Vector3 vdir = Vector3.up * _verticalVelocity;
            _controller.Move((v + vdir) * Time.deltaTime );


            if (_controller.isGrounded)
            {
                break;
            }
            yield return null;
        }
    }

    public float dashSpeed = 15.0f;
    public float _dist = 15.0f;
    public IEnumerator Dash(System.Action onAttack)
    {
        Vector3 v = Vector3.zero;
        float dist = _dist;
        v = transform.forward;
        _verticalVelocity = 0;

        float speed = dashSpeed;


        float time = 0.0f;
        float atkDelay = 0.1f;
        while (true)
        {
            float deltaTime = Time.deltaTime;

            _verticalVelocity -= _gravity * deltaTime;
            Vector3 vdir = Vector3.up * _verticalVelocity;


            dist -= (v * deltaTime).magnitude * speed;
            if (dist <= 0)
                break;
            _controller.Move((v + vdir) * deltaTime * speed);


            time += Time.deltaTime;

            if (time >= atkDelay)
            {
                time = 0;

                if (onAttack != null)
                    onAttack();
            }
            yield return null;
        }
    }
}
