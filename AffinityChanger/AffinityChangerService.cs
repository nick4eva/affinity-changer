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
	using System.Text.RegularExpressions;
	using Microsoft.Win32;

	public partial class AffinityChangerService : ServiceBase
	{
		#region Свойства

		#region Путь к конфигурационному файлу

		/// <summary>
		/// Путь к конфигурационному файлу
		/// </summary>
		private string configFilePath;

		#endregion

		#endregion

		#region Конструктор

		/// <summary>
		/// Конструктор
		/// </summary>
		public AffinityChangerService()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Методы

		#region Предикат для сравнения процессов по имени

		/// <summary>
		/// Предикат для сравнения процессов по имени
		/// </summary>
		/// <param name="x">первый процесс</param>
		/// <param name="y">второй процесс</param>
		/// <returns></returns>
		private static int CompareProcessesByName(Process x, Process y)
		{
			return String.Compare(x.ProcessName, y.ProcessName, true);
		}

		#endregion

		#region Проверка вхождения значения строки в перечисление без учета регистра

		/// <summary>
		/// Проверка вхождения значения строки в перечисление без учета регистра
		/// </summary>
		/// <param name="enumType">тип перечисления</param>
		/// <param name="value">значение экземпляра перечисления</param>
		/// <returns>флаг, указывающий входит данная строка в перечисление или нет</returns>
		public bool IsDefined(Type enumType, string value)
		{
			string[] enumNames = Enum.GetNames(enumType);

			for (int i = 0; i < enumNames.Length; i++)
			{
				if (string.Compare(enumNames[i], value, true) == 0)
				{
					return true;
				}
			}

			return false;
		}
		#endregion

		#region Установка интервала таймера

		/// <summary>
		/// Установка интервала таймера
		/// </summary>
		private void SetTimerInterval()
		{
			// задаем значения для ключа реестра
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
			this.timer.Interval = (int)Registry.GetValue(keyName, valueName, defaultValue);
		}

		#endregion

		#region Установка конфигурационного файла

		/// <summary>
		/// Установка конфигурационного файла
		/// </summary>
		private void SetConfigFile()
		{
			// задаем значения для ключа реестра
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

			// получаем путь к конфигурационному файлу из реестра
			this.configFilePath = ((string)Registry.GetValue(keyName, valueName, defaultValue)).Replace("\"", string.Empty);
		}

		#endregion

		#region Установка привязки процессов к процессорам

		/// <summary>
		/// Установка привязки процессов к процессорам
		/// </summary>
		private void SetAffinity()
		{
			// если нет конфигурационного файла, то ничего не делаем
			if (!File.Exists(this.configFilePath))
			{
				return;
			}

			// считываем конфигурационный файл в набор строк (каждая строка - конфигурация для процесса)
			List<string> configItems;
			try
			{
				configItems = new List<string>(File.ReadAllLines(this.configFilePath));
			}
			catch (IOException)
			{
				return;
			}

			// если конфигурационный файл пустой, то ничего не делаем
			if (configItems == null || configItems.Count == 0)
			{
				return;
			}

			// получаем список процессов в системе
			List<Process> processList = new List<Process>(Process.GetProcesses());
			// сортируем список по имени процесса
			processList.Sort(CompareProcessesByName);
			// создаем словарь, в котором будет храниться процесс и его желаемый приоритет
			Dictionary<Process, ProcessPriorityClass> processNameDictionaryWithPriority = new Dictionary<Process, ProcessPriorityClass>();
			// временная переменная для приоритета процесса
			ProcessPriorityClass priority;

			// перебираем все строки из конфигурационного файла
			foreach (string item in configItems)
			{
				// разделяем строку на параметры (разделитель параметров - запятая, первый параметр имя процесса, второй - приоритет)
				string[] itemParams = item.Split(new char[] { ',' });
				// находим все процессы, у которых в имени присутствует строка из конфигурационного файла (первый параметр)
				List<Process> procList = processList.FindAll(x => Regex.IsMatch(x.ProcessName, itemParams[0].Trim()));
				// перебираем все найденные процессы
				foreach (var process in procList)
				{
					// если указан приоритет для процесса(ов), и это правильный приоритет
					if ((itemParams.Length > 1) && IsDefined(typeof(ProcessPriorityClass), itemParams[1].Trim()))
					{
						// парсим значение приоритета
						priority = (ProcessPriorityClass)System.Enum.Parse(typeof(ProcessPriorityClass), itemParams[1].Trim(), true);
					}
					else
					{
						// иначе ставим значение приоритета по умолчанию
						priority = ProcessPriorityClass.Normal;
					}

					// добавляем в словарь процесс и его желаемый приоритет
					processNameDictionaryWithPriority.Add(process, priority);
				}
			}

			// получаем список процессов которые нужно привязывать к процессорам из словаря
			List<Process> processesList = new List<Process>(processNameDictionaryWithPriority.Keys);
			foreach (KeyValuePair<Process, ProcessPriorityClass> kvp in processNameDictionaryWithPriority)
			{
				// получаем номер процессора, к которому нужно привязывать процесс
				int coreNumber = processesList.FindIndex(x => x.Id == kvp.Key.Id) % AffinityHelper.CpuCount;

				// если привязка процесса не совпадает с необходимой привязкой
				if (kvp.Key.ProcessorAffinity != AffinityHelper.GetAffinityForOneCore(coreNumber))
				{
					// устанавливаем процессу необходимую привязку
					kvp.Key.ProcessorAffinity = AffinityHelper.GetAffinityForOneCore(coreNumber);
				}
				
				// если приоритет процесса не совпадает с необходимым приоритетом
				if (kvp.Key.PriorityClass != kvp.Value)
				{
					// устанавливаем процессу необходимый приоритет
					kvp.Key.PriorityClass = kvp.Value;
				}
			}
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

			// устанавливаем привязку процессов к процессорам
			this.SetAffinity();

			// запускаем таймер
			this.timer.Start();
		}

		#endregion

		#region Остановка сервиса

		/// <summary>
		/// Остановка сервиса
		/// </summary>
		protected override void OnStop()
		{
			// останавливаем таймер
			this.timer.Stop();
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

			// устанавливаем привязку процессов к процессорам
			this.SetAffinity();
		}

		#endregion

		#endregion
	}
}