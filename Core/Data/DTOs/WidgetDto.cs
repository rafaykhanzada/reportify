namespace Core.Data.DTOs
{
    public class WidgetDto
    {
        public int Id { get; set; }
        public string Desc { get; set; }
        public string Name { get; set; }
        public string DefaultImage { get; set; }
        public string DragImage { get; set; }

        public List<WidgetSettingsDto> WidgetSettings { get; set; }
    }
}
