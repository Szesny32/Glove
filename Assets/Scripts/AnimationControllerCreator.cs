using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.IO;

public class AnimationControllerCreator : MonoBehaviour
{

    void Start(){
        CreateAnimationController();
    }

    [MenuItem("Assets/Create/Animation Controller from FBX")]
    static void CreateAnimationController()
    {
        string path = "Assets/Animations";
        string[] fbxFiles = Directory.GetFiles(path, "*.fbx", SearchOption.AllDirectories);

        if (fbxFiles.Length == 0)
        {
            Debug.LogError("No FBX files found in the specified path.");
            return;
        }

        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/AnimationController.controller");

        AnimatorControllerLayer layer = animatorController.layers[0];
        AnimatorStateMachine stateMachine = layer.stateMachine;

        foreach (string fbxFile in fbxFiles)
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(fbxFile);

            if (clip != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(fbxFile);
                clip.name = fileName; // Zmień nazwę klipu na nazwę pliku

                AnimatorState state = stateMachine.AddState(fileName);
                state.motion = clip;
            }
        }


        Debug.Log("Animation Controller created with " + fbxFiles.Length + " animations.");
    }
}