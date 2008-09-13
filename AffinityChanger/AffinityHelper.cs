//-----------------------------------------------------------------------------
// <copyright file="AffinityHelper.cs" company="nick4eva's software">
//     Copyright (c) nick4eva's software. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace AffinityChanger
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Вспомогательный класс
    /// </summary>
    static class AffinityHelper
    {
        #region Поля

        /// <summary>
        /// Количество процессоров
        /// </summary>
        private static int cpuCount = Environment.ProcessorCount;

        #endregion

        #region Свойства

        #region Количество процессоров
        /// <summary>
        /// Количество процессоров
        /// </summary>
        public static int CpuCount
        {
            get
            {
                return cpuCount;
            }
        } 
        #endregion

        #endregion

        #region Методы

        #region Установка маски соответствия процессоров для процесса
        /// <summary>
        /// Установка маски соответствия процессоров для процесса
        /// </summary>
        /// <param name="process">процесс</param>
        /// <param name="cpuNumber">процессор</param>
        public static void SetAffinity(Process process, int cpuNumber)
        {
            IntPtr affinityMask;

            // если маска соответствия процессоров для текущего процесса не соответствует текущему процессу
            if (process.ProcessorAffinity != (affinityMask = (IntPtr)(Math.Pow(2, cpuNumber))))
            {
                // выставляем соответствующую маску соответствия процессоров для текущего процесса
                process.ProcessorAffinity = affinityMask;
            }
        }
        #endregion

        #region Получение маски привязки процессов по-умолчанию
        /// <summary>
        /// Получение маски привязки процессов по-умолчанию
        /// </summary>
        /// <returns>маска привязки</returns>
        public static IntPtr GetDefaultAffinity()
        {
            int bitmask = 0;

            for (int i = 0; i < AffinityHelper.CpuCount; i++)
            {
                bitmask += (int)Math.Pow(2, i);
            }

            return (IntPtr)bitmask;
        } 
        #endregion

        #region Нахождение минимального числа в списке
        /// <summary>
        /// Нахождение минимального числа в списке
        /// </summary>
        /// <param name="list">список чисед</param>
        /// <returns>минимальное число</returns>
        public static int Min(List<int> list)
        {
            int value = 0;
            bool hasValue = false;
            foreach (int x in list)
            {
                if (hasValue)
                {
                    if (x < value)
                    {
                        value = x;
                    }
                }
                else
                {
                    value = x;
                    hasValue = true;
                }
            }

            if (hasValue)
            {
                return value;
            }

            throw new NotSupportedException();
        } 
        #endregion

        #endregion
    }
}
