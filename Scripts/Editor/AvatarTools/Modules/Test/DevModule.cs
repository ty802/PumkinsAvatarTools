﻿#if UNITY_EDITOR && PUMKIN_DEV
using Pumkin.AvatarTools2.UI;
using Pumkin.Core;
using Pumkin.Core.Helpers;
using Pumkin.Core.UI;
using UnityEngine;

namespace Pumkin.AvatarTools2.Modules
{
    [AutoLoad(DefaultIDs.Modules.Debug, "Debug")]
    class DevModule : UIModuleBase
    {
        public override UIDefinition UIDefs { get; set; }
            = new UIDefinition("Debug", "Debug and test stuff", 100);

        public override void DrawContent()
        {
            base.DrawContent();

            if(GUILayout.Button("Remove reset transforms from tools"))
                PumkinToolsWindow.UI.FindModule("tools")?.SubItems.RemoveAll(s =>
                string.Equals(s.UIDefs.Name, "reset transforms", System.StringComparison.InvariantCultureIgnoreCase));

            if(GUILayout.Button("Remove tools module"))
                PumkinToolsWindow.UI.RemoveModule("tools");

            if(GUILayout.Button("Build UI"))
                PumkinToolsWindow.UI = UIBuilder.BuildUI();

            if(GUILayout.Button("Dump Default GUISkin"))
                DevHelpers.DumpDefaultGUISkin();

            if(GUILayout.Button("Log current pose muscles"))
            {
                var muscles = DevHelpers.GetHumanMusclesFromCurrentPose(PumkinTools.SelectedAvatar);
                string s = "float[] pose = \n{";
                for(int i = 0; i < muscles.Length; i++)
                {
                    s += $"{muscles[i]}f {((i != muscles.Length - 1) ? ", " : "")}";
                    s += i % 13 == 0 ? "\n" : "";
                }
                s += "\n};";
                Debug.Log(s);
            }

            if(GUILayout.Button("Test dynamic bones in project"))
            {
                Debug.Log(TypeHelpers.GetTypeAnwhere("DynamicBone")?.FullName ?? "Not here");
            }
        }
    }
}
#endif