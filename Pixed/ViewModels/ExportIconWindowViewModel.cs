using Pixed.Controls;

namespace Pixed.ViewModels;
internal class ExportIconWindowViewModel : PixedViewModel
{
    private bool _check512;
    private bool _check256;
    private bool _check128;
    private bool _check64;
    private bool _check32;
    private bool _check16;

    public bool Check512
    {
        get => _check512;
        set
        {
            _check512 = value;
            OnPropertyChanged();
        }
    }

    public bool Check256
    {
        get => _check256;
        set
        {
            _check256 = value;
            OnPropertyChanged();
        }
    }

    public bool Check128
    {
        get => _check128;
        set
        {
            _check128 = value;
            OnPropertyChanged();
        }
    }

    public bool Check64
    {
        get => _check64;
        set
        {
            _check64 = value;
            OnPropertyChanged();
        }
    }

    public bool Check32
    {
        get => _check32;
        set
        {
            _check32 = value;
            OnPropertyChanged();
        }
    }

    public bool Check16
    {
        get => _check16;
        set
        {
            _check16 = value;
            OnPropertyChanged();
        }
    }
}