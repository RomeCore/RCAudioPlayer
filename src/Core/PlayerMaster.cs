using System;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.Core
{
    public class PlayerMaster : IDisposable
	{
		public AudioOutput Output { get; }
		public PipelineStream PipelineStream { get; }

		public Players.Player? CurrentPlayer { get; internal set; }
		public AudioInput? Current { get; internal set; }

		public float Volume { get => Output.Volume; set => Output.Volume = value; }

		public PipelineManager PipelineManager { get => PipelineStream.PipelineManager; }
		public bool PipelineProcessingEnabled { get => PipelineManager.Enabled; set => PipelineManager.Enabled = value; }

		public PlayerMaster()
		{
			PipelineStream = new PipelineStream();
			Output = new AudioOutput();
        }

        public void Dispose()
        {
			Save();
            PipelineStream.Dispose();
			Output.Dispose();
        }

        public void Save()
		{
			PipelineManager.Save();
		}

		public void Load(AudioInput input)
		{
			PipelineStream.PipelineManager.Current?.ClearState();
			PipelineStream.InputStream = input;
			Output.Init(PipelineStream);
			Output.Play();
        }
	}
}