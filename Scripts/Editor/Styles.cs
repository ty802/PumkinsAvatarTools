﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Pumkin.UnityTools.UI
{
    static class Styles
    {
        public static GUIStyle MenuFoldout { get; private set; }
        public static GUIStyle TitleLabel { get; private set; }
        public static GUIStyle EditorLine { get; private set; }        
        public static GUIStyle RegionBG { get; private set; }

        public static GUIStyle Box { get; private set; }

        static Styles()
        {            
            MenuFoldout = new GUIStyle("foldout")
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,                
            };            

            //MenuFoldout = new GUIStyle("ToolbarDropDown")
            //{
            //    fontSize = 13,
            //    fixedHeight = 26,
            //    fontStyle = FontStyle.Bold,
            //    contentOffset = new Vector2(5f, 0),
            //    stretchWidth = true
            //};

            TitleLabel = new GUIStyle("label")
            {
                fontSize = 16,
                stretchHeight = true,
                fixedHeight = 24,
            };

            EditorLine = new GUIStyle("box")
            {
                border = new RectOffset(1, 1, 1, 1),
                margin = new RectOffset(5, 5, 1, 1),
                padding = new RectOffset(1, 1, 1, 1),
            };
            
            RegionBG = new GUIStyle("RegionBg")
            {
                margin = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(10, 10, 6, 14),
                border = new RectOffset(6, 6, 6, 6),
                fontSize = 12,
                alignment = TextAnchor.UpperLeft
            };
            string regionBgTexGUID = AssetDatabase.FindAssets("RegionBgStyleNormalState")[0];
            Texture2D regionBgTex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(regionBgTexGUID));
            RegionBG.normal.background = regionBgTex;

            Box = new GUIStyle("box")
            {
                margin = new RectOffset(3, 3, 3, 3),
                padding = new RectOffset(10, 10, 6, 6),
                border = new RectOffset(6, 6, 6, 6),
                fontSize = 12,
                alignment = TextAnchor.UpperLeft
            };
        }
    }
}