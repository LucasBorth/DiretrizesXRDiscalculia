using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem;
using MatematicaHXR.Atelier;

namespace MatematicaHXR.Editor
{
    public static class AtelierBuild
    {
        public const string ScenePath="Assets/Scenes/C_Atelie.unity";
        [MenuItem("HXR/Ateliê/1 - Preparar nova versão")]
        public static void Prepare()
        {
            if(EditorApplication.isPlaying)throw new Exception("Saia de Play antes de preparar.");
            if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var experience=new GameObject("Ateliê HXR").AddComponent<AtelierExperience>();
            experience.controls=AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/AtelierControls.inputactions");
            if(experience.controls==null)throw new Exception("AtelierControls não importado.");
            EditorSceneManager.SaveScene(scene,ScenePath);AssetDatabase.SaveAssets();
            ValidateModel();Debug.Log("ATELIER_PREPARED");
        }
        [MenuItem("HXR/Ateliê/2 - Verificar e capturar")]
        public static void Preview()
        {
            if(EditorApplication.isPlaying)throw new Exception("Saia de Play antes da captura em edição.");
            ValidateModel();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var e=new GameObject("Ateliê HXR preview").AddComponent<AtelierExperience>();
            e.controls=AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/AtelierControls.inputactions");e.Initialize();
            e.Eye.transform.rotation=Quaternion.Euler(12,0,0);
            Capture(e,"01-explorar");
            e.Model.Practice();e.Primary();e.Model.Place(0,1);e.Model.Place(1,1);e.Model.Place(2,1);e.Model.Place(3,1);e.Model.Place(4,1);e.Model.Place(5,1);e.Model.Place(6,1);e.Organize();
            Capture(e,"02-dois-grupos");e.Primary();
            for(int i=0;i<15;i++)e.Model.Place(i,2);e.Organize(); // Organize separates Result; capture real result below.
            e.Model.BeginGather();for(int i=0;i<15;i++)e.Model.Place(i,2);
            foreach(var b in e.Blocks){b.transform.position=e.Destination(b.Id);b.Body.isKinematic=true;}
            e.Refresh();Capture(e,"03-soma");
            Directory.CreateDirectory("../AtelieHXR");
            File.WriteAllText("../AtelieHXR/Verificacao.txt","PASS: modelo de conservação, 16 divisões, rejeição de zonas inválidas, proveniência, coleta única, reinício e pré-requisito de exploração.\nCapturas em editor. Interação física, áudio e vibração no Quest exigem validação no dispositivo.\n");
            EditorSceneManager.OpenScene(ScenePath);Debug.Log("ATELIER_PREVIEW_OK");
        }
        public static void ValidateModel()
        {
            var m=new AtelierModel();m.Begin();Require(m.Phase==AtelierModel.Stage.Explore,"Exploração deve anteceder tarefa");m.Practice();m.Begin();
            for(int step=0;step<=15;step++)
            {
                Require(m.Left==15-step&&m.Right==step,"Divisão incorreta");
                Require(!m.Place(-1,0)&&!m.Place(15,2)&&!m.Place(0,-1),"Índices inválidos aceitos");
                Require(m.BeginGather(),"Coleta não iniciou");
                for(int i=0;i<15;i++){Require(m.Place(i,2),"Coleta rejeitada");if(i<14){Require(m.Place(i,2),"Repetição deve ser idempotente");Require(m.Joined==i+1,"Unidade duplicada");}}
                Require(m.Joined==15&&m.Phase==AtelierModel.Stage.Result&&m.Left+m.Right==15,"Total incorreto");
                Require(!m.Place(0,0),"Resultado deve permanecer estável");m.NextPartition();
                if(step<15)
                {
                    Require(m.TransferRequired&&!m.BeginGather(),"Deve aguardar transferência manual");
                    int id=Array.FindIndex(m.Group,g=>g==0);Require(m.Place(id,1),"Transferência A para B falhou");
                    Require(!m.TransferRequired&&m.Left==14-step,"Transferência inválida");
                    if(m.Left>0){int another=Array.FindIndex(m.Group,g=>g==0);Require(!m.Place(another,1),"Desafio deve transferir uma unidade");}
                }
                else Require(m.Left==15&&!m.TransferRequired,"Ciclo não reiniciou");
            }
            m.BeginGather();m.Place(0,2);m.Place(0,0);Require(m.Joined==0,"Retirada não contabilizada");
            m.Separate();Require(m.Joined==0&&m.Left+m.Right==15,"Separação perdeu unidades");
            Debug.Log("ATELIER_MODEL_PASS");
        }
        static void Require(bool ok,string message){if(!ok)throw new Exception(message);}
        static void Capture(AtelierExperience e,string name)
        {
            Canvas.ForceUpdateCanvases();var camera=e.Eye;var rt=new RenderTexture(1600,1200,24);var old=RenderTexture.active;
            camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;var texture=new Texture2D(1600,1200,TextureFormat.RGB24,false);
            texture.ReadPixels(new Rect(0,0,1600,1200),0,0);texture.Apply();Directory.CreateDirectory("../AtelieHXR/Previews");File.WriteAllBytes("../AtelieHXR/Previews/"+name+".png",texture.EncodeToPNG());
            camera.targetTexture=null;RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(texture);UnityEngine.Object.DestroyImmediate(rt);
        }
        [MenuItem("HXR/Ateliê/3 - Gerar APK separado")]
        public static void Build()
        {
            ValidateModel();
            if(!File.Exists(ScenePath))throw new Exception("Prepare a cena do Ateliê primeiro.");
            if(EditorUserBuildSettings.activeBuildTarget!=BuildTarget.Android)throw new Exception("Selecione Android e aguarde a importação antes de gerar.");
            string name=PlayerSettings.productName,id=PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android),version=PlayerSettings.bundleVersion;int code=PlayerSettings.Android.bundleVersionCode;
            try
            {
                PlayerSettings.productName="Ateliê HXR";PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"br.edu.prototipo.ateliehxr");PlayerSettings.bundleVersion="0.2.0";PlayerSettings.Android.bundleVersionCode=2;
                Directory.CreateDirectory("../AtelieHXR");
                var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="../AtelieHXR/AtelieHXR-Quest2.apk",target=BuildTarget.Android,options=BuildOptions.Development});
                if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Ateliê: "+r.summary.result);
                File.WriteAllText("../AtelieHXR/Build.txt",$"Build: {r.summary.result}\nTamanho: {r.summary.totalSize}\nTempo: {r.summary.totalTime}\nUnity: {Application.unityVersion}\nPacote: br.edu.prototipo.ateliehxr\n");Debug.Log("ATELIER_BUILD_OK");
            }
            finally{PlayerSettings.productName=name;PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,id);PlayerSettings.bundleVersion=version;PlayerSettings.Android.bundleVersionCode=code;AssetDatabase.SaveAssets();}
        }
    }
}
