namespace RCAudioPlayer.Core
{
	public static class Utils
	{
		public static string[] ParseTitle(string fullTitle)
        {
            var splitted = fullTitle.Split(" - ", 2);
            var result = new string[2];
            if (splitted.Length == 1)
            {
                result[0] = string.Empty;
                result[1] = fullTitle.Trim();
            }
            else
            {
                result[0] = splitted[0].Trim();
                result[1] = splitted[1].Trim();
            }
            return result;
        }

        public static void ShowInExplorer(string filename)
        {
            string argument = "/select, \"" + filename + "\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
	}
}