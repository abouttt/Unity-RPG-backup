using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CameraController _cameraController;

    private Vector2 _look;

    private void Awake()
    {
        _cameraController = GetComponent<CameraController>();
    }

    private void LateUpdate()
    {
        _cameraController.Rotate(_look.y, _look.x);
    }

    private void OnLook(InputValue value)
    {
        _look = Managers.Input.CursorLocked ? value.Get<Vector2>() : Vector2.zero;
    }
}
