using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public Camera MainCamera;

    public Transform CameraPosition;

    [Header("Control Settings")]
    public float MouseSensitivity = 100.0f;
    public float PlayerSpeed = 5.0f;
    public float RunningSpeed = 7.0f;
    public float JumpSpeed = 5.0f;

    float m_VerticalSpeed = 0.0f;

    float m_VerticalAngle, m_HorizontalAngle;

    public float Speed { get; private set; } = 0.0f;

    public bool Grounded => m_Grounded;

    CharacterController m_CharacterController;

    bool m_Grounded;
    float m_GroundedTimer;
    float m_SpeedAtJump = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        m_Grounded = true;

        MainCamera.transform.SetParent(CameraPosition, false);
        MainCamera.transform.localPosition = Vector3.zero;
        MainCamera.transform.localRotation = Quaternion.identity;
        m_CharacterController = GetComponent<CharacterController>();

        m_VerticalAngle = 0.0f;
        m_HorizontalAngle = transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        bool wasGrounded = m_Grounded;
        bool loosedGrounding = false;

        //ChracterController에 있는 isGrounded를 쓰면 불안정해서 땅에 발을 딛고 있을때의 판단 변수를 하나 더 추가해서 사용
        if(!m_CharacterController.isGrounded)
        {
            if(m_Grounded)
            {
                m_GroundedTimer += Time.deltaTime;
                if(m_GroundedTimer >= 0.5f)
                {
                    loosedGrounding = true;
                    m_Grounded = false;
                }
            }
        }
        else
        {
            m_GroundedTimer = 0.0f;
            m_Grounded = true;
        }

        Speed = 0;
        Vector3 move = Vector3.zero;
        if(m_Grounded && Input.GetButtonDown("Jump"))
        {
            m_VerticalSpeed = JumpSpeed;
            m_Grounded = false;
            loosedGrounding = true;
        }

        //TODO: 나중에 달리기나 걷기 기능 추가했을때 변경
        float actualSpeed = PlayerSpeed;    //일단 걷기 속도 대입

        if(loosedGrounding)
        {
            m_SpeedAtJump = actualSpeed;
        }

        //WASD 이동
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (move.sqrMagnitude > 1.0f)   //sqrMagnitude: 벡터의 길이의 제곱한 값을 반환
            move.Normalize();

        float usedSpeed = m_Grounded ? actualSpeed : m_SpeedAtJump;

        move = move * usedSpeed * Time.deltaTime;

        move = transform.TransformDirection(move);
        m_CharacterController.Move(move);

        //캐릭터 회전
        float turnPlayer = Input.GetAxis("Mouse X") * MouseSensitivity;
        m_HorizontalAngle = m_HorizontalAngle + turnPlayer;

        if (m_HorizontalAngle > 360) m_HorizontalAngle -= 360.0f;
        if (m_HorizontalAngle < 0) m_HorizontalAngle += 360.0f;

        Vector3 currentAngles = transform.localEulerAngles;
        currentAngles.y = m_HorizontalAngle;
        transform.localEulerAngles = currentAngles;

        //카메라 위 아래
        var turnCam = -Input.GetAxis("Mouse Y");
        turnCam = turnCam * MouseSensitivity;
        m_VerticalAngle = Mathf.Clamp(turnCam + m_VerticalAngle, -89.0f, 70.0f);
        currentAngles = CameraPosition.transform.localEulerAngles;
        currentAngles.x = m_VerticalAngle;
        CameraPosition.transform.localEulerAngles = currentAngles;

        Speed = move.magnitude / (PlayerSpeed * Time.deltaTime);

        //떨어질때 / 중력
        m_VerticalSpeed = m_VerticalSpeed - 10.0f * Time.deltaTime;
        if (m_VerticalSpeed < -10.0f)
            m_VerticalSpeed = -10.0f;   //max fall speed
        var verticalMove = new Vector3(0, m_VerticalSpeed * Time.deltaTime, 0);
        var flag = m_CharacterController.Move(verticalMove);
        if ((flag & CollisionFlags.Below) != 0)
            m_VerticalSpeed = 0;

        //착지시 사운드 재생
        if(!wasGrounded && m_Grounded)
        {
            
        }
    }
}
