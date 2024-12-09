using Avalonia.Platform.Storage;
using Pixed.Application.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Application.ViewModels;
internal class SaveDialogFormatSelectionViewModel : PixedViewModel
{
    private List<FilePickerFileType> _formats = [];
    private List<string> _listFormats = [];
    private int _selectionIndex = -1;

    public List<FilePickerFileType> Formats
    {
        get => _formats;
        set
        {
            _formats = value;
            OnPropertyChanged();
            ListFormats = Formats.Select(f => f.Name).ToList();
        }
    }

    public List<string> ListFormats
    {
        get => _listFormats;
        set
        {
            _listFormats = value;
            OnPropertyChanged();
            SelectionIndex = -1;
        }
    }

    public int SelectionIndex
    {
        get => _selectionIndex;
        set
        {
            _selectionIndex = value;
            OnPropertyChanged();

            if(value >= 0)
            {
                CloseAction?.Invoke();
            }
        }
    }

    public Action? CloseAction { get; set; }
}
