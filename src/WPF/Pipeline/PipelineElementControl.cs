using System;
using System.Windows.Controls;
using RCAudioPlayer.Core.Pipeline;

namespace RCAudioPlayer.WPF.Pipeline
{
	public class PipelineElementControl : UserControl
	{
		public PipelineElement PipelineElement { get; }

		public PipelineElementControl(PipelineElement pipelineElement)
		{
			PipelineElement = pipelineElement;
		}

		public virtual void Reset()
		{
			PipelineElement.Reset();
		}
	}
}