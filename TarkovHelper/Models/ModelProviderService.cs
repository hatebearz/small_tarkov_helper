using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Newtonsoft.Json;

namespace TarkovHelper.Models
{
    public class ModelProviderService : IDisposable
    {
        public const string ItemsPath = @"F:\Documents\tarkov-items.json";

        private readonly IDisposable _cleanUp;

        private readonly SourceList<Item> _loadedItems;

        public ModelProviderService(ApplicationSettingsService applicationSettingsService)
        {
            _loadedItems = new SourceList<Item>();
            var consoleWriting = _loadedItems
                .Connect()
                .Subscribe(Console.WriteLine);
            var autoSaving = _loadedItems
                .Connect()
                .AutoRefresh()
                .Throttle(TimeSpan.FromSeconds(3))
                .Subscribe(x => SaveItems());

            _cleanUp = Disposable.Create(() =>
            {
                _loadedItems.Dispose();
                autoSaving.Dispose();
                consoleWriting.Dispose();
            });
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }

        public IObservable<IChangeSet<Item>> Connect()
        {
            return _loadedItems.Connect();
        }

        public void LoadItems()
        {
            if (!File.Exists(ItemsPath))
                return;
            _loadedItems.Clear();
            foreach (var item in JsonConvert.DeserializeObject<List<ItemDto>>(File.ReadAllText(ItemsPath))
                .Select(x => x.Map()))
                _loadedItems.Add(item);
        }

        public void SaveItems()
        {
            File.WriteAllText(ItemsPath,
                JsonConvert.SerializeObject(_loadedItems.Items.Select(x => new ItemDto(x)),
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.All,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        Formatting = Formatting.Indented
                    }));
        }

        public void AddItem(Item item)
        {
            _loadedItems.Add(item);
        }

        public void DeleteItem(Item item)
        {
            _loadedItems.Remove(item);
        }
    }
}