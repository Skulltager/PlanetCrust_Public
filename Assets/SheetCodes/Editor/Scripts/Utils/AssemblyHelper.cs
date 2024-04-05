using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SheetCodesEditor
{
    public static class AssemblyHelper
    {
        private static Type[] allComponentTypes;
        private static Type[] unityComponentTypes;
        private static Type[] commonComponentTypes;
        private static Type[] customComponentTypes;
        private static Type[] enumTypes;

        public static Type[] GetComponentTypes(ComponentTypeSelection componentTypeSelection)
        {
            switch(componentTypeSelection)
            {
                case (ComponentTypeSelection.All):
                    return GetAllComponentTypes();
                case (ComponentTypeSelection.AllUnity):
                    return GetUnityComponentTypes();
                case (ComponentTypeSelection.CommonUnity):
                    return GetCommonComponentTypes();
                case (ComponentTypeSelection.Custom):
                    return GetCustomComponentTypes();
            }

            throw new MissingMemberException(string.Format("GetComponentType is missing implementation for ComponentTypeSelection ", componentTypeSelection));
        }

        public static ComponentTypeSelection GetComponentTypeSelection(Type componentType)
        {
            if (GetCommonComponentTypes().Contains(componentType))
                return ComponentTypeSelection.CommonUnity;
            if (GetCustomComponentTypes().Contains(componentType))
                return ComponentTypeSelection.Custom;
            if (GetUnityComponentTypes().Contains(componentType))
                return ComponentTypeSelection.AllUnity;

            return ComponentTypeSelection.All;
        }

        public static Type[] GetAllComponentTypes()
        {
            if (allComponentTypes != null)
                return allComponentTypes;
            
            List<Type> types = new List<Type>();
            types.AddRange(GetUnityComponentTypes());
            types.AddRange(GetCustomComponentTypes());
            allComponentTypes = types.ToArray();
            return allComponentTypes;
        }

        public static Type[] GetUnityComponentTypes()
        {
            if (unityComponentTypes != null)
                return unityComponentTypes;

            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (!assembly.ToString().StartsWith("UnityEngine"))
                    continue;

                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsSubclassOf(typeof(UnityEngine.Object)))
                        continue;

                    types.Add(type);
                }
            }
            types.Sort(new TypeSorter());
            unityComponentTypes = types.ToArray();
            return unityComponentTypes;
        }
         
        public static Type[] GetCustomComponentTypes()
        {
            if (customComponentTypes != null)
                return customComponentTypes;

            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly assembly = assemblies.First(i => i.FullName.StartsWith("Assembly-CSharp,"));
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(UnityEngine.Object)))
                    continue;

                if (type.Namespace == SheetStringDefinitions.NAMESPACE)
                    continue;
                
                types.Add(type);
            }
            types.Sort(new TypeSorter());
            customComponentTypes = types.ToArray();
            return customComponentTypes;
        }

        public static Type[] GetEnumTypes()
        {
            if (enumTypes != null)
                return enumTypes;

            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly assembly = Array.Find(assemblies, i => i.ToString().StartsWith("Assembly-CSharp,"));
            Type[] dataSheetTypes = DatasheetTypeExtension.GetAllIdentifierTypes();
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsEnum)
                    continue;

                if (type.Namespace == SheetStringDefinitions.NAMESPACE)
                    continue;

                types.Add(type);
            }
            types.Sort(new TypeSorter());
            enumTypes = types.ToArray();
            return enumTypes;
        }
        
        public static Type[] GetCommonComponentTypes()
        {
            if (commonComponentTypes != null)
                return commonComponentTypes;

            List<Type> types = new List<Type>();

            //List of common components. Add your types to this list.
            types.Add(typeof(Texture));
            types.Add(typeof(Sprite));
            types.Add(typeof(Transform));
            types.Add(typeof(GameObject));
            types.Add(typeof(ParticleSystem));
            types.Add(typeof(RectTransform));
            types.Add(typeof(Animation));
            types.Add(typeof(Animator));
            types.Add(typeof(Material));
            types.Add(typeof(ScriptableObject));
            types.Add(typeof(AudioClip));
            types.Add(typeof(Shader));
            
            types.Sort(new TypeSorter());
            commonComponentTypes = types.ToArray();
            return commonComponentTypes;
        }
    }
}