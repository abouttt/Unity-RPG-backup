using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemData), true)]
public class ItemDataEditor : Editor
{
    private SerializedProperty _itemIdProp;

    private void OnEnable()
    {
        _itemIdProp = serializedObject.FindProperty($"<{nameof(ItemData.ItemId)}>k__BackingField");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        // ������ ���̵� �Ľ�Į�����̸� ������ �빮�ڿ� ������ũ �������� ��ȯ
        string originalItemId = _itemIdProp.stringValue;
        string newItemId = originalItemId.ToSnake().ToUpper();
        if (!originalItemId.Equals(newItemId))
        {
            _itemIdProp.stringValue = newItemId;
            //var itemData = ItemDatabase.Instance.FindItemById(newItemId);
            //if (itemData != null)
            //{
            //    Debug.LogWarning($"{newItemId} id already exist : {AssetDatabase.GetAssetPath(itemData)}");
            //}
        }

        serializedObject.ApplyModifiedProperties();
    }
}
