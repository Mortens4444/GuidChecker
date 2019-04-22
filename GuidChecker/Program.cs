using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuidChecker
{
    class MainClass
    {
		private static string ProjectGuidTag = "<ProjectGuid>";
		private static string ProjectGuidClosingTag = "</ProjectGuid>";
		
        public static void Main(string[] args)
        {
			if (args.Length != 1)
			{
				throw new NotSupportedException("Provide a folder path. For example: C:\\Projects\\MyApplication");
			}

			var folder = args.First();
			if (!Directory.Exists(folder))
			{
				throw new NotSupportedException($"Folder not found: {folder}");
			}

			var guids = new Dictionary<string, List<string>>();
			var projectFileNames = Directory.GetFiles(folder, "*.csproj", SearchOption.AllDirectories);
			foreach (var projectFileName in projectFileNames)
			{
				var projectGuid = GetProjectGuid(projectFileName);
				if (!guids.ContainsKey(projectGuid))
				{
					guids.Add(projectGuid, new List<string> { projectFileName });
				}
				else
				{
					// This is a duplicate
					guids[projectGuid].Add(projectFileName);
				}
			}

			var conflictingProjectGuids = guids.Where(projectGuid => projectGuid.Value.Count > 1);
			if (conflictingProjectGuids.Any())
			{
				Console.WriteLine("The following project GUIDs are conflicting.");
				foreach (var conflictingProjectGuid in conflictingProjectGuids)
				{
					Console.WriteLine($"GUID: {conflictingProjectGuid.Key}");
					Console.WriteLine(String.Join(Environment.NewLine, conflictingProjectGuid.Value));
					Console.WriteLine();
				}
			}

			Console.ReadKey();
        }

		private static string GetProjectGuid(string projectFile)
		{
			var fileContent = File.ReadAllText(projectFile);
			var startIndex = fileContent.IndexOf(ProjectGuidTag, StringComparison.Ordinal);
            if (startIndex == -1)
			{
				return null;
			}
			startIndex += ProjectGuidTag.Length;

			var endIndex = fileContent.IndexOf(ProjectGuidClosingTag, StringComparison.Ordinal);
            if (endIndex == -1)
            {
                return null;
            }

			var length = endIndex - startIndex;
			return fileContent.Substring(startIndex, length);
		}
	}
}
