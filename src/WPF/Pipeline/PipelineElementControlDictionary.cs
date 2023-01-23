using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Pipeline;

namespace RCAudioPlayer.WPF.Pipeline
{
	public static class PipelineElementControlDictionary
	{
		static private Dictionary<Type, ConstructorInfo> Specials { get; }

		static PipelineElementControlDictionary()
		{
			var pipelineElementControlType = typeof(PipelineElementControl);
			Specials = (from type in PluginManager.Types
						where pipelineElementControlType.IsAssignableFrom(type)
						let attribute = type.GetCustomAttribute<PipelineElementControlAttribute>()
						where attribute != null
						let constructor = type.GetConstructor(new Type[] { attribute.PipelineElementType })
						where constructor != null
						select (attribute.PipelineElementType, constructor))
					   .ToDictionary(k => k.Item1, s => s.constructor);
		}

		public static PipelineElementControl? GetFor(PipelineElement pipelineElement)
		{
			if (Specials.TryGetValue(pipelineElement.GetType(), out var constructor))
				return (PipelineElementControl)constructor.Invoke(new object[] { pipelineElement });
			return null;
		}
	}
}