using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

namespace TestApp;

public class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        // Initialize with some test data
        for (int i = 0; i < 500; i++)
        {
            LotsOfItems.Add(new ItemViewModel($"Item {i}"));
        }
    }

    private double _itemSize = 70;

    public double ItemSize
    {
        get { return _itemSize; }
        set
        {
            _itemSize = value;
            OnPropertyChanged();
        }
    }


    public ObservableCollection<ItemViewModel> LotsOfItems { get; } = new();
}

public class ItemViewModel : ObservableObject
{
    public ItemViewModel(string name)
    {
        Name = name;
        // Assign a random color
        var rand = new Random(name.GetHashCode());
        Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256)));
    }
    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public Brush Color { get; }
}

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}