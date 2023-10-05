using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using Mirror;




public class Player : NetworkBehaviour
{
    [Header("Speeds")]
    public float speed = 0.5f;

    //_moveState состояние обьета. Ниже описаны 2 состояния:
    //Idle и Walk - состояние покоя и ходьбы
    private MoveState _moveState = MoveState.Idle;
    // _directionState - направление взгляда игрока. Ниже описаны 4 возможных направления
    //Right,Left,Up,Down
    private DirectionState _directionState = DirectionState.Right;
    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private Animator _animatorController;
    private Vector2 moveVector;
    private float _walkTime = 0, _walkCooldown = 0.1f;
    private Camera mainCam;

    //метод для движения влево
    public void MoveLeft()
    {
        //выйти из метода, если игрок не локальный
        //данная проверка убирает возможность управления чужими персонажами
        if (!isLocalPlayer) return;
        _moveState = MoveState.Walk;

        if (_directionState == DirectionState.Right)
        {
            moveVector.x = Input.GetAxis("Horizontal");
            _directionState = DirectionState.Left;
        }
        _walkTime = _walkCooldown;
        //запуск анимации ходьбы влево
        _animatorController.Play("WalkLeft");

    }


    //метод для движения вправо
    public void MoveRight()
    {
        //выйти из метода, если игрок не локальный
        //данная проверка убирает возможность управления чужими персонажами
        if (!isLocalPlayer) return;
        _moveState = MoveState.Walk;
        if (_directionState == DirectionState.Left)
        {
            moveVector.x = Input.GetAxis("Horizontal");
            _directionState = DirectionState.Right;
        }
        _walkTime = _walkCooldown;
        //запуск анимации ходьбы вправо
        _animatorController.Play("WalkRight");
    }

    //метод для движения вверх
    public void MoveUp()
    {
        //выйти из метода, если игрок не локальный
        //данная проверка убирает возможность управления чужими персонажами
        if (!isLocalPlayer) return;
        _moveState = MoveState.Walk;
        if ((_directionState == DirectionState.Down))
        {
            moveVector.y = Input.GetAxis("Vertical");
            _directionState = DirectionState.Up;
        }
        //анимация не будет проигрываться, если игрок
        //одновременно с движением вверх идет в левую или в правую сторону
        if (moveVector.x == 0)
        {
            _walkTime = _walkCooldown;
            _animatorController.Play("WalkUp");
        }
    }

    //метод для движения ввиз
    public void MoveDown()
    {
        //выйти из метода, если игрок не локальный
        //данная проверка убирает возможность управления чужими персонажами
        if (!isLocalPlayer) return;
        _moveState = MoveState.Walk;
        if (_directionState == DirectionState.Up)
        {
            moveVector.y = Input.GetAxis("Vertical");
            _directionState = DirectionState.Down;
        }
        //анимация не будет проигрываться, если игрок
        //одновременно с движением вверх идет в левую или в правую сторону
        if (moveVector.x == 0)
        {
            _walkTime = _walkCooldown;
            _animatorController.Play("WalkDown");
        }
    }


    //смена состояния на Idle и запуск анимации покоя
    private void Idle()
    {
        _moveState = MoveState.Idle;
        _animatorController.Play("Idle");
    }

    //инициализация камеры
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
        //выйти из метода, если игрок не локальный
        //данная проверка убирает возможность управления чужими персонажами
        if (!isLocalPlayer) return;
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");
        CameraMovement();
    }

    //метод для задания движения камеры за игроком
    private void CameraMovement()
    {
        mainCam.transform.localPosition = new Vector3(_transform.position.x, _transform.position.y, -1f);
        _transform.position = Vector2.MoveTowards(_transform.position, mainCam.transform.localPosition, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //смена позиции персонажа
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

    //направления взгляда игрока
    enum DirectionState
    {
        Right,
        Left,
        Up,
        Down
    }

    //состояния персонажа
    enum MoveState
    {
        Idle,
        Walk
    }
}