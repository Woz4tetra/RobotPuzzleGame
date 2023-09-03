class NearestSearch
{
    // From: https://www.geeksforgeeks.org/find-closest-number-array/
    public static (int, int) findSortedClosest(float[] array, float value)
    {
        int n = array.Length;

        // Corner cases
        if (n == 0)
        {
            return (-1, -1);
        }
        if (value <= array[0])
        {
            return (-1, 0);
        }
        if (value >= array[n - 1])
        {
            return (n - 1, -1);
        }

        // Doing binary search
        int i = 0, j = n, mid = 0;
        while (i < j)
        {
            mid = (i + j) / 2;

            if (array[mid] == value)
            {
                return getClosestNextIndex(array.Length, mid);
            }

            // If target is less than array element, then search in left
            if (value < array[mid])
            {
                // If target is greater than previous to mid, return closest of two
                if (mid > 0 && value > array[mid - 1])
                {
                    return (mid - 1, mid);
                }

                // Repeat for left half
                j = mid;
            }

            // If target is greater than mid
            else
            {
                if (mid < n - 1 && value < array[mid + 1])
                {
                    return (mid, mid + 1);
                }
                // update i
                i = mid + 1;
            }
        }

        // Only single element left after search
        return getClosestNextIndex(array.Length, mid);
    }

    public static (int, int) getClosestNextIndex(int arraySize, int index)
    {
        if (index < 0)
        {
            return (-1, index);
        }
        else if (index >= arraySize)
        {
            return (index, -1);
        }
        else
        {
            return (index, index + 1);
        }
    }
}