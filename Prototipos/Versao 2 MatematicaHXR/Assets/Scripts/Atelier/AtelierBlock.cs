using UnityEngine;
namespace MatematicaHXR.Atelier
{
    public sealed class AtelierBlock : MonoBehaviour
    {
        public int Id;
        public int HeldBy=-1;
        public Rigidbody Body;
        public Renderer Shell;
        public UnityEngine.UI.Text Mark;
        public Vector3 LastSafe;
        public float LastSound;
        public AtelierExperience Owner;
        void OnCollisionEnter(Collision c)
        {
            if(Owner==null || Owner.Paused || Time.time-LastSound<.15f || c.relativeVelocity.magnitude<.12f)return;
            LastSound=Time.time;Owner.Impact(transform.position,Mathf.Clamp01(c.relativeVelocity.magnitude*.2f));
        }
    }
}
