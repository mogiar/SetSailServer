// Create a folder (right click in the Assets directory, click Create>New Folder)
// and name it “Editor” if one doesn’t exist already. Place this script in that folder.

// This script creates a new menu item Examples>Create Prefab in the main menu.
// Use it to create Prefab(s) from the selected GameObject(s).
// It will be placed in the root Assets folder.

using UnityEngine;
using UnityEditor;

public class Example
{
#if (UNITY_EDITOR) 
    // Creates a new menu item 'Examples > Create Prefab' in the main menu.
    [MenuItem("GalTools/Create Prefab")]
    static void CreatePrefab()
    {
        // Keep track of the currently selected GameObject(s)
        GameObject[] objectArray = Selection.gameObjects;

        // Loop through every GameObject in the array above
        foreach (GameObject gameObject in objectArray)
        {
            GameObject a = gameObject.transform.GetChild(0).gameObject;
            a.AddComponent<MeshCollider>();
            a.GetComponent<MeshCollider>().sharedMesh = gameObject.transform.GetChild(1).gameObject.GetComponent<MeshFilter>().sharedMesh;
            // Set the path as within the Assets folder,
            // and name it as the GameObject's name with the .Prefab format
            string localPath = "Assets/AutomatePrefabs/2_5/" + gameObject.name + ".prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab.
            PrefabUtility.SaveAsPrefabAssetAndConnect(a, localPath, InteractionMode.UserAction);
        }
    }

    [MenuItem("GalTools/AlignObjects")]
    static void AlignObjects()
    {
        GameObject[] objectArray = Selection.gameObjects;
        float a = objectArray[0].transform.position.x;

        foreach (GameObject obj in objectArray)
        {
            a += 15f;
            obj.transform.position = new Vector3(a, obj.transform.position.y, obj.transform.position.z);
        }
    }


    [MenuItem("GalTools/AlignObjects", true)]
    static bool ValidateAlignObjects()
    {
        return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
    }

    // Disable the menu item if no selection is in place.
    [MenuItem("GalTools/Create Prefab", true)]
    static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
    }
#endif
}