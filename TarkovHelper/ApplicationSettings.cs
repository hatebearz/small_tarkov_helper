using System;
using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TarkovHelper
{
    [DataContract]
    public class ApplicationSettings: ReactiveObject
    {
        [DataMember] [Reactive] public bool AutoSavingEnabled { get; set; } = true;

        [DataMember] [Reactive] public TimeSpan AutoSavingDelay { get; set; } = TimeSpan.FromSeconds(3);

        [DataMember] [Reactive] public string DataSavingPath { get; set; }
    }
}