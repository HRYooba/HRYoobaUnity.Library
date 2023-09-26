using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HRYooba.Library
{
    public class SceneParentProfile : ScriptableObject
    {
        public const string ResourcePath = "SceneParentProfile";

        [SerializeField] private List<Info> _infos = new();
        public IReadOnlyList<Info> Infos => _infos;

        [Serializable]
        public class Info
        {
            [SerializeField] private List<SceneAsset> _parentScenes = new();
            [SerializeField] private SceneAsset _childScene = default;

            public IReadOnlyList<SceneAsset> ParentScenes => _parentScenes;
            public SceneAsset ChildScene => _childScene;
        }

        [MenuItem("Assets/Create/HRYooba/Library/SceneParentProfile")]
        private static void Create()
        {
            var folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!folderPath.EndsWith("/Resources"))
            {
                EditorUtility.DisplayDialog("Error", "SceneParentProfile objects must be placed directly underneath a folder named 'Resources'.  Please try again.", "Ok");
                return;
            }

            var path = Path.Combine(folderPath, $"{ResourcePath}.asset");
            var asset = CreateInstance<SceneParentProfile>();
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            asset.name = ResourcePath;
        }
    }
}