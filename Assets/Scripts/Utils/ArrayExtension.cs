
public static class ArrayExtension
{
    public static bool Contains<T>(this T[] array, T item, int count)
    {
        for (int i = 0; i < count; i++)
            if (item.Equals(array[i]))
                return true;

        return false;
    }
}
