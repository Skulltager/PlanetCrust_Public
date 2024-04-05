using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using SheetCodes;

namespace SheetCodesEditor
{
    public class AddColumnWindow : EditorWindow
    {
        private DataSheet dataSheet;
        private SheetPage sheetPage;
        private Action<SheetColumn, int> callback;
        private SheetDataType dataType;
        private string serializationName;
        private string propertyName;
        private bool isCollection;
        private string referenceSheet;
        private bool propertyManuallyChanged;
        private int insertIndex;
        
        private string componentNameFilter;
        private int selectedComponentIndex;
        private ComponentTypeSelection componentTypeSelection;
        private Type[] filteredComponentTypes;
        private string[] filteredComponentTypeNames;

        private string enumNameFilter;
        private int selectedEnumIndex;
        private Type[] filteredEnumTypes;
        private string[] filteredEnumTypeNames;

        private const int WIDTH = 400;

        private void Awake()
        {
            titleContent.text = "Add Column";
            minSize = new Vector2(WIDTH, 200);

            dataType = SheetDataType.Int;
            isCollection = false;
            referenceSheet = default;
            propertyManuallyChanged = false;

            serializationName = string.Empty;
            propertyName = string.Empty;

            componentTypeSelection = ComponentTypeSelection.CommonUnity;
            selectedComponentIndex = -1;
            componentNameFilter = string.Empty;
            SetFilteredComponentCollection();

            selectedEnumIndex = -1;
            enumNameFilter = string.Empty;
            SetFilteredEnumCollection();
        }

        private void SetFilteredComponentCollection()
        {
            string loweredNameFilter = componentNameFilter.ToLower();

            if (selectedComponentIndex != -1)
            {
                Type selectedType = filteredComponentTypes[selectedComponentIndex];

                filteredComponentTypes = AssemblyHelper.GetComponentTypes(componentTypeSelection).Where(i => i.Name.ToString().ToLower().Contains(loweredNameFilter)).ToArray();
                filteredComponentTypeNames = filteredComponentTypes.Select(i => i.Name.ToString()).ToArray();

                selectedComponentIndex = Array.IndexOf(filteredComponentTypes, selectedType);
                if (selectedComponentIndex == -1 && filteredComponentTypes.Length > 0)
                    selectedComponentIndex = 0;
            }
            else
            {
                filteredComponentTypes = AssemblyHelper.GetComponentTypes(componentTypeSelection).Where(i => i.Name.ToString().ToLower().Contains(loweredNameFilter)).ToArray();
                filteredComponentTypeNames = filteredComponentTypes.Select(i => i.Name.ToString()).ToArray();
                selectedComponentIndex = filteredComponentTypes.Length > 0 ? 0 : -1;
            }
        }

        private void SetFilteredEnumCollection()
        {
            string loweredNameFilter = enumNameFilter.ToLower();

            if (selectedEnumIndex != -1)
            {
                Type selectedType = filteredComponentTypes[selectedEnumIndex];

                filteredEnumTypes = AssemblyHelper.GetEnumTypes().Where(i => i.Name.ToString().ToLower().Contains(loweredNameFilter)).ToArray();
                filteredEnumTypeNames = filteredEnumTypes.Select(i => i.Name.ToString()).ToArray();

                selectedEnumIndex = Array.IndexOf(filteredEnumTypes, selectedType);
                if (selectedEnumIndex == -1 && filteredEnumTypes.Length > 0)
                    selectedEnumIndex = 0;
            }
            else
            {
                filteredEnumTypes = AssemblyHelper.GetEnumTypes().Where(i => i.Name.ToString().ToLower().Contains(loweredNameFilter)).ToArray();
                filteredEnumTypeNames = filteredEnumTypes.Select(i => i.Name.ToString()).ToArray();
                selectedEnumIndex = filteredEnumTypes.Length > 0 ? 0 : -1;
            }
        }

        public void Initialize(DataSheet dataSheet, SheetPage sheetPage, Action<SheetColumn, int> callback, int insertIndex)
        {
            this.dataSheet = dataSheet;
            this.sheetPage = sheetPage;
            this.callback = callback;
            this.insertIndex = insertIndex;
            string[] options = dataSheet.datasheetPages.Select(i => i.sheetName).ToArray();
            referenceSheet = options[0];
        }

        private void OnGUI()
        {
            if (focusedWindow != this)
                Focus();

            EditorGUILayout.BeginVertical();

            DrawSerializationName();
            DrawPropertyName();
            DrawIsCollection();
            DrawDataType();

            string exception;
            bool containsExceptions = ContainsExceptions(out exception);
            if (containsExceptions)
                EditorGUILayout.HelpBox(exception, MessageType.Error);

            EditorGUI.BeginDisabledGroup(containsExceptions);
            DrawGenerateButton();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                minSize = new Vector2(WIDTH, lastRect.yMax - 4);
                maxSize = minSize;
            }
        }

        private void DrawSerializationName()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_COLUMN_NAME, GUILayout.Width(120));
            string newSerializationName = GUILayout.TextField(serializationName).RemoveBreakingCharacter();
            EditorGUILayout.EndHorizontal();

            if (newSerializationName == serializationName)
                return;

            serializationName = newSerializationName;
            if (propertyManuallyChanged)
                return;

            propertyName = serializationName.ConvertStringToEnumString();
        }

        private void DrawPropertyName()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_PROPERTY_NAME, GUILayout.Width(120));
            string newPropertyName = GUILayout.TextField(propertyName).RemoveBreakingCharacter().CreatePropertyName();
            EditorGUILayout.EndHorizontal();

            if (newPropertyName == propertyName)
                return;

            propertyName = newPropertyName;
            propertyManuallyChanged = true;
        }

        private void DrawIsCollection()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_IS_ARRAY, GUILayout.Width(120));
            isCollection = GUILayout.Toggle(isCollection, "");
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDataType()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Localization.COLUMN_DATA_TYPE, GUILayout.Width(120));
            dataType = (SheetDataType)EditorGUILayout.EnumPopup(dataType);
            EditorGUILayout.EndHorizontal();

            if (dataType == SheetDataType.Reference)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Localization.COLUMN_REFERENCE_SHEET, GUILayout.Width(120));
                string[] options = dataSheet.datasheetPages.Select(i => i.sheetName).ToArray();
                int currentOption = Array.IndexOf(options, referenceSheet);
                int selectedOption = EditorGUILayout.Popup(currentOption, options);
                referenceSheet = options[selectedOption];
                EditorGUILayout.EndHorizontal();
            }
            else if (dataType == SheetDataType.Component)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Localization.COLUMN_COMPONENT_LIST, GUILayout.Width(120));
                ComponentTypeSelection newComponentTypeSelection = (ComponentTypeSelection) EditorGUILayout.EnumPopup(componentTypeSelection);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Localization.COLUMN_FILTER, GUILayout.Width(120));
                string newComponentNameFilter = EditorGUILayout.TextField(componentNameFilter);
                EditorGUILayout.EndHorizontal();

                if (newComponentNameFilter != componentNameFilter || newComponentTypeSelection != componentTypeSelection)
                {
                    componentNameFilter = newComponentNameFilter;
                    componentTypeSelection = newComponentTypeSelection;

                    SetFilteredComponentCollection();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Localization.COLUMN_COMPONENT_TYPE, GUILayout.Width(120));
                selectedComponentIndex = EditorGUILayout.Popup(selectedComponentIndex, filteredComponentTypeNames);
                EditorGUILayout.EndHorizontal();
            }
            else if (dataType == SheetDataType.Enum)
            {
                string oldEnumNameFilter = enumNameFilter;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Localization.COLUMN_FILTER, GUILayout.Width(120));
                enumNameFilter = EditorGUILayout.TextField(enumNameFilter);
                EditorGUILayout.EndHorizontal();

                if (oldEnumNameFilter != enumNameFilter)
                    SetFilteredComponentCollection();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Localization.COLUMN_ENUM_TYPE, GUILayout.Width(120));
                selectedEnumIndex = EditorGUILayout.Popup(selectedEnumIndex, filteredEnumTypeNames);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawGenerateButton()
        {
            if (!GUILayout.Button(Localization.GENERATE))
                return;

            string reference = dataType == SheetDataType.Reference ? referenceSheet : "";
            Type componentType = dataType == SheetDataType.Component ? filteredComponentTypes[selectedComponentIndex] : default;
            Type enumType = dataType == SheetDataType.Enum ? filteredEnumTypes[selectedEnumIndex] : default;

            SheetColumn sheetColumn = new SheetColumn(sheetPage, serializationName, propertyName, dataType, reference, componentType, enumType, isCollection, insertIndex);
            callback(sheetColumn, insertIndex);
            Close();
        }

        private bool ContainsExceptions(out string error)
        {
            if (string.IsNullOrEmpty(serializationName))
            {
                error = Localization.ERROR_COLUMN_SERIALIZATIONNAME_EMPTY;
                return true;
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                error = Localization.ERROR_COLUMN_PROPERTYNAME_EMPTY;
                return true;
            }

            if (propertyName == "Identifier")
            {
                error = Localization.ERROR_COLUMN_PROPERTYNAME_MATCHES_IDENTIFIER;
                return true;
            }

            if (sheetPage.columns.Any(i => i.serializationName == serializationName))
            {
                error = Localization.ERROR_COLUMN_SERIALIZATIONNAME_MATCH;
                return true;
            }

            foreach (SheetColumn otherColumn in sheetPage.columns)
            {
                if (otherColumn.propertyName == propertyName)
                {
                    error = Localization.ERROR_COLUMN_PROPERTYNAME_MATCH;
                    return true;
                }

                if (otherColumn.dataType == SheetDataType.Reference)
                {
                    if (otherColumn.isCollection && otherColumn.propertyName + "Records" == propertyName)
                    {
                        error = Localization.ERROR_COLUMN_PROPERTYNAME_MATCH_EXTRA;
                        return true;
                    }
                    else if (!otherColumn.isCollection && otherColumn.propertyName + "Record" == propertyName)
                    {
                        error = Localization.ERROR_COLUMN_PROPERTYNAME_MATCH_EXTRA;
                        return true;
                    }
                }
            }

            if (dataType == SheetDataType.Component && selectedComponentIndex == -1)
            {
                error = Localization.ERROR_COLUMN_COMPONENT_EMPTY;
                return true;
            }

            if (dataType == SheetDataType.Enum && selectedEnumIndex == -1)
            {
                error = Localization.ERROR_COLUMN_ENUM_EMPTY;
                return true;
            }

            error = string.Empty;
            return false;
        }
    }
}