using UnityEngine;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_Interactor : UI_AutoSubitem, ISystemConnectable<Interactor>
{
    enum Objects
    {
        Body,
    }

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

        BindObject(typeof(Objects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        _followTarget = GetComponent<UI_FollowWorldObject>();

        GetText((int)Texts.KeyText).text = Managers.Input.GetBindingPath("Interact");
    }

    private void LateUpdate()
    {
        if (_interactorRef.Target.IsInteracted)
        {
            GetObject((int)Objects.Body).SetActive(false);
            return;
        }

        if (!GetObject((int)Objects.Body).activeSelf)
        {
            GetObject((int)Objects.Body).SetActive(true);
        }

        if (GetImage((int)Images.LoadingTimeImage).IsActive())
        {
            GetImage((int)Images.LoadingTimeImage).fillAmount =
                _interactorRef.InteractionHoldingTime / _interactorRef.Target.InteractionHoldTime;
        }
    }

    public void ConnectSystem(Interactor interactor)
    {
        if (interactor == null)
        {
            return;
        }

        DeconnectSystem();

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
            bool canInteract = target.CanInteract;
            GetImage((int)Images.BG).gameObject.SetActive(canInteract);
            GetText((int)Texts.KeyText).gameObject.SetActive(canInteract);

            bool hasHoldTime = canInteract && target.InteractionHoldTime > 0f;
            GetImage((int)Images.LoadingTimeImage).gameObject.SetActive(hasHoldTime);
            GetImage((int)Images.Frame).gameObject.SetActive(hasHoldTime);

            var interactionText = GetText((int)Texts.InteractionText);
            interactionText.text = target.InteractionMessage;
            interactionText.gameObject.SetActive(canInteract);

            var name = GetText((int)Texts.NameText);
            name.text = target.InteractionObjectName;
            name.gameObject.SetActive(!string.IsNullOrEmpty(target.InteractionObjectName));

            _followTarget.SetTargetAndOffset(target.transform, target.UIOffset);
        }

        gameObject.SetActive(isNotNull);
    }
}
