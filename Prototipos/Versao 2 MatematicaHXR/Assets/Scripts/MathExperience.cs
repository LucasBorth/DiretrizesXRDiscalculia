using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using UnityEngine.XR;

namespace MatematicaHXR
{
    public sealed class MathExperience : MonoBehaviour
    {
        [Tooltip("0 = selection, 1 = numerical, 2 = concrete")]
        public int initialMode;
        public InputActionAsset controls;
        public SumModel Model { get; } = new SumModel();
        public bool Busy { get; private set; }
        public bool Gathered { get; private set; }
        public bool Paused { get; private set; }
        public bool Animate { get; private set; } = true;
        public int Mode { get; private set; }
        Camera eye;
        Transform board, cubeRoot;
        GameObject menu, activity;
        Text equation, instruction, progress, leftText, rightText, resultText, previous;
        WorldButton back, next, sum, repeat, pause, motion;
        readonly List<WorldButton> buttons = new List<WorldButton>();
        readonly List<GameObject> blocks = new List<GameObject>();
        readonly Color ink = new Color(.93f,.95f,.98f);
        readonly Color blue = new Color(.18f,.64f,.95f);
        readonly Color amber = new Color(1f,.67f,.22f);
        Material blueMat, amberMat;
        Font font;
        InputActionMap input;
        InputAction headPos, headRot;
        InputAction[] handPos, handRot, trigger, tracking;
        LineRenderer[] rays;
        WorldButton[] focused = new WorldButton[2];
        bool anchored;
        float anchorElapsed;

        void Start() => Initialize();
        public void Initialize()
        {
            if (board != null) return;
            Application.targetFrameRate = 72;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            eye = new GameObject("XR Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            eye.tag = "MainCamera";
            eye.clearFlags = CameraClearFlags.SolidColor;
            eye.backgroundColor = new Color(.025f,.04f,.065f);
            eye.nearClipPlane = .05f;
            eye.farClipPlane = 30;
            eye.fieldOfView = 65;
            SetupInput();
            board = new GameObject("Stable workspace").transform;
            board.position = new Vector3(0, 0, 2.6f);
            menu = new GameObject("Condition selection"); menu.transform.SetParent(board,false);
            activity = new GameObject("Exercise"); activity.transform.SetParent(board,false);
            TextAt(menu.transform,"A soma continua igual",0,.75f,2.8f,.22f,70);
            TextAt(menu.transform,"Escolha uma apresentação",0,.42f,2.5f,.14f,38);
            Button(menu.transform,"A · Somente números",0,.08f,1.9f,()=>SelectMode(1));
            Button(menu.transform,"B · Números e blocos",0,-.22f,1.9f,()=>SelectMode(2));
            TextAt(menu.transform,"Aponte para um botão e aperte o gatilho.\nNo Mac, clique com o mouse.",0,-.64f,2.6f,.24f,31);
            progress = TextAt(activity.transform,"",0,1.05f,2.8f,.10f,28);
            equation = TextAt(activity.transform,"",0,.77f,2.8f,.24f,78);
            previous = TextAt(activity.transform,"",0,.54f,2.7f,.11f,28);
            instruction = TextAt(activity.transform,"",0,.36f,2.9f,.16f,32);
            cubeRoot = new GameObject("One cube per unit").transform;
            cubeRoot.SetParent(activity.transform,false);
            leftText = TextAt(cubeRoot,"",-.72f,.17f,1.25f,.13f,37);
            rightText = TextAt(cubeRoot,"",.72f,.17f,1.25f,.13f,37);
            resultText = TextAt(cubeRoot,"",0,-.62f,2.8f,.12f,35);
            blueMat = new Material(Resources.Load<Shader>("HXRUnlit")); blueMat.color = blue;
            amberMat = new Material(Resources.Load<Shader>("HXRUnlit")); amberMat.color = amber;
            for(int i=0;i<SumModel.Total;i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Unit " + (i+1); cube.transform.SetParent(cubeRoot,false);
                cube.transform.localScale = Vector3.one * .065f;
                cube.transform.localRotation = Quaternion.Euler(8,12,0);
                if(Application.isPlaying) Destroy(cube.GetComponent<Collider>()); else DestroyImmediate(cube.GetComponent<Collider>());
                blocks.Add(cube);
            }
            back = Button(activity.transform,"Voltar",-.99f,-.83f,.60f,()=>Move(-1));
            sum = Button(activity.transform,"Somar",0,-.83f,1.18f,()=>StartSum());
            next = Button(activity.transform,"Próximo",.99f,-.83f,.60f,()=>Move(1));
            repeat = Button(activity.transform,"Separar / repetir",-.82f,-1.09f,1.04f,()=>Separate());
            Button(activity.transform,"Reiniciar",.22f,-1.09f,.82f,()=>Restart());
            Button(activity.transform,"Menu",1.01f,-1.09f,.62f,()=>ShowMenu());
            pause = Button(board,"Pausar",-1.01f,-1.37f,.64f,()=>TogglePause());
            motion = Button(board,"Movimento: sim",-.02f,-1.37f,1.20f,()=>ToggleMotion());
            Button(board,"Centralizar",1.02f,-1.37f,.78f,()=>Center());
            if(initialMode==0) ShowMenu(); else SelectMode(initialMode);
        }
        void SetupInput()
        {
            input = controls.FindActionMap("HXR", true);
            headPos=input.FindAction("Head position",true);
            headRot=input.FindAction("Head rotation",true);
            var driver=eye.gameObject.AddComponent<TrackedPoseDriver>();
            driver.positionInput=new InputActionProperty(headPos); driver.rotationInput=new InputActionProperty(headRot);
            handPos=new InputAction[2]; handRot=new InputAction[2]; trigger=new InputAction[2]; tracking=new InputAction[2]; rays=new LineRenderer[2];
            for(int i=0;i<2;i++)
            {
                string hand=i==0?"LeftHand":"RightHand";
                handPos[i]=input.FindAction(hand+" position",true);
                handRot[i]=input.FindAction(hand+" rotation",true);
                trigger[i]=input.FindAction(hand+" select",true);
                tracking[i]=input.FindAction(hand+" tracked",true);
                var line=new GameObject(hand+" pointer").AddComponent<LineRenderer>();
                line.material=new Material(Resources.Load<Shader>("HXRUnlit")); line.material.color=new Color(.65f,.85f,1);
                line.positionCount=2; line.startWidth=.004f; line.endWidth=.002f; line.enabled=false; rays[i]=line;
            }
            input.Enable();
        }
        Text TextAt(Transform parent,string value,float x,float y,float width,float height,int size)
        {
            var go=new GameObject("Text",typeof(RectTransform),typeof(Canvas));
            go.transform.SetParent(parent,false); go.transform.localPosition=new Vector3(x,y,-.10f);
            go.transform.localScale=Vector3.one*.002f;
            var canvas=go.GetComponent<Canvas>(); canvas.renderMode=RenderMode.WorldSpace;
            var rect=go.GetComponent<RectTransform>(); rect.sizeDelta=new Vector2(width/.002f,height/.002f);
            var child=new GameObject("Label",typeof(RectTransform),typeof(Text)); child.transform.SetParent(go.transform,false);
            var cr=child.GetComponent<RectTransform>(); cr.anchorMin=Vector2.zero;cr.anchorMax=Vector2.one;cr.sizeDelta=Vector2.zero;
            var text=child.GetComponent<Text>();text.font=font;text.fontSize=size;text.alignment=TextAnchor.MiddleCenter;text.color=ink;
            text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;text.raycastTarget=false;text.text=value;
            return text;
        }
        WorldButton Button(Transform parent,string label,float x,float y,float width,System.Action action)
        {
            var text=TextAt(parent,label,x,y,width,.20f,30);
            var root=text.transform.parent.gameObject;
            var bg=new GameObject("Background",typeof(RectTransform),typeof(Image));bg.transform.SetParent(root.transform,false);bg.transform.SetAsFirstSibling();
            var r=bg.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.sizeDelta=Vector2.zero;
            var collider=root.AddComponent<BoxCollider>();collider.size=new Vector3(width/.002f,100,15);
            var b=root.AddComponent<WorldButton>();b.Label=text;b.Background=bg.GetComponent<Image>();b.Action=action;b.Paint(false);buttons.Add(b);return b;
        }
        public void SelectMode(int mode)
        {
            CancelAnimation();Mode=mode;Gathered=false;Paused=false;
            menu.SetActive(false);activity.SetActive(true);cubeRoot.gameObject.SetActive(mode==2);
            LayoutBlocks();Refresh();
        }
        public void ShowMenu()
        {
            CancelAnimation();Mode=0;Paused=false;menu.SetActive(true);activity.SetActive(false);RefreshButtons();
        }
        void CancelAnimation(){StopAllCoroutines();Busy=false;}
        public void Restart(){CancelAnimation();Model.Reset();Gathered=false;Paused=false;LayoutBlocks();Refresh();}
        public void Separate(){if(Busy||Paused)return;Gathered=false;LayoutBlocks();Refresh();}
        public void ToggleMotion(){Animate=!Animate;RefreshButtons();}
        public void TogglePause(){Paused=!Paused;RefreshButtons();}
        public void Move(int delta)
        {
            if(Busy||Paused)return;
            if((delta<0&&Model.Step==0)||(delta>0&&Model.Step==15))return;
            Gathered=false;LayoutBlocks();
            var starts=blocks.ConvertAll(b=>b.transform.localPosition);
            Model.Move(delta);Refresh();
            if(Mode==2&&Animate)StartCoroutine(Transition(starts,false));else LayoutBlocks();
        }
        public void StartSum()
        {
            if(Busy||Paused||Gathered)return;
            if(Mode==2&&Animate)StartCoroutine(Transition(blocks.ConvertAll(b=>b.transform.localPosition),true));
            else{Gathered=true;LayoutBlocks();Refresh();}
        }
        Vector3 Destination(int i,bool together)
        {
            if(together)return new Vector3((i%5-2)*.12f,-.28f-(i/5)*.12f,-.15f);
            bool a=i<Model.Left;int n=a?i:i-Model.Left;
            return new Vector3((a?-.72f:.72f)+(n%5-2)*.12f,-.01f-(n/5)*.12f,-.15f);
        }
        void LayoutBlocks()
        {
            for(int i=0;i<blocks.Count;i++)
            {blocks[i].transform.localPosition=Destination(i,Gathered);blocks[i].GetComponent<Renderer>().sharedMaterial=i<Model.Left?blueMat:amberMat;}
        }
        IEnumerator Transition(List<Vector3> starts,bool gather)
        {
            Busy=true;RefreshButtons();
            instruction.text=gather?"Os dois grupos formam um só conjunto.":"Uma unidade passa de um grupo para o outro.";
            float t=0;
            while(t<1)
            {
                if(!Paused)
                {
                    t=Animate?Mathf.Min(1,t+Time.deltaTime/1.5f):1;
                    float smooth=t*t*(3-2*t);
                    for(int i=0;i<blocks.Count;i++)blocks[i].transform.localPosition=Vector3.Lerp(starts[i],Destination(i,gather),smooth);
                }
                yield return null;
            }
            Gathered=gather;Busy=false;LayoutBlocks();Refresh();
        }
        void Refresh()
        {
            progress.text=$"{(Mode==1?"A · SOMENTE NÚMEROS":"B · NÚMEROS E BLOCOS")}     |     {Model.Step+1} de 16";
            equation.text=Gathered?Model.Equation:$"{Model.Left} + {Model.Right} = ?";
            previous.text=Model.Step==0?"":$"Antes: {Model.Left+1} + {Model.Right-1} = 15";
            leftText.text=$"Grupo A: {Model.Left}";rightText.text=$"Grupo B: {Model.Right}";
            resultText.text=Gathered?"Total: 15 blocos":"Cada bloco vale 1.";
            instruction.text=Gathered?"A soma continua igual a 15.":Mode==1?"Observe os números e selecione Somar.":"Observe os dois grupos. Selecione Somar para juntar.";
            RefreshButtons();
        }
        void RefreshButtons()
        {
            if(pause==null)return;
            pause.Label.text=Paused?"Continuar":"Pausar";motion.Label.text=Animate?"Movimento: sim":"Movimento: não";
            back.Available=!Busy&&!Paused&&Model.Step>0;next.Available=!Busy&&!Paused&&Model.Step<15;
            sum.Available=!Busy&&!Paused&&!Gathered;repeat.Available=!Busy&&!Paused;
            sum.Label.text=Gathered?"Total: 15":"Somar";
            foreach(var b in buttons)b.Paint(false);
        }
        public void Center()
        {
            Vector3 forward=Vector3.ProjectOnPlane(eye.transform.forward,Vector3.up).normalized;
            if(forward.sqrMagnitude<.1f)forward=Vector3.forward;
            board.position=eye.transform.position+forward*2.6f+Vector3.up*.12f;
            board.rotation=Quaternion.LookRotation(forward,Vector3.up);
        }
        void Update()
        {
            if(eye==null)return;
            if(!anchored)
            {
                anchorElapsed+=Time.deltaTime;
                if(XRSettings.isDeviceActive||anchorElapsed>1){Center();anchored=true;}
            }
            foreach(var b in buttons)if(b.gameObject.activeInHierarchy)b.Paint(false);
            if(!XRSettings.isDeviceActive)
            {
                if(Mouse.current!=null)Point(eye.ScreenPointToRay(Mouse.current.position.ReadValue()),Mouse.current.leftButton.wasPressedThisFrame,-1);
                if(Keyboard.current!=null&&Keyboard.current.rKey.wasPressedThisFrame)Center();
            }
            for(int i=0;i<2;i++)
            {
                bool active=tracking[i].IsPressed();rays[i].enabled=active;
                if(!active)continue;
                Quaternion rotation=handRot[i].ReadValue<Quaternion>();
                if(rotation==default)continue;
                Point(new Ray(handPos[i].ReadValue<Vector3>(),rotation*Vector3.forward),trigger[i].WasPressedThisFrame(),i);
            }
        }
        void Point(Ray ray,bool click,int hand)
        {
            Vector3 end=ray.GetPoint(4);
            if(Physics.Raycast(ray,out var hit,8))
            {
                end=hit.point;
                var b=hit.collider.GetComponent<WorldButton>();
                if(b!=null){b.Paint(true);if(click)b.Invoke();}
            }
            if(hand>=0){rays[hand].SetPosition(0,ray.origin);rays[hand].SetPosition(1,end);}
        }
        void OnDestroy(){input?.Disable();if(Application.isPlaying){if(blueMat)Destroy(blueMat);if(amberMat)Destroy(amberMat);}else{if(blueMat)DestroyImmediate(blueMat);if(amberMat)DestroyImmediate(amberMat);}}
    }
}
