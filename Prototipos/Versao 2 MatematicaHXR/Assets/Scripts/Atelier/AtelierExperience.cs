using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Input;

namespace MatematicaHXR.Atelier
{
    public sealed class AtelierExperience : MonoBehaviour
    {
        public InputActionAsset controls;
        public readonly AtelierModel Model = new AtelierModel();
        public bool Paused { get; private set; }
        public bool Ready => world != null;
        public int FrameTicks { get; private set; }
        public Camera Eye { get; private set; }
        public IReadOnlyList<AtelierBlock> Blocks => blocks;
        public bool AudioEnabled => soundLevel>0;
        Transform world, decor, table, board;
        Text title, prompt, equation, countA, countB, countTotal, subtitle, helpText;
        WorldButton primary, repeat, soundButton, hapticButton, calmButton, pauseButton;
        readonly List<WorldButton> buttons=new List<WorldButton>();
        readonly List<AtelierBlock> blocks=new List<AtelierBlock>();
        readonly List<Material> materials=new List<Material>();
        readonly List<Transform> pulses=new List<Transform>();
        readonly List<float> pulseTimes=new List<float>();
        readonly Transform[] palms=new Transform[2];
        readonly LineRenderer[] rays=new LineRenderer[2];
        readonly AtelierBlock[] held=new AtelierBlock[2];
        readonly bool[] remote=new bool[2];
        readonly float[] holdDistance=new float[2];
        readonly Quaternion[] holdRotation=new Quaternion[2];
        readonly Vector3[] holdOffset=new Vector3[2];
        readonly InputAction[] pos=new InputAction[2], rot=new InputAction[2], aimPos=new InputAction[2], aimRot=new InputAction[2], grip=new InputAction[2], select=new InputAction[2], tracked=new InputAction[2], buzz=new InputAction[2];
        InputActionMap input;
        Font font;
        Material blue, amber, cream, ink, mint, wood, darkwood, teal, coral, paper, white;
        AudioSource audioSource;
        AudioClip pickClip, dropClip, successClip, clickClip;
        float soundLevel=.45f, hapticLevel=.35f, elapsed, lastFeedback;
        bool calm, anchored, help, largeText;
        float tableAdjustment;
        AtelierBlock mouseBlock;
        Vector3 mouseOffset;
        int lastHand=-1;
        readonly Color textColor=new Color(.94f,.97f,.96f);
        const float Top=.74f, BlockSize=.061f;
        static readonly Vector3[] Zones={new Vector3(-.43f,Top,.76f),new Vector3(.43f,Top,.76f),new Vector3(0,Top,.33f)};
        void Start()=>Initialize();
        public void Initialize()
        {
            if(Ready)return;
            Application.targetFrameRate=72; Application.runInBackground=true;
            soundLevel=PlayerPrefs.GetFloat("atelier.sound",.45f);hapticLevel=PlayerPrefs.GetFloat("atelier.haptic",.35f);
            calm=PlayerPrefs.GetInt("atelier.calm",0)==1;largeText=PlayerPrefs.GetInt("atelier.large",0)==1;
            font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            MakeMaterials();
            world=new GameObject("Ateliê • sala estável").transform;world.SetParent(transform,false);
            Eye=new GameObject("XR Camera",typeof(Camera),typeof(AudioListener)).GetComponent<Camera>();Eye.transform.SetParent(transform,false);
            Eye.tag="MainCamera";Eye.nearClipPlane=.025f;Eye.farClipPlane=25;Eye.fieldOfView=70;Eye.backgroundColor=new Color(.78f,.86f,.85f);Eye.clearFlags=CameraClearFlags.SolidColor;
            if(!XRSettings.isDeviceActive)Eye.transform.localPosition=new Vector3(0,1.25f,-.08f);
            SetupInput(); MakeRoom(); MakeTable(); MakeBoard();MakeBlocks();MakeAudio();
            SetCalm(calm);Center();Refresh();
        }
        Material Mat(string name,Color color)
        {var m=new Material(Resources.Load<Shader>("AtelierLit"));m.name=name;m.color=color;materials.Add(m);return m;}
        void MakeMaterials()
        {
            blue=Mat("Azul · grupo A",new Color(.13f,.48f,.78f));amber=Mat("Âmbar · grupo B",new Color(.98f,.59f,.12f));
            cream=Mat("Parede creme",new Color(.92f,.89f,.79f));ink=Mat("Petróleo",new Color(.055f,.14f,.17f));
            mint=Mat("Verde suave",new Color(.53f,.76f,.64f));wood=Mat("Madeira clara",new Color(.72f,.49f,.29f));darkwood=Mat("Madeira escura",new Color(.32f,.21f,.14f));
            teal=Mat("Verde quadro",new Color(.10f,.31f,.31f));coral=Mat("Coral",new Color(.86f,.37f,.28f));
            paper=Mat("Marfim",new Color(.98f,.95f,.84f));white=Mat("Branco",Color.white);
        }
        GameObject Shape(string name,PrimitiveType type,Transform parent,Vector3 p,Vector3 scale,Material mat,bool collider=false)
        {
            var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=p;g.transform.localScale=scale;
            g.GetComponent<Renderer>().sharedMaterial=mat;
            if(!collider){var c=g.GetComponent<Collider>();c.enabled=false;if(Application.isPlaying)Destroy(c);else DestroyImmediate(c);}
            return g;
        }
        void Box(string name,Transform parent,Vector3 p,Vector3 size,Material mat,bool collider=false)=>Shape(name,PrimitiveType.Cube,parent,p,size,mat,collider);
        void MakeRoom()
        {
            Box("Piso",world,new Vector3(0,-.04f,1.4f),new Vector3(7,.08f,7),cream,true);
            Box("Parede do fundo",world,new Vector3(0,1.8f,4.8f),new Vector3(7,3.6f,.12f),cream);
            Box("Parede esquerda",world,new Vector3(-3.5f,1.8f,1.3f),new Vector3(.12f,3.6f,7),mint);
            Box("Parede direita",world,new Vector3(3.5f,1.8f,1.3f),new Vector3(.12f,3.6f,7),cream);
            Box("Rodapé",world,new Vector3(0,.12f,4.69f),new Vector3(7,.24f,.09f),teal);
            Box("Tapete da atividade",world,new Vector3(0,.007f,.6f),new Vector3(2.5f,.015f,2.2f),teal);
            for(int k=0;k<5;k++)Box("Faixa no tapete",world,new Vector3(-1.15f+k*.58f,.018f,-.35f),new Vector3(.26f,.006f,.04f),mint);
            decor=new GameObject("Decoração reduzível").transform;decor.SetParent(world,false);
            // Static geometric scenery avoids extra textures, shadows and distracting motion.
            Box("Janela · moldura",decor,new Vector3(-1.95f,2.15f,4.68f),new Vector3(1.8f,1.45f,.10f),wood);
            Box("Janela · céu",decor,new Vector3(-1.95f,2.15f,4.60f),new Vector3(1.64f,1.30f,.05f),Mat("Céu",new Color(.58f,.82f,.88f)));
            Shape("Sol",PrimitiveType.Sphere,decor,new Vector3(-2.35f,2.48f,4.55f),Vector3.one*.29f,amber);
            Box("Travessa vertical",decor,new Vector3(-1.95f,2.15f,4.52f),new Vector3(.06f,1.34f,.06f),paper);
            Box("Travessa horizontal",decor,new Vector3(-1.95f,2.15f,4.52f),new Vector3(1.7f,.06f,.06f),paper);
            Box("Painel da sala",decor,new Vector3(.55f,2.1f,4.65f),new Vector3(2.7f,1.4f,.09f),teal);
            Label(decor,"PEQUENAS PEÇAS\nGRANDES DESCOBERTAS",new Vector3(.55f,2.22f,4.57f),2.4f,.5f,48,textColor);
            Label(decor,"ATELIÊ  /  MATEMÁTICA",new Vector3(.55f,1.83f,4.57f),2.4f,.13f,25,textColor);
            for(int j=0;j<3;j++)
            {
                Box("Prateleira",decor,new Vector3(2.7f,.6f+j*.47f,3.9f),new Vector3(.9f,.055f,.45f),wood);
                for(int k=0;k<4;k++)Box("Livro",decor,new Vector3(2.4f+k*.15f,.77f+j*.47f,3.9f),new Vector3(.095f,.29f,.23f),(k%2==0)?coral:mint);
            }
            for(int side=-1;side<=1;side+=2)
            {
                Shape("Vaso",PrimitiveType.Cylinder,decor,new Vector3(side*2.5f,.26f,2.6f),new Vector3(.43f,.26f,.43f),coral);
                Box("Caule",decor,new Vector3(side*2.5f,.78f,2.6f),new Vector3(.045f,.8f,.045f),darkwood);
                for(int k=0;k<4;k++)Shape("Folha",PrimitiveType.Sphere,decor,new Vector3(side*2.5f+(k%2==0?.16f:-.16f),.6f+k*.18f,2.6f),new Vector3(.5f,.16f,.23f),mint);
            }
            // A stationary low-poly pendant and side seating establish a classroom scale.
            Shape("Luminária",PrimitiveType.Cylinder,decor,new Vector3(0,3.15f,1.6f),new Vector3(.75f,.12f,.75f),paper);
            for(int s=-1;s<=1;s+=2)
            {
                Box("Banco",decor,new Vector3(s*2.1f,.44f,.4f),new Vector3(.7f,.10f,.6f),coral);
                Box("Pé do banco",decor,new Vector3(s*2.1f,.22f,.4f),new Vector3(.5f,.44f,.4f),wood);
            }
        }
        void MakeTable()
        {
            table=new GameObject("Mesa ao alcance").transform;table.SetParent(world,false);
            Box("Tampo",table,new Vector3(0,Top-.045f,.65f),new Vector3(1.7f,.09f,1.15f),wood,true);
            Box("Superfície",table,new Vector3(0,Top-.004f,.65f),new Vector3(1.64f,.012f,1.09f),paper);
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Box("Pé",table,new Vector3(x*.70f,Top/2-.05f,.65f+z*.43f),new Vector3(.075f,Top-.1f,.075f),darkwood);
            for(int i=0;i<3;i++)
            {
                var p=Zones[i];Box("Bandeja "+i,table,p+Vector3.down*.001f,new Vector3(.50f,.018f,.31f),i==0?blue:i==1?amber:teal);
                // Fifteen visible cells make cardinality readable without relying on color.
                for(int n=0;n<15;n++)Box("Casa de unidade",table,Slot(i,n)+Vector3.down*.031f,new Vector3(.065f,.003f,.065f),i==2?mint:paper);
            }
            countA=Label(table,"",new Vector3(-.43f,Top+.04f,1.02f),.55f,.14f,32,ink.color);countA.transform.parent.localRotation=Quaternion.Euler(25,0,0);
            countB=Label(table,"",new Vector3(.43f,Top+.04f,1.02f),.55f,.14f,32,ink.color);countB.transform.parent.localRotation=Quaternion.Euler(25,0,0);
            countTotal=Label(table,"",new Vector3(0,Top+.04f,.10f),.6f,.12f,29,ink.color);countTotal.transform.parent.localRotation=Quaternion.Euler(55,0,0);
        }
        void MakeBoard()
        {
            board=new GameObject("Instruções estáveis").transform;board.SetParent(world,false);board.localPosition=new Vector3(0,1.34f,1.54f);
            Box("Moldura",board,Vector3.zero,new Vector3(1.65f,.67f,.06f),wood);
            Box("Quadro",board,new Vector3(0,0,-.035f),new Vector3(1.59f,.61f,.02f),ink);
            title=Label(board,"",new Vector3(0,.23f,-.055f),1.5f,.07f,23,new Color(.59f,.86f,.75f));
            equation=Label(board,"",new Vector3(0,.08f,-.055f),1.45f,.18f,57,textColor);
            prompt=Label(board,"",new Vector3(0,-.10f,-.055f),1.48f,.13f,27,textColor);
            subtitle=Label(board,"",new Vector3(0,-.23f,-.055f),1.45f,.09f,21,textColor);
            primary=Button(world,"Preparar grupos",new Vector3(0,1.02f,1.20f),.52f,Primary);
            repeat=Button(world,"Organizar",new Vector3(-.58f,1.02f,1.20f),.45f,Organize);
            Button(world,"Ajuda",new Vector3(.58f,1.02f,1.20f),.45f,()=>{help=!help;Refresh();});
            helpText=Label(world,"",new Vector3(0,1.87f,1.52f),1.72f,.30f,26,ink.color);
            var options=new GameObject("Controles de conforto").transform;options.SetParent(world,false);options.localPosition=new Vector3(-1.05f,.97f,.7f);options.localRotation=Quaternion.Euler(0,-28,0);
            Box("Painel lateral",options,new Vector3(0,-.12f,.04f),new Vector3(.49f,1.0f,.055f),ink);
            Label(options,"SEU RITMO",new Vector3(0,.29f,-.01f),.44f,.075f,23,textColor);
            pauseButton=Button(options,"Pausar",new Vector3(0,.18f,-.01f),.42f,TogglePause);
            soundButton=Button(options,"",new Vector3(0,.065f,-.01f),.42f,()=>{soundLevel=soundLevel==0?.2f:soundLevel<.3f?.45f:0;SavePreferences();Refresh();});
            hapticButton=Button(options,"",new Vector3(0,-.05f,-.01f),.42f,()=>{hapticLevel=hapticLevel==0?.15f:hapticLevel<.2f?.35f:0;SavePreferences();Refresh();});
            calmButton=Button(options,"",new Vector3(0,-.165f,-.01f),.42f,()=>{SetCalm(!calm);SavePreferences();Refresh();});
            Button(options,"Centralizar",new Vector3(0,-.28f,-.01f),.42f,Center);
            Button(options,"Texto + / −",new Vector3(0,-.395f,-.01f),.42f,()=>{largeText=!largeText;SavePreferences();Refresh();});
            Button(options,"Mesa + / −",new Vector3(0,-.51f,-.01f),.42f,()=>{tableAdjustment=tableAdjustment<.05f?.10f:tableAdjustment>.05f?-.10f:0;table.localPosition=Vector3.up*tableAdjustment;Organize();});
        }
        Text Label(Transform parent,string value,Vector3 p,float width,float height,int size,Color color)
        {
            var root=new GameObject(value.Length<25?value:"Texto",typeof(RectTransform),typeof(Canvas));root.transform.SetParent(parent,false);root.transform.localPosition=p;root.transform.localScale=Vector3.one*.001f;
            root.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;root.GetComponent<RectTransform>().sizeDelta=new Vector2(width*1000,height*1000);
            var child=new GameObject("Rótulo",typeof(RectTransform),typeof(Text));child.transform.SetParent(root.transform,false);
            var r=child.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.sizeDelta=Vector2.zero;
            var t=child.GetComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.color=color;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;
            return t;
        }
        WorldButton Button(Transform p,string name,Vector3 at,float width,Action action)
        {
            var text=Label(p,name,at,width,.10f,24,textColor);var root=text.transform.parent.gameObject;
            var bg=new GameObject("Fundo",typeof(RectTransform),typeof(Image));bg.transform.SetParent(root.transform,false);bg.transform.SetAsFirstSibling();var rt=bg.GetComponent<RectTransform>();rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.sizeDelta=Vector2.zero;
            var c=root.AddComponent<BoxCollider>();c.size=new Vector3(width*1000,100,24);
            var b=root.AddComponent<WorldButton>();b.Label=text;b.Background=bg.GetComponent<Image>();b.Action=()=>{Play(clickClip,world.position,.25f);action();};buttons.Add(b);return b;
        }
        public Vector3 Slot(int zone,int n)=>Zones[zone]+new Vector3((n%5-2)*.083f,.043f,(n/5-1)*.083f);
        public Vector3 Destination(int id)
        {
            int zone=Model.Collected[id]?2:Model.Group[id],n=0;
            for(int j=0;j<id;j++)if((Model.Collected[j]?2:Model.Group[j])==zone)n++;
            return table.TransformPoint(Slot(zone,n));
        }
        void MakeBlocks()
        {
            for(int n=0;n<15;n++)
            {
                var g=Shape("Unidade "+(n+1),PrimitiveType.Cube,table,Slot(n<8?0:1,n<8?n:n-8),Vector3.one*BlockSize,n<8?blue:amber,true);
                var b=g.AddComponent<AtelierBlock>();b.Id=n;b.Owner=this;b.Shell=g.GetComponent<Renderer>();b.LastSafe=g.transform.position;
                b.Body=g.AddComponent<Rigidbody>();b.Body.mass=.08f;b.Body.linearDamping=.8f;b.Body.angularDamping=1.8f;b.Body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;b.Body.interpolation=RigidbodyInterpolation.Interpolate;
                // Each physical piece bears one pip on top and the numeral 1 on its front.
                Shape("Um ponto",PrimitiveType.Cylinder,g.transform,new Vector3(0,.506f,0),new Vector3(.30f,.014f,.30f),paper);
                var one=Label(g.transform,n<8?"1\nA":"1\nB",new Vector3(0,0,-.506f),.9f,.9f,360,Color.white);one.transform.parent.localScale=Vector3.one*.001f;
                b.Mark=one;blocks.Add(b);
            }
        }
        void MakeAudio()
        {
            var audioObject=new GameObject("Feedback sonoro");audioObject.transform.SetParent(transform,false);audioSource=audioObject.AddComponent<AudioSource>();audioSource.spatialBlend=.55f;audioSource.rolloffMode=AudioRolloffMode.Linear;audioSource.minDistance=.3f;audioSource.maxDistance=4;audioSource.playOnAwake=false;
            pickClip=Tone("Pegar",new[]{523f,659f},.11f);dropClip=Tone("Encaixar",new[]{392f,523f},.14f);
            successClip=Tone("Conjunto completo",new[]{523f,659f,784f},.48f);clickClip=Tone("Confirmar",new[]{440f},.055f);
        }
        AudioClip Tone(string name,float[] notes,float duration)
        {
            int rate=22050;var data=new float[(int)(rate*duration)];
            for(int i=0;i<data.Length;i++){float t=(float)i/rate;float f=notes[Mathf.Min(notes.Length-1,(int)(t/duration*notes.Length))];float env=Mathf.Sin(Mathf.PI*t/duration);data[i]=.22f*env*env*(Mathf.Sin(2*Mathf.PI*f*t)+.22f*Mathf.Sin(4*Mathf.PI*f*t));}
            var clip=AudioClip.Create(name,data.Length,1,rate,false);clip.SetData(data,0);return clip;
        }
        void Play(AudioClip clip,Vector3 position,float volume=1)
        {if(!AudioEnabled||Paused||audioSource==null)return;audioSource.transform.position=position;audioSource.PlayOneShot(clip,soundLevel*volume);}
        public void Impact(Vector3 position,float intensity)
        {if(Time.time-lastFeedback<.08f)return;lastFeedback=Time.time;Play(dropClip,position,intensity*.5f);}
        void Pulse(Vector3 at)
        {
            if(calm||!Application.isPlaying)return;
            var g=new GameObject("Realce suave de encaixe");g.transform.SetParent(world,true);g.transform.position=at+Vector3.up*.075f;
            for(int i=0;i<4;i++){float a=i*Mathf.PI*.5f;Shape("Pétala",PrimitiveType.Sphere,g.transform,new Vector3(Mathf.Cos(a),0,Mathf.Sin(a)),new Vector3(.3f,.15f,.3f),mint);}
            g.transform.localScale=Vector3.one*.03f;pulses.Add(g.transform);pulseTimes.Add(0);
        }
        void Haptic(int hand,float scale=1)
        {if(hand<0||hapticLevel==0||Paused)return;OpenXRInput.SendHapticImpulse(buzz[hand],hapticLevel*scale,.055f);}
        void SetupInput()
        {
            input=controls.FindActionMap("HXR",true);
            var driver=Eye.gameObject.AddComponent<TrackedPoseDriver>();driver.positionInput=new InputActionProperty(input.FindAction("Head position"));driver.rotationInput=new InputActionProperty(input.FindAction("Head rotation"));
            for(int i=0;i<2;i++)
            {
                string h=i==0?"LeftHand":"RightHand";
                pos[i]=input.FindAction(h+" palm position",true);rot[i]=input.FindAction(h+" palm rotation",true);aimPos[i]=input.FindAction(h+" position",true);aimRot[i]=input.FindAction(h+" rotation",true);
                grip[i]=input.FindAction(h+" grip",true);select[i]=input.FindAction(h+" select",true);tracked[i]=input.FindAction(h+" tracked",true);buzz[i]=input.FindAction(h+" haptic",true);
                palms[i]=Shape(h+" controle",PrimitiveType.Sphere,transform,Vector3.zero,new Vector3(.045f,.045f,.065f),i==0?blue:amber).transform;
                rays[i]=new GameObject(h+" raio").AddComponent<LineRenderer>();rays[i].transform.SetParent(transform,false);rays[i].sharedMaterial=white;rays[i].startWidth=.002f;rays[i].endWidth=.001f;rays[i].positionCount=2;rays[i].enabled=false;
            }
            input.Enable();
        }
        public void Center()
        {
            if(Eye==null)return;ReleaseAll();
            var forward=Vector3.ProjectOnPlane(Eye.transform.forward,Vector3.up).normalized;if(forward.sqrMagnitude<.1f)forward=Vector3.forward;
            world.SetPositionAndRotation(Eye.transform.position-Vector3.up*1.25f,Quaternion.LookRotation(forward));
            foreach(var b in blocks)b.LastSafe=b.transform.position;
        }
        void Update()
        {
            if(!Ready)return;FrameTicks++;
            if(!Paused)for(int j=pulses.Count-1;j>=0;j--){pulseTimes[j]+=Time.deltaTime;float t=pulseTimes[j]/.55f;if(t>=1){Destroy(pulses[j].gameObject);pulses.RemoveAt(j);pulseTimes.RemoveAt(j);}else pulses[j].localScale=Vector3.one*(.03f+.05f*Mathf.Sin(t*Mathf.PI));}
            if(!anchored){elapsed+=Time.deltaTime;if((XRSettings.isDeviceActive&&elapsed>.7f)||elapsed>1.2f){Center();anchored=true;}}
            foreach(var b in buttons)b.Paint(false);
            for(int h=0;h<2;h++)UpdateHand(h);
            if(!XRSettings.isDeviceActive)MouseInput();
            if(!Paused)foreach(var b in blocks)
            {
                if(b.HeldBy>=0)continue;
                var local=world.InverseTransformPoint(b.transform.position);
                if(local.y<.12f||Mathf.Abs(local.x)>2.2f||local.z>2.3f||local.z<-.6f)
                {ReturnBlock(b);prompt.text="O bloco voltou à mesa. Pode tentar de novo.";}
            }
        }
        void UpdateHand(int h)
        {
            bool active=tracked[h].IsPressed();palms[h].gameObject.SetActive(active);rays[h].enabled=active;
            if(!active){if(held[h]!=null)Drop(h);return;}
            var pr=rot[h].ReadValue<Quaternion>();if(pr==default)pr=Quaternion.identity;
            palms[h].SetPositionAndRotation(pos[h].ReadValue<Vector3>(),pr);
            var ar=aimRot[h].ReadValue<Quaternion>();if(ar==default)ar=pr;
            var ray=new Ray(aimPos[h].ReadValue<Vector3>(),ar*Vector3.forward);var end=ray.GetPoint(2.5f);
            WorldButton button=null;AtelierBlock hitBlock=null;
            if(Physics.Raycast(ray,out var hit,4)) {end=hit.point;button=hit.collider.GetComponent<WorldButton>();hitBlock=hit.collider.GetComponent<AtelierBlock>();if(button)button.Paint(true);}
            rays[h].SetPosition(0,ray.origin);rays[h].SetPosition(1,end);
            if(button!=null&&select[h].WasPressedThisFrame()&&held[h]==null){lastHand=h;button.Invoke();Haptic(h,.5f);return;}
            if(Paused)return;
            if(held[h]!=null)
            {
                var b=held[h];b.transform.position=remote[h]?ray.GetPoint(holdDistance[h]):palms[h].position+palms[h].rotation*holdOffset[h];b.transform.rotation=palms[h].rotation*holdRotation[h];
                if(remote[h]?select[h].WasPressedThisFrame():grip[h].WasReleasedThisFrame())Drop(h);
                return;
            }
            if(grip[h].WasPressedThisFrame())
            {
                AtelierBlock near=null;float distance=.14f;
                foreach(var b in blocks){float d=Vector3.Distance(b.transform.position,palms[h].position);if(b.HeldBy<0&&d<distance){distance=d;near=b;}}
                if(near!=null)Pick(near,h,false,ray);
                else if(hitBlock!=null)Pick(hitBlock,h,false,ray);
            }
            else if(select[h].WasPressedThisFrame()&&hitBlock!=null)Pick(hitBlock,h,true,ray);
        }
        void Pick(AtelierBlock b,int hand,bool distant,Ray ray)
        {
            if(b.HeldBy>=0||Paused||Model.Phase==AtelierModel.Stage.Result)return;
            held[hand]=b;b.HeldBy=hand;remote[hand]=distant;holdDistance[hand]=Mathf.Clamp(Vector3.Distance(ray.origin,b.transform.position),.20f,1.5f);
            holdOffset[hand]=Quaternion.Inverse(palms[hand].rotation)*(b.transform.position-palms[hand].position);
            if(!distant&&holdOffset[hand].magnitude>.16f)holdOffset[hand]=Vector3.forward*.07f;
            holdRotation[hand]=Quaternion.Inverse(palms[hand].rotation)*b.transform.rotation;
            b.Body.isKinematic=true;Play(pickClip,b.transform.position,.7f);Haptic(hand);lastHand=hand;
            prompt.text=distant?"Mova o bloco. Aperte o gatilho outra vez para soltar.":"Leve o bloco até a bandeja e solte o botão lateral.";
        }
        void Drop(int h)
        {
            var b=held[h];if(b==null)return;held[h]=null;b.HeldBy=-1;
            CompleteDrop(b,h);
        }
        public void CompleteDrop(AtelierBlock b,int hand=-1)
        {
            if(Model.Phase==AtelierModel.Stage.Explore)
            {b.Body.isKinematic=Paused;b.Body.linearVelocity=Vector3.zero;b.Body.angularVelocity=Vector3.zero;Model.Practice();Refresh();Play(dropClip,b.transform.position,.45f);return;}
            var p=table.InverseTransformPoint(b.transform.position);int zone=-1;
            for(int i=0;i<3;i++)if(Mathf.Abs(p.x-Zones[i].x)<.30f&&Mathf.Abs(p.z-Zones[i].z)<.20f&&p.y>Top-.1f&&p.y<Top+.55f){zone=i;break;}
            bool accepted=Model.Place(b.Id,zone);
            Arrange();Refresh();
            if(accepted){Pulse(b.transform.position);Play(Model.Phase==AtelierModel.Stage.Result?successClip:dropClip,b.transform.position);Haptic(hand);}
            else prompt.text=Model.TransferRequired?"Leve um bloco de A para B. O total será o mesmo.":Model.Phase==AtelierModel.Stage.Gather?"Solte na bandeja JUNTOS. O bloco voltou ao lugar.":"Solte dentro de A ou B. Pode tentar de novo.";
        }
        public void Primary()
        {
            if(Paused||AnyHeld())return;
            switch(Model.Phase)
            {
                case AtelierModel.Stage.Explore:Model.Begin();if(Model.Phase!=AtelierModel.Stage.Explore)Arrange();break;
                case AtelierModel.Stage.Split:Model.BeginGather();break;
                case AtelierModel.Stage.Gather:prompt.text="Leve os blocos até JUNTOS, um por vez.";return;
                case AtelierModel.Stage.Result:Model.NextPartition();Arrange();break;
            }
            Refresh();
        }
        public void Organize()
        {
            ReleaseAll();
            if(Model.Phase==AtelierModel.Stage.Explore)
            {foreach(var b in blocks){b.transform.localPosition=Slot(b.Id<8?0:1,b.Id<8?b.Id:b.Id-8);b.transform.localRotation=Quaternion.identity;b.Body.isKinematic=Paused;b.Body.linearVelocity=Vector3.zero;b.Body.angularVelocity=Vector3.zero;b.LastSafe=b.transform.position;}}
            else{if(Model.Phase==AtelierModel.Stage.Result)Model.Separate();Arrange();}
            Refresh();
        }
        void Arrange()
        {
            foreach(var b in blocks)
            {if(b.HeldBy>=0)continue;b.Body.isKinematic=true;b.transform.SetPositionAndRotation(Destination(b.Id),table.rotation);b.LastSafe=b.transform.position;b.Shell.sharedMaterial=Model.Group[b.Id]==0?blue:amber;b.Mark.text=Model.Group[b.Id]==0?"1\nA":"1\nB";}
        }
        void ReturnBlock(AtelierBlock b)
        {b.Body.isKinematic=true;b.transform.SetPositionAndRotation(Model.Phase==AtelierModel.Stage.Explore?table.TransformPoint(Slot(b.Id<8?0:1,b.Id<8?b.Id:b.Id-8)):Destination(b.Id),table.rotation);if(Model.Phase==AtelierModel.Stage.Explore&&!Paused)b.Body.isKinematic=false;}
        bool AnyHeld()=>held[0]!=null||held[1]!=null||mouseBlock!=null;
        void ReleaseAll(){for(int h=0;h<2;h++)if(held[h]!=null){var b=held[h];held[h]=null;b.HeldBy=-1;ReturnBlock(b);}if(mouseBlock!=null){mouseBlock.HeldBy=-1;ReturnBlock(mouseBlock);mouseBlock=null;}}
        public void TogglePause()
        {
            ReleaseAll();Paused=!Paused;
            foreach(var b in blocks)b.Body.isKinematic=Paused||Model.Phase!=AtelierModel.Stage.Explore;
            if(Paused)audioSource.Stop();Refresh();
        }
        void OnApplicationPause(bool value){if(value&&Ready&&!Paused)TogglePause();}
        void SetCalm(bool value){calm=value;if(decor)decor.gameObject.SetActive(!value);if(value){foreach(var p in pulses)if(p)Destroy(p.gameObject);pulses.Clear();pulseTimes.Clear();}}
        void SavePreferences(){PlayerPrefs.SetFloat("atelier.sound",soundLevel);PlayerPrefs.SetFloat("atelier.haptic",hapticLevel);PlayerPrefs.SetInt("atelier.calm",calm?1:0);PlayerPrefs.SetInt("atelier.large",largeText?1:0);PlayerPrefs.Save();}
        public void Refresh()
        {
            if(title==null)return;
            var stage=Model.Phase;
            title.text=stage==AtelierModel.Stage.Explore?"01  /  EXPLORE COM AS MÃOS":stage==AtelierModel.Stage.Split?"02  /  DOIS GRUPOS":stage==AtelierModel.Stage.Gather?"03  /  JUNTE AS UNIDADES":"04  /  A SOMA SE CONSERVA";
            equation.text=stage==AtelierModel.Stage.Explore?"Cada bloco vale 1":stage==AtelierModel.Stage.Result?$"{Model.Left} + {Model.Right} = 15":stage==AtelierModel.Stage.Gather?$"{Model.Left} + {Model.Right} = ?":"Organize os dois grupos";
            prompt.text=stage==AtelierModel.Stage.Explore?"Aproxime o controle. Segure o botão lateral e pegue um bloco.":Model.TransferRequired?"Passe 1 bloco de A para B.":stage==AtelierModel.Stage.Split?"Pode mudar os grupos. Depois, escolha Juntar.":stage==AtelierModel.Stage.Gather?"Leve os blocos à bandeja JUNTOS.":"Você juntou 15 unidades. A quantidade não mudou.";
            subtitle.text=Model.BeforeLeft>=0?$"Antes: {Model.BeforeLeft} + {15-Model.BeforeLeft} = 15":stage==AtelierModel.Stage.Explore?"Explore, empilhe e solte. Sem pressa.":"Sem tempo limite. Você escolhe quando continuar.";
            if(stage==AtelierModel.Stage.Split&&Model.BeforeLeft>=0&&!Model.TransferRequired)prompt.text="A perdeu 1. B ganhou 1. Vamos juntar de novo?";
            countA.text=stage==AtelierModel.Stage.Explore?"A · EXPLORE":$"A · {Model.Left}";countB.text=stage==AtelierModel.Stage.Explore?"B · EXPLORE":$"B · {Model.Right}";
            countTotal.text=stage==AtelierModel.Stage.Explore?"JUNTOS":$"JUNTOS · {Model.Joined} / 15";
            primary.Label.text=stage==AtelierModel.Stage.Explore?"Preparar grupos":stage==AtelierModel.Stage.Split?"Juntar":stage==AtelierModel.Stage.Gather?"Junte os blocos":Model.Left==0?"Recomeçar":"Outra divisão";
            primary.Available=!Paused&&!AnyHeld()&&(stage!=AtelierModel.Stage.Explore||Model.Practiced)&&!Model.TransferRequired&&stage!=AtelierModel.Stage.Gather;
            repeat.Available=!Paused;repeat.Label.text=stage==AtelierModel.Stage.Result?"Separar de novo":"Organizar";
            soundButton.Label.text=soundLevel==0?"Som: desligado":soundLevel<.3f?"Som: baixo":"Som: médio";
            hapticButton.Label.text=hapticLevel==0?"Vibração: não":hapticLevel<.2f?"Vibração: leve":"Vibração: média";
            calmButton.Label.text=calm?"Cenário: simples":"Cenário: completo";pauseButton.Label.text=Paused?"Continuar":"Pausar";
            helpText.text=help?"DE PERTO: segure o botão lateral para pegar.\nÀ DISTÂNCIA: aponte e toque o gatilho para pegar; toque de novo para soltar.\nSe precisar, pause e peça ajuda ao mediador.":"";
            prompt.fontSize=largeText?31:27;equation.fontSize=largeText?65:57;
            if(Paused){prompt.text="Pausa. Descanse ou peça ajuda ao mediador.";subtitle.text="Selecione Continuar quando estiver pronto.";}
            foreach(var b in buttons)b.Paint(false);
        }
        void MouseInput()
        {
            if(Mouse.current==null)return;var ray=Eye.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Keyboard.current!=null&&Keyboard.current.rKey.wasPressedThisFrame)Center();
            if(mouseBlock!=null)
            {
                var plane=new Plane(table.up,table.TransformPoint(new Vector3(0,Top+.13f,0)));
                if(plane.Raycast(ray,out float distance))mouseBlock.transform.position=ray.GetPoint(distance)+mouseOffset;
                if(Mouse.current.leftButton.wasReleasedThisFrame()){var b=mouseBlock;mouseBlock=null;b.HeldBy=-1;CompleteDrop(b);}return;
            }
            if(!Physics.Raycast(ray,out var hit,10))return;
            var button=hit.collider.GetComponent<WorldButton>();if(button){button.Paint(true);if(Mouse.current.leftButton.wasPressedThisFrame())button.Invoke();return;}
            var block=hit.collider.GetComponent<AtelierBlock>();
            if(block!=null&&!Paused&&Model.Phase!=AtelierModel.Stage.Result&&Mouse.current.leftButton.wasPressedThisFrame())
            {mouseBlock=block;block.HeldBy=2;block.Body.isKinematic=true;mouseOffset=Vector3.zero;Play(pickClip,block.transform.position);}
        }
        void OnDestroy(){input?.Disable();foreach(var m in materials)if(m){if(Application.isPlaying)Destroy(m);else DestroyImmediate(m);}}
    }
}
