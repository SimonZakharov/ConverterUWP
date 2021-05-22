using System;
using Windows.Foundation;
using Windows.UI.ViewManagement;
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
            ApplicationView.PreferredLaunchViewSize = new Size(1366, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Width < 1300) this.Width = 1300;
            if (this.Height < 600) this.Height = 600;
        }
    }
}
