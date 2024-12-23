using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EXAM.Scripts
{
    public class TemperatureMonitor
    {
        private string productName; // Название продукта
        private DateTime transportDate; // Дата транспортировки
        private int transportTime; // Время транспортировки (в минутах)
        private int? minTemp;
        private int? maxTemp;
        private int[] temperatures;
        private int allowedViolationTime; // Допустимое время в неподходящей температуре
        private bool isTripCompleted; // Флаг, указывающий, завершена ли поездка

        public TemperatureMonitor(string productName, DateTime transportDate, int transportTime, int? minTemp, int? maxTemp, int[] temperatures, int allowedViolationTime, bool isTripCompleted)
        {
            this.productName = productName;
            this.transportDate = transportDate;
            this.transportTime = transportTime;
            this.minTemp = minTemp;
            this.maxTemp = maxTemp;
            this.temperatures = temperatures;
            this.allowedViolationTime = allowedViolationTime;
            this.isTripCompleted = isTripCompleted;
        }

        public string CheckConditions()
        {
            StringBuilder report = new StringBuilder();
            List<string> violations = new List<string>();

            int totalMinutes = transportTime * 60; // Общее время транспортировки в минутах
            int violationCount = 0; // Количество минут с нарушением температуры

            for (int i = 0; i < temperatures.Length; i++)
            {
                int minute = i * 10; // Каждый элемент массива соответствует 10 минутам

                if ((minTemp.HasValue && temperatures[i] < minTemp.Value) || (maxTemp.HasValue && temperatures[i] > maxTemp.Value))
                {
                    violations.Add($"Нарушение: Температура {temperatures[i]}°C на минуте {minute}.");
                    violationCount += 10; // Увеличиваем счетчик нарушений на 10 минут
                }
            }

            // Формирование отчета
            report.AppendLine($"Название продукта: {productName}");
            report.AppendLine($"Дата транспортировки: {transportDate.ToShortDateString()}");
            report.AppendLine($"Время транспортировки: {transportTime} часов");

            if (!isTripCompleted)
            {
                report.AppendLine("Внимание: Поездка еще не завершена.");
            }

            if (violations.Count > 0)
            {
                report.AppendLine("Обнаружены нарушения условий хранения:");
                foreach (var violation in violations)
                {
                    report.AppendLine(violation);
                }

                report.AppendLine($"Общее время выхода из диапазона температур: {violationCount} минут.");

                // Проверка пригодности продукта
                if (violationCount > allowedViolationTime)
                {
                    report.AppendLine("Внимание: Рыба не пригодна к употреблению, так как превышено допустимое время в неподходящей температуре.");
                }
                else
                {
                    report.AppendLine("Рыба пригодна к употреблению, но требуется проверка качества.");
                }
            }
            else
            {
                report.AppendLine("Условия хранения соблюдены. Рыба пригодна к употреблению.");
            }

            return report.ToString();
        }
    }
}