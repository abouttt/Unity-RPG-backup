using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _runSpeed;

    [SerializeField]
    private float _sprintSpeed;

    [SerializeField]
    private float _landingSpeed;

    private GameObject _mainCamera;
    private CharacterMovement _movement;
    private CameraController _cameraController;

    // Input
    private Vector2 _move;
    private Vector2 _look;
    private bool _isPressedSprint;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _movement = GetComponent<CharacterMovement>();
        _cameraController = GetComponent<CameraController>();
    }

    private void Update()
    {
        MoveAndRotate();
    }

    private void LateUpdate()
    {
        _cameraController.Rotate(_look.y, _look.x);
    }

    private void MoveAndRotate()
    {
        var inputDirection = new Vector3(_move.x, 0f, _move.y);
        float cameraYaw = _mainCamera.transform.eulerAngles.y;

        _movement.MoveSpeed = _movement.IsLanding ? _landingSpeed
                            : _isPressedSprint ? _sprintSpeed
                            : _runSpeed;

        _movement.Move(inputDirection, cameraYaw);
        _movement.Rotate(inputDirection, cameraYaw);
    }

    //Input

    private void OnMove(InputValue inputValue)
    {
        _move = inputValue.Get<Vector2>();
    }

    private void OnLook(InputValue inputValue)
    {
        _look = Managers.Input.CursorLocked ? inputValue.Get<Vector2>() : Vector2.zero;
    }

    private void OnSprint(InputValue inputValue)
    {
        _isPressedSprint = inputValue.isPressed;
    }

    private void OnJump(InputValue inputValue)
    {
        _movement.Jump();
    }
}
