using System.Collections;
using System.Collections.Generic;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace Patchkit {
    public static class QuickLinkPatchkit {
        
        [MenuItem("Tools/PatchKit/Quick Account Setup", false, 3)]
        public static void LinkPatchkitAccount() {
            var apiKey = new ApiKey("1ca99e5bb4d6ed9f999f25157b9e5302");
            Config.LinkAccount(apiKey);
            
            Config.ConnectApp(new AppSecret("3e6aea9d6b3c3d90550ae4121130604e"), AppPlatform.Windows32);
        }
    }
}