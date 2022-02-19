namespace AmethystWindows.Settings
{
    public class MySettings : SettingsManager<MySettings>
    {
        public int Padding { get; set; } = 0;
        public int Step { get; set; } = 25;
        public int LayoutPadding { get; set; } = 4;
        public int MarginTop { get; set; } = 0;
        public int MarginRight { get; set; } = 0;
        public int MarginBottom { get; set; } = 0;
        public int MarginLeft { get; set; } = 0;
        public int VirtualDesktops { get; set; } = 0;


        public string ?Filters { get; set; } = "[]";
        public string ?Factors { get; set; } = "[]";
        public string ?Layouts { get; set; } = "[]";
    }
}
