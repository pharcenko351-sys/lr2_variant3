using DormitoryXml.Code.Models;
using DormitoryXml.Code.Services;
using DormitoryXml.Code.Strategies;

namespace DormitoryXml.UI;

public partial class MainPage : ContentPage
{
    private const string Any = "(будь-який)";

    private string? xmlFilePath;
    private string? xslFilePath;
    private readonly XmlSearchContext searchContext = new();
    private readonly XslTransformer transformer = new();
    private readonly HtmlExporter htmlExporter;
    private readonly XmlAttributeReader attributeReader = new();
    private List<Student> results = new();

    private readonly List<IXmlSearchStrategy> strategies = new()
    {
        new SaxSearchStrategy(), new DomSearchStrategy(), new LinqSearchStrategy()
    };

    public MainPage()
    {
        InitializeComponent();
        htmlExporter = new HtmlExporter(transformer, new ResultsXmlBuilder());

        StrategyPicker.ItemsSource = strategies.Select(s => s.Name).ToList();
        StrategyPicker.SelectedIndex = 0;
        ResetFilters(new Dictionary<string, List<string>>());
    }


    private async void OnLoadXmlClicked(object? sender, EventArgs e)
    {
        var file = await PickFileAsync("Оберіть XML-файл", ".xml");
        if (file is null) return;

        try
        {
            var values = attributeReader.LoadValues(file);
            xmlFilePath = file;
            XmlPathLabel.Text = Path.GetFileName(file);
            ResetFilters(values);
            ClearOutput();
            StatusLabel.Text = "Файл завантажено. Задайте параметри та натисніть «Знайти».";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка", $"Не вдалося прочитати XML: {ex.Message}", "OK");
        }
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        if (xmlFilePath is null)
        {
            await DisplayAlertAsync("Увага", "Спочатку завантажте XML-файл.", "OK");
            return;
        }

        try
        {
            RunSearch(xmlFilePath);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка пошуку", ex.Message, "OK");
        }
    }

    private void RunSearch(string xmlPath)
    {
        searchContext.SetStrategy(strategies[StrategyPicker.SelectedIndex]);
        results = searchContext.ExecuteSearch(xmlPath, ReadCriteria());
        ShowResults();
    }

    private async void OnTransformClicked(object? sender, EventArgs e)
    {
        if (xmlFilePath is null)
        {
            await DisplayAlertAsync("Увага", "Спочатку завантажте XML-файл.", "OK");
            return;
        }

        var xsl = xslFilePath ?? await SelectXslAsync();
        if (xsl is null) return;

        string xmlPath = xmlFilePath;

        try
        {
            RunSearch(xmlPath);
            if (results.Count == 0)
            {
                await DisplayAlertAsync("Увага", "За цими параметрами нікого не знайдено.", "OK");
                return;
            }

            var selected = results.ToList();
            string htmlPath = await Task.Run(() => htmlExporter.Export(selected, xmlPath, xsl));
            StatusLabel.Text = $"HTML збережено ({selected.Count} студ.): {htmlPath}";
            OpenInBrowser(htmlPath);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка трансформації", ex.Message, "OK");
        }
    }

    private async void OnSelectXslClicked(object? sender, EventArgs e)
    {
        await SelectXslAsync();
    }

    private async Task<string?> SelectXslAsync()
    {
        var xsl = await PickFileAsync("Оберіть XSL-файл", ".xsl", ".xslt");
        if (xsl is null) return null;
        xslFilePath = xsl;
        XslLabel.Text = "XSL: " + Path.GetFileName(xsl);
        return xsl;
    }

    private static void OpenInBrowser(string path)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true });
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        FacultyPicker.SelectedIndex = DepartmentPicker.SelectedIndex =
            CoursePicker.SelectedIndex = BuildingPicker.SelectedIndex = 0;
        KeywordEntry.Text = "";
        ClearOutput();
        StatusLabel.Text = "Параметри та результати очищено.";
    }

    private async void OnExitRequested(object? sender, EventArgs e)
    {
        bool yes = await DisplayAlertAsync("Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
        if (!yes) return;
        App.ExitConfirmed = true;
        Application.Current?.Quit();
    }


    private SearchCriteria ReadCriteria() => new()
    {
        Faculty = Selected(FacultyPicker),
        Department = Selected(DepartmentPicker),
        Course = int.TryParse(Selected(CoursePicker), out int c) ? c : null,
        Building = Selected(BuildingPicker),
        Keyword = KeywordEntry.Text ?? ""
    };

    private static string Selected(Picker p) =>
        p.SelectedItem is string s && s != Any ? s : "";

    private void ResetFilters(Dictionary<string, List<string>> values)
    {
        Fill(FacultyPicker, values, "Faculty");
        Fill(DepartmentPicker, values, "Department");
        Fill(CoursePicker, values, "Course");
        Fill(BuildingPicker, values, "Building");
        KeywordEntry.Text = "";
    }

    private static void Fill(Picker picker, Dictionary<string, List<string>> values, string attr)
    {
        var items = new List<string> { Any };
        if (values.TryGetValue(attr, out var list)) items.AddRange(list);
        picker.ItemsSource = items;
        picker.SelectedIndex = 0;
    }

    private void ShowResults()
    {
        ResultsView.ItemsSource = results;
        ResultsLabel.Text = $"Результати: знайдено {results.Count}";
        StatusLabel.Text = $"Пошук виконано способом: {strategies[StrategyPicker.SelectedIndex].Name}";
    }

    private void ClearOutput()
    {
        results = new List<Student>();
        ResultsView.ItemsSource = results;
        ResultsLabel.Text = "Результати";
    }

    private static async Task<string?> PickFileAsync(string title, params string[] extensions)
    {
        var types = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, extensions }
        });
        var result = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = title, FileTypes = types });
        return result?.FullPath;
    }
}
