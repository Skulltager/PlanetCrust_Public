using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SheetCodesEditor
{
    public enum SheetDataType
    {
        [Identifier("long")] Long = 0,
        [Identifier("int")] Int = 1,
        [Identifier("short")] Short = 2,
        [Identifier("byte")] Byte = 3,
        [Identifier("double")] Double = 4,
        [Identifier("float")] Float = 5,
        [Identifier("string")] String = 6,
        [Identifier("bool")] Boolean = 7,
        [Identifier("Color")] Color = 8,
        [Identifier("reference")] Reference = 9,
        [Identifier("enum")] Enum = 10,
        [Identifier("component")] Component = 11
    }

    public static class SheetDataTypeExtension
    {
        #region Default Value
        public static object GetDefaultValue(this SheetDataType dataType, Type componentType, Type enumType, bool isCollection)
        {
            if (isCollection)
                return dataType.GetDefaultValue_Collection(componentType, enumType);
            else
                return dataType.GetDefaultValue_Single(componentType, enumType);
        }

        private static object GetDefaultValue_Single(this SheetDataType dataType, Type componentType, Type enumType)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return default(long);
                case SheetDataType.Int:
                    return default(int);
                case SheetDataType.Short:
                    return default(short);
                case SheetDataType.Byte:
                    return default(byte);
                case SheetDataType.Double:
                    return default(double);
                case SheetDataType.Float:
                    return default(float);
                case SheetDataType.String:
                    return string.Empty;
                case SheetDataType.Boolean:
                    return default(bool);
                case SheetDataType.Color:
                    return default(Color);
                case SheetDataType.Reference:
                    return "None";
                case SheetDataType.Enum:
                    return Activator.CreateInstance(enumType);
                case SheetDataType.Component:
                    return default(Component);
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETDEFAULTVALUE_SINGLE, dataType));
        }

        private static object GetDefaultValue_Collection(this SheetDataType dataType, Type componentType, Type enumType)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return new long[0];
                case SheetDataType.Int:
                    return new int[0];
                case SheetDataType.Short:
                    return new short[0];
                case SheetDataType.Byte:
                    return new byte[0];
                case SheetDataType.Double:
                    return new double[0];
                case SheetDataType.Float:
                    return new float[0];
                case SheetDataType.String:
                    return new string[0];
                case SheetDataType.Boolean:
                    return new bool[0];
                case SheetDataType.Color:
                    return new Color[0];
                case SheetDataType.Reference:
                    return new string[0];
                case SheetDataType.Enum:
                    return Array.CreateInstance(enumType, 0);
                case SheetDataType.Component:
                    return Array.CreateInstance(componentType, 0);
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETDEFAULTVALUE_COLLECTION, dataType));
        }
        #endregion

        #region String to data
        public static object GetDataValue(this SheetDataType dataType, string value, Type componentType, Type enumType, bool isCollection)
        {
            if (isCollection)
                return dataType.GetDataValue_Collection(value, componentType, enumType);
            else
                return dataType.GetDataValue_Single(value, componentType, enumType);
        }

        private static object GetDataValue_Single(this SheetDataType dataType, string value, Type componentType, Type enumType)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return long.Parse(value);
                case SheetDataType.Int:
                    return int.Parse(value);
                case SheetDataType.Short:
                    return short.Parse(value);
                case SheetDataType.Byte:
                    return byte.Parse(value);
                case SheetDataType.Double:
                    return double.Parse(value);
                case SheetDataType.Float:
                    return float.Parse(value);
                case SheetDataType.String:
                    return value;
                case SheetDataType.Boolean:
                    return bool.Parse(value);
                case SheetDataType.Color:
                    Color color;
                    ColorUtility.TryParseHtmlString("#" + value, out color);
                    return color;
                case SheetDataType.Reference:
                    return value;
                case SheetDataType.Enum:
                    Type subType = Enum.GetUnderlyingType(enumType);
                    object subTypeValue = Convert.ChangeType(value, subType);
                    object result = Enum.ToObject(enumType, subTypeValue);
                    return result;
                case SheetDataType.Component:
                    string assetPath = AssetDatabase.GUIDToAssetPath(value);
                    return AssetDatabase.LoadAssetAtPath(assetPath, componentType);
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETDATAVALUE_SINGLE, dataType));
        }

        private static object GetDataValue_Collection(this SheetDataType dataType, string value, Type componentType, Type enumType)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return GetDataCollection_Long(value);
                case SheetDataType.Int:
                    return GetDataCollection_Int(value);
                case SheetDataType.Short:
                    return GetDataCollection_Short(value);
                case SheetDataType.Byte:
                    return GetDataCollection_Byte(value);
                case SheetDataType.Double:
                    return GetDataCollection_Double(value);
                case SheetDataType.Float:
                    return GetDataCollection_Float(value);
                case SheetDataType.String:
                    return GetDataCollection_String(value);
                case SheetDataType.Boolean:
                    return GetDataCollection_Bool(value);
                case SheetDataType.Color:
                    return GetDataCollection_Color(value);
                case SheetDataType.Reference:
                    return GetDataCollection_String(value);
                case SheetDataType.Enum:
                    return GetDataCollection_Enum(value, enumType);
                case SheetDataType.Component:
                    return GetDataCollection_Component(value, componentType);
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETDATAVALUE_COLLECTION, dataType));
        }

        private static long[] GetDataCollection_Long(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            long[] convertedValues = new long[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = long.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static int[] GetDataCollection_Int(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            int[] convertedValues = new int[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = int.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static short[] GetDataCollection_Short(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            short[] convertedValues = new short[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = short.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static float[] GetDataCollection_Float(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            float[] convertedValues = new float[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = float.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static double[] GetDataCollection_Double(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            double[] convertedValues = new double[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = double.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static byte[] GetDataCollection_Byte(string value)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            byte[] convertedValues = new byte[valueSplit.Length - 1];

            for (int i = 0; i < valueSplit.Length - 1; i++)
                convertedValues[i] = byte.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static bool[] GetDataCollection_Bool(string value)
        {
            string[] valueSplit = GetDataCollection_String(value);
            bool[] convertedValues = new bool[valueSplit.Length];

            for (int i = 0; i < valueSplit.Length; i++)
                convertedValues[i] = bool.Parse(valueSplit[i]);

            return convertedValues;
        }

        private static Color[] GetDataCollection_Color(string value)
        {
            string[] valueSplit = GetDataCollection_String(value);
            Color[] convertedValues = new Color[valueSplit.Length];

            for (int i = 0; i < valueSplit.Length; i++)
                ColorUtility.TryParseHtmlString("#" + valueSplit[i], out convertedValues[i]);

            return convertedValues;
        }

        private static string[] GetDataCollection_String(string value)
        {
            List<string> stringSplit = new List<string>();

            while (value.Length > 0)
            {
                int index = value.IndexOf(' ');
                int stringSize = int.Parse(value.Substring(0, index));
                string stringItem = value.Substring(index + 1, stringSize);
                stringSplit.Add(stringItem);
                int totalSize = index + stringItem.Length + 1;
                value = value.Substring(totalSize, value.Length - totalSize);
            }

            return stringSplit.ToArray();
        }

        private static object GetDataCollection_Enum(string value, Type enumType)
        {
            string[] stringSplit = GetDataCollection_String(value);
            Array array = Array.CreateInstance(enumType, stringSplit.Length);

            for (int i = 0; i < stringSplit.Length; i++)
            {
                Type subType = Enum.GetUnderlyingType(enumType);
                object subTypeValue = Convert.ChangeType(stringSplit[i], subType);
                object enumValue = Enum.ToObject(enumType, subTypeValue);
                
                array.SetValue(enumValue, i);
            }

            return array;
        }

        private static object GetDataCollection_Component(string value, Type componentType)
        {
            string[] valueSplit = value.Split(SheetStringDefinitions.SPLIT_CHARACTER);
            Array array = Array.CreateInstance(componentType, valueSplit.Length - 1);

            for (int i = 0; i < valueSplit.Length - 1; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(valueSplit[i]);
                UnityEngine.Object component = AssetDatabase.LoadAssetAtPath(assetPath, componentType);
                array.SetValue(component, i);
            }
            return array;
        }
        #endregion

        #region Data to string
        public static string GetStringValue(this SheetDataType dataType, object value, bool isCollection)
        {
            if (isCollection)
                return dataType.GetStringValue_Collection(value);
            else
                return dataType.GetStringValue_Single(value);
        }

        private static string GetStringValue_Single(this SheetDataType dataType, object value)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return value.ToString();
                case SheetDataType.Int:
                    return value.ToString();
                case SheetDataType.Short:
                    return value.ToString();
                case SheetDataType.Byte:
                    return value.ToString();
                case SheetDataType.Double:
                    return value.ToString();
                case SheetDataType.Float:
                    return value.ToString();
                case SheetDataType.String:
                    return (string)value;
                case SheetDataType.Boolean:
                    return value.ToString();
                case SheetDataType.Color:
                    return ColorUtility.ToHtmlStringRGBA((Color)value);
                case SheetDataType.Reference:
                    return (string)value;
                case SheetDataType.Enum:
                    Enum enumValue = (Enum)value;
                    object val = Convert.ChangeType(enumValue, enumValue.GetTypeCode());
                    return val.ToString();
                case SheetDataType.Component:
                    return ConvertComponent(value);
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETSTRINGVALUE_SINGLE, dataType));
        }

        private static string GetStringValue_Collection(this SheetDataType dataType, object value)
        {
            switch (dataType)
            {
                case SheetDataType.Long:
                    return GetStringCollection_Long((long[])value);
                case SheetDataType.Int:
                    return GetStringCollection_Int((int[])value);
                case SheetDataType.Short:
                    return GetStringCollection_Short((short[])value);
                case SheetDataType.Byte:
                    return GetStringCollection_Byte((byte[])value);
                case SheetDataType.Double:
                    return GetStringCollection_Double((double[])value);
                case SheetDataType.Float:
                    return GetStringCollection_Float((float[])value);
                case SheetDataType.String:
                    return GetStringCollection_String((string[])value);
                case SheetDataType.Boolean:
                    return GetStringCollection_Bool((bool[])value);
                case SheetDataType.Color:
                    return GetStringCollection_Color((Color[])value);
                case SheetDataType.Reference:
                    return GetStringCollection_String((string[])value);
                case SheetDataType.Enum:
                    return GetStringCollection_Enum((Array)value);
                case SheetDataType.Component:
                    return GetStringCollection_Component((Array)value);
            }

            throw new MissingMemberException(string.Format(Localization.EXCEPTION_GETSTRINGVALUE_COLLECTION, dataType));
        }

        private static string GetStringCollection_Reference(Array array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += ConvertReference(array.GetValue(i)) + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Long(long[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Int(int[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Short(short[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Byte(byte[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Double(double[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Float(float[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += array[i] + SheetStringDefinitions.SPLIT_STRING;

            return result;
        }

        private static string GetStringCollection_Bool(bool[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += ConvertString(array[i].ToString());

            return result;
        }

        private static string GetStringCollection_Color(Color[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += ConvertString(ColorUtility.ToHtmlStringRGBA(array[i]));

            return result;
        }

        private static string GetStringCollection_String(string[] array)
        {
            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
                result += ConvertString(array[i]);

            return result;
        }

        private static string GetStringCollection_Enum(Array array)
        {
            string result = string.Empty;
            
            for (int i = 0; i < array.Length; i++)
            { 
                Enum enumValue = (Enum)array.GetValue(i);
                object val = Convert.ChangeType(enumValue, enumValue.GetTypeCode());
                result += ConvertString(val.ToString());
            }

            return result;
        }

        private static string GetStringCollection_Component(Array array)
        {
            if (array.Length == 0)
                return string.Empty;

            string result = string.Empty;

            for (int i = 0; i < array.Length; i++)
            {
                UnityEngine.Object component = array.GetValue(i) as UnityEngine.Object;
                if (component == null)
                {
                    result += SheetStringDefinitions.SPLIT_STRING;
                    continue;
                }

                string GUID;
                long ID;
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(component, out GUID, out ID))
                {
                    result += SheetStringDefinitions.SPLIT_STRING;
                    continue;
                }

                result += GUID + SheetStringDefinitions.SPLIT_STRING;
            }

            return result;
        }

        private static string ConvertReference(object value)
        {
            return ((short)value).ToString();
        }

        private static string ConvertComponent(object value)
        {
            UnityEngine.Object component = value as UnityEngine.Object;

            if (component == null)
                return string.Empty;

            string GUID;
            long ID;
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(component, out GUID, out ID))
                return string.Empty;

            return GUID;
        }

        private static string ConvertString(string value)
        {
            return value.Length + " " + value;
        }
        #endregion
    }
}