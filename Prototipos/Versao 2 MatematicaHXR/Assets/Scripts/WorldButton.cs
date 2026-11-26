using System;
using UnityEngine;
using UnityEngine.UI;
namespace MatematicaHXR
{
    public sealed class WorldButton : MonoBehaviour
    {
        public Action Action;
        public bool Available = true;
        public Text Label;
        public Image Background;
        public void Paint(bool hover)
        {
            Background.color = !Available ? new Color(.14f,.18f,.23f) :
                hover ? new Color(.23f,.40f,.53f) : new Color(.18f,.28f,.37f);
            Label.color = Available ? Color.white : new Color(.48f,.53f,.58f);
        }
        public void Invoke() { if (Available) Action?.Invoke(); }
    }
}
