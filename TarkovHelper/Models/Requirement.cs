using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TarkovHelper.Models
{
    public class Requirement : ReactiveObject
    {
        public Item Item { get; set; }

        [Reactive] public int Amount { get; set; } = 1;

        [Reactive] public string For { get; set; } = string.Empty;

        [Reactive] public RequirementKind Kind { get; set; } = RequirementKind.Hideout;
    }
}