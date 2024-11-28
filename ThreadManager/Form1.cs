using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

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
            Array = PotokDll.Potok.GenerateRandomArray(N, -100, 100);
            PrintArray(richTextBoxArray, Array);

            // Розбиття масиву на частини для кожного потоку
            int chunkSize = N / ThreadsCount;
            int[][] sortedParts = new int[ThreadsCount][];

            SetComboBox(ThreadsCount);

            for (int i = 0; i < ThreadsCount; i++)
            {
                int start = i * chunkSize;
                int end = (i == ThreadsCount - 1) ? N : (i + 1) * chunkSize;
                int[] part = Array.Skip(start).Take(end - start).ToArray();

                // Створюємо новий потік для сортування частини масиву
                int threadIndex = i; // Зберігаємо індекс для доступу всередині потоку
                Thread thread = new Thread(() =>
                {
                    PotokDll.Potok.SelectionSort(part);
                    sortedParts[threadIndex] = part; // Зберігаємо відсортовану частину
                });

                Threads.Add(thread);
                thread.Start();
            }

            // Чекаємо, поки всі потоки завершать роботу
            foreach (Thread thread in Threads)
            {
                thread.Join();
            }

            // Злиття всіх частин в один масив
            Array = sortedParts.SelectMany(x => x).ToArray();

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
