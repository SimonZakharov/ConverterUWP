using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.Foundation;

namespace ConverterUWP
{
    /// <summary>
    /// Страница калькулятора
    /// </summary>
    public sealed partial class CalcPage : Page
    {
        /// <summary>
        /// Полный JSON, переданный со страницы загрузки.
        /// </summary>
        private string Text { get; set; }
        /// <summary>
        /// Список валют, полученный сериализацией JSON.
        /// </summary>
        public Valutes Valutes { get; set; }
        private enum ValuteComponents
        {
            Id,
            NumCode,
            CharCode,
            Nominal,
            Name,
            Value,
            Previous
        }
        public static string LeftValuteName = "RUB", RightValuteName = "USD";
        public CalcPage()
        {
            this.InitializeComponent();
            this.UserInterfaceSetup();
            //  возможный способ зафиксировать минимальный размер окна
            ApplicationView.PreferredLaunchViewSize = new Size(1366, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }
        /// <summary>
        /// При переходе на страницу калькулятора полученный JSON десериализуется в коллекцию объектов класса Valute.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Width = 1500; this.Height = 1000;
            if (e.Parameter != null)
            {
                Text = e.Parameter.ToString();
                JObject jsonDe = JsonConvert.DeserializeObject<JObject>(Text);
                Valutes = TryParseValutes(jsonDe, out Valutes temp) ? temp : null;
                if (Valutes != null)
                {
                    LeftValuteBlock.Text = "RUB";
                    RightValuteBlock.Text = Valutes.Find(elem => elem.CharCode == "USD").CharCode;
                }
                else
                {
                    ErrorBlock.Visibility = Visibility.Visible;
                    LeftValuteBlock.Text = RightValuteBlock.Text = string.Empty;
                    LeftButtonChange.Visibility = RightButtonChange.Visibility = Visibility.Collapsed;
                    LeftBlock.Visibility = RightBlock.Visibility = Visibility.Collapsed;
                    return;
                }
            }
        }
        /// <summary>
        /// Метод производит десериализацию JSON и в случае успеха записывает результат в выходной параметр.
        /// </summary>
        /// <param name="jObject">Исходный JSON</param>
        /// <param name="valutes">Список валют, выходной параметр.</param>
        /// <returns></returns>
        private bool TryParseValutes(JObject jObject, out Valutes valutes)
        {
            valutes = new Valutes();
            try
            {
                foreach (var child in jObject.Children())
                {
                    if (string.Equals("Valute", child.Path))
                    {
                        int i = 0;
                        foreach (var valute in child.Value<JToken>().Children().First().Children())
                        {
                            var anotherChild = valute.Children().ElementAt(i);
                            var id = anotherChild.ElementAt((int)ValuteComponents.Id).ToArray()[0].ToString();
                            var numCode = anotherChild.ElementAt((int)ValuteComponents.NumCode).ToArray()[0].ToString();
                            var charCode = anotherChild.ElementAt((int)ValuteComponents.CharCode).ToArray()[0].ToString();
                            var nom = anotherChild.ElementAt((int)ValuteComponents.Nominal).ToArray()[0].ToString();
                            var name = anotherChild.ElementAt((int)ValuteComponents.Name).ToArray()[0].ToString();
                            var val = anotherChild.ElementAt((int)ValuteComponents.Value).ToArray()[0].ToString();
                            var prev = anotherChild.ElementAt((int)ValuteComponents.Previous).ToArray()[0].ToString();

                            Valute result = new Valute
                                (
                                id,
                                numCode,
                                charCode,
                                int.TryParse(nom, out int nominal) ? nominal : 0,
                                name,
                                double.TryParse(val, out double recValue) ? recValue : 0.0,
                                double.TryParse(prev, out double prevValue) ? prevValue : 0.0
                                );
                            valutes.AddValute(result);
                        }
                    }
                }
                if (valutes.Count() != 0)
                {
                    valutes.AddValute(
                        new Valute
                            (
                                string.Empty,
                                string.Empty,
                                "RUB",
                                1,
                                "Российский рубль",
                                1.0,
                                1.0
                            )
                        );
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Настройка пользовательского интерфейса страницы калькулятора.
        /// </summary>
        private void UserInterfaceSetup()
        {
            LeftButtonChange.Content = RightButtonChange.Content = "Изменить валюту";
            ErrorBlock.Visibility = Visibility.Collapsed;
            HeaderBlock.SelectionHighlightColor = null;
            HeaderBlock.Height = Height / 8;
        }

        private void RightBlock_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (Valutes.Count() == 0) return;
            if (string.IsNullOrWhiteSpace(RightBlock.Text))
            {
                LeftBlock.Text = "0";
                return;
            }
            double right = double.TryParse(RightBlock.Text, out double t1) ? t1 : double.MinValue;
            if (right == double.MinValue)
            {
                LeftBlock.Text = "???";
                return;
            }
            var rubAmount = right * Valutes.Find(elem => elem.CharCode == RightValuteBlock.Text).Value;
            var leftAmount = rubAmount / Valutes.Find(elem => elem.CharCode == LeftValuteBlock.Text).Value;
            LeftBlock.Text = string.Format("{0:N4}", leftAmount);
            if (LeftBlock.Text == "0,0000") LeftBlock.Text = "0";
        }

        private void LeftBlock_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (Valutes.Count() == 0) return;
            if (string.IsNullOrWhiteSpace(LeftBlock.Text))
            {
                RightBlock.Text = "0";
                return;
            }
            RightBlock.Text = string.Empty;
            double left = double.TryParse(LeftBlock.Text, out double t) ? t : double.MinValue;
            if (left == double.MinValue)
            {
                RightBlock.Text = "???";
                return;
            }
            var rubAmount = left * Valutes.Find(elem => elem.CharCode == LeftValuteBlock.Text).Value;
            var rightAmount = rubAmount / Valutes.Find(elem => elem.CharCode == RightValuteBlock.Text).Value;
            RightBlock.Text = string.Format("{0:N4}", rightAmount);
            if (RightBlock.Text == "0,0000") RightBlock.Text = "0";
        }
        /// <summary>
        /// Возможный способ зафиксировать минимальный размер окна.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Width < 1500) this.Width = 1500;
            if (this.Height < 1000) this.Height = 1000;
        }

        /// <summary>
        /// Выбор валюты (один метод для обеих кнопок)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonValuteChange_Click(object sender, RoutedEventArgs e)
        {
            List<string> valNames = Valutes.GetValuteCharCodes();
            ValuteSelectPage valuteSelectDialog = new ValuteSelectPage(valNames);
            await valuteSelectDialog.ShowAsync();
            if (!string.IsNullOrEmpty(valuteSelectDialog.SelectedValute))
            {
                Button s = (Button)sender;
                if (string.Equals(s.Name, "RightButtonChange"))
                {
                    RightValuteBlock.Text = valuteSelectDialog.SelectedValute;
                    double left = double.TryParse(LeftBlock.Text, out double t) ? t : double.MinValue;
                    if (left == double.MinValue)
                    {
                        RightBlock.Text = "???";
                        return;
                    }
                    var rubAmount = left * Valutes.Find(elem => elem.CharCode == LeftValuteBlock.Text).Value;
                    var rightAmount = rubAmount / Valutes.Find(elem => elem.CharCode == RightValuteBlock.Text).Value;
                    RightBlock.Text = string.Format("{0:N4}", rightAmount);
                    if (RightBlock.Text == "0,0000") RightBlock.Text = "0";
                }
                else
                {
                    LeftValuteBlock.Text = valuteSelectDialog.SelectedValute;
                    double right = double.TryParse(RightBlock.Text, out double t1) ? t1 : double.MinValue;
                    if (right == double.MinValue)
                    {
                        LeftBlock.Text = "???";
                        return;
                    }
                    var rubAmount = right * Valutes.Find(elem => elem.CharCode == RightValuteBlock.Text).Value;
                    var leftAmount = rubAmount / Valutes.Find(elem => elem.CharCode == LeftValuteBlock.Text).Value;
                    LeftBlock.Text = string.Format("{0:N4}", leftAmount);
                    if (LeftBlock.Text == "0,0000") LeftBlock.Text = "0";
                }
            }
        }
    }
}