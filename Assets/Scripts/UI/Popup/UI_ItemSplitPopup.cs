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

    public int Quantity { get; private set; }

    private int _price;
    private int _minQuantity;
    private int _maxQuantity;

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

    public void SetEvent(UnityAction callback, string text, int minQuantity, int maxQuantity, int price = -1)
    {
        var yesButton = GetButton((int)Buttons.YesButton);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(callback);
        yesButton.onClick.AddListener(Managers.UI.Close<UI_ItemSplitPopup>);

        Quantity = maxQuantity;
        _maxQuantity = maxQuantity;
        _minQuantity = minQuantity;
        _price = price;

        GetObject((int)GameObjects.ItemPrice).SetActive(price >= 0);
        GetText((int)Texts.GuideText).text = text;
        var inputField = GetInputField((int)InputFields.InputField);
        inputField.text = Quantity.ToString();
        inputField.ActivateInputField();
        RefreshPriceText();
    }

    private void OnValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        Quantity = Mathf.Clamp(int.Parse(value), _minQuantity, _maxQuantity);
        GetInputField((int)InputFields.InputField).text = Quantity.ToString();
        RefreshPriceText();
    }

    private void OnEndEdit(string value)
    {
        Quantity = Mathf.Clamp(string.IsNullOrEmpty(value) ? _maxQuantity : int.Parse(value), _minQuantity, _maxQuantity);
        GetInputField((int)InputFields.InputField).text = Quantity.ToString();
    }

    private void OnClickUpOrDownButton(int quantity)
    {
        Quantity = Mathf.Clamp(Quantity + quantity, _minQuantity, _maxQuantity);
        GetInputField((int)InputFields.InputField).text = Quantity.ToString();
    }

    private void RefreshPriceText()
    {
        if (_price < 0)
        {
            return;
        }

        int totalPrice = _price * Quantity;
        GetText((int)Texts.PriceText).text = totalPrice.ToString();
        // TODO : Player Gold
    }
}
