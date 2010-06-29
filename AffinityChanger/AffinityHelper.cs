//-----------------------------------------------------------------------------
// <copyright file="AffinityHelper.cs" company="nick4eva's software">
//     Copyright (c) nick4eva's software. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace AffinityChanger
{
	using System;

	/// <summary>
	/// Вспомогательный класс
	/// </summary>
	static class AffinityHelper
	{
		#region Поля

		/// <summary>
		/// Количество процессоров
		/// </summary>
		public static readonly int CpuCount = Environment.ProcessorCount;

		#endregion

		#region Свойства

		#region Количество процессоров

        ///// <summary>
        ///// Количество процессоров
        ///// </summary>
        //public static int CpuCount
        //{
        //    get
        //    {
        //        return cpuCount;
        //    }
        //}

		#endregion

		#endregion

		#region Методы

		#region Получение маски привязки процессов по-умолчанию

		/// <summary>
		/// Получение маски привязки процессов по-умолчанию
		/// </summary>
		/// <returns>маска привязки</returns>
		public static IntPtr GetDefaultAffinity()
		{
			int bitmask = 0;

			for (int i = 0; i < CpuCount; i++)
			{
				bitmask += (int)Math.Pow(2, i);
			}

			return (IntPtr)bitmask;
		}

		#endregion

		#region Получение маски привязки для определенного ядра процессора

		/// <summary>
		/// Получение маски привязки для определенного ядра процессора
		/// </summary>
		/// <param name="coreNumber">номер ядра процессора</param>
		/// <returns>маска привязки</returns>
		public static IntPtr GetAffinityForOneCore(int coreNumber)
		{
			// возвращаем маску привязки для определенного ядра процессора
			return (IntPtr)Math.Pow(2, coreNumber);
		}

		#endregion

		#endregion
	}
}
