using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Squad))]
public class SquadDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        Debug.Log("create prop");

        // Create property container element.
        var container = new VisualElement();

        // Create a horizontal container for the fields.
        var horizontalContainer = new VisualElement();
        horizontalContainer.style.flexDirection = FlexDirection.Row;

        // Create property fields.
        var prefabField = new ObjectField
        {
            objectType = typeof(GameObject),
            allowSceneObjects = false,
            value = property.FindPropertyRelative("Prefab").objectReferenceValue
        };

        prefabField.RegisterValueChangedCallback(evt =>
        {
            property.FindPropertyRelative("Prefab").objectReferenceValue = evt.newValue;
            property.serializedObject.ApplyModifiedProperties();
        });

        // var prefabField = CreateLargerThumbnailObjectField(property.FindPropertyRelative("Prefab"));

        var countField = new PropertyField(property.FindPropertyRelative("Count"));
        var spawnDelayField = new PropertyField(property.FindPropertyRelative("SpawnDelay"));
        var entranceField = new PropertyField(property.FindPropertyRelative("EntranceId"));
        var exitField = new PropertyField(property.FindPropertyRelative("ExitId"));

        prefabField.style.width = 500; // Adjust the width as needed
        countField.style.width = 200; // Adjust the width as needed
        spawnDelayField.style.width = 200; // Adjust the width as needed
        entranceField.style.width = 200; // Adjust the width as needed
        exitField.style.width = 200; // Adjust the width as needed

        // Add fields to the horizontal container.
        horizontalContainer.Add(prefabField);
        horizontalContainer.Add(countField);
        horizontalContainer.Add(spawnDelayField);
        horizontalContainer.Add(entranceField);
        horizontalContainer.Add(exitField);

        // Add the horizontal container to the main container.
        container.Add(horizontalContainer);

        return container;
    }

    private VisualElement CreateLargerThumbnailObjectField(SerializedProperty prefabProperty)
    {
        // Create a container for the prefab field and the thumbnail.
        var prefabContainer = new VisualElement();
        prefabContainer.style.flexDirection = FlexDirection.Row;
        // prefabContainer.style.alignItems = Align.Center;

        // Create an Image element for the thumbnail.
        var thumbnailImage = new Image();
        UpdateThumbnail(prefabProperty, thumbnailImage);

        // Create the ObjectField for the prefab.
        var prefabField = new ObjectField
        {
            objectType = typeof(GameObject),
            allowSceneObjects = false,
            value = prefabProperty.objectReferenceValue
        };
        prefabField.RegisterValueChangedCallback(evt =>
        {
            prefabProperty.objectReferenceValue = evt.newValue;
            prefabProperty.serializedObject.ApplyModifiedProperties();
            UpdateThumbnail(prefabProperty, thumbnailImage);
        });

        // Style the Image element.
        thumbnailImage.style.width = 50;
        thumbnailImage.style.height = 50;
        thumbnailImage.style.marginRight = 10;
        prefabField.style.flexGrow = 1; // Adjust the width as needed

        // Add the Image and ObjectField to the prefab container.
        prefabContainer.Add(thumbnailImage);
        prefabContainer.Add(prefabField);

        return prefabContainer;
    }

    private void UpdateThumbnail(SerializedProperty prefabProperty, Image thumbnailImage)
    {
        var prefab = prefabProperty.objectReferenceValue as GameObject;
        if (prefab != null)
        {
            Texture2D thumbnail = AssetPreview.GetAssetPreview(prefab);
            thumbnailImage.image = thumbnail;
        }
        else
        {
            thumbnailImage.image = null;
        }
    }
}
