using UnityEngine;

public class GameScene : BaseScene
{
    private void Start()
    {
        Managers.Input.Enabled = true;
        Managers.Input.CursorLocked = true;
    }
}
