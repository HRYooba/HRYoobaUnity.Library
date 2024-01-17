using UnityEngine;
using UnityEditor;

namespace HRYooba.Editor
{
    public class MenuItemHRYoobaUI
    {
        private static void InstantiateGameObject(MenuCommand menuCommand, string path)
        {
            // Create a custom game object
            GameObject obj = Resources.Load<GameObject>(path);
            GameObject go = UnityEngine.GameObject.Instantiate(obj) as GameObject;
            go.name = obj.name;
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/HRYoobaUI/OperationCanvas", false, 21)]
        private static void CreateOperationCanvas(MenuCommand menuCommand)
        {
            InstantiateGameObject(menuCommand, "OperationCanvas");
        }

        [MenuItem("GameObject/HRYoobaUI/IntController", false, 21)]
        private static void CreateIntController(MenuCommand menuCommand)
        {
            InstantiateGameObject(menuCommand, "IntController");
        }

        [MenuItem("GameObject/HRYoobaUI/FloatController", false, 21)]
        private static void CreateFloatController(MenuCommand menuCommand)
        {
            InstantiateGameObject(menuCommand, "FloatController");
        }

        [MenuItem("GameObject/HRYoobaUI/Vector2Controller", false, 21)]
        private static void CreateVector2Controller(MenuCommand menuCommand)
        {
            InstantiateGameObject(menuCommand, "Vector2Controller");
        }

        [MenuItem("GameObject/HRYoobaUI/Vector3Controller", false, 21)]
        private static void CreateVector3Controller(MenuCommand menuCommand)
        {
            InstantiateGameObject(menuCommand, "Vector3Controller");
        }

        [MenuItem("GameObject/HRYoobaUI/Vector4Controller", false, 21)]
        private static void CreateVector4Controller(MenuCommand menuCommand)
        {
            InstantiateGameObject(menuCommand, "Vector4Controller");
        }

        [MenuItem("GameObject/HRYoobaUI/Button", false, 21)]
        private static void CreateButton(MenuCommand menuCommand)
        {
            InstantiateGameObject(menuCommand, "Button");
        }
    }
}