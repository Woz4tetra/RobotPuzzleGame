class NearestSearch
{
    // From: https://www.geeksforgeeks.org/find-closest-number-array/
    public static int findSortedClosest(float[] array, float value)
    {
        int n = array.Length;

        // Corner cases
        if (n == 0)
        {
            return -1;
        }
        if (value <= array[0])
        {
            return 0;
        }
        if (value >= array[n - 1])
        {
            return n - 1;
        }

        // Doing binary search
        int i = 0, j = n, mid = 0;
        while (i < j)
        {
            mid = (i + j) / 2;

            if (array[mid] == value)
            {
                return mid;
            }

            // If target is less than array element, then search in left
            if (value < array[mid])
            {
                // If target is greater than previous to mid, return closest of two
                if (mid > 0 && value > array[mid - 1])
                {
                    return getClosest(array, mid - 1, mid, value);
                }

                // Repeat for left half
                j = mid;
            }

            // If target is greater than mid
            else
            {
                if (mid < n - 1 && value < array[mid + 1])
                {
                    return getClosest(array, mid, mid + 1, value);
                }
                // update i
                i = mid + 1;
            }
        }

        // Only single element left after search
        return mid;
    }

    /**
     Method to compare which one is the more close We find the closest by taking the difference between the target 
     and both values. It assumes that val2 is greater than val1 and target lies between these two.
     */
    public static int getClosest(float[] array, int index1, int index2, float target)
    {
        if (target - array[index1] >= array[index2] - target)
        {
            return index2;
        }
        else
        {
            return index1;
        }
    }
}