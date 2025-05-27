using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using TaskModule3.Context;

namespace TaskModule3.Views;

public partial class ResetPassword : Window
{
    public ResetPassword()
    {
        InitializeComponent();
    }

    private async void ResetPasswordButton(object? sender, RoutedEventArgs e)
    {
        if (String.IsNullOrEmpty(OldPassword.Text) || String.IsNullOrWhiteSpace(OldPassword.Text) ||
            String.IsNullOrEmpty(NewPassword.Text) || String.IsNullOrWhiteSpace(NewPassword.Text) ||
            String.IsNullOrEmpty(RepeatPassword.Text) || String.IsNullOrWhiteSpace(RepeatPassword.Text))
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Не все поля были заполнены", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error)
                .ShowAsync();
            return;
        }

        if (AppDbContext.CurrentUser.Password != OldPassword.Text)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Пароль не совпадает со старым паролем", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error)
                .ShowAsync();
            return;
        }
        
        if (NewPassword.Text != RepeatPassword.Text)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Введенные пароли не совпадают", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error)
                .ShowAsync();
            return;
        }
        
        AppDbContext.CurrentUser.Password = NewPassword.Text;
        AppDbContext.CurrentUser.LastEnter = DateTime.Now;
        AppDbContext.AppContext.Users.Update(AppDbContext.CurrentUser);
        await AppDbContext.AppContext.SaveChangesAsync();
        
        await MessageBoxManager.GetMessageBoxStandard("Информация", "Пароль изменен", ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Success)
            .ShowAsync();
        
        if (AppDbContext.CurrentUser.RoleId == 1)
        {
            new WorkWindowAdmin().Show();
            this.Close();
        }
        else
        {
            new WorkWindowUser().Show();
            this.Close();
        }
    }
}