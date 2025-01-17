using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public abstract class UI_Base : MonoBehaviour
{
    [field: SerializeField]
    public UIType UIType { get; private set; }

    private readonly Dictionary<Type, List<Object>> _objects = new();

    private void Awake()
    {
        Init();
    }

    protected abstract void Init();

    // 본인을 포함한 자식 오브젝트에서 type을 찾아 바인딩하고 이미 존재하는 type이면 뒤이어 추가한다.
    protected void Bind<T>(Type type) where T : Object
    {
        var names = Enum.GetNames(type);
        var newObjects = new List<Object>(names.Length);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                newObjects.Add(gameObject.FindChild(names[i], true));
            }
            else
            {
                newObjects.Add(gameObject.FindChild<T>(names[i], true));
            }

            if (newObjects[i] == null)
            {
                Debug.LogWarning($"{gameObject.name} failed to bind({names[i]})");
            }
        }

        if (_objects.TryGetValue(typeof(T), out var objects))
        {
            objects.AddRange(newObjects);
        }
        else
        {
            _objects.Add(typeof(T), newObjects);
        }
    }

    protected void BindObject(Type type) => Bind<GameObject>(type);
    protected void BindRT(Type type) => Bind<RectTransform>(type);
    protected void BindImage(Type type) => Bind<Image>(type);
    protected void BindText(Type type) => Bind<TextMeshProUGUI>(type);
    protected void BindButton(Type type) => Bind<Button>(type);
    protected void BindToggle(Type type) => Bind<Toggle>(type);
    protected void BindSlider(Type type) => Bind<Slider>(type);
    protected void BindScrollbar(Type type) => Bind<Scrollbar>(type);
    protected void BindScrollRect(Type type) => Bind<ScrollRect>(type);
    protected void BindDropdown(Type type) => Bind<TMP_Dropdown>(type);
    protected void BindInputField(Type type) => Bind<TMP_InputField>(type);

    protected T Get<T>(int index) where T : Object
    {
        if (_objects.TryGetValue(typeof(T), out var objects))
        {
            return objects[index] as T;
        }

        return null;
    }

    protected GameObject GetObject(int index) => Get<GameObject>(index);
    protected RectTransform GetRT(int index) => Get<RectTransform>(index);
    protected Image GetImage(int index) => Get<Image>(index);
    protected TextMeshProUGUI GetText(int index) => Get<TextMeshProUGUI>(index);
    protected Button GetButton(int index) => Get<Button>(index);
    protected Toggle GetToggle(int index) => Get<Toggle>(index);
    protected Slider GetSlider(int index) => Get<Slider>(index);
    protected Scrollbar GetScrollbar(int index) => Get<Scrollbar>(index);
    protected ScrollRect GetScrollRect(int index) => Get<ScrollRect>(index);
    protected TMP_Dropdown GetDropdown(int index) => Get<TMP_Dropdown>(index);
    protected TMP_InputField GetInputField(int index) => Get<TMP_InputField>(index);
}
