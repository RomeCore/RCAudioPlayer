using System.Linq;
using System.Windows;
using RCAudioPlayer.Core;
using RCAudioPlayer.WPF.Pipeline;

namespace RCAudioPlayer.WPF.Management
{
    public partial class PipelineManagerControl
	{
		public PipelineManager? PipelineManager { get; private set; }
		public string? CurrentName { get; private set; }
		public Core.Pipeline.Pipeline? Current { get; private set; }
		public PipelineControl? CurrentControl { get; private set; }

		public bool Enabled { get => PipelineManager?.Enabled ?? false; set
			{
				enabler.IsChecked = value;
				if (PipelineManager != null)
					PipelineManager.Enabled = value;
			}
		}

		public PipelineManagerControl()
		{
			InitializeComponent();
		}

		public void Load(PipelineManager pipelineManager)
		{
			Load();

			PipelineManager = pipelineManager;
			Enabled = PipelineManager.Enabled;

			foreach (var chain in PipelineManager.Pipelines)
				effectChainSelector.Items.Add(chain);
			effectChainSelector.SelectionChanged += (s, e) => Select((string)effectChainSelector.SelectedValue);

			enabler.Click += (s, e) => Enabled = enabler.IsChecked ?? false;
			addButton.Click += AddButton_Click;
			deleteButton.Click += DeleteButton_Click;

			var startPipeline = Files.State.Get("pipeline", string.Empty);
			if (!string.IsNullOrEmpty(startPipeline) && PipelineManager.Pipelines.Contains(startPipeline))
				try
                {
                    Select(startPipeline);
                }
				catch
				{

				}
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Dialogs.AddPipelineDialog();
			if (dialog.ShowDialog() ?? false)
				Add(dialog.EffectChainName, dialog.BasedOn, true);
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			if (effectChainSelector.SelectedIndex != -1 && 
				(new Dialogs.QuestionDialog($"{"remove_sure_frag".Translate()} {effectChainSelector.Text}?", "remove_sure_header".Translate()).ShowDialog() ?? false))
				Remove(effectChainSelector.Text);
		}

		public bool Add(string name, string basedOn = "", bool select = false)
		{
			if (PipelineManager?.Add(name, basedOn) ?? false)
			{
				effectChainSelector.Items.Add(name);
				if (select)
					Select(name);
				return true;
			}
			return false;
		}

		public bool Remove(string name)
		{
			if (PipelineManager?.Remove(name) ?? false)
			{
				if (CurrentName == name)
					Deselect();
				effectChainSelector.Items.Remove(name);
				return true;
			}
			return false;
		}

		public bool Select(string name)
		{
			if (PipelineManager?.Select(name) ?? false)
			{
				Files.State.Set("pipeline", name);

				effectChainSelector.SelectedValue = CurrentName = name;
				Current = PipelineManager.Current;
				if (Current != null)
					CurrentControl = new PipelineControl(Current);
				else
					CurrentControl = null;
				pipelineHolder.Content = CurrentControl;

				return true;
			}

			Deselect();
			return false;
		}

		public void Deselect()
		{
			PipelineManager?.Deselect();
			effectChainSelector.SelectedIndex = -1;

			CurrentName = null;
			Current = null;
			pipelineHolder.Content = CurrentControl = null;
		}
	}
}