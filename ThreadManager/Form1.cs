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

            // ���������� �����
            Array = PotokDll.Potok.GenerateRandomArray(N, -100, 100);
            PrintArray(richTextBoxArray, Array);

            // �������� ������ �� ������� ��� ������� ������
            int chunkSize = N / ThreadsCount;
            int[][] sortedParts = new int[ThreadsCount][];

            SetComboBox(ThreadsCount);

            for (int i = 0; i < ThreadsCount; i++)
            {
                int start = i * chunkSize;
                int end = (i == ThreadsCount - 1) ? N : (i + 1) * chunkSize;
                int[] part = Array.Skip(start).Take(end - start).ToArray();

                // ��������� ����� ���� ��� ���������� ������� ������
                int threadIndex = i; // �������� ������ ��� ������� �������� ������
                Thread thread = new Thread(() =>
                {
                    PotokDll.Potok.SelectionSort(part);
                    sortedParts[threadIndex] = part; // �������� ����������� �������
                });

                Threads.Add(thread);
                thread.Start();
            }

            // ������, ���� �� ������ ��������� ������
            foreach (Thread thread in Threads)
            {
                thread.Join();
            }

            // ������ ��� ������ � ���� �����
            Array = sortedParts.SelectMany(x => x).ToArray();

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

        private void comboBoxThreads_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ����������, �� ������� ����
            if (comboBoxThreads.SelectedIndex == -1)
            {
                return; // ���� �� �������, ����� �� ������
            }

            // �������� �������� ���� �� ��������
            int selectedThreadIndex = comboBoxThreads.SelectedIndex;
            Thread selectedThread = Threads[selectedThreadIndex];

            // �������� �������� ������
            ThreadPriority threadPriority = selectedThread.Priority;

            // �������� �������� ������ � comboBoxPriority
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
