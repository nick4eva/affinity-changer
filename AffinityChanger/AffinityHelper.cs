using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AffinityChanger
{
    static class AffinityHelper
    {
        #region Поля

        static int cpuCount = Environment.ProcessorCount;

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
        /// <param name="process"></param>
        /// <param name="cpuNumber"></param>
        public static void SetAffinity(Process process, int cpuNumber)
        {
            IntPtr AffinityMask;

            // если маска соответствия процессоров для текущего процесса не соответствует текущему процессу
            if (process.ProcessorAffinity != (AffinityMask = (IntPtr)(Math.Pow(2, cpuNumber))))
            {
                // выставляем соответствующую маску соответствия процессоров для текущего процесса
                process.ProcessorAffinity = AffinityMask;
            }
        }
        #endregion

        #region Получение маски привязки процессов по-умолчанию
        /// <summary>
        /// Получение маски привязки процессов по-умолчанию
        /// </summary>
        /// <returns></returns>
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
        /// <param name="list"></param>
        /// <returns></returns>
        public static int Min(List<int> list)
        {
            int value = 0;
            bool hasValue = false;
            foreach (int x in list)
            {
                if (hasValue)
                {
                    if (x < value) value = x;
                }
                else
                {
                    value = x;
                    hasValue = true;
                }
            }

            if (hasValue) return value;
            throw new NotSupportedException();
        } 
        #endregion

        #endregion
    }
}
