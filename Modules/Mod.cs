namespace Modules
{
    class Mod
    {
        public string mod { get; set; } = null;
        public string uid { get; set; } = null;
        public string name { get; set; } = null;
        public int? kills { get; set; } = null;
        public int? deaths { get; set; } = null;
        public int? points { get; set; } = null;
        public bool? track { get; set; } = null;
        public bool? pointFeed { get; set; } = null;
        public double? speed { get; set; } = null;
        public float? minutesPlayed { get; set; } = null;
        public bool? aboveAnnounceSpeed { get; set; } = false; 
    }
}
