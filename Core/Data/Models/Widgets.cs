namespace Core.Data.Models
{
    public class Widgets
    {
        public int Id { get; set; }
        public string Desc { get; set; }
        public string Name { get; set; }
        public string DefaultImage { get; set; }
        public string DragImage { get; set; }
      
        public ICollection<WidgetSettings> WidgetSettings { get; set; } = new List<WidgetSettings>();

    }
}
