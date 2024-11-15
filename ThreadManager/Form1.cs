using System.Threading;

namespace ThreadManager
{
    public partial class Form1 : Form
    {
        public int[] Array;
        public List<Thread> Threads = new List<Thread>();
        public Form1()
        {
            InitializeComponent();
        }

        static int[] GenerateRandomArray(int N, int a, int b)
        {
            Random random = new Random();
            int[] array = new int[N];

            for (int i = 0; i < N; i++)
            {
                array[i] = random.Next(a, b + 1);
            }

            return array;
        }

        static void SelectionSort(int[] array)
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

        public void SetComboBox(int ThreadsCount)
        {
            comboBoxThreads.Items.Clear();
            for (int i = 0; i < ThreadsCount; i++)
            {
                comboBoxThreads.Items.Add(i);
            }
        }

        static void PrintArray(RichTextBox richTextBox, int[] array)
        {
            richTextBox.Clear();
            foreach (var item in array)
            {
                richTextBox.AppendText(item.ToString() + " ");
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            int N = Int32.Parse(textBoxN.Text);
            int ThreadsCount = Int32.Parse(textBoxThreadsCount.Text);

            // Ініціалізуємо масив
            Array = GenerateRandomArray(N, -100, 100);
            PrintArray(richTextBoxArray, Array);

            // Розбиття масиву на частини для кожного потоку
            int chunkSize = N / ThreadsCount;

            SetComboBox(ThreadsCount);

            for (int i = 0; i < ThreadsCount; i++)
            {
                int start = i * chunkSize;
                int end = (i == ThreadsCount - 1) ? N : (i + 1) * chunkSize;
                int[] part = Array.Skip(start).Take(end - start).ToArray();

                // Створюємо новий потік для сортування частини масиву
                Thread thread = new Thread(() =>
                {
                    SelectionSort(part);
                    // Після сортування частини, зливаємо її назад в основний масив
                    Array = Array.Take(start).Concat(part).Concat(Array.Skip(end)).ToArray();
                });

                Threads.Add(thread);
                thread.Start();
            }
            // Чекаємо, поки всі потоки завершать роботу
            foreach (Thread thread in Threads)
            {
                thread.Join();
            }

            // Після того як всі потоки завершили роботу, виводимо відсортований масив
            PrintArray(richTextBoxSortedArray, Array);
        }

        private void buttonChangePriority_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи вибрано потік та пріоритет
            if (comboBoxThreads.SelectedIndex == -1 || comboBoxPriority.SelectedIndex == -1)
            {
                MessageBox.Show("Будь ласка, виберіть потік та пріоритет.");
                return;
            }

            // Отримуємо обраний потік і вибраний пріоритет
            int selectedThreadIndex = comboBoxThreads.SelectedIndex;
            Thread selectedThread = Threads[selectedThreadIndex];

            // Перевіряємо, чи потік ще не завершив роботу
            if (!selectedThread.IsAlive)
            {
                MessageBox.Show("Цей потік вже завершив свою роботу. Пріоритет не можна змінити.");
                return;
            }

            // Отримуємо обраний пріоритет з ComboBox
            ThreadPriority priority = ThreadPriority.Normal; // За замовчуванням
            switch (comboBoxPriority.SelectedItem.ToString())
            {
                case "Lowest":
                    priority = ThreadPriority.Lowest;
                    break;
                case "BelowNormal":
                    priority = ThreadPriority.BelowNormal;
                    break;
                case "Normal":
                    priority = ThreadPriority.Normal;
                    break;
                case "AboveNormal":
                    priority = ThreadPriority.AboveNormal;
                    break;
                case "Highest":
                    priority = ThreadPriority.Highest;
                    break;
            }

            // Змінюємо пріоритет потоку
            selectedThread.Priority = priority;

            // Показуємо повідомлення, що пріоритет був змінений
            MessageBox.Show($"Пріоритет потоку {selectedThreadIndex} змінено на {priority.ToString()}");
        }

        private void comboBoxThreads_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Перевіряємо, чи вибрано потік
            if (comboBoxThreads.SelectedIndex == -1)
            {
                return; // Якщо не вибрано, нічого не робимо
            }

            // Отримуємо вибраний потік за індексом
            int selectedThreadIndex = comboBoxThreads.SelectedIndex;
            Thread selectedThread = Threads[selectedThreadIndex];

            // Отримуємо пріоритет потоку
            ThreadPriority threadPriority = selectedThread.Priority;

            // Виводимо пріоритет потоку в comboBoxPriority
            switch (threadPriority)
            {
                case ThreadPriority.Lowest:
                    comboBoxPriority.SelectedItem = "Lowest";
                    break;
                case ThreadPriority.BelowNormal:
                    comboBoxPriority.SelectedItem = "BelowNormal";
                    break;
                case ThreadPriority.Normal:
                    comboBoxPriority.SelectedItem = "Normal";
                    break;
                case ThreadPriority.AboveNormal:
                    comboBoxPriority.SelectedItem = "AboveNormal";
                    break;
                case ThreadPriority.Highest:
                    comboBoxPriority.SelectedItem = "Highest";
                    break;
                default:
                    comboBoxPriority.SelectedItem = null;
                    break;
            }
        }
    }
}
