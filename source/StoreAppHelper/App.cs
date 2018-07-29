namespace StoreAppHelper
{
    public sealed class App
    {
        public App(string fullName, string displayName, string publisherDisplayName, string logo,
            string installedLocation, bool isProtected)
        {
            FullName = fullName;
            DisplayName = displayName;
            PublisherDisplayName = publisherDisplayName;
            Logo = logo;
            InstalledLocation = installedLocation;
            IsProtected = isProtected;
        }

        public string FullName { get; }
        public string DisplayName { get; }
        public string PublisherDisplayName { get; }
        public string Logo { get; }
        public string InstalledLocation { get; }
        public bool IsProtected { get; }
    }
}