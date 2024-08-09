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

    [SerializeField]
    private float _lockOnRotationSpeed;

    private GameObject _mainCamera;
    private CharacterMovement _movement;
    private CameraController _cameraController;
    private FieldOfView _lockOnFov;
    private Interactor _interactor;

    #region Input Value
    private Vector2 _move;
    private Vector2 _look;
    private bool _isPressedSprint;
    #endregion

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _movement = GetComponent<CharacterMovement>();
        _cameraController = GetComponent<CameraController>();
        _lockOnFov = _mainCamera.GetComponent<FieldOfView>();
        _interactor = GetComponentInChildren<Interactor>();
    }

    private void Update()
    {
        MoveAndRotate();
    }

    private void LateUpdate()
    {
        CameraRotate();
    }

    private void MoveAndRotate()
    {
        var inputDirection = new Vector3(_move.x, 0f, _move.y);
        float cameraYaw = _mainCamera.transform.eulerAngles.y;

        _movement.MoveSpeed = _movement.IsLanding ? _landingSpeed
                            : _isPressedSprint ? _sprintSpeed
                            : _runSpeed;
        _movement.Move(inputDirection, cameraYaw);

        if (_lockOnFov.HasTarget && IsOnlyRun())
        {
            var rotationDirection = inputDirection == Vector3.zero
                                  ? Vector3.zero
                                  : (_lockOnFov.Target.position - transform.position).normalized;
            _movement.Rotate(rotationDirection);
        }
        else
        {
            _movement.Rotate(inputDirection, cameraYaw);
        }
    }

    private void CameraRotate()
    {
        if (_lockOnFov.HasTarget)
        {
            var lookPosition = (_lockOnFov.Target.position + transform.position) * 0.5f;
            _cameraController.LookRotate(lookPosition, _lockOnRotationSpeed);
        }
        else
        {
            _cameraController.Rotate(_look.y, _look.x);
        }
    }

    private bool IsOnlyRun()
    {
        return !(_isPressedSprint || _movement.IsJumping || _movement.IsFalling || _movement.IsLanding);
    }

    #region Input
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

    private void OnLockOn(InputValue inputValue)
    {
        if (_lockOnFov.HasTarget)
        {
            _lockOnFov.Target = null;
        }
        else
        {
            _lockOnFov.FindTarget();
        }
    }

    private void OnInteract(InputValue inputValue)
    {
        _interactor.Interact = inputValue.isPressed;
    }
    #endregion
}
