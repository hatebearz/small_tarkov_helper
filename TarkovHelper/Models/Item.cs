using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TarkovHelper.Models
{
    public class Item : ReactiveObject, IDisposable, IEquatable<Item>
    {
        private readonly IDisposable _cleanUp;

        public SourceList<Requirement> _requirements { get; set; } = new SourceList<Requirement>();

        private Item()
        {
            var requirementsBinding = _requirements.Connect()
                .Bind(out var collection)
                .DisposeMany()
                .Subscribe();

            var propertyChanged = _requirements.Connect()
                .WhenPropertyChanged(x => x.For)
                .Throttle(TimeSpan.FromSeconds(1))
                .Select(_ => Unit.Default);
            var sort = _requirements.Connect().Sort(SortExpressionComparer<Requirement>.Ascending(x => x.For), resort: propertyChanged);

            Requirements = collection;

            var aggregateDuplicatedRequirements = _requirements.Connect()
                .AutoRefresh(x => x.For)
                .Throttle(TimeSpan.FromSeconds(2))
                .Subscribe(changes =>
                {
                    foreach (var change in changes.Where(x =>
                        x.Reason == ListChangeReason.Add || x.Reason == ListChangeReason.Refresh ||
                        x.Reason == ListChangeReason.Replace))
                    {
                        var item = change.Item.Current;
                        if (string.IsNullOrWhiteSpace(item.For)) return;
                        var anotherExistingItem = _requirements.Items.Except(new[] { item })
                            .FirstOrDefault(x => x.For.Equals(item.For, StringComparison.OrdinalIgnoreCase));
                        if (anotherExistingItem == null) return;
                        anotherExistingItem.Amount += item.Amount;
                        _requirements.Remove(item);
                    }

                    foreach (var change in changes.Where(x => x.Reason == ListChangeReason.AddRange))
                    {
                        foreach (var item in change.Range)
                        {
                            if (string.IsNullOrWhiteSpace(item.For)) return;
                            var anotherExistingItem = _requirements.Items.Except(new[] { item })
                                .FirstOrDefault(x => x.For.Equals(item.For, StringComparison.OrdinalIgnoreCase));
                            if (anotherExistingItem == null) return;
                            anotherExistingItem.Amount += item.Amount;
                            _requirements.Remove(item);
                        }
                    }
                });

            var deleteRequirementsWithZeroAmount = _requirements.Connect()
                .AutoRefresh(x => x.Amount)
                .Subscribe(changes =>
                {
                    foreach (var change in changes.Where(x => x.Reason == ListChangeReason.Add || x.Reason == ListChangeReason.Refresh || x.Reason == ListChangeReason.Replace))
                    {
                        var item = change.Item.Current;
                        if (item.Amount <= 0)
                            _requirements.Remove(item);
                    }

                    foreach (var change in changes.Where(x => x.Reason == ListChangeReason.AddRange))
                    {
                        foreach (var item in change.Range)
                        {
                            if (item.Amount <= 0)
                                _requirements.Remove(item);
                        }
                    }
                });

            var totalAmount = _requirements.Connect()
                .AutoRefresh(x => x.Amount)
                .QueryWhenChanged(query => { return query.Sum(x => x.Amount); })
                .Subscribe(x => Total = x);

            _cleanUp = Disposable.Create(() =>
            {
                totalAmount.Dispose();
                requirementsBinding.Dispose();
                deleteRequirementsWithZeroAmount.Dispose();
                aggregateDuplicatedRequirements.Dispose();
                _requirements.Dispose();
            });
        }

        public Item(string name, int price): this()
        {
            Name = name;
            Price = price;
        }

        [Reactive] public string Name { get; set; }

        [Reactive] public int Price { get; set; }

        [Reactive] public int Total { get; private set; }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }

        public bool Equals(Item other)
        {
            return Name == other.Name;
        }

        public void AddRequirement(Requirement requirement)
        {
            requirement.Item = this;
            _requirements.Add(requirement);
        }

        public ReadOnlyObservableCollection<Requirement> Requirements { get; }

        public IObservable<IChangeSet<Requirement>> Connect()
        {
            return _requirements.Connect();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}