using System;
using System.Text;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Common.Translate
{
    public static class RusNumber
    {
        public class GrammaticalNumbers
        {
            public GrammaticalNumbers(string singular, string paucal, string plural)
            {
                Singular = singular;
                Paucal = paucal;
                Plural = plural;
            }

            public string Singular { get; private set; }
            public string Paucal { get; private set; }
            public string Plural { get; private set; }
        }

        public static GrammaticalNumbers Years = new GrammaticalNumbers("год", "года", "лет");
        public static GrammaticalNumbers Months = new GrammaticalNumbers("месяц", "месяца", "месяцев");
        public static GrammaticalNumbers Days = new GrammaticalNumbers("день", "дня", "дней");

        private static readonly string[] _hundreds =
        {
            string.Empty, "сто ", "двести ", "триста ", "четыреста ",
            "пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
        };

        private static readonly string[] _tens =
        {
            string.Empty, "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
            "шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
        };

        public static string ToYearsPluralString(this int years)
        {
            return years + " " + GrammaticalNumberForValue(years, "год", "года", "лет");
        }

        public static string ToYearsPluralString(this double years)
        {
            var intYears = (int) Math.Ceiling(years);
            return years + " " + GrammaticalNumberForValue(intYears, "год", "года", "лет");
        }

        public static string Str(int val, bool male, string one, string two, string five)
        {
            var result = ToNumbersInWords(val, male);
            return string.IsNullOrEmpty(result)
                       ? string.Empty
                       : result + GrammaticalNumberForValue(val % 1000, one, two, five) + " ";
        }

        public static string ToNumbersInWords(int value, bool male = true)
        {
            Condition.Requires(value, "val")
                .IsGreaterOrEqual(0, "Параметр не может быть отрицательным");

            string[] frac20 =
            {
                string.Empty, "один ", "два ", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
            };

            var modulo100 = value % 1000;
            if (0 == modulo100)
                return string.Empty;

            if (!male)
            {
                frac20[1] = "одна ";
                frac20[2] = "две ";
            }

            var result = new StringBuilder(_hundreds[modulo100 / 100]);

            if (modulo100 % 100 < 20)
            {
                result.Append(frac20[modulo100 % 100]);
            }
            else
            {
                result.Append(_tens[modulo100 % 100 / 10]);
                result.Append(frac20[modulo100 % 10]);
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Подставляет существительное в требуемом падеже/числе, в соответствии с числом (кол-вом чего-либо)
        /// </summary>
        /// <param name="value">числовое значение</param>
        /// <param name="sungular">существителньое для чисел, оканчивающихся на 1</param>
        /// <param name="paucal">существительное для чисел, оканчивающихся на 2, 3 или 4</param>
        /// <param name="plural">существительное для остальных чисел</param>
        /// <returns>Существительное в требуемом падеже/числе</returns>
        /// <example>GrammaticalNumberForValue("пациент", "пациента", "пациентов")</example>
        public static string GrammaticalNumberForValue(this double value, string sungular, string paucal, string plural)
        {
            var intValue = (int) Math.Ceiling(value);
            return GrammaticalNumberForValue(intValue, sungular, paucal, plural);
        }

        /// <summary>
        /// Подставляет существительное в требуемом падеже/числе, в соответствии с числом (кол-вом чего-либо)
        /// </summary>
        /// <param name="value">числовое значение</param>
        /// <param name="sungular">существителньое для чисел, оканчивающихся на 1</param>
        /// <param name="paucal">существительное для чисел, оканчивающихся на 2, 3 или 4</param>
        /// <param name="plural">существительное для остальных чисел</param>
        /// <returns>Существительное в требуемом падеже/числе</returns>
        /// <example>GrammaticalNumberForValue("пациент", "пациента", "пациентов")</example>
        public static string GrammaticalNumberForValue(this int value, string sungular, string paucal, string plural)
        {
            return value.GrammaticalNumberForValue(new GrammaticalNumbers(sungular, paucal, plural));
        }

        public static string GrammaticalNumberForValue(this int value, GrammaticalNumbers grammaticalNumbers)
        {
            var remainer = (value % 100 > 20) ? value % 10 : value % 20;

            switch (remainer)
            {
                case 1:
                    return grammaticalNumbers.Singular;
                case 2:
                case 3:
                case 4:
                    return grammaticalNumbers.Paucal;
                default:
                    return grammaticalNumbers.Plural;
            }
        }

        /// <summary>
        /// Подставляет существительное в требуемом падеже/числе, в соответствии с числом (кол-вом чего-либо)
        /// </summary>
        /// <param name="value">числовое значение</param>
        /// <param name="sungular">существителньое для чисел, оканчивающихся на 1</param>
        /// <param name="paucal">существительное для чисел, оканчивающихся на 2, 3 или 4</param>
        /// <param name="plural">существительное для остальных чисел</param>
        /// <returns>Существительное в требуемом падеже/числе</returns>
        /// <example>GrammaticalNumberForValue("пациент", "пациента", "пациентов")</example>
        public static string GrammaticalNumberForValue(this int? value, string sungular, string paucal, string plural)
        {
            return value.HasValue ? GrammaticalNumberForValue(value.Value, sungular, paucal, plural) : string.Empty;
        }
    };
}