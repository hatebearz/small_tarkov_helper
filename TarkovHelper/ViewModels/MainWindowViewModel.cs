using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TarkovHelper.Models;

namespace TarkovHelper.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
    {
        private readonly ModelProviderService _modelProviderService;
        private readonly ApplicationSettingsService _applicationSettingsService;

        public MainWindowViewModel(ModelProviderService modelProviderService, ApplicationSettingsService applicationSettingsService)
        {
            Activator = new ViewModelActivator();
            _modelProviderService = modelProviderService;
            _applicationSettingsService = applicationSettingsService;
            ApplicationSettings = applicationSettingsService.Settings;
            var dynamicFilter = this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Select(text => (Func<Item, bool>) (item => item.Name.Contains(text)));
            var binding = _modelProviderService.Connect()
                .Filter(dynamicFilter)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var items)
                .DisposeMany()
                .Subscribe(Console.WriteLine);
            //var requirementsBinding = _modelProviderService.Connect()
            //    .AutoRefresh()
            //    .GroupBy(x => x.)
            Items = items;
            var canDeleteItem = this.WhenAnyValue(x => x.SelectedItem,
                (Func<Item, bool>) (selectedItem => selectedItem != null));

            this.WhenActivated(disposables =>
            {
                Disposable.Create(() =>
                    {
                        _modelProviderService.Dispose();
                        binding.Dispose();
                        AddItemCommand.Dispose();
                        DeleteItemCommand.Dispose();
                        AddRequirementCommand.Dispose();
                    })
                    .DisposeWith(disposables);
            });


            AddItemCommand = ReactiveCommand.Create(AddItem);
            DeleteItemCommand = ReactiveCommand.Create(DeleteItem, canDeleteItem);
            AddRequirementCommand = ReactiveCommand.Create(AddReqirement, canDeleteItem);
            IncrementAmountCommand = ReactiveCommand.Create<Requirement, Unit>(requirement =>
            {
                requirement.Amount++;
                return Unit.Default;
            });
            DecrementAmountCommand = ReactiveCommand.Create<Requirement, Unit>(requirement =>
            {
                requirement.Amount--;
                return Unit.Default;
            });
        }

        public ReadOnlyObservableCollection<Item> Items { get; }
        public ReadOnlyObservableCollection<Requirement> Requirements { get; }

        public IReadOnlyCollection<RequirementKind> RequirementKinds { get; } = new [] {RequirementKind.Quest, RequirementKind.Hideout};

        [Reactive] public Item SelectedItem { get; set; }
        
        public ApplicationSettings ApplicationSettings { get; set; }

        public ReactiveCommand<Unit, Unit> AddItemCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteItemCommand { get; }
        public ReactiveCommand<Unit, Unit> AddRequirementCommand { get; }
        public ReactiveCommand<Requirement, Unit> IncrementAmountCommand { get; }
        public ReactiveCommand<Requirement, Unit> DecrementAmountCommand { get; }

        [Reactive] public string SearchText { get; set; } = string.Empty;

        public string Greeting => "Welcome to Avalonia!";

        public ViewModelActivator Activator { get; }

        private void AddReqirement()
        {
            SelectedItem.AddRequirement(new Requirement{Amount = 1, Item = SelectedItem});
        }

        private void AddItem()
        {
            _modelProviderService.AddItem(new Item(string.Empty, 0));
        }

        private void DeleteItem()
        {
            _modelProviderService.DeleteItem(SelectedItem);
        }
    }
}