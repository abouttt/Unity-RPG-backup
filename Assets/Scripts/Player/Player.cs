using UnityEngine;

public class Player : MonoBehaviour
{
    public static GameObject GameObject { get; private set; }
    public static Animator Animator { get; private set; }
    public static CharacterMovement Movement { get; private set; }
    public static FieldOfView LockOnFov { get; private set; }
    public static Interactor Interactor { get; private set; }
    public static ItemInventory ItemInventory { get; private set; }

    private void Awake()
    {
        GameObject = gameObject;
        Animator = GetComponent<Animator>();
        Movement = GetComponent<CharacterMovement>();
        LockOnFov = Camera.main.GetComponent<FieldOfView>();
        Interactor = GetComponentInChildren<Interactor>();
        ItemInventory = GetComponent<ItemInventory>();
    }
}
