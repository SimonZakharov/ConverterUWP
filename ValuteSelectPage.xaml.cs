using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace ConverterUWP
{
    /// <summary>
    /// Диалоговое окно выбора валюты
    /// </summary>
    public sealed partial class ValuteSelectPage : ContentDialog
    {
        public string SelectedValute { get; set; }
        public ValuteSelectPage()
        {
            this.InitializeComponent();
            SelectedValute = string.Empty;
        }

        public ValuteSelectPage(List<string> valuteNames)
        {
            this.InitializeComponent();
            ValuteSelectCB.DataContext = valuteNames;
            ValuteSelectCB.SelectedIndex = 0;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SelectedValute = ValuteSelectCB.SelectedValue.ToString();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SelectedValute = string.Empty;
        }
    }
}