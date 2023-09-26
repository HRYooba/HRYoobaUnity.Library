using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HRYooba.Library
{
    [InitializeOnLoad]
    public static class SceneParentResolver
    {
        static SceneParentResolver()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                try
                {
                    var proflie = Resources.Load<SceneParentProfile>(SceneParentProfile.ResourcePath);
                    if (proflie == null) return;

                    var infos = proflie.Infos;
                    foreach (var info in infos)
                    {
                        var parentScenes = info.ParentScenes;
                        var childScene = info.ChildScene;
                        var currentScene = EditorSceneManager.GetActiveScene();
                        if (childScene == null || currentScene.name != childScene.name) continue;

                        foreach (var parentScene in parentScenes)
                        {
                            if (parentScene == null) continue;

                            var scene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(parentScene), OpenSceneMode.Additive);
                            EditorSceneManager.MoveSceneBefore(scene, currentScene);
                        }
                    }
                }
                catch (Exception)
                {
                    EditorApplication.isPlaying = false;
                    throw;
                }
            }
        }
    }
}