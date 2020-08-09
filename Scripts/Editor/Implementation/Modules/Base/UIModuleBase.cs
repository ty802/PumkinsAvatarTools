﻿using Pumkin.UnityTools.Attributes;
using Pumkin.UnityTools.Helpers;
using Pumkin.UnityTools.Interfaces;
using Pumkin.UnityTools.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Pumkin.UnityTools.Implementation.Modules
{
    abstract class UIModuleBase : IUIModule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string GameConfigurationString { get; set; }
        public bool IsExpanded { get; set; }
        public List<IUIModule> ChildModules { get; set; }
        public List<ISubTool> SubTools { get; set; }
        public virtual bool IsHidden { get; set; }        
        public GUIContent LabelContent
        {
            get
            {
                return _content ?? (_content = new GUIContent(Name, Description));
            }
            set
            {
                _content = value;
            }
        }              
        public int OrderInUI { get; set; }

        protected GUIContent _content;        

        public UIModuleBase()
        {
            var uiDefAttr = GetType().GetCustomAttribute<UIDefinitionAttribute>(false);
            if(uiDefAttr != null)   //Don't want default values if attribute missing, so not using uiDefAttr?.Description ?? "whatever"
            {
                Name = uiDefAttr.FriendlyName;
                Description = uiDefAttr.Description;
                OrderInUI = uiDefAttr.OrderInUI;
            }            
            else
            {
                Name = "Generic Module";
                Description = "A generic module";
                GameConfigurationString = "generic";
            }

            IsExpanded = false;
            
            SubTools = new List<ISubTool>();
            ChildModules = new List<IUIModule>();
        }

        public virtual void Draw()
        {
            UIHelpers.VerticalBox(() =>
            {
                DrawHeader();
                if(IsExpanded)
                    DrawContent();
            });            
        }

        public virtual void DrawHeader()
        {
            IsExpanded = UIHelpers.DrawFoldout(IsExpanded, LabelContent, true, Styles.MenuFoldout);            
        }

        public virtual void DrawContent()
        {
            EditorGUILayout.Space();
            if(!string.IsNullOrEmpty(Description))
            {
                EditorGUILayout.HelpBox($"{Description}", MessageType.Info);
                EditorGUILayout.Space();
            }

            foreach(var tool in SubTools)
                tool?.DrawUI();

            EditorGUILayout.Space();
            foreach(var child in ChildModules)
                child?.Draw();
            
        }

        public virtual void OrderSubTools()
        {
            if(SubTools == null || SubTools.Count == 0)
                return;

            SubTools = SubTools.OrderBy(t => t.OrderInUI).ToList();
        }

        public static implicit operator bool(UIModuleBase module)
        {
            return !ReferenceEquals(module, null);
        }
    }
}