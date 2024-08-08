using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute), true)]
public class ReadOnlyAttributeDrawer : PropertyDrawer
{
    // ��Ȱ��ȭ�� �Ӽ� �ʵ� �׸���
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = !Application.isPlaying && ((ReadOnlyAttribute)attribute).RuntimeOnly;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }

    // �Ϻ� �Ӽ��� �ش� ���������� �۰� ��ҵǴ� ������ �����Ƿ� �ʿ�
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
