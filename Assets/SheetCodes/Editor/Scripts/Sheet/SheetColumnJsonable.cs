using System;

namespace SheetCodesEditor
{
    [Serializable]
    public class SheetColumnJsonable
    {
        public string serializationName;
        public string propertyName;
        public SheetDataType dataType;
        public bool isCollection;
        public string referenceSheet;
        public string componentTypeName;
        public string enumTypeName;

        public SheetColumnJsonable(SheetColumn sheetColumn)
        {
            serializationName = sheetColumn.serializationName;
            propertyName = sheetColumn.propertyName;
            dataType = sheetColumn.dataType;
            isCollection = sheetColumn.isCollection;
            referenceSheet = sheetColumn.referenceSheet;
            componentTypeName = sheetColumn.componentType != null ? sheetColumn.componentType.FullName + ", " + sheetColumn.componentType.Assembly.FullName : string.Empty;
            enumTypeName = sheetColumn.enumType != null ? sheetColumn.enumType.FullName + ", " + sheetColumn.enumType.Assembly.FullName : string.Empty;
        }
    }
}
