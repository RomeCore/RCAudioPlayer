using System;

namespace RCAudioPlayer.WPF.Pipeline
{
	public class PipelineElementControlAttribute : Attribute
	{
		public Type PipelineElementType { get; }

		public PipelineElementControlAttribute(Type pipelineElementType)
		{
			PipelineElementType = pipelineElementType;
		}
	}
}