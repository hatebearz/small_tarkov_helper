namespace TarkovHelper.Models
{
    public class RequirementDto
    {
        public int Amount { get; set; }
        public string For { get; set; }
        public RequirementKind Kind { get; set; }
    }
}