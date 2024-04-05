using System;

namespace SheetCodesEditor
{
    public class SheetColumn
    {
        public string serializationName;
        public string propertyName;
        public SheetDataType dataType;
        public bool isCollection;
        public string referenceSheet;
        public Type componentType;
        public Type enumType;
        public bool enumFlags;

        public SheetColumn() { }

        public SheetColumn(SheetPage sheetPage, string serializationName, string propertyName, SheetDataType dataType, string referenceSheet, Type componentType, Type enumType, bool isCollection, int insertIndex)
        {
            if (insertIndex < sheetPage.columns.Count)
                sheetPage.columns.Insert(insertIndex, this);
            else
                sheetPage.columns.Add(this);

            this.serializationName = serializationName;
            this.propertyName = propertyName;
            this.dataType = dataType;
            this.isCollection = isCollection;
            this.referenceSheet = referenceSheet;
            this.componentType = componentType;
            this.enumType = enumType;
            this.enumFlags = enumType != null ? enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0 : false;

            object defaultData = dataType.GetDefaultValue(componentType, enumType, isCollection);

            foreach (SheetRow sheetRow in sheetPage.rows)
            {
                SheetCell sheetCell = new SheetCell(defaultData);
                if (insertIndex < sheetPage.columns.Count)
                    sheetRow.cells.Insert(insertIndex, sheetCell);
                else
                    sheetRow.cells.Add(sheetCell);
            }
        }

        public string GetColumnText()
        {
            string text = serializationName + "\n";
            if (dataType == SheetDataType.Reference)
            {
                if (isCollection)
                    text += string.Format(Localization.ADDITION_REFERENCE_ARRAY, referenceSheet);
                else
                    text += string.Format(Localization.ADDITION_REFERENCE, referenceSheet);
            }
            else
            {
                if (dataType == SheetDataType.Component)
                    text += componentType.Name;
                else if (dataType == SheetDataType.Enum)
                    text += enumType.Name;
                else
                    text += dataType;

                if (isCollection)
                    text += string.Format(Localization.ADDITION_ARRAY, referenceSheet);
            }
            return text;
        }

        public SheetColumn(SheetColumnJsonable jsonable)
        {
            serializationName = jsonable.serializationName;
            propertyName = jsonable.propertyName;
            dataType = jsonable.dataType;
            isCollection = jsonable.isCollection;
            referenceSheet = jsonable.referenceSheet;
            componentType = Type.GetType(jsonable.componentTypeName);
            enumType = Type.GetType(jsonable.enumTypeName);
            enumFlags = enumType != null ? enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0 : false;
        }

        public bool CheckIfSameCodebase(SheetColumn other)
        {
            if (serializationName != other.serializationName)
                return false;

            if (propertyName != other.propertyName)
                return false;

            if (dataType != other.dataType)
                return false;

            if (isCollection != other.isCollection)
                return false;

            if (dataType == SheetDataType.Reference && referenceSheet != other.referenceSheet)
                return false;

            if (dataType == SheetDataType.Enum && enumType != other.enumType)
                return false;

            if (dataType == SheetDataType.Component && componentType != other.componentType)
                return false;

            return true;
        }

        public object GetDefaultCellValue()
        {
            return dataType.GetDefaultValue(componentType, enumType, isCollection);
        }
    }
}