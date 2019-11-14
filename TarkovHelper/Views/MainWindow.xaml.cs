using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TarkovHelper.ViewModels;

namespace TarkovHelper.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public DataGrid DataGrid => this.FindControl<DataGrid>("ItemsDataGrid");

        public MainWindow()
        {
            this.WhenActivated(disposables =>
            { 
                this.OneWayBind(ViewModel, x => x.Items, x => x.DataGrid.Items)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, x => x.SelectedItem, x => x.DataGrid.SelectedItem)
                    .DisposeWith(disposables);
            });

            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
