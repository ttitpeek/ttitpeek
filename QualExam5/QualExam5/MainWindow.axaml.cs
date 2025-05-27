using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;

namespace QualExam5;

public partial class MainWindow : Window
{
    private string _dataFromApi = "";
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void GetUserData_OnClick(object? sender, RoutedEventArgs e)
    {
        GetUserData();
    }
    
    private void CheckUserData_OnClick(object? sender, RoutedEventArgs e)
    {
        CheckUserData();
    }

    /// <summary>
    /// Асинхронно получает данные из API и выводит их на пользовательский интерфейс
    /// </summary>
    private async void GetUserData()
    {
        var httpClient = new HttpClient();
        var content = await httpClient.GetStringAsync("http://prb.sylas.ru/TransferSimulator/fullName");
        var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        
        if (data == null) return;

        _dataFromApi = data["value"];
        GetFullName.Text = _dataFromApi;
    }

    /// <summary>
    /// Проверяет данные на валидность и обновляет текстовый документ
    /// </summary>
    private void CheckUserData()
    {
        var validationResults = Regex.IsMatch(_dataFromApi, @"[^A-Za-zА-Яа-яЁё]\s");
        CheckFullName.Text =
            validationResults ? "ФИО содержит запрещенные символы" : "ФИО не содержит запрещенные символы";

        using var doc = WordprocessingDocument.Open("TestCase.docx", true);
        var document = doc.MainDocumentPart?.Document;

        if (document?.Descendants<Text>().FirstOrDefault(text => text.Text.Contains("Result 1")) != null)
            ReplaceTextCase("Result 1", validationResults, document);
        else if (document?.Descendants<Text>().FirstOrDefault(text => text.Text.Contains("Result 2")) != null)
            ReplaceTextCase("Result 2", validationResults, document);
        doc.MainDocumentPart?.Document.Save();
    }

    /// <summary>
    /// Заменяет указанный текст в документе на результат проверки валидности
    /// </summary>
    /// <param name="replacedText"></param>
    /// <param name="validationResult"></param>
    /// <param name="doc"></param>
    private void ReplaceTextCase(string replacedText, bool validationResult, Document doc)
    {
        foreach (var text in doc.Descendants<Text>())
        {
            if (text.Text == replacedText)
                text.Text = text.Text.Replace(replacedText, validationResult ? "Не успешно" : "Успешно");
        }
    }
}