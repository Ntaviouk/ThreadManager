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
                // ��������� ������ ���������� ��������
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (array[j] < array[minIndex])
                    {
                        minIndex = j;
                    }
                }

                // �������� �������� ������� �� ���������
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

            // ����������� �����
            Array = GenerateRandomArray(N, -100, 100);
            PrintArray(richTextBoxArray, Array);

            // �������� ������ �� ������� ��� ������� ������
            int chunkSize = N / ThreadsCount;

            SetComboBox(ThreadsCount);

            for (int i = 0; i < ThreadsCount; i++)
            {
                int start = i * chunkSize;
                int end = (i == ThreadsCount - 1) ? N : (i + 1) * chunkSize;
                int[] part = Array.Skip(start).Take(end - start).ToArray();

                // ��������� ����� ���� ��� ���������� ������� ������
                Thread thread = new Thread(() =>
                {
                    SelectionSort(part);
                    // ϳ��� ���������� �������, ������� �� ����� � �������� �����
                    Array = Array.Take(start).Concat(part).Concat(Array.Skip(end)).ToArray();
                });

                Threads.Add(thread);
                thread.Start();
            }
            // ������, ���� �� ������ ��������� ������
            foreach (Thread thread in Threads)
            {
                thread.Join();
            }

            // ϳ��� ���� �� �� ������ ��������� ������, �������� ������������ �����
            PrintArray(richTextBoxSortedArray, Array);
        }

        private void buttonChangePriority_Click(object sender, EventArgs e)
        {
            // ����������, �� ������� ���� �� ��������
            if (comboBoxThreads.SelectedIndex == -1 || comboBoxPriority.SelectedIndex == -1)
            {
                MessageBox.Show("���� �����, ������� ���� �� ��������.");
                return;
            }

            // �������� ������� ���� � �������� ��������
            int selectedThreadIndex = comboBoxThreads.SelectedIndex;
            Thread selectedThread = Threads[selectedThreadIndex];

            // ����������, �� ���� �� �� �������� ������
            if (!selectedThread.IsAlive)
            {
                MessageBox.Show("��� ���� ��� �������� ���� ������. �������� �� ����� ������.");
                return;
            }

            // �������� ������� �������� � ComboBox
            ThreadPriority priority = ThreadPriority.Normal; // �� �������������
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

            // ������� �������� ������
            selectedThread.Priority = priority;

            // �������� �����������, �� �������� ��� �������
            MessageBox.Show($"�������� ������ {selectedThreadIndex} ������ �� {priority.ToString()}");
        }
    }
}