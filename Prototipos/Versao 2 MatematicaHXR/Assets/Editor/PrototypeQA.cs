using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace MatematicaHXR.Editor
{
    [InitializeOnLoad]
    public static class PrototypeQA
    {
        static double next;
        static int phase;
        static MathExperience demo;
        static PrototypeQA(){EditorApplication.update+=Tick;}
        [MenuItem("HXR/4 - Testar e capturar apresentações")]
        public static void Start()
        {
            if(EditorApplication.isPlaying)return;
            EditorSceneManager.OpenScene("Assets/Scenes/Inicio.unity");
            SessionState.SetBool("HXR_QA",true);SessionState.SetInt("HXR_QA_PHASE",0);
            EditorApplication.isPlaying=true;
        }
        static void Tick()
        {
            if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            if(!EditorApplication.isPlayingOrWillChangePlaymode && File.Exists("build.request"))
            {File.Delete("build.request");CaptureInEditor();ProjectSetup.BuildQuest();return;}
            if(!EditorApplication.isPlayingOrWillChangePlaymode && File.Exists("qa.request"))
            {File.Delete("qa.request");Start();return;}
            if(!EditorApplication.isPlaying||!SessionState.GetBool("HXR_QA",false)||EditorApplication.timeSinceStartup<next)return;
            next=EditorApplication.timeSinceStartup+.4;
            try
            {
                demo=UnityEngine.Object.FindAnyObjectByType<MathExperience>();
                if(demo==null)return;
                phase=SessionState.GetInt("HXR_QA_PHASE",0);
                if(phase==0){demo.Initialize();demo.SelectMode(1);demo.ToggleMotion();demo.Model.Move(7);demo.Separate();demo.Center();}
                if(phase==1){demo.StartSum();Capture("A-numeros.png");demo.SelectMode(2);}
                if(phase==2){Capture("B-grupos.png");demo.StartSum();}
                if(phase==3){Capture("B-soma.png");ValidateStates();demo.Restart();demo.ToggleMotion();demo.Move(1);}
                if(phase==4){Require(demo.Busy,"Animação não iniciou");demo.TogglePause();}
                if(phase==5){Require(demo.Busy&&demo.Paused,"Pausa não preservada");demo.TogglePause();next=EditorApplication.timeSinceStartup+2;}
                if(phase==6)
                {
                    Require(!demo.Busy&&demo.Model.Left==14&&demo.Model.Right==1,"Transferência incompleta");
                    File.WriteAllText("QA-result.txt","PASS: ambas condições; 16 combinações por condição; soma, separação, reinício, limites; transferência animada e pausa.\nValidação em editor macOS. Headset ainda não validado.\n");
                    Debug.Log("HXR_QA_OK");SessionState.SetBool("HXR_QA",false);EditorApplication.isPlaying=false;
                    if(File.Exists("qa-then-build.request")){File.Delete("qa-then-build.request");File.WriteAllText("build.request", "ready");}
                }
                SessionState.SetInt("HXR_QA_PHASE",phase+1);
            }
            catch(Exception e){File.WriteAllText("QA-result.txt","FAIL: "+e);Debug.LogException(e);SessionState.SetBool("HXR_QA",false);EditorApplication.isPlaying=false;}
        }
        static void Require(bool value,string message){if(!value)throw new Exception(message);}
        static void ValidateStates()
        {
            foreach(int mode in new[]{1,2})
            {
                demo.SelectMode(mode);demo.Restart();
                for(int i=0;i<16;i++)
                {
                    Require(demo.Model.Left+demo.Model.Right==15&&demo.Model.Step==i,"Quantidade inválida");
                    demo.StartSum();Require(demo.Gathered,"Soma não concluída");
                    if(mode==2)Require(GameObject.Find("One cube per unit").GetComponentsInChildren<MeshRenderer>().Length==15,"Quantidade de blocos inválida");
                    demo.Separate();Require(!demo.Gathered,"Separação falhou");demo.Move(1);
                }
                Require(demo.Model.Step==15,"Limite superior");
                for(int i=0;i<16;i++)demo.Move(-1);
                Require(demo.Model.Step==0,"Limite inferior");
                demo.TogglePause();demo.Move(1);Require(demo.Model.Step==0,"Avançou durante pausa");demo.TogglePause();
            }
        }
        [MenuItem("HXR/5 - Atualizar imagens sem Play")]
        public static void CaptureInEditor()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            demo=new GameObject("Preview HXR").AddComponent<MathExperience>();
            demo.controls=AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>("Assets/HXRControls.inputactions");
            demo.Initialize();demo.ToggleMotion();demo.Model.Move(7);demo.Center();
            demo.SelectMode(1);demo.StartSum();Capture("A-numeros.png");
            demo.SelectMode(2);Capture("B-grupos.png");demo.StartSum();Capture("B-soma.png");
            ValidateStates();
            Debug.Log("HXR_PREVIEW_OK: imagens e estados atualizados sem recarga do modo Play.");
            EditorSceneManager.OpenScene("Assets/Scenes/Inicio.unity");
        }
        static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases();
            var camera=Camera.main;var rt=new RenderTexture(1600,1200,24);
            camera.targetTexture=rt;camera.Render();var old=RenderTexture.active;RenderTexture.active=rt;
            var tex=new Texture2D(1600,1200,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1600,1200),0,0);tex.Apply();
            Directory.CreateDirectory("../Previews");File.WriteAllBytes("../Previews/"+name,tex.EncodeToPNG());
            camera.targetTexture=null;RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);
        }
    }
}
