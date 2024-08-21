using UnityEngine;
using UnityEngine.Events;

public class UI_ItemSplitPopup : UI_Popup
{
    enum GameObjects
    {
        ItemPrice,
    }

    enum Texts
    {
        GuideText,
        PriceText,
    }

    enum Buttons
    {
        UpButton,
        DownButton,
        YesButton,
        NoButton,
    }

    enum InputFields
    {
        InputField,
    }

    public int Count { get; private set; }

    private int _price;
    private int _minCount;
    private int _maxCount;

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindInputField(typeof(InputFields));

        var inputField = GetInputField((int)InputFields.InputField);
        inputField.onValueChanged.AddListener(value => OnValueChanged(value));
        inputField.onEndEdit.AddListener(value => OnEndEdit(value));
        inputField.onSubmit.AddListener(value => GetButton((int)Buttons.YesButton).onClick?.Invoke());

        GetButton((int)Buttons.UpButton).onClick.AddListener(() => OnClickUpOrDownButton(1));
        GetButton((int)Buttons.DownButton).onClick.AddListener(() => OnClickUpOrDownButton(-1));
        GetButton((int)Buttons.NoButton).onClick.AddListener(Managers.UI.Close<UI_ItemSplitPopup>);

        Managers.UI.Register(this);
    }

    public void SetEvent(UnityAction callback, string text, int minCount, int maxCount, int price = -1)
    {
        var yesButton = GetButton((int)Buttons.YesButton);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(callback);
        yesButton.onClick.AddListener(Managers.UI.Close<UI_ItemSplitPopup>);

        Count = maxCount;
        _maxCount = maxCount;
        _minCount = minCount;
        _price = price;

        GetObject((int)GameObjects.ItemPrice).SetActive(price >= 0);
        GetText((int)Texts.GuideText).text = text;
        var inputField = GetInputField((int)InputFields.InputField);
        inputField.text = Count.ToString();
        inputField.ActivateInputField();
        RefreshPriceText();
    }

    private void OnValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        Count = Mathf.Clamp(int.Parse(value), _minCount, _maxCount);
        GetInputField((int)InputFields.InputField).text = Count.ToString();
        RefreshPriceText();
    }

    private void OnEndEdit(string value)
    {
        Count = Mathf.Clamp(string.IsNullOrEmpty(value) ? _maxCount : int.Parse(value), _minCount, _maxCount);
        GetInputField((int)InputFields.InputField).text = Count.ToString();
    }

    private void OnClickUpOrDownButton(int count)
    {
        Count = Mathf.Clamp(Count + count, _minCount, _maxCount);
        GetInputField((int)InputFields.InputField).text = Count.ToString();
    }

    private void RefreshPriceText()
    {
        if (_price < 0)
        {
            return;
        }

        int totalPrice = _price * Count;
        GetText((int)Texts.PriceText).text = totalPrice.ToString();
        // TODO : Player Gold
    }
}
