using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace RCAudioPlayer.Core
{
	public static class PluginManager
	{
		private static List<Assembly> assemblies { get; }
		private static List<Type> types { get; }
		private static Dictionary<string, string> assemblyFiles { get; }

		public const string Folder = "plugins";
		public const string DependenciesFolder = Folder + "\\dependencies";
		public static IReadOnlyList<Assembly> Assemblies { get; }
		public static IReadOnlyList<Type> Types { get; }

		static PluginManager()
		{
			Directory.CreateDirectory(DependenciesFolder);
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

			assemblyFiles = new Dictionary<string, string>();
            foreach (var filename in Directory.EnumerateFiles(DependenciesFolder, "*.dll"))
				LoadAssembly(filename);

			assemblies = new List<Assembly>();
			assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName?.StartsWith("RCAudioPlayer") ?? false));
            foreach (var filename in Directory.EnumerateFiles(Folder, "*.dll"))
			{
				var assembly = LoadAssembly(filename);
				if (assembly != null)
					assemblies.Add(assembly);
			}
			Assemblies = assemblies.AsReadOnly();

			types = new List<Type>();
			foreach (var assembly in Assemblies)
				try
				{
					var references = assembly.GetReferencedAssemblies();
					types.AddRange(assembly.GetTypes());
				}
				catch (ReflectionTypeLoadException reflExc)
				{
					Log.Exception(reflExc, $"while loading assembly ({assembly})");
				}
			Types = types.AsReadOnly();
		}

		static private Assembly? LoadAssembly(string filename)
		{
			var name = Path.GetFileNameWithoutExtension(filename);
			var path = Path.GetFullPath(filename);
			try
			{
				assemblyFiles.Add(name, path);
				return AppDomain.CurrentDomain.Load(name);
			}
			catch (Exception ex)
			{
				assemblyFiles.Remove(name);
				Log.Exception(ex, $"while loading assembly file ({filename})");
			}
			return null;
		}

		private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
		{
			if (args.Name != null && assemblyFiles.TryGetValue(args.Name.Split(", ")[0], out var fileName))
				return Assembly.Load(File.ReadAllBytes(fileName));

			return null;
		}
	}
}