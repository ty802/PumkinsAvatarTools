﻿using Pumkin.AvatarTools2.Interfaces;
using Pumkin.AvatarTools2.Modules;
using Pumkin.AvatarTools2.UI;
using Pumkin.Core.Extensions;
using Pumkin.Core.Helpers;
using Pumkin.Core.UI;
using System;
using UnityEditor;
using UnityEngine;

namespace Pumkin.AvatarTools2.Destroyers
{
    /// <summary>
    /// Base class that destroys all components of type by it's fullname
    /// </summary>
    public abstract class ComponentDestroyerBase : IComponentDestroyer, IItem
    {
        public abstract string ComponentTypeFullName { get; }

        public string GameConfigurationString { get; set; }

        public bool EnabledInUI { get; set; }

        public virtual GUIContent Content
        {
            get
            {
                if(_content == null)
                    _content = CreateGUIContent();
                return _content;
            }
        }

        public Type ComponentType
        {
            get
            {
                if(_componentType == null)
                    _componentType = TypeHelpers.GetType(ComponentTypeFullName);
                return _componentType;
            }
        }

        public virtual ISettingsContainer Settings => null;

        public virtual UIDefinition UIDefs { get; set; }

        Type _componentType;
        GUIContent _content;

        public ComponentDestroyerBase()
        {
            if(!string.IsNullOrWhiteSpace(ComponentTypeFullName))
                _componentType = TypeHelpers.GetType(ComponentTypeFullName);
            else
                throw new ArgumentNullException(ComponentTypeFullName, $"{ComponentTypeFullName} is invalid");

            if(!UIDefs)
                UIDefs = new UIDefinition(StringHelpers.ToTitleCase(_componentType?.Name) ?? "Invalid Destroyer");

            SetupSettings();
        }

        protected virtual void SetupSettings() { }

        protected virtual void Finish(GameObject target)
        {
            PumkinTools.Log($"Successfully removed all <b>{_componentType?.Name ?? "Unknown Type"}</b> from <b>{target.name}</b>");
        }

        protected virtual bool Prepare(GameObject target)
        {
            if(_componentType == null)
            {
                PumkinTools.LogWarning($"Can't use {GetType().Name} as it doesn't have a valid type.");
                return false;
            }

            if(!target)
                return false;

            Undo.RegisterCompleteObjectUndo(target, $"Destroy Components {_componentType.Name}");
            return true;
        }

        public bool TryDestroyComponents(GameObject target)
        {
            try
            {
                if(Prepare(target) && DoDestroyComponents(target))
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

        protected virtual bool DoDestroyComponents(GameObject target)
        {
            foreach(var co in target.GetComponentsInChildren(_componentType, true))
            {
                if(ShouldIgnoreObject(co.gameObject))
                    continue;

                try
                {
                    UnityObjectHelpers.DestroyAppropriate(co);
                }
                catch(Exception e)
                {
                    PumkinTools.LogWarning(e.Message);
                }
            }
            return true;
        }

        protected virtual GUIContent CreateGUIContent()
        {
            return new GUIContent(UIDefs.Name, Icons.GetIconTextureFromType(ComponentType));
        }

        protected virtual bool ShouldIgnoreObject(GameObject obj)
        {
            bool ignore = RemoveComponentsModule.IgnoreList.ShouldIgnoreTransform(obj.transform);
            if(ignore)
                PumkinTools.Log($"Ignoring <b>{obj.name}</b> because it's in the ignore list.");
            return ignore;
        }

        public virtual void DrawUI(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button(Content, Styles.SubToolButton, options))
                    TryDestroyComponents(PumkinTools.SelectedAvatar);
                if(Settings != null)
                    UIDefs.ExpandSettings = GUILayout.Toggle(UIDefs.ExpandSettings, Icons.Options, Styles.MediumIconButton);
            }
            EditorGUILayout.EndHorizontal();

            //Draw settings here
            if(Settings != null && UIDefs.ExpandSettings)
            {
                UIHelpers.VerticalBox(() =>
                {
                    EditorGUILayout.Space();
                    Settings?.Editor?.OnInspectorGUINoScriptField();
                });
            }
        }
    }
}