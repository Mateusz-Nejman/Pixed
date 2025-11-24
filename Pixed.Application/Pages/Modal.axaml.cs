using Avalonia;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;
using Pixed.Common.DependencyInjection;
using System.Threading.Tasks;

namespace Pixed.Application.Pages;

public partial class Modal : Page
{
    public IPixedServiceProvider? Provider => App.ServiceProvider;
    public object? Dialog
    {
        get { return GetValue(DialogProperty); }
        set { SetValue(DialogProperty, value); }
    }

    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly StyledProperty<object?> DialogProperty = AvaloniaProperty.Register<Modal, object?>(nameof(Dialog));
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<Modal, string>(nameof(Title), "Title");
    public Modal()
    {
        InitializeComponent();
    }

    public Task Close()
    {
        if (Navigator == null)
        {
            return Task.CompletedTask;
        }
        
        return Navigator.BackAsync();
    }

    public Task Close(object result)
    {
        if (Navigator == null)
        {
            return Task.CompletedTask;
        }

        return Navigator.BackAsync(result);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}

