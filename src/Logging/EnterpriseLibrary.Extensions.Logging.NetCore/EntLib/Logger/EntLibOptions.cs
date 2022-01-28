
namespace EnterpriseLibrary.EntLibExtensions.Logging.EntLib
{
    public sealed class EntLibOptions
    {
        private const string _defaultEntLibFileName = "App.config";
        public EntLibOptions()
            : this(_defaultEntLibFileName)
        {

        }
        public EntLibOptions(string entLibConfigFileName)
            : this(entLibConfigFileName, false)
        {

        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public EntLibOptions(string entLibConfigFileName, bool watch)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.ConfigFileName = entLibConfigFileName;
            this.Watch = watch;
        }
        public string ConfigFileName { get; set; }
        public bool Watch { get; set; }
        public string ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string AppDomainName { get; set; }
        public ICollection<string> Categories { get; set; }
        public string[] CategoriesStrings { get; set; }
        public string[] ErrorMessages { get; set; }
        public int EventId { get; set; }
        public string Severity { get; set; }
        public string MachineName { get; set; }
        public string ManagedThreadName { get; set; }
        public string Message { get; set; }
        public int Priority { get; set; }
        public string ProcessId { get; set; }
        public string ProcessName { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Title { get; set; }
    }
}
