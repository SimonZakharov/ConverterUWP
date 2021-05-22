﻿using System.Collections.Generic;

namespace ConverterUWP
{
    public class Valute
    {
        public string Id { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Previous { get; set; }
        /// <summary>
        /// Конструктор валюты
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="numCode">Числовой код</param>
        /// <param name="charCode">Обозначение</param>
        /// <param name="nom">Номинал (по умолчанию 1)</param>
        /// <param name="name">Название</param>
        /// <param name="val">Текущая стоимость</param>
        /// <param name="prev">Предыдущая стоимость</param>
        public Valute(string id, string numCode, string charCode, int nom, string name, double val, double prev)
        {
            Id = id;
            NumCode = numCode;
            CharCode = charCode;
            Nominal = nom;
            Name = name;
            Value = val;
            Previous = prev;
        }
    }
    /// <summary>
    /// Список валют.
    /// </summary>
    public class Valutes
    {
        private List<Valute> Data { get; set; }
        public Valutes()
        {
            Data = new List<Valute>();
        }
        /// <summary>
        /// Добавление валюты в список валют.
        /// </summary>
        /// <param name="valute"></param>
        public void AddValute(Valute valute)
        {
            if (Data != null)
                Data.Add(valute);
        }
        /// <summary>
        /// Возвращает количество валют в списке.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            if (Data == null)
                return 0;
            return Data.Count;
        }
        /// <summary>
        /// Поиск по списку валют по заданному предикату.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Valute Find(System.Predicate<Valute> predicate)
        {
            if (Data == null)
                return null;
            return Data.Find(predicate);
        }
        public List<string> GetValuteCharCodes()
        {
            if (Data == null)
                return null;
            List<string> result = new List<string>();
            foreach (var valute in Data)
                result.Add(valute.CharCode);
            return result;
        }
    }
}
