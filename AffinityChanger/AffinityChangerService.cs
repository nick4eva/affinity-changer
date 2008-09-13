//-----------------------------------------------------------------------------
// <copyright file="AffinityChangerService.cs" company="nick4eva's software">
//     Copyright (c) nick4eva's software. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------
[assembly: System.CLSCompliant(true)]
namespace AffinityChanger
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.ServiceProcess;
    using Microsoft.Win32;
    using System.Text;
    using System.Text.RegularExpressions;


    public partial class AffinityChangerService : ServiceBase
    {
        /// <summary>
        /// Путь к конфигурационному файлу
        /// </summary>
        private string configFilePath;

        /// <summary>
        /// Приоритет процесса
        /// </summary>
        private string processPriority;

        #region Конструктор
        /// <summary>
        /// Конструктор
        /// </summary>
        public AffinityChangerService()
        {
            InitializeComponent();
        } 
        #endregion

        #region Установка интервала таймера
        /// <summary>
        /// Установка интервала таймера
        /// </summary>
        private void SetTimerInterval()
        {
            double defaultValue = 300000;
            string keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\nick4eva's software\Affinity Changer";
            string valueName = "CheckInterval";

            // если нет ветки в реестре
            if (Registry.GetValue(keyName, valueName, defaultValue) == null)
            {
                // ставим параметр по умолчанию
                Registry.SetValue(keyName, valueName, defaultValue, RegistryValueKind.DWord);
            }

            // если стоит нулевое значение или нет параметра
            if ((int)Registry.GetValue(keyName, valueName, 0) == 0)
            {
                // ставим параметр по умолчанию
                Registry.SetValue(keyName, valueName, defaultValue, RegistryValueKind.DWord);
            }

            // выставляем интервал таймера из реестра
            timer.Interval = (int)Registry.GetValue(keyName, valueName, defaultValue);
        } 
        #endregion

        #region Установка конфигурационного файла
        /// <summary>
        /// Установка конфигурационного файла
        /// </summary>
        private void SetConfigFile()
        {
            string defaultValue = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            string keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\nick4eva's software\Affinity Changer";
            string valueName = "ConfigurationFile";

            // если нет ветки в реестре
            if (Registry.GetValue(keyName, valueName, defaultValue) == null)
            {
                // ставим параметр по умолчанию
                Registry.SetValue(keyName, valueName, defaultValue, RegistryValueKind.String);
            }

            // если стоит пустое значение или нет параметра
            if (string.IsNullOrEmpty((string)Registry.GetValue(keyName, valueName, string.Empty)))
            {
                // ставим параметр по умолчанию
                Registry.SetValue(keyName, valueName, defaultValue, RegistryValueKind.String);
            }

            this.configFilePath = (string)Registry.GetValue(keyName, valueName, defaultValue);
        } 
        #endregion

        #region Установка приоритета процесса
        /// <summary>
        /// Установка приоритета процесса
        /// </summary>
        private void SetProcessPriority()
        {
            string defaultValue = @"normal";
            string keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\nick4eva's software\Affinity Changer";
            string valueName = "ProcessPriority";

            // если нет ветки в реестре
            if (Registry.GetValue(keyName, valueName, defaultValue) == null)
            {
                // ставим параметр по умолчанию
                Registry.SetValue(keyName, valueName, defaultValue, RegistryValueKind.String);
            }

            // если стоит нулевое значение или нет параметра
            //if ((string)Registry.GetValue(keyName, valueName, string.Empty) == string.Empty)
            if (string.IsNullOrEmpty((string)Registry.GetValue(keyName, valueName, string.Empty)))
            {
                // ставим параметр по умолчанию
                Registry.SetValue(keyName, valueName, defaultValue, RegistryValueKind.String);
            }

            this.processPriority = (string)Registry.GetValue(keyName, valueName, defaultValue);
        } 
        #endregion

        #region Запуск сервиса
        /// <summary>
        /// Запуск сервиса
        /// </summary>
        /// <param name="args">параметры запуска сервиса</param>
        protected override void OnStart(string[] args)
        {           
            // устанавливаем интервал таймера
            this.SetTimerInterval();

            // устанавливаем путь к конфигурационному файлу
            this.SetConfigFile();

            // запускаем таймер
            timer.Start();
        }
        #endregion

        #region Остановка сервиса
        /// <summary>
        /// Остановка сервиса
        /// </summary>
        protected override void OnStop()
        {
            // останавливаем таймер
            timer.Stop();
        } 
        #endregion

        #region Обработчик таймера
        /// <summary>
        /// Обработчик таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // устанавливаем интервал таймера
            this.SetTimerInterval();

            // устанавливаем конфигурационный файл
            this.SetConfigFile();

            // устанавливаем приоритет для процессов
            this.SetProcessPriority();

            if (!File.Exists(this.configFilePath))
            {
                return;
            }

            string configLine;
            List<string> configItems = new List<string>();
            using (StreamReader reader = new StreamReader(this.configFilePath))
            {
                while ((configLine = reader.ReadLine()) != null)
                {
                    configItems.Add(configLine);
                }
            }

            if (configItems.Count == 0)
            {
                return;
            }

            Process[] prosesses = Process.GetProcesses();
            List<Process> processList = new List<Process>();
            foreach (Process process in prosesses)
            {
                processList.Add(process);
            }

            List<Process> processNameList = new List<Process>();
            List<int> cpuCoresClients = new List<int>();

            //foreach (string item in configItems)
            //{
            //    processNameList = processList.FindAll(x => Regex.IsMatch(x.ProcessName, item));

            //    foreach (Process process in processNameList)
            //    {
            //        // если у процесса выставлена маска по-умолчанию (на все ядра)
            //        if (process.ProcessorAffinity == AffinityHelper.GetDefaultAffinity())
            //        {
            //            cpuCoresClients.Clear();

            //            for (int i = 0; i < AffinityHelper.CpuCount; i++)
            //            {
            //                cpuCoresClients.Add(processNameList.FindAll(x => x.ProcessorAffinity == (IntPtr)Math.Pow(2, i)).Count);
            //            }

            //            AffinityHelper.SetAffinity(process, cpuCoresClients.IndexOf(AffinityHelper.Min(cpuCoresClients)));
            //            process.PriorityClass = (ProcessPriorityClass)System.Enum.Parse(typeof(ProcessPriorityClass), this.processPriority, true);
            //        }
            //    }
            //}

            foreach (string item in configItems)
            {
                processNameList.AddRange(processList.FindAll(x => Regex.IsMatch(x.ProcessName, item)));
            }

            foreach (Process process in processNameList)
            {
                // если у процесса выставлена маска по-умолчанию (на все ядра)
                if (process.ProcessorAffinity == AffinityHelper.GetDefaultAffinity())
                {
                    cpuCoresClients.Clear();

                    for (int i = 0; i < AffinityHelper.CpuCount; i++)
                    {
                        cpuCoresClients.Add(processNameList.FindAll(x => x.ProcessorAffinity == (IntPtr)Math.Pow(2, i)).Count);
                    }

                    AffinityHelper.SetAffinity(process, cpuCoresClients.IndexOf(AffinityHelper.Min(cpuCoresClients)));
                    process.PriorityClass = (ProcessPriorityClass)System.Enum.Parse(typeof(ProcessPriorityClass), this.processPriority, true);
                }
            }
        }
        #endregion
    }
}