using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ConverterUWP
{
    /// <summary>
    /// Страница загрузки
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            this.InitializeComponent();
            this.LoadingImage.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Загрузка страницы. Пока содержимое не загружено, логотип загрузки остается видимым.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Browser_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            this.LoadingImage.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Метод запрашивает внутренний JSON с загруженной Web-страницы и передает его на страницу калькулятора.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void Browser_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (args.Uri != null)
            {
                string[] arguments = new string[] { "document.getElementsByTagName(\"pre\")[0].innerHTML;" };
                var s = await Browser.InvokeScriptAsync("eval", arguments);
                this.LoadingImage.Visibility = Visibility.Collapsed;
                Frame.Navigate(typeof(CalcPage), s);
            }
        }
    }
}
