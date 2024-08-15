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

    public Interactor SystemRef { get; private set; }

    private UI_FollowWorldObject _followTarget;

    protected override void Init()
    {
        base.Init();

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        _followTarget = GetComponent<UI_FollowWorldObject>();

        GetText((int)Texts.KeyText).text = Managers.Input.GetBindingPath("Interact");

        Managers.UI.Get<UI_AutoCanvas>().AddUniqueAutoUI(this);
    }

    private void LateUpdate()
    {
        if (SystemRef.Target == null)
        {
            return;
        }

        if (SystemRef.Target.IsInteracted)
        {
            _body.SetActive(false);
            return;
        }

        if (!_body.activeSelf)
        {
            _body.SetActive(true);
        }

        if (GetImage((int)Images.LoadingTimeImage).IsActive())
        {
            GetImage((int)Images.LoadingTimeImage).fillAmount = SystemRef.InteractLoadingTime / SystemRef.Target.MaxLoadingTime;
        }
    }

    public void ConnectSystem(Interactor interactor)
    {
        SystemRef = interactor;
        interactor.TargetChanged += SetTarget;
    }

    public void DeconnectSystem()
    {
        if (SystemRef == null)
        {
            return;
        }

        SystemRef.TargetChanged -= SetTarget;
        SystemRef = null;
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

        _body.SetActive(isNotNull);
    }

    private void OnDestroy()
    {
        DeconnectSystem();
    }
}
