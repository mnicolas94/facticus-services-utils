using System.Linq;
using ServicesUtils.Authentication;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ServicesUtils.Editor.Drawers
{
    [CustomEditor(typeof(GPGSAuthenticationInitializer))]
    public class GPGSAuthentiationInitializerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var inspector = new VisualElement();
            InspectorElement.FillDefaultInspector(inspector, serializedObject, this);
#if UNITY_ANDROID && !GPGP_ENABLED
            var helpBox = new HelpBox("GPGP_ENABLED is not added as a scripting define symbol", HelpBoxMessageType.Error);
            var button = new Button();
            button.clickable.clicked += () =>
            {
                PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android, out var defines);
                bool alreadyHasIt = defines.Contains("GPGP_ENABLED");
                if (!alreadyHasIt) // check double clicks
                {
                    var newDefines = new string[defines.Length + 1];
                    defines.CopyTo(newDefines, 0);
                    newDefines[defines.Length] = "GPGP_ENABLED";
                    PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, newDefines);
                    button.SetEnabled(false);
                    helpBox.text = "Added. Compiling...";
                    helpBox.messageType = HelpBoxMessageType.Info;
                }
            };
            button.text = "Add GPGP_ENABLED";
            inspector.Add(helpBox);
            inspector.Add(button);
#endif
            return inspector;
        }
    }
}