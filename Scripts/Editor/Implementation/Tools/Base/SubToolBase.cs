﻿using Pumkin.UnityTools.Attributes;
using Pumkin.UnityTools.Implementation.Settings;
using Pumkin.UnityTools.Interfaces;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Pumkin.UnityTools.Implementation.Tools
{
    abstract class SubToolBase : ISubTool
    {
        public string Name { get; set; }
        public string Description { get; set; }        
        public string GameConfigurationString { get; set; }
        public bool AllowUpdate
        {
            get
            {
                return _allowUpdate;
            }

            set
            {
                if(_allowUpdate == value)
                    return;
                
                if(_allowUpdate = value)    //Intentional assign + check
                {
                    SetupUpdateCallback();
                    EditorApplication.update += updateCallback;
                }
                else
                {
                    EditorApplication.update -= updateCallback;
                }
            }
        }
        public int OrderInUI { get; set; }
        protected virtual GUIContent Content 
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
        public SettingsContainer Settings { get; protected set; }
        public bool ExpandSettings { get; protected set; }
        protected abstract SerializedObject SerializedSettings { get; }

        bool _allowUpdate;
        GUIContent _content;

        EditorApplication.CallbackFunction updateCallback;


        public SubToolBase() 
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
                Name = "Base Tool";
                Description = "Base Tool description";
                OrderInUI = 0;
            }
            SetupSettings();
        }

        void SetupUpdateCallback()
        {
            if(updateCallback == null)
            {
                Debug.Log($"Setting up Update callback for {Name}");
                updateCallback = new EditorApplication.CallbackFunction(Update);
            }            
        }        

        protected abstract void SetupSettings();

        public virtual void DrawUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button(Content))
                    TryExecute(PumkinTools.SelectedAvatar);
                if(GUILayout.Button("S", GUILayout.MaxWidth(20)))
                    ExpandSettings = !ExpandSettings;
            }
            EditorGUILayout.EndHorizontal();

            //Draw settings here
            if(SerializedSettings == null || !ExpandSettings)
                return;

            

            var it = SerializedSettings.GetIterator();
            it.Next(true);
            while(it.Next(false))
            {
                EditorGUILayout.PropertyField(it);
            }

            SerializedSettings.ApplyModifiedProperties();
        }
        
        public bool TryExecute(GameObject target)
        {
            try
            {
                if(Prepare(target) && DoAction(target))
                {
                    Finish(target);
                    return true;
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
            return false;
        }

        protected virtual bool Prepare(GameObject target)
        {
            if(!target)
            {
                Debug.LogError("No avatar selected");
                return false;
            }

            Debug.Log($"Registering undo: {Name}");
            Undo.RegisterFullObjectHierarchyUndo(target, Name);
            if(target.scene.name == null) //In case it's a prefab instance, which it probably is
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);

            return true;
        }        

        protected abstract bool DoAction(GameObject target);

        protected virtual void Finish(GameObject target)
        {
            Debug.Log($"{Name} completed successfully.");
        }

        public virtual void Update()
        {
            if(!AllowUpdate)
                return;
        }

    }
}
