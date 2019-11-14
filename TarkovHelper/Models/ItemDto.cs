using System.Collections.Generic;
using System.Linq;

namespace TarkovHelper.Models
{
    public class ItemDto
    {
        public ItemDto()
        {
        }

        public ItemDto(Item item)
        {
            Name = item.Name;
            Price = item.Price;
            Requirements = item._requirements.Items
                .Select(x => new RequirementDto {Amount = x.Amount, For = x.For, Kind = x.Kind}).ToList();
        }

        public string Name { get; set; }

        public int Price { get; set; }

        public List<RequirementDto> Requirements { get; set; } = new List<RequirementDto>();

        public Item Map()
        {
            var ret = new Item(Name, Price);
            foreach (var requirement in Requirements)
                ret.AddRequirement(new Requirement {Amount = requirement.Amount, For = requirement.For, Item = ret});
            return ret;
        }
    }
}