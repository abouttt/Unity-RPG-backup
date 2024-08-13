using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
public class SpritePreviewAttributeDrawer : PropertyDrawer
{
    private readonly float Spacing = 5f; // �ʵ�� ������ ���� ����

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float previewSize = (attribute as SpritePreviewAttribute).Size;
        float singleLineHeight = EditorGUIUtility.singleLineHeight;

        // ����ȭ�� �ʵ�� ������ �̸����⸦ ���� ����
        var fieldRect = new Rect
        {
            x = position.x,
            y = position.y,
            width = position.width,
            height = singleLineHeight
        };

        var spriteRect = new Rect
        {
            x = position.x + (position.width - previewSize) * 0.5f,
            y = position.y + singleLineHeight + Spacing,
            width = previewSize,
            height = previewSize,
        };

        // ����ȭ�� �ʵ带 ������
        EditorGUI.PropertyField(fieldRect, property, label);

        // �������� ������ ��� �̸����⸦ ������
        if (property.objectReferenceValue is Sprite sprite)
        {
            GUI.DrawTexture(spriteRect, sprite.texture, ScaleMode.ScaleToFit, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + (attribute as SpritePreviewAttribute).Size + Spacing;
    }
}
