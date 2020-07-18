﻿using Pumkin.UnityTools.Helpers;
using Pumkin.UnityTools.Implementation.Modules;
using Pumkin.UnityTools.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Pumkin.UnityTools.UI
{
    class MainUI
    {
        public List<IUIModule> UIModules = new List<IUIModule>();        

        public IUIModule OrphanHolder
        {
            get => _orphanHolder ?? (_orphanHolder = new OrphanHolderModule());
            private set => _orphanHolder = value;
        }

        IUIModule _orphanHolder;

        public MainUI() { }

        public MainUI(List<IUIModule> modules)
        {
            UIModules = modules;   
        }

        public void Draw()
        {            
            EditorGUILayout.LabelField("Pumkin's Avatar Tools", Styles.TitleLabel);

            GUILayout.Space(20f);

            PumkinTools.SelectedAvatar = EditorGUILayout.ObjectField("Avatar", PumkinTools.SelectedAvatar, typeof(GameObject), true) as GameObject;
            
            if(GUILayout.Button("Select from Scene"))
                PumkinTools.SelectedAvatar = Selection.activeGameObject ?? PumkinTools.SelectedAvatar;

            UIHelpers.DrawGUILine();

            //Draw modules
            foreach(var mod in UIModules)
            {
                if(mod != null)
                {
                    if(!mod.IsHidden)
                    {
                        mod.Draw();
                        UIHelpers.DrawGUILine();
                    }
                }
                else
                {
                    Debug.Log($"'{mod}' is null");
                }
            }

            //Draw Orphan Holder
            if(OrphanHolder != null)
            {
                if(!OrphanHolder.IsHidden)
                {
                    OrphanHolder.Draw();
                    UIHelpers.DrawGUILine();
                }
            }
        }

        public IUIModule FindModule(string name)
        {
            return UIModules.FirstOrDefault(s => string.Equals(name, s.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void AddModule(IUIModule module)
        {
            if(module != null)
                UIModules.Add(module);
        }

        public bool RemoveModule(IUIModule module)
        {
            return UIModules.Remove(module);
        }
        
        public int RemoveModule(string name)
        {
            return UIModules.RemoveAll(m => string.Equals(m.Name, name, StringComparison.InvariantCultureIgnoreCase));            
        }

        /// <summary>
        /// Orders modules by their OrderInUI value, then place OrphanHolder as the last module
        /// </summary>
        public void OrderModules()
        {
            UIModules = UIModules.OrderBy(c => c.OrderInUI).ToList();            
        }

        public static implicit operator bool(MainUI ui)
        {
            return !ReferenceEquals(ui, null);
        }
    }
}
