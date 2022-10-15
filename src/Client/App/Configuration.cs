namespace Functionland.FxFiles.Client.App
{
    public static class Configuration
    {
#if Local
        public const string EnvironmentName = "Local";
        public const string AppCenterAndroidAppSecret = "c6db6bea-416d-4688-9e14-ca8b30af1775";
        public const string AppCenteriOSAppSecret = "8b71c972-bb4b-4429-a8b7-5aae33857d1a";
        public const string AppCenterWindowsAppSecret = "7f2ed707-46a6-480d-bf29-d6f027eaed61";

#elif Dev
        public const string EnvironmentName = "Dev";
        public const string AppCenterAndroidAppSecret = "c6db6bea-416d-4688-9e14-ca8b30af1775";
        public const string AppCenteriOSAppSecret = "8b71c972-bb4b-4429-a8b7-5aae33857d1a";
        public const string AppCenterWindowsAppSecret = "7f2ed707-46a6-480d-bf29-d6f027eaed61";
#elif QA
        public const string EnvironmentName = "QA";
        public const string AppCenterAndroidAppSecret = "bb0472f4-1167-48d1-87d1-2624b529184b";
        public const string AppCenteriOSAppSecret = "f96deeac-cb16-4b6c-970f-43f129f176b6";
        public const string AppCenterWindowsAppSecret = "9d9fe7df-0bfe-4d47-802d-983c28a026b9";
#else
        public const string EnvironmentName = "Production";
        public const string AppCenterAndroidAppSecret = "5d0f3db2-33e5-488b-8b0f-6c625b1136a7";
        public const string AppCenteriOSAppSecret = "eab12d9b-918e-47c1-a103-d94c09f711b8";
        public const string AppCenterWindowsAppSecret = "2e8c97c8-135f-41c2-bdba-056334302fc8";
#endif
    }
}
