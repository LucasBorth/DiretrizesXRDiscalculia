using System.IO;
using UnityEditor;
namespace MatematicaHXR.Editor
{
    [InitializeOnLoad]
    static class FirstOpen
    {
        static FirstOpen()
        {
            EditorApplication.delayCall += () =>
            {
                if (!File.Exists("Assets/Scenes/Inicio.unity") && !EditorApplication.isPlayingOrWillChangePlaymode)
                    ProjectSetup.Generate();
            };
        }
    }
}
