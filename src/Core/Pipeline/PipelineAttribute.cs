using System;

namespace RCAudioPlayer.Core.Pipeline
{
	public class PipelineAttribute : Attribute
	{
		public string Id { get; }

		public PipelineAttribute(string id)
		{
			Id = id;
		}
	}
}