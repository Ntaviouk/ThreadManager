namespace PotokDll
{
    public class Potok
    {
        public static int[] GenerateRandomArray(int N, int a, int b)
        {
            Random random = new Random();
            int[] array = new int[N];

            for (int i = 0; i < N; i++)
            {
                array[i] = random.Next(a, b + 1);
            }

            return array;
        }

        public static void SelectionSort(int[] array)
        {
            int n = array.Length;

            for (int i = 0; i < n - 1; i++)
            {
                // Знаходимо індекс найменшого елемента
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (array[j] < array[minIndex])
                    {
                        minIndex = j;
                    }
                }

                // Обмінюємо поточний елемент із найменшим
                if (minIndex != i)
                {
                    int temp = array[i];
                    array[i] = array[minIndex];
                    array[minIndex] = temp;
                }
            }
        }
    }
}
