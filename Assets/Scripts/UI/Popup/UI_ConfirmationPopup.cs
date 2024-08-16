using UnityEngine;
using UnityEngine.Events;

public class UI_ConfirmationPopup : UI_Popup
{
    enum Texts
    {
        GuideText,
        YesText,
        NoText,
    }

    enum Buttons
    {
        YesButton,
        NoButton,
    }

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.NoButton).onClick.AddListener(Managers.UI.Close<UI_ConfirmationPopup>);

        Managers.UI.Register(this);
    }

    public void SetEvent(UnityAction callback, string text, string yesText = "예", string noText = "아니오", bool yes = true, bool no = true)
    {
        var yesButton = GetButton((int)Buttons.YesButton);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(callback);
        yesButton.onClick.AddListener(Managers.UI.Close<UI_ConfirmationPopup>);
        yesButton.gameObject.SetActive(yes);
        GetButton((int)Buttons.NoButton).gameObject.SetActive(no);
        GetText((int)Texts.YesText).text = yesText;
        GetText((int)Texts.NoText).text = noText;
        GetText((int)Texts.GuideText).text = text;
    }
}
