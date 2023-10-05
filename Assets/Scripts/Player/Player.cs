using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using Mirror;




public class Player : NetworkBehaviour
{
    [Header("Speeds")]
    public float speed = 0.5f;

    //_moveState ��������� ������. ���� ������� 2 ���������:
    //Idle � Walk - ��������� ����� � ������
    private MoveState _moveState = MoveState.Idle;
    // _directionState - ����������� ������� ������. ���� ������� 4 ��������� �����������
    //Right,Left,Up,Down
    private DirectionState _directionState = DirectionState.Right;
    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private Animator _animatorController;
    private Vector2 moveVector;
    private float _walkTime = 0, _walkCooldown = 0.1f;
    private Camera mainCam;

    //����� ��� �������� �����
    public void MoveLeft()
    {
        //����� �� ������, ���� ����� �� ���������
        //������ �������� ������� ����������� ���������� ������ �����������
        if (!isLocalPlayer) return;
        _moveState = MoveState.Walk;

        if (_directionState == DirectionState.Right)
        {
            moveVector.x = Input.GetAxis("Horizontal");
            _directionState = DirectionState.Left;
        }
        _walkTime = _walkCooldown;
        //������ �������� ������ �����
        _animatorController.Play("WalkLeft");

    }


    //����� ��� �������� ������
    public void MoveRight()
    {
        //����� �� ������, ���� ����� �� ���������
        //������ �������� ������� ����������� ���������� ������ �����������
        if (!isLocalPlayer) return;
        _moveState = MoveState.Walk;
        if (_directionState == DirectionState.Left)
        {
            moveVector.x = Input.GetAxis("Horizontal");
            _directionState = DirectionState.Right;
        }
        _walkTime = _walkCooldown;
        //������ �������� ������ ������
        _animatorController.Play("WalkRight");
    }

    //����� ��� �������� �����
    public void MoveUp()
    {
        //����� �� ������, ���� ����� �� ���������
        //������ �������� ������� ����������� ���������� ������ �����������
        if (!isLocalPlayer) return;
        _moveState = MoveState.Walk;
        if ((_directionState == DirectionState.Down))
        {
            moveVector.y = Input.GetAxis("Vertical");
            _directionState = DirectionState.Up;
        }
        //�������� �� ����� �������������, ���� �����
        //������������ � ��������� ����� ���� � ����� ��� � ������ �������
        if (moveVector.x == 0)
        {
            _walkTime = _walkCooldown;
            _animatorController.Play("WalkUp");
        }
    }

    //����� ��� �������� ����
    public void MoveDown()
    {
        //����� �� ������, ���� ����� �� ���������
        //������ �������� ������� ����������� ���������� ������ �����������
        if (!isLocalPlayer) return;
        _moveState = MoveState.Walk;
        if (_directionState == DirectionState.Up)
        {
            moveVector.y = Input.GetAxis("Vertical");
            _directionState = DirectionState.Down;
        }
        //�������� �� ����� �������������, ���� �����
        //������������ � ��������� ����� ���� � ����� ��� � ������ �������
        if (moveVector.x == 0)
        {
            _walkTime = _walkCooldown;
            _animatorController.Play("WalkDown");
        }
    }


    //����� ��������� �� Idle � ������ �������� �����
    private void Idle()
    {
        _moveState = MoveState.Idle;
        _animatorController.Play("Idle");
    }

    //������������� ������
    private void Awake()
    {
        mainCam = Camera.main;
    }

    

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animatorController = GetComponent<Animator>();
        _directionState = moveVector.x > 0 ? DirectionState.Right : DirectionState.Left;
    }


    private void Update()
    {
        //����� �� ������, ���� ����� �� ���������
        //������ �������� ������� ����������� ���������� ������ �����������
        if (!isLocalPlayer) return;
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");
        CameraMovement();
    }

    //����� ��� ������� �������� ������ �� �������
    private void CameraMovement()
    {
        mainCam.transform.localPosition = new Vector3(_transform.position.x, _transform.position.y, -1f);
        _transform.position = Vector2.MoveTowards(_transform.position, mainCam.transform.localPosition, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //����� ������� ���������
        _rigidbody.MovePosition(_rigidbody.position + moveVector * speed * Time.fixedDeltaTime);
        if (_moveState == MoveState.Walk)
        {
            _walkTime -= Time.deltaTime;
            if (_walkTime <= 0)
            {
                Idle();
            }
        }

    }

    //����������� ������� ������
    enum DirectionState
    {
        Right,
        Left,
        Up,
        Down
    }

    //��������� ���������
    enum MoveState
    {
        Idle,
        Walk
    }
}