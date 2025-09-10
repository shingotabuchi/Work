namespace Fwk.Addressables
{
    public static class AddressableAssetKeys
    {
        public const string Root = "Assets/AddressableResources/";
        public const string CueSheets = Root + "Sounds/CueSheets/";

        public static string GetCueSheetKey(string cueSheetName)
        {
            return $"{CueSheets}{cueSheetName}.asset";
        }
    }
}
