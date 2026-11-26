using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

namespace MatematicaHXR.Editor
{
    public static class ProjectSetup
    {
        [MenuItem("HXR/1 - Preparar cenas e Quest 2")]
        public static void Generate()
        {
            Directory.CreateDirectory("Assets/Scenes");Directory.CreateDirectory("Assets/XR");
            PlayerSettings.companyName="Pesquisa HXR";
            PlayerSettings.productName="Matemática HXR";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"br.edu.prototipo.matematicahxr");
            PlayerSettings.bundleVersion="0.1.0";
            PlayerSettings.Android.bundleVersionCode=1;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel32;
            PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.Vulkan});
            PlayerSettings.colorSpace=ColorSpace.Linear;
            PlayerSettings.runInBackground=true;
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
            var ps=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            var active=ps.FindProperty("activeInputHandler");if(active!=null){active.intValue=1;ps.ApplyModifiedPropertiesWithoutUndo();}
            XRGeneralSettingsPerBuildTarget all;
            if(!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey,out all))
            {
                all=ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
                AssetDatabase.CreateAsset(all,"Assets/XR/XRGeneralSettings.asset");
                EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey,all,true);
            }
            if(!all.HasSettingsForBuildTarget(BuildTargetGroup.Android))all.CreateDefaultSettingsForBuildTarget(BuildTargetGroup.Android);
            if(all.SettingsForBuildTarget(BuildTargetGroup.Android).Manager==null)all.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);
            var general=all.SettingsForBuildTarget(BuildTargetGroup.Android);general.InitManagerOnStart=true;
            if(!XRPackageMetadataStore.AssignLoader(general.Manager,"UnityEngine.XR.OpenXR.OpenXRLoader",BuildTargetGroup.Android))
                throw new Exception("Não foi possível configurar o carregador OpenXR.");
            var xr=OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            xr.renderMode=OpenXRSettings.RenderMode.SinglePassInstanced;
            var touch=xr.GetFeature<OculusTouchControllerProfile>();
            var quest=xr.GetFeature<MetaQuestFeature>();
            if(touch==null||quest==null)throw new Exception("Perfil Touch ou suporte Meta Quest ausente.");
            touch.enabled=true;quest.enabled=true;
            EditorUtility.SetDirty(xr);EditorUtility.SetDirty(touch);EditorUtility.SetDirty(quest);EditorUtility.SetDirty(all);EditorUtility.SetDirty(general);
            var scenes=new string[]{"Inicio","A_Numeros","B_Blocos"};
            for(int i=0;i<scenes.Length;i++)
            {
                var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                var demo=new GameObject("Matematica HXR").AddComponent<MathExperience>();demo.initialMode=i;
                demo.controls=AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/HXRControls.inputactions");
                EditorSceneManager.SaveScene(scene,"Assets/Scenes/"+scenes[i]+".unity");
            }
            EditorBuildSettings.scenes=scenes.Select(s=>new EditorBuildSettingsScene("Assets/Scenes/"+s+".unity",true)).ToArray();
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/Scenes/Inicio.unity");
            ValidateModel();
            Debug.Log("HXR_SETUP_OK: cenas e OpenXR configurados.");
        }
        [MenuItem("HXR/2 - Verificar matemática")]
        public static void ValidateModel()
        {
            var m=new SumModel();
            for(int i=0;i<=15;i++)
            {
                if(m.Step!=i||m.Left!=15-i||m.Right!=i||m.Left+m.Right!=15)throw new Exception("Invariante incorreta.");
                bool moved=m.Move(1);if(moved!=(i<15))throw new Exception("Limite superior incorreto.");
            }
            for(int i=15;i>=0;i--)
            {if(m.Step!=i)throw new Exception("Retorno incorreto.");bool moved=m.Move(-1);if(moved!=(i>0))throw new Exception("Limite inferior incorreto.");}
            m.Move(7);m.Reset();if(m.Left!=15||m.Right!=0)throw new Exception("Reinício incorreto.");
            Debug.Log("HXR_TEST_OK: 16 combinações, limites, retorno e reinício.");
        }
        [MenuItem("HXR/3 - Gerar APK Quest 2")]
        public static void BuildQuest()
        {
            ValidateModel();
            if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android,BuildTarget.Android))throw new Exception("Instale Android Build Support pelo Unity Hub.");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
            Directory.CreateDirectory("Builds");
            var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{
                scenes=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray(),
                locationPathName="Builds/MatematicaHXR-Quest2.apk",target=BuildTarget.Android,options=BuildOptions.Development});
            if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Falha ao gerar APK: "+result.summary.result);
            Debug.Log("HXR_BUILD_OK: "+result.summary.outputPath);
        }
    }
}
