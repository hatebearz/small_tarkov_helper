using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Direct2D1;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TarkovHelper
{
    public class ApplicationSettingsService: ReactiveObject, IDisposable
    {
        private IDisposable _cleanUp; 

        public ApplicationSettingsService()
        {
            var autoSaving = this.WhenAny(x => x.Settings, x => x.Value)
                .Delay(TimeSpan.FromSeconds(3))
                .Subscribe(x => SaveSettings());

            _cleanUp = Disposable.Create(() => { autoSaving.Dispose(); });
        }

        [Reactive] public ApplicationSettings Settings { get; private set; }

        public void LoadSettings()
        {
            if (!File.Exists("settings.json"))
            {
                Settings = new ApplicationSettings();
                return;
            }

            Settings = JsonConvert.DeserializeObject<ApplicationSettings>(File.ReadAllText("settings.json"));
        }

        public void SaveSettings()
        {
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}