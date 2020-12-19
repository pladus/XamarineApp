using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CurrencyParser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPage : ContentPage
    {
        public ErrorPage(string errorMessage)
        {
            InitializeComponent();
            TitleAlert.Text = "Сбой в работе приложения";

            ExceptionStateAlert.Text =
                "Подробности ошибки ниже. " +
                "Если Вы хотите помочь развитию приложения, пожалуйста, скопируйте текст " +
                "описания ошибки ниже и отправьте его на адрес support@bizzbro.ru";

            Advice.Text =
                "Пожалуйста, перезапустите приложение и повторите попытку позже. Возможно ошибка вызвана состоянием сервера ЦБ или Вашего интернет-провайдера.";

            ExceptionLabel.Text = errorMessage;
        }

    }
}