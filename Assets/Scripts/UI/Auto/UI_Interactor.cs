using UnityEngine;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_Interactor : UI_Auto, ISystemConnectable<Interactor>
{
    enum Images
    {
        LoadingTimeImage,
        BG,
        Frame,
    }

    enum Texts
    {
        KeyText,
        InteractionText,
        NameText,
    }

    private Interactor _interactorRef;
    private UI_FollowWorldObject _followTarget;

    protected override void Init()
    {
        base.Init();

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        _followTarget = GetComponent<UI_FollowWorldObject>();

        GetText((int)Texts.KeyText).text = Managers.Input.GetBindingPath("Interact");
    }

    private void LateUpdate()
    {
        if (_interactorRef.Target.IsInteracted)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (GetImage((int)Images.LoadingTimeImage).IsActive())
        {
            GetImage((int)Images.LoadingTimeImage).fillAmount = _interactorRef.InteractLoadingTime / _interactorRef.Target.MaxLoadingTime;
        }
    }

    public void ConnectSystem(Interactor interactor)
    {
        _interactorRef = interactor;
        interactor.TargetChanged += SetTarget;
    }

    public void DeconnectSystem()
    {
        if (_interactorRef != null)
        {
            _interactorRef.TargetChanged -= SetTarget;
            _interactorRef = null;
        }
    }

    private void SetTarget(Interactable target)
    {
        bool isNotNull = target != null;

        if (isNotNull)
        {
            _followTarget.SetTargetAndOffset(target.transform, target.UIOffset);

            bool canInteract = target.CanInteract;
            GetImage((int)Images.BG).gameObject.SetActive(canInteract);
            GetText((int)Texts.KeyText).gameObject.SetActive(canInteract);

            bool hasLoadingTime = canInteract && target.MaxLoadingTime > 0f;
            GetImage((int)Images.LoadingTimeImage).gameObject.SetActive(hasLoadingTime);
            GetImage((int)Images.Frame).gameObject.SetActive(hasLoadingTime);

            var interactionText = GetText((int)Texts.InteractionText);
            interactionText.text = target.InteractionMessage;
            interactionText.gameObject.SetActive(canInteract);

            var name = GetText((int)Texts.NameText);
            name.text = target.InteractionObjectName;
            name.gameObject.SetActive(!string.IsNullOrEmpty(target.InteractionObjectName));
        }

        gameObject.SetActive(isNotNull);
    }
}
