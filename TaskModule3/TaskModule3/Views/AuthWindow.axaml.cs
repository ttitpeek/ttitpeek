using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using TaskModule3.Context;
using TaskModule3.Models;
using TaskModule3.Views;

namespace TaskModule3;

public partial class AuthWindow : Window
{
    private static int countTry = 3;
    private static string currentLogin = null;
    public AuthWindow()
    {
        InitializeComponent();
        AppDbContext.AppContext = new AppDbContext();
    }
    
    private async void AuthButtonClick(object? sender, RoutedEventArgs e)
    {
        if (String.IsNullOrEmpty(Login.Text) || String.IsNullOrWhiteSpace(Login.Text) ||
            String.IsNullOrEmpty(Password.Text) || String.IsNullOrWhiteSpace(Password.Text))
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Не все поля были заполнены", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error)
                .ShowAsync();
            return;
        }
        
        User foundUser = AppDbContext.AppContext.Users.Where(u => u.Login == Login.Text).FirstOrDefault();
        
        if (foundUser == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error)
                .ShowAsync();
            return;
        }
        
        //
        if (AppDbContext.CurrentUser == null || foundUser.Login != Login.Text)
        {
            countTry = 3;
            AppDbContext.CurrentUser = foundUser;
            
        }

        if (AppDbContext.CurrentUser.LastEnter != null)
        {
            //((date1.Year - date2.Year) * 12) + date1.Month - date2.Month
            if (DateTime.Now.Year != AppDbContext.CurrentUser.LastEnter.Value.Year || (DateTime.Now.Month - AppDbContext.CurrentUser.LastEnter.Value.Month) > 1)
            {
                AppDbContext.CurrentUser.IsBlocked = true;
                AppDbContext.AppContext.Update(AppDbContext.CurrentUser);
                await AppDbContext.AppContext.SaveChangesAsync();
            }
        }
        
        if (AppDbContext.CurrentUser.IsBlocked)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Вы заблокированы. Обратитесь к администратору", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error)
                .ShowAsync();
            return;
        }
        //

        if (AppDbContext.CurrentUser.Password != Password.Text)
        {
            countTry--;
            if (countTry == 0)
            {
                AppDbContext.CurrentUser.IsBlocked = true;
                AppDbContext.AppContext.Users.Update(AppDbContext.CurrentUser);
                await AppDbContext.AppContext.SaveChangesAsync();
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Вы заблокированы. Обратитесь к администратору", ButtonEnum.Ok,
                        MsBox.Avalonia.Enums.Icon.Error)
                    .ShowAsync();
                countTry = 3;
                return;
            }
            
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", $"Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные. Осталось попыток {countTry}", ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error)
                .ShowAsync();
            return;
        }
        
        
        
        //Вы успешно авторизовались
        
        await MessageBoxManager.GetMessageBoxStandard("Информация", "Вы успешно авторизовались", ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Info)
            .ShowAsync();

        if (AppDbContext.CurrentUser.LastEnter == null)
        {
            new ResetPassword().Show();
            this.Close();
            return;
        }
        AppDbContext.CurrentUser.LastEnter = DateTime.Now;
        AppDbContext.AppContext.Update(AppDbContext.CurrentUser);
        await AppDbContext.AppContext.SaveChangesAsync();

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