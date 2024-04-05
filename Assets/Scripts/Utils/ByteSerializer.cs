using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

public static partial class ByteSerializer
{
    #region Constants
    private static readonly Type TYPE_LONG;
    private static readonly int BYTE_ARRAY_SIZE_LONG;

    private static readonly Type TYPE_INT;
    private static readonly int BYTE_ARRAY_SIZE_INT;

    private static readonly Type TYPE_UINT;
    private static readonly int BYTE_ARRAY_SIZE_UINT;

    private static readonly Type TYPE_SHORT;
    private static readonly int BYTE_ARRAY_SIZE_SHORT;

    private static readonly Type TYPE_USHORT;
    private static readonly int BYTE_ARRAY_SIZE_USHORT;

    private static readonly Type TYPE_BYTE;
    private static readonly int BYTE_ARRAY_SIZE_BYTE;

    private static readonly Type TYPE_FLOAT;
    private static readonly int BYTE_ARRAY_SIZE_FLOAT;

    private static readonly Type TYPE_DOUBLE;
    private static readonly int BYTE_ARRAY_SIZE_DOUBLE;

    private static readonly Type TYPE_BOOL;
    private static readonly int BYTE_ARRAY_SIZE_BOOL;

    private static readonly Type TYPE_STRING;

    private static Dictionary<Type, short> saveableTypes;
    private static Dictionary<short, Type> loadableTypes;
    private static List<FieldInfo[]> cachedFieldInfos;

    private const short stringSizeForNull = -1;
    private const int arraySizeForNull = -1;
    private const int listSizeForNull = -1;
    private const short objectIDForNull = -1;

    #endregion

    #region Constructor
    static ByteSerializer()
    {
        TYPE_LONG = typeof(long);
        BYTE_ARRAY_SIZE_LONG = Marshal.SizeOf(TYPE_LONG);

        TYPE_INT = typeof(int);
        BYTE_ARRAY_SIZE_INT = Marshal.SizeOf(TYPE_INT);

        TYPE_UINT = typeof(uint);
        BYTE_ARRAY_SIZE_UINT = Marshal.SizeOf(TYPE_UINT);

        TYPE_SHORT = typeof(short);
        BYTE_ARRAY_SIZE_SHORT = Marshal.SizeOf(TYPE_SHORT);

        TYPE_USHORT = typeof(ushort);
        BYTE_ARRAY_SIZE_USHORT = Marshal.SizeOf(TYPE_USHORT);

        TYPE_BYTE = typeof(byte);
        BYTE_ARRAY_SIZE_BYTE = Marshal.SizeOf(TYPE_BYTE);

        TYPE_FLOAT = typeof(float);
        BYTE_ARRAY_SIZE_FLOAT = Marshal.SizeOf(TYPE_FLOAT);

        TYPE_DOUBLE = typeof(double);
        BYTE_ARRAY_SIZE_DOUBLE = Marshal.SizeOf(TYPE_DOUBLE);

        TYPE_STRING = typeof(string);

        //Custom implementation of bool conversion saves 8 booleans in 1 byte.
        TYPE_BOOL = typeof(bool);
        BYTE_ARRAY_SIZE_BOOL = 1;

        saveableTypes = new Dictionary<Type, short>();
        loadableTypes = new Dictionary<short, Type>();
        cachedFieldInfos = new List<FieldInfo[]>();
        AssignConvertableTypes();
    }

    private static void AssignConvertableType(Type type, short ID)
    {
        saveableTypes.Add(type, ID);
        loadableTypes.Add(ID, type);
        cachedFieldInfos.Add(null);
    }
    #endregion

    #region FieldInfo Caching 
    private static FieldInfo[] GetFieldInfos(short ID)
    {
        if (cachedFieldInfos[ID] != null)
            return cachedFieldInfos[ID];

        FieldInfo[] fieldInfos = loadableTypes[ID].GetFields(BindingFlags.Public | BindingFlags.Instance);
        QuickSort(fieldInfos);

        cachedFieldInfos[ID] = fieldInfos;
        return cachedFieldInfos[ID];
    }

    static void QuickSort(FieldInfo[] a)
    {
        QuickSort(a, 0, a.Length - 1);
    }

    static void QuickSort(FieldInfo[] a, int start, int end)
    {
        if (start >= end)
            return;

        FieldInfo num = a[start];
        int i = start, j = end;

        while (i < j)
        {
            while (i < j && a[j].MetadataToken > num.MetadataToken)
                j--;

            a[i] = a[j];

            while (i < j && a[i].MetadataToken < num.MetadataToken)
                i++;

            a[j] = a[i];
        }

        a[i] = num;
        QuickSort(a, start, i - 1);
        QuickSort(a, i + 1, end);
    }
    #endregion

    #region Public methods

    public static bool TryConvertToObject(byte[] byteArray, out object result, out byte[] leftOverBytes)
    {
        result = null;
        leftOverBytes = null;
        if (byteArray.Length < 4)
            return false;

        int targetIndex = 0;
        int byteSize = ReadFromByteArray_Int(byteArray, ref targetIndex);

        if (byteSize != byteArray.Length)
            return false;

        Type type;
        short ID = ReadFromByteArray_Short(byteArray, ref targetIndex);

        if (!loadableTypes.TryGetValue(ID, out type))
        {
            throw new Exception(string.Format("SaveData tried to save an object of type {0} that doesn't exist", type));
        }

        result = FormatterServices.GetUninitializedObject(type);

        FieldInfo[] fieldInfos = GetFieldInfos(ID);

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            object subResult = ReadFromByteArray_Unsafe(byteArray, ref targetIndex, fieldInfo.FieldType);
            fieldInfo.SetValue(result, subResult);
        }

        leftOverBytes = new byte[byteArray.Length - byteSize];
        Array.Copy(byteArray, byteSize, leftOverBytes, 0, leftOverBytes.Length);
        return true;
    }

    public static bool TryConvertToObject_IncludeSize(byte[] byteArray, out object result, out byte[] leftOverBytes, out int bytesUsed)
    {
        result = null;
        leftOverBytes = null;
        bytesUsed = 0;
        if (byteArray.Length < 4)
            return false;

        int targetIndex = 0;
        int byteSize = ReadFromByteArray_Int(byteArray, ref targetIndex);

        if (byteSize != byteArray.Length)
            return false;

        Type type;
        short ID = ReadFromByteArray_Short(byteArray, ref targetIndex);

        if (!loadableTypes.TryGetValue(ID, out type))
        {
            Debug.LogError("Byte array could not be converted to an object");
            return false;
        }

        result = FormatterServices.GetUninitializedObject(type);

        FieldInfo[] fieldInfos = GetFieldInfos(ID);

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            object subResult = ReadFromByteArray_Unsafe(byteArray, ref targetIndex, fieldInfo.FieldType);
            fieldInfo.SetValue(result, subResult);
        }

        leftOverBytes = new byte[byteArray.Length - byteSize];
        Array.Copy(byteArray, byteSize, leftOverBytes, 0, leftOverBytes.Length);
        bytesUsed = byteSize;
        return true;
    }

    public static byte[] ConvertToByteArray_Unsafe_IncludeSize(object convertingObject)
    {
        int byteArraySize = CalculateByteArraySize(convertingObject);
        byteArraySize += CalculateByteArraySize(byteArraySize);

        int targetIndex = 0;
        byte[] byteArray = new byte[byteArraySize];
        WriteToByteArray_Unsafe(byteArray, ref targetIndex, byteArraySize);

        WriteToByteArray_Unsafe(byteArray, ref targetIndex, convertingObject);

        return byteArray;
    }

    public static byte[] ConvertToByteArray_Unsafe(object convertingObject)
    {
        int byteArraySize = CalculateByteArraySize(convertingObject);

        byte[] byteArray = new byte[byteArraySize];

        int targetIndex = 0;

        WriteToByteArray_Unsafe(byteArray, ref targetIndex, convertingObject);

        return byteArray;
    }

    public static byte[] ConvertToByteArray_Safe(object convertingObject)
    {
        int byteArraySize = CalculateByteArraySize(convertingObject);

        int targetIndex = 0;
        byte[] byteArray = new byte[byteArraySize];

        WriteToByteArray_Safe(byteArray, ref targetIndex, convertingObject);

        return byteArray;
    }

    public static T ConvertToObject_Unsafe<T>(byte[] byteArray)
    {
        int targetIndex = 0;
        T result =  (T)ReadFromByteArray_Unsafe(byteArray, ref targetIndex, typeof(T));
        if (targetIndex != byteArray.Length)
            Debug.LogWarning("Something is weird in the savefile");

        return result;
    }

    #endregion

    private static int CalculateByteArraySize(object convertingObject)
    {
        Type type = convertingObject.GetType();

        if (type == TYPE_LONG)
        {
            return BYTE_ARRAY_SIZE_LONG;
        }
        else if (type == TYPE_INT)
        {
            return BYTE_ARRAY_SIZE_INT;
        }
        else if (type == TYPE_UINT)
        {
            return BYTE_ARRAY_SIZE_UINT;
        }
        else if (type == TYPE_SHORT)
        {
            return BYTE_ARRAY_SIZE_SHORT;
        }
        else if (type == TYPE_USHORT)
        {
            return BYTE_ARRAY_SIZE_USHORT;
        }
        else if (type == TYPE_BYTE)
        {
            return BYTE_ARRAY_SIZE_BYTE;
        }
        else if (type == TYPE_FLOAT)
        {
            return BYTE_ARRAY_SIZE_FLOAT;
        }
        else if (type == TYPE_DOUBLE)
        {
            return BYTE_ARRAY_SIZE_DOUBLE;
        }
        else if (type == TYPE_BOOL)
        {
            return BYTE_ARRAY_SIZE_BOOL;
        }
        else if (type == TYPE_STRING)
        {
            if (convertingObject == null)
                return CalculateByteArraySize_NullObject(type);
            string value = (string)convertingObject;
            return Encoding.UTF8.GetByteCount(value) + BYTE_ARRAY_SIZE_SHORT;
        }
        else if (type.IsEnum)
        {
            Enum value = (Enum)convertingObject;
            object val = Convert.ChangeType(convertingObject, value.GetTypeCode());
            return CalculateByteArraySize(val);
        }
        else if (type.IsArray)
        {
            if (convertingObject == null)
                return CalculateByteArraySize_NullObject(type);

            return CalculateByteArraySize_Array(convertingObject);
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (convertingObject == null)
                return CalculateByteArraySize_NullObject(type);

            IList value = (IList)convertingObject;
            int arrayLength = value.Count;
            int byteCount = BYTE_ARRAY_SIZE_INT;

            for (int i = 0; i < arrayLength; i++)
            {
                object objectValue = value[i];
                byteCount += CalculateByteArraySize(objectValue);
            }

            return byteCount;
        }
        else
        {
            short ID;
            int byteCount = BYTE_ARRAY_SIZE_SHORT;
            if (!saveableTypes.TryGetValue(type, out ID))
                throw new Exception(string.Format("SaveData tried to load an object with ID {0} that doesn't exist", ID));

            FieldInfo[] fieldInfos = GetFieldInfos(ID);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsNotSerialized)
                    continue;

                object objectValue = fieldInfo.GetValue(convertingObject);
                byteCount += CalculateByteArraySize(objectValue);
            }

            return byteCount;
        }
    }

    private static int CalculateByteArraySize_Array(object convertingObject)
    {
        Array array = (Array)convertingObject;
        Type arrayType = convertingObject.GetType();
        Type arrayElementType = arrayType.GetElementType();
        int arrayLength = array.Length;

        if (arrayElementType == TYPE_LONG)
        {
            return BYTE_ARRAY_SIZE_LONG * arrayLength + BYTE_ARRAY_SIZE_INT;
        }
        else if (arrayElementType == TYPE_INT)
        {
            return BYTE_ARRAY_SIZE_INT * arrayLength + BYTE_ARRAY_SIZE_INT;
        }
        else if (arrayElementType == TYPE_UINT)
        {
            return BYTE_ARRAY_SIZE_UINT * arrayLength + BYTE_ARRAY_SIZE_INT;
        }
        else if (arrayElementType == TYPE_SHORT)
        {
            return BYTE_ARRAY_SIZE_SHORT * arrayLength + BYTE_ARRAY_SIZE_INT;
        }
        else if (arrayElementType == TYPE_USHORT)
        {
            return BYTE_ARRAY_SIZE_USHORT * arrayLength + BYTE_ARRAY_SIZE_INT;
        }
        else if (arrayElementType == TYPE_BYTE)
        {
            return BYTE_ARRAY_SIZE_BYTE * arrayLength + BYTE_ARRAY_SIZE_INT;
        }
        else if (arrayElementType == TYPE_FLOAT)
        {
            return BYTE_ARRAY_SIZE_FLOAT * arrayLength + BYTE_ARRAY_SIZE_INT;
        }
        else if (arrayElementType == TYPE_DOUBLE)
        {
            return BYTE_ARRAY_SIZE_DOUBLE * arrayLength + BYTE_ARRAY_SIZE_INT;
        }
        else if (arrayElementType == TYPE_BOOL)
        {
            return BYTE_ARRAY_SIZE_BOOL * arrayLength + BYTE_ARRAY_SIZE_INT;
        }
        else if (arrayElementType == TYPE_STRING)
        {
            int byteCount = BYTE_ARRAY_SIZE_INT;
            for (int i = 0; i < arrayLength; i++)
            {
                object arrayItem = array.GetValue(i);
                if (arrayItem == null)
                    byteCount += CalculateByteArraySize_NullObject(arrayElementType);
                else
                {
                    string value = (string)arrayItem;
                    byteCount += Encoding.UTF8.GetByteCount(value) + BYTE_ARRAY_SIZE_SHORT;
                }
            }
            return byteCount;
        }
        else if (arrayElementType.IsEnum)
        {
            int byteCount = BYTE_ARRAY_SIZE_INT;
            for (int i = 0; i < arrayLength; i++)
            {
                object arrayItem = array.GetValue(i);
                Enum arrayItemValue = (Enum)arrayItem;
                object val = Convert.ChangeType(arrayItem, arrayItemValue.GetTypeCode());
                byteCount += CalculateByteArraySize(val);
            }
            return byteCount;
        }
        else if (arrayElementType.IsArray)
        {
            int byteCount = BYTE_ARRAY_SIZE_INT;
            for (int i = 0; i < arrayLength; i++)
            {
                object arrayItem = array.GetValue(i);
                if (arrayItem == null)
                {
                    byteCount += CalculateByteArraySize_NullObject(arrayElementType);
                    continue;
                }

                byteCount += CalculateByteArraySize_Array(arrayItem);

            }
            return byteCount;
        }
        else if (arrayElementType.IsGenericType && arrayElementType.GetGenericTypeDefinition() == typeof(List<>))
        {
            int byteCount = BYTE_ARRAY_SIZE_INT;
            for (int i = 0; i < arrayLength; i++)
            {
                object arrayItem = array.GetValue(i);
                if (arrayItem == null)
                    return CalculateByteArraySize_NullObject(arrayElementType);

                IList list = (IList)arrayItem;
                int listLength = list.Count;
                byteCount += BYTE_ARRAY_SIZE_INT;

                for (int j = 0; j < listLength; j++)
                {
                    object objectValue = list[j];
                    byteCount += CalculateByteArraySize(objectValue);
                }
            }

            return byteCount;
        }
        else
        {
            int byteCount = BYTE_ARRAY_SIZE_INT;
            for (int i = 0; i < arrayLength; i++)
            {
                object arrayItem = array.GetValue(i);
                short ID;
                byteCount += BYTE_ARRAY_SIZE_SHORT;
                if (!saveableTypes.TryGetValue(arrayItem.GetType(), out ID))
                    throw new Exception(string.Format("SaveData tried to load an object with ID {0} that doesn't exist", ID));

                FieldInfo[] fieldInfos = GetFieldInfos(ID);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.IsNotSerialized)
                        continue;

                    object objectValue = fieldInfo.GetValue(arrayItem);
                    byteCount += CalculateByteArraySize(objectValue);
                }
            }

            return byteCount;
        }
    }

    private static int CalculateByteArraySize_NullObject(Type nullObjectType)
    {
        if (nullObjectType == typeof(string))
        {
            return CalculateByteArraySize(stringSizeForNull);
        }
        else if (nullObjectType.IsArray)
        {
            return CalculateByteArraySize(arraySizeForNull);
        }
        else if (nullObjectType.IsGenericType && nullObjectType.GetGenericTypeDefinition() == typeof(List<>))
        {
            return CalculateByteArraySize(listSizeForNull);
        }
        else
        {
            return CalculateByteArraySize(objectIDForNull);
        }
    }

    private static void WriteToByteArray_Unsafe(byte[] byteArray, ref int targetIndex, object convertingObject)
    {
        Type type = convertingObject.GetType();

        if (type == TYPE_LONG)
        {
            long value = (long)convertingObject;
            value.WriteToByteArray_Unsafe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_INT)
        {
            int value = (int)convertingObject;
            value.WriteToByteArray_Unsafe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_UINT)
        {
            uint value = (uint)convertingObject;
            value.WriteToByteArray_Unsafe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_SHORT)
        {
            short value = (short)convertingObject;
            value.WriteToByteArray_Unsafe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_USHORT)
        {
            ushort value = (ushort)convertingObject;
            value.WriteToByteArray_Unsafe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_BYTE)
        {
            byte value = (byte)convertingObject;
            value.WriteToByteArray(byteArray, ref targetIndex);
        }
        else if (type == TYPE_FLOAT)
        {
            float value = (float)convertingObject;
            value.WriteToByteArray_Unsafe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_DOUBLE)
        {
            double value = (double)convertingObject;
            value.WriteToByteArray_Unsafe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_BOOL)
        {
            bool value = (bool)convertingObject;
            value.WriteToByteArray(byteArray, ref targetIndex);
        }
        else if (type == TYPE_STRING)
        {
            if (convertingObject == null)
                WriteToByteArray_Unsafe_NullObject(byteArray, ref targetIndex, type);
            else
            {
                string value = (string)convertingObject;
                value.WriteToByteArray_Unsafe(byteArray, ref targetIndex);
            }
        }
        else if (type.IsEnum)
        {
            Enum value = (Enum)convertingObject;
            object val = Convert.ChangeType(convertingObject, value.GetTypeCode());
            WriteToByteArray_Unsafe(byteArray, ref targetIndex, val);
        }
        else if (type.IsArray)
        {
            if (convertingObject == null)
                WriteToByteArray_Unsafe_NullObject(byteArray, ref targetIndex, type);
            else
                WriteToByteArray_Unsafe_Array(byteArray, ref targetIndex, convertingObject);
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (convertingObject == null)
            {
                WriteToByteArray_Unsafe_NullObject(byteArray, ref targetIndex, type);
            }
            else
            {
                IList value = (IList)convertingObject;
                Type elementType = type.GetGenericArguments()[0];
                int arrayLength = value.Count;
                WriteToByteArray_Unsafe(byteArray, ref targetIndex, arrayLength);

                for (int i = 0; i < arrayLength; i++)
                {
                    object listValue = value[i];
                    WriteToByteArray_Unsafe(byteArray, ref targetIndex, listValue);
                }
            }
        }
        else
        {
            if (convertingObject == null)
            {
                WriteToByteArray_Unsafe_NullObject(byteArray, ref targetIndex, type);
            }
            else
            {
                short ID = saveableTypes[type];
                WriteToByteArray_Unsafe(byteArray, ref targetIndex, ID);

                FieldInfo[] fieldInfos = GetFieldInfos(ID);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.IsNotSerialized)
                        continue;

                    object objectValue = fieldInfo.GetValue(convertingObject);
                    WriteToByteArray_Unsafe(byteArray, ref targetIndex, objectValue);
                }
            }
        }
    }

    private unsafe static void WriteToByteArray_Unsafe_Array(byte[] byteArray, ref int targetIndex, object convertingObject)
    {
        Type arrayType = convertingObject.GetType();
        Array array = (Array)convertingObject;
        Type arrayElementType = arrayType.GetElementType();

        int arrayLength = array.Length;

        WriteToByteArray_Unsafe(byteArray, ref targetIndex, arrayLength);

        if (arrayElementType == TYPE_LONG)
        {
            long[] valueArray = array as long[];
            fixed (byte* ptr1 = byteArray)
            {
                long* targetPtr = (long*)&ptr1[targetIndex];

                fixed (long* ptr2 = valueArray)
                {
                    long* valuePtr = &ptr2[0];
                    for (int i = 0; i < arrayLength; i++)
                    {
                        *targetPtr = *valuePtr;
                        targetPtr++;
                        valuePtr++;
                    }
                }
            }
            targetIndex += BYTE_ARRAY_SIZE_LONG * valueArray.Length;
        }
        else if (arrayElementType == TYPE_INT)
        {
            int[] valueArray = array as int[];
            fixed (byte* ptr1 = byteArray)
            {
                int* targetPtr = (int*)&ptr1[targetIndex];

                fixed (int* ptr2 = valueArray)
                {
                    int* valuePtr = &ptr2[0];
                    for (int i = 0; i < arrayLength; i++)
                    {
                        *targetPtr = *valuePtr;
                        targetPtr++;
                        valuePtr++;
                    }
                }
            }
            targetIndex += BYTE_ARRAY_SIZE_INT * arrayLength;
        }
        else if (arrayElementType == TYPE_UINT)
        {
            uint[] valueArray = array as uint[];
            fixed (byte* ptr1 = byteArray)
            {
                uint* targetPtr = (uint*)&ptr1[targetIndex];

                fixed (uint* ptr2 = valueArray)
                {
                    uint* valuePtr = &ptr2[0];
                    for (uint i = 0; i < arrayLength; i++)
                    {
                        *targetPtr = *valuePtr;
                        targetPtr++;
                        valuePtr++;
                    }
                }
            }
            targetIndex += BYTE_ARRAY_SIZE_UINT * arrayLength;
        }
        else if (arrayElementType == TYPE_SHORT)
        {
            short[] valueArray = array as short[];
            fixed (byte* ptr1 = byteArray)
            {
                short* targetPtr = (short*)&ptr1[targetIndex];

                fixed (short* ptr2 = valueArray)
                {
                    short* valuePtr = &ptr2[0];
                    for (int i = 0; i < arrayLength; i++)
                    {
                        *targetPtr = *valuePtr;
                        targetPtr++;
                        valuePtr++;
                    }
                }
            }
            targetIndex += BYTE_ARRAY_SIZE_SHORT * arrayLength;
        }
        else if (arrayElementType == TYPE_USHORT)
        {
            ushort[] valueArray = array as ushort[];
            fixed (byte* ptr1 = byteArray)
            {
                ushort* targetPtr = (ushort*)&ptr1[targetIndex];

                fixed (ushort* ptr2 = valueArray)
                {
                    ushort* valuePtr = &ptr2[0];
                    for (int i = 0; i < arrayLength; i++)
                    {
                        *targetPtr = *valuePtr;
                        targetPtr++;
                        valuePtr++;
                    }
                }
            }
            targetIndex += BYTE_ARRAY_SIZE_USHORT * arrayLength;
        }
        else if (arrayElementType == TYPE_BYTE)
        {
            byte[] valueArray = array as byte[];
            fixed (byte* ptr1 = byteArray)
            {
                byte* targetPtr = &ptr1[targetIndex];

                fixed (byte* ptr2 = valueArray)
                {
                    byte* valuePtr = &ptr2[0];
                    for (int i = 0; i < arrayLength; i++)
                    {
                        *targetPtr = *valuePtr;
                        targetPtr++;
                        valuePtr++;
                    }
                }
            }
            targetIndex += BYTE_ARRAY_SIZE_BYTE * arrayLength;
        }
        else if (arrayElementType == TYPE_FLOAT)
        {
            float[] valueArray = array as float[];
            fixed (byte* ptr1 = byteArray)
            {
                float* targetPtr = (float*)&ptr1[targetIndex];

                fixed (float* ptr2 = valueArray)
                {
                    float* valuePtr = &ptr2[0];
                    for (int i = 0; i < arrayLength; i++)
                    {
                        *targetPtr = *valuePtr;
                        targetPtr++;
                        valuePtr++;
                    }
                }
            }
            targetIndex += BYTE_ARRAY_SIZE_FLOAT * arrayLength;
        }
        else if (arrayElementType == TYPE_DOUBLE)
        {
            double[] valueArray = array as double[];
            fixed (byte* ptr1 = byteArray)
            {
                double* targetPtr = (double*)&ptr1[targetIndex];

                fixed (double* ptr2 = valueArray)
                {
                    double* valuePtr = &ptr2[0];
                    for (int i = 0; i < arrayLength; i++)
                    {
                        *targetPtr = *valuePtr;
                        targetPtr++;
                        valuePtr++;
                    }
                }
            }
            targetIndex += BYTE_ARRAY_SIZE_DOUBLE * arrayLength;
        }
        else if (arrayElementType == TYPE_BOOL)
        {
            bool[] valueArray = array as bool[];
            fixed (byte* ptr1 = byteArray)
            {
                byte* targetPtr = &ptr1[targetIndex];

                fixed (bool* ptr2 = valueArray)
                {
                    bool* valuePtr = &ptr2[0];
                    for (int i = 0; i < arrayLength; i++)
                    {
                        *targetPtr = *valuePtr ? (byte)1 : (byte)0;
                        targetPtr++;
                        valuePtr++;
                    }
                }
            }
            targetIndex += BYTE_ARRAY_SIZE_BOOL * arrayLength;
        }
        else if (arrayElementType == TYPE_STRING)
        {
            string[] valueArray = array as string[];

            for (int i = 0; i < arrayLength; i++)
            {
                string subValue = valueArray[i];
                if (subValue == null)
                    WriteToByteArray_Unsafe_NullObject(byteArray, ref targetIndex, typeof(string));
                else
                    subValue.WriteToByteArray_Unsafe(byteArray, ref targetIndex);
            }
        }
        else if (arrayElementType.IsEnum)
        {
            for (int i = 0; i < arrayLength; i++)
            {
                Enum subValue = (Enum)array.GetValue(i);
                object val = Convert.ChangeType(subValue, subValue.GetTypeCode());
                WriteToByteArray_Unsafe(byteArray, ref targetIndex, val);
            }
        }
        else if (arrayElementType.IsArray)
        {
            for (int i = 0; i < arrayLength; i++)
            {
                object subValue = array.GetValue(i);
                if (subValue == null)
                    WriteToByteArray_Unsafe_NullObject(byteArray, ref targetIndex, subValue.GetType());
                else
                    WriteToByteArray_Unsafe_Array(byteArray, ref targetIndex, subValue);
            }
        }
        else if (arrayElementType.IsGenericType && arrayElementType.GetGenericTypeDefinition() == typeof(List<>))
        {
            for (int i = 0; i < arrayLength; i++)
            {
                object subValue = array.GetValue(i);
                if (subValue == null)
                    WriteToByteArray_Unsafe_NullObject(byteArray, ref targetIndex, subValue.GetType());
                else
                {
                    Type subType = subValue.GetType();
                    IList list = (IList)subValue;
                    Type elementType = subType.GetGenericArguments()[0];
                    int listLength = list.Count;
                    WriteToByteArray_Unsafe(byteArray, ref targetIndex, listLength);

                    for (int j = 0; j < arrayLength; j++)
                    {
                        object listItem = list[j];
                        WriteToByteArray_Unsafe(byteArray, ref targetIndex, listItem);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < arrayLength; i++)
            {
                object subValue = array.GetValue(i);
                Type subType = subValue.GetType();

                if (subValue == null)
                {
                    WriteToByteArray_Unsafe_NullObject(byteArray, ref targetIndex, subType);
                    continue;
                }

                short ID = saveableTypes[subType];

                WriteToByteArray_Unsafe(byteArray, ref targetIndex, ID);

                FieldInfo[] fieldInfos = GetFieldInfos(ID);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.IsNotSerialized)
                        continue;

                    object objectValue = fieldInfo.GetValue(subValue);
                    WriteToByteArray_Unsafe(byteArray, ref targetIndex, objectValue);
                }
            }
        }
    }

    private static void WriteToByteArray_Safe(byte[] byteArray, ref int targetIndex, object convertingObject)
    {
        Type type = convertingObject.GetType();

        if (type == TYPE_LONG)
        {
            long value = (long)convertingObject;
            value.WriteToByteArray_Safe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_INT)
        {
            int value = (int)convertingObject;
            value.WriteToByteArray_Safe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_UINT)
        {
            uint value = (uint)convertingObject;
            value.WriteToByteArray_Safe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_SHORT)
        {
            short value = (short)convertingObject;
            value.WriteToByteArray_Safe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_USHORT)
        {
            ushort value = (ushort)convertingObject;
            value.WriteToByteArray_Safe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_BYTE)
        {
            byte value = (byte)convertingObject;
            value.WriteToByteArray(byteArray, ref targetIndex);
        }
        else if (type == TYPE_FLOAT)
        {
            float value = (float)convertingObject;
            value.WriteToByteArray_Safe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_DOUBLE)
        {
            double value = (double)convertingObject;
            value.WriteToByteArray_Safe(byteArray, ref targetIndex);
        }
        else if (type == TYPE_BOOL)
        {
            bool value = (bool)convertingObject;
            value.WriteToByteArray(byteArray, ref targetIndex);
        }
        else if (type == TYPE_STRING)
        {
            string value = (string)convertingObject;
            value.WriteToByteArray_Safe(byteArray, ref targetIndex);
        }
        else if (type.IsEnum)
        {
            Enum value = (Enum)convertingObject;
            object val = Convert.ChangeType(convertingObject, value.GetTypeCode());
            WriteToByteArray_Safe(byteArray, ref targetIndex, val);
        }
        else if (type.IsArray)
        {
            Array value = (Array)convertingObject;
            Type elementType = type.GetElementType();
            int arrayLength = value.Length;
            WriteToByteArray_Safe(byteArray, ref targetIndex, arrayLength);

            for (int i = 0; i < arrayLength; i++)
            {
                object objectValue = value.GetValue(i);
                if (objectValue != null)
                    WriteToByteArray_Safe(byteArray, ref targetIndex, objectValue);
                else
                    WriteToByteArray_Safe_NullObject(byteArray, ref targetIndex, elementType);
            }
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            IList value = (IList)convertingObject;
            Type elementType = type.GetGenericArguments()[0];
            int arrayLength = value.Count;
            WriteToByteArray_Safe(byteArray, ref targetIndex, arrayLength);

            for (int i = 0; i < arrayLength; i++)
            {
                object objectValue = value[i];
                if (objectValue != null)
                    WriteToByteArray_Safe(byteArray, ref targetIndex, objectValue);
                else
                    WriteToByteArray_Safe_NullObject(byteArray, ref targetIndex, elementType);
            }
        }
        else
        {
            short ID;
            if (!saveableTypes.TryGetValue(type, out ID))
                throw new Exception(string.Format("SaveData tried to save an object of type {0} that doesn't exist", type));

            WriteToByteArray_Safe(byteArray, ref targetIndex, ID);

            FieldInfo[] fieldInfos = GetFieldInfos(ID);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsNotSerialized)
                    continue;

                object objectValue = fieldInfo.GetValue(convertingObject);

                if (objectValue != null)
                    WriteToByteArray_Safe(byteArray, ref targetIndex, objectValue);
                else
                    WriteToByteArray_Safe_NullObject(byteArray, ref targetIndex, fieldInfo.FieldType);
            }
        }
    }

    private static void WriteToByteArray_Safe_NullObject(byte[] byteArray, ref int targetIndex, Type nullObjectType)
    {
        if (nullObjectType == typeof(string))
        {
            WriteToByteArray_Safe(byteArray, ref targetIndex, stringSizeForNull);
        }
        else if (nullObjectType.IsArray)
        {
            WriteToByteArray_Safe(byteArray, ref targetIndex, arraySizeForNull);
        }
        else if (nullObjectType.IsGenericType && nullObjectType.GetGenericTypeDefinition() == typeof(List<>))
        {
            WriteToByteArray_Safe(byteArray, ref targetIndex, listSizeForNull);
        }
        else
        {
            WriteToByteArray_Safe(byteArray, ref targetIndex, objectIDForNull);
        }
    }

    private static void WriteToByteArray_Unsafe_NullObject(byte[] array, ref int targetIndex, Type nullObjectType)
    {
        if (nullObjectType == typeof(string))
        {
            WriteToByteArray_Unsafe(array, ref targetIndex, stringSizeForNull);
        }
        else if (nullObjectType.IsArray)
        {
            WriteToByteArray_Unsafe(array, ref targetIndex, arraySizeForNull);
        }
        else if (nullObjectType.IsGenericType && nullObjectType.GetGenericTypeDefinition() == typeof(List<>))
        {
            WriteToByteArray_Unsafe(array, ref targetIndex, listSizeForNull);
        }
        else
        {
            WriteToByteArray_Unsafe(array, ref targetIndex, objectIDForNull);
        }
    }

    private static object ReadFromByteArray_Unsafe(byte[] byteArray, ref int targetIndex, Type type)
    {
        if (type == TYPE_LONG)
        {
            return ReadFromByteArray_Long(byteArray, ref targetIndex);
        }
        else if (type == TYPE_INT)
        {
            return ReadFromByteArray_Int(byteArray, ref targetIndex);
        }
        else if (type == TYPE_UINT)
        {
            return ReadFromByteArray_UInt(byteArray, ref targetIndex);
        }
        else if (type == TYPE_SHORT)
        {
            return ReadFromByteArray_Short(byteArray, ref targetIndex);
        }
        else if (type == TYPE_USHORT)
        {
            return ReadFromByteArray_UShort(byteArray, ref targetIndex);
        }
        else if (type == TYPE_BYTE)
        {
            return ReadFromByteArray_Byte(byteArray, ref targetIndex);
        }
        else if (type == TYPE_FLOAT)
        {
            return ReadFromByteArray_Float(byteArray, ref targetIndex);
        }
        else if (type == TYPE_DOUBLE)
        {
            return ReadFromByteArray_Double(byteArray, ref targetIndex);
        }
        else if (type == TYPE_BOOL)
        {
            return ReadFromByteArray_Boolean(byteArray, ref targetIndex);
        }
        else if (type == TYPE_STRING)
        {
            return ReadFromByteArray_String(byteArray, ref targetIndex);
        }
        else if (type.IsEnum)
        {
            Type subType = Enum.GetUnderlyingType(type);
            object val = ReadFromByteArray_Unsafe(byteArray, ref targetIndex, subType);
            return Enum.ToObject(type, val);
        }
        else if (type.IsArray)
        {
            return ReadFromByteArray_Unsafe_Array(byteArray, ref targetIndex, type);
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            int arraySize = ReadFromByteArray_Int(byteArray, ref targetIndex);

            if (arraySize == listSizeForNull)
                return null;

            Type elementType = type.GetGenericArguments()[0];
            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(int) });

            IList list = (IList)constructor.Invoke(new object[] { arraySize });

            for (int i = 0; i < arraySize; i++)
            {
                object result = ReadFromByteArray_Unsafe(byteArray, ref targetIndex, elementType);
                list.Add(result);
            }

            return list;
        }
        else
        {
            short ID = ReadFromByteArray_Short(byteArray, ref targetIndex);
            if (ID == objectIDForNull)
                return null;

            Type subType = loadableTypes[ID];

            object subObject = FormatterServices.GetUninitializedObject(subType);

            FieldInfo[] fieldInfos = GetFieldInfos(ID);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsNotSerialized)
                    continue;

                object result = ReadFromByteArray_Unsafe(byteArray, ref targetIndex, fieldInfo.FieldType);
                fieldInfo.SetValue(subObject, result);
            }

            return subObject;
        }
    }

    private unsafe static object ReadFromByteArray_Unsafe_Array(byte[] byteArray, ref int targetIndex, Type type)
    {
        int arraySize = ReadFromByteArray_Int(byteArray, ref targetIndex);

        if (arraySize == arraySizeForNull)
            return null;

        Type elementType = type.GetElementType();
        Array value = Array.CreateInstance(elementType, arraySize);

        if (elementType == TYPE_LONG)
        {
            long[] valueArray = value as long[];
            fixed (byte* ptr1 = byteArray)
            {
                long* targetPtr = (long*)&ptr1[targetIndex];
                fixed (long* ptr2 = valueArray)
                {
                    long* valuePtr = &ptr2[0];

                    for (int i = 0; i < arraySize; i++)
                    {
                        *valuePtr = *targetPtr;
                        valuePtr++;
                        targetPtr++;
                    }
                }
            }

            targetIndex += BYTE_ARRAY_SIZE_LONG * arraySize;
        }
        else if (elementType == TYPE_INT)
        {
            int[] valueArray = value as int[];
            fixed (byte* ptr1 = byteArray)
            {
                int* targetPtr = (int*)&ptr1[targetIndex];
                fixed (int* ptr2 = valueArray)
                {
                    int* valuePtr = &ptr2[0];

                    for (int i = 0; i < arraySize; i++)
                    {
                        *valuePtr = *targetPtr;
                        valuePtr++;
                        targetPtr++;
                    }
                }
            }

            targetIndex += BYTE_ARRAY_SIZE_INT * arraySize;
        }
        else if (elementType == TYPE_UINT)
        {
            uint[] valueArray = value as uint[];
            fixed (byte* ptr1 = byteArray)
            {
                uint* targetPtr = (uint*)&ptr1[targetIndex];
                fixed (uint* ptr2 = valueArray)
                {
                    uint* valuePtr = &ptr2[0];

                    for (int i = 0; i < arraySize; i++)
                    {
                        *valuePtr = *targetPtr;
                        valuePtr++;
                        targetPtr++;
                    }
                }
            }

            targetIndex += BYTE_ARRAY_SIZE_UINT * arraySize;
        }
        else if (elementType == TYPE_SHORT)
        {
            short[] valueArray = value as short[];
            fixed (byte* ptr1 = byteArray)
            {
                short* targetPtr = (short*)&ptr1[targetIndex];
                fixed (short* ptr2 = valueArray)
                {
                    short* valuePtr = &ptr2[0];

                    for (int i = 0; i < arraySize; i++)
                    {
                        *valuePtr = *targetPtr;
                        valuePtr++;
                        targetPtr++;
                    }
                }
            }

            targetIndex += BYTE_ARRAY_SIZE_SHORT * arraySize;
        }
        else if (elementType == TYPE_USHORT)
        {
            ushort[] valueArray = value as ushort[];
            fixed (byte* ptr1 = byteArray)
            {
                ushort* targetPtr = (ushort*)&ptr1[targetIndex];
                fixed (ushort* ptr2 = valueArray)
                {
                    ushort* valuePtr = &ptr2[0];

                    for (int i = 0; i < arraySize; i++)
                    {
                        *valuePtr = *targetPtr;
                        valuePtr++;
                        targetPtr++;
                    }
                }
            }

            targetIndex += BYTE_ARRAY_SIZE_USHORT * arraySize;
        }
        else if (elementType == TYPE_BYTE)
        {
            byte[] valueArray = value as byte[];
            fixed (byte* ptr1 = byteArray)
            {
                byte* targetPtr = &ptr1[targetIndex];
                fixed (byte* ptr2 = valueArray)
                {
                    byte* valuePtr = &ptr2[0];

                    for (int i = 0; i < arraySize; i++)
                    {
                        *valuePtr = *targetPtr;
                        valuePtr++;
                        targetPtr++;
                    }
                }
            }

            targetIndex += BYTE_ARRAY_SIZE_BYTE * arraySize;
        }
        else if (elementType == TYPE_FLOAT)
        {
            float[] valueArray = value as float[];
            fixed (byte* ptr1 = byteArray)
            {
                float* targetPtr = (float*)&ptr1[targetIndex];
                fixed (float* ptr2 = valueArray)
                {
                    float* valuePtr = &ptr2[0];

                    for (int i = 0; i < arraySize; i++)
                    {
                        *valuePtr = *targetPtr;
                        valuePtr++;
                        targetPtr++;
                    }
                }
            }

            targetIndex += BYTE_ARRAY_SIZE_FLOAT * arraySize;
        }
        else if (elementType == TYPE_DOUBLE)
        {
            double[] valueArray = value as double[];
            fixed (byte* ptr1 = byteArray)
            {
                double* targetPtr = (double*)&ptr1[targetIndex];
                fixed (double* ptr2 = valueArray)
                {
                    double* valuePtr = &ptr2[0];

                    for (int i = 0; i < arraySize; i++)
                    {
                        *valuePtr = *targetPtr;
                        valuePtr++;
                        targetPtr++;
                    }
                }
            }

            targetIndex += BYTE_ARRAY_SIZE_DOUBLE * arraySize;
        }
        else if (elementType == TYPE_BOOL)
        {
            bool[] valueArray = value as bool[];
            fixed (byte* ptr1 = byteArray)
            {
                byte* targetPtr = &ptr1[targetIndex];
                fixed (bool* ptr2 = valueArray)
                {
                    bool* valuePtr = &ptr2[0];

                    for (int i = 0; i < arraySize; i++)
                    {
                        *valuePtr = *targetPtr == 1;
                        valuePtr++;
                        targetPtr++;
                    }
                }
            }

            targetIndex += BYTE_ARRAY_SIZE_BOOL * arraySize;
        }
        else if (elementType == TYPE_STRING)
        {
            for (int i = 0; i < arraySize; i++)
            {
                object result = ReadFromByteArray_String(byteArray, ref targetIndex);
                value.SetValue(result, i);
            }
        }
        else if (elementType.IsEnum)
        {
            Type subType = Enum.GetUnderlyingType(elementType);
            for (int i = 0; i < arraySize; i++)
            {
                object val = ReadFromByteArray_Unsafe(byteArray, ref targetIndex, subType);
                object result = Enum.ToObject(elementType, val);
                value.SetValue(result, i);
            }
        }
        else if (elementType.IsArray)
        {
            for (int i = 0; i < arraySize; i++)
            {
                object result = ReadFromByteArray_Unsafe_Array(byteArray, ref targetIndex, type);
                value.SetValue(result, i);
            }

            return value;
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            for (int i = 0; i < arraySize; i++)
            {
                int listSize = ReadFromByteArray_Int(byteArray, ref targetIndex);

                if (listSize == listSizeForNull)
                    return null;

                Type subElementType = type.GetGenericArguments()[0];
                ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(int) });

                IList list = (IList)constructor.Invoke(new object[] { listSize });

                for (int j = 0; j < listSize; j++)
                {
                    object result = ReadFromByteArray_Unsafe(byteArray, ref targetIndex, subElementType);
                    list.Add(result);
                }

                value.SetValue(list, i);
            }
        }
        else
        {
            for (int i = 0; i < arraySize; i++)
            {
                short ID = ReadFromByteArray_Short(byteArray, ref targetIndex);
                if (ID == objectIDForNull)
                    return null;

                Type subType;
                if (!loadableTypes.TryGetValue(ID, out subType))
                    throw new Exception(string.Format("SaveData tried to load an object with ID {0} that doesn't exist", ID));

                object subObject = FormatterServices.GetUninitializedObject(subType);

                FieldInfo[] fieldInfos = GetFieldInfos(ID);
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.IsNotSerialized)
                        continue;

                    object result = ReadFromByteArray_Unsafe(byteArray, ref targetIndex, fieldInfo.FieldType);
                    fieldInfo.SetValue(subObject, result);
                }

                value.SetValue(subObject, i);
            }
        }

        return value;
    }

    #region Long
    private static unsafe void WriteToByteArray_Unsafe(this long value, byte[] byteArray, ref int targetIndex)
    {
        fixed (byte* ptr = byteArray)
        {
            byte* targetPtr = &ptr[targetIndex];
            long* valuePtr = (long*)targetPtr;
            *valuePtr = value;
        }
        targetIndex += BYTE_ARRAY_SIZE_LONG;
    }

    private static void WriteToByteArray_Safe(this long value, byte[] byteArray, ref int targetIndex)
    {
        byte[] subArray = BitConverter.GetBytes(value);
        Array.Copy(subArray, 0, byteArray, targetIndex, BYTE_ARRAY_SIZE_LONG);
        targetIndex += BYTE_ARRAY_SIZE_LONG;
    }


    private static long ReadFromByteArray_Long(byte[] byteArray, ref int targetIndex)
    {
        long value = BitConverter.ToInt64(byteArray, targetIndex);
        targetIndex += BYTE_ARRAY_SIZE_LONG;
        return value;
    }
    #endregion

    #region Int
    private static unsafe void WriteToByteArray_Unsafe(this int value, byte[] byteArray, ref int targetIndex)
    {
        fixed (byte* ptr = byteArray)
        {
            byte* targetPtr = &ptr[targetIndex];
            int* valuePtr = (int*)targetPtr;
            *valuePtr = value;
        }
        targetIndex += BYTE_ARRAY_SIZE_INT;
    }

    private static void WriteToByteArray_Safe(this int value, byte[] byteArray, ref int targetIndex)
    {
        byte[] subArray = BitConverter.GetBytes(value);
        Array.Copy(subArray, 0, byteArray, targetIndex, BYTE_ARRAY_SIZE_INT);
        targetIndex += BYTE_ARRAY_SIZE_INT;
    }

    private static int ReadFromByteArray_Int(byte[] byteArray, ref int targetIndex)
    {
        int value = BitConverter.ToInt32(byteArray, targetIndex);
        targetIndex += BYTE_ARRAY_SIZE_INT;
        return value;
    }
    #endregion


    #region UInt
    private static unsafe void WriteToByteArray_Unsafe(this uint value, byte[] byteArray, ref int targetIndex)
    {
        fixed (byte* ptr = byteArray)
        {
            byte* targetPtr = &ptr[targetIndex];
            uint* valuePtr = (uint*)targetPtr;
            *valuePtr = value;
        }
        targetIndex += BYTE_ARRAY_SIZE_UINT;
    }

    private static void WriteToByteArray_Safe(this uint value, byte[] byteArray, ref int targetIndex)
    {
        byte[] subArray = BitConverter.GetBytes(value);
        Array.Copy(subArray, 0, byteArray, targetIndex, BYTE_ARRAY_SIZE_UINT);
        targetIndex += BYTE_ARRAY_SIZE_UINT;
    }

    private static uint ReadFromByteArray_UInt(byte[] byteArray, ref int targetIndex)
    {
        uint value = BitConverter.ToUInt32(byteArray, targetIndex);
        targetIndex += BYTE_ARRAY_SIZE_UINT;
        return value;
    }
    #endregion

    #region Short
    private static unsafe void WriteToByteArray_Unsafe(this short value, byte[] byteArray, ref int targetIndex)
    {
        fixed (byte* ptr = byteArray)
        {
            byte* targetPtr = &ptr[targetIndex];
            short* valuePtr = (short*)targetPtr;
            *valuePtr = value;
        }
        targetIndex += BYTE_ARRAY_SIZE_SHORT;
    }

    private static void WriteToByteArray_Safe(this short value, byte[] byteArray, ref int targetIndex)
    {
        byte[] subArray = BitConverter.GetBytes(value);
        Array.Copy(subArray, 0, byteArray, targetIndex, BYTE_ARRAY_SIZE_SHORT);
        targetIndex += BYTE_ARRAY_SIZE_SHORT;
    }

    private static short ReadFromByteArray_Short(byte[] byteArray, ref int targetIndex)
    {
        short value = BitConverter.ToInt16(byteArray, targetIndex);
        targetIndex += BYTE_ARRAY_SIZE_SHORT;
        return value;
    }
    #endregion


    #region UShort
    private static unsafe void WriteToByteArray_Unsafe(this ushort value, byte[] byteArray, ref int targetIndex)
    {
        fixed (byte* ptr = byteArray)
        {
            byte* targetPtr = &ptr[targetIndex];
            ushort* valuePtr = (ushort*)targetPtr;
            *valuePtr = value;
        }
        targetIndex += BYTE_ARRAY_SIZE_USHORT;
    }

    private static void WriteToByteArray_Safe(this ushort value, byte[] byteArray, ref int targetIndex)
    {
        byte[] subArray = BitConverter.GetBytes(value);
        Array.Copy(subArray, 0, byteArray, targetIndex, BYTE_ARRAY_SIZE_USHORT);
        targetIndex += BYTE_ARRAY_SIZE_USHORT;
    }

    private static ushort ReadFromByteArray_UShort(byte[] byteArray, ref int targetIndex)
    {
        ushort value = BitConverter.ToUInt16(byteArray, targetIndex);
        targetIndex += BYTE_ARRAY_SIZE_USHORT;
        return value;
    }
    #endregion

    #region Byte
    private static void WriteToByteArray(this byte value, byte[] byteArray, ref int targetIndex)
    {
        byteArray[targetIndex] = value;
        targetIndex += BYTE_ARRAY_SIZE_BYTE;
    }

    private static byte ReadFromByteArray_Byte(byte[] byteArray, ref int targetIndex)
    {
        byte value = byteArray[targetIndex];
        targetIndex += BYTE_ARRAY_SIZE_BYTE;
        return value;
    }
    #endregion

    #region Float
    private static unsafe void WriteToByteArray_Unsafe(this float value, byte[] byteArray, ref int targetIndex)
    {
        fixed (byte* ptr = byteArray)
        {
            byte* targetPtr = &ptr[targetIndex];
            float* valuePtr = (float*)targetPtr;
            *valuePtr = value;
        }
        targetIndex += BYTE_ARRAY_SIZE_FLOAT;
    }

    private static void WriteToByteArray_Safe(this float value, byte[] byteArray, ref int targetIndex)
    {
        byte[] subArray = BitConverter.GetBytes(value);
        Array.Copy(subArray, 0, byteArray, targetIndex, BYTE_ARRAY_SIZE_FLOAT);
        targetIndex += BYTE_ARRAY_SIZE_FLOAT;
    }

    private static float ReadFromByteArray_Float(byte[] byteArray, ref int targetIndex)
    {
        float value = BitConverter.ToSingle(byteArray, targetIndex);
        targetIndex += BYTE_ARRAY_SIZE_FLOAT;
        return value;
    }
    #endregion

    #region Double
    private static unsafe void WriteToByteArray_Unsafe(this double value, byte[] byteArray, ref int targetIndex)
    {
        fixed (byte* ptr = byteArray)
        {
            byte* targetPtr = &ptr[targetIndex];
            double* valuePtr = (double*)targetPtr;
            *valuePtr = value;
        }
        targetIndex += BYTE_ARRAY_SIZE_DOUBLE;
    }

    private static void WriteToByteArray_Safe(this double value, byte[] byteArray, ref int targetIndex)
    {
        byte[] subArray = BitConverter.GetBytes(value);
        Array.Copy(subArray, 0, byteArray, targetIndex, BYTE_ARRAY_SIZE_DOUBLE);
        targetIndex += BYTE_ARRAY_SIZE_DOUBLE;
    }

    private static double ReadFromByteArray_Double(byte[] byteArray, ref int targetIndex)
    {
        double value = BitConverter.ToDouble(byteArray, targetIndex);
        targetIndex += BYTE_ARRAY_SIZE_DOUBLE;
        return value;
    }
    #endregion

    #region Boolean
    private static unsafe void WriteToByteArray(this bool value, byte[] byteArray, ref int targetIndex)
    {
        byte[] subArray = BitConverter.GetBytes(value);
        Array.Copy(subArray, 0, byteArray, targetIndex, BYTE_ARRAY_SIZE_BOOL);
        targetIndex += BYTE_ARRAY_SIZE_BOOL;
    }

    private static bool ReadFromByteArray_Boolean(byte[] byteArray, ref int targetIndex)
    {
        bool value = BitConverter.ToBoolean(byteArray, targetIndex);
        targetIndex += BYTE_ARRAY_SIZE_BOOL;
        return value;
    }
    #endregion

    #region String
    private static unsafe void WriteToByteArray_Unsafe(this string value, byte[] byteArray, ref int targetIndex)
    {
        int stringSizeIndex = targetIndex;
        short stringSize = (short)Encoding.UTF8.GetBytes(value, 0, value.Length, byteArray, targetIndex + BYTE_ARRAY_SIZE_SHORT);
        stringSize.WriteToByteArray_Unsafe(byteArray, ref stringSizeIndex);
        targetIndex += stringSize + BYTE_ARRAY_SIZE_SHORT;
    }

    private static unsafe void WriteToByteArray_Safe(this string value, byte[] byteArray, ref int targetIndex)
    {
        int stringSizeIndex = targetIndex;
        short stringSize = (short)Encoding.UTF8.GetBytes(value, 0, value.Length, byteArray, targetIndex + BYTE_ARRAY_SIZE_SHORT);
        stringSize.WriteToByteArray_Safe(byteArray, ref stringSizeIndex);
        targetIndex += stringSize + BYTE_ARRAY_SIZE_SHORT;
    }

    private static string ReadFromByteArray_String(byte[] byteArray, ref int targetIndex)
    {
        short stringSize = ReadFromByteArray_Short(byteArray, ref targetIndex);
        if (stringSize == stringSizeForNull)
            return null;

        string result = Encoding.UTF8.GetString(byteArray, targetIndex, stringSize);
        targetIndex += stringSize;
        return result;
    }
    #endregion
}
