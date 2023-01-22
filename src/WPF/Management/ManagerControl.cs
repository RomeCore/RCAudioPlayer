using System;
using System.Windows.Controls;

namespace RCAudioPlayer.WPF.Management
{
	public class ManagerControl : UserControl
	{
		private bool _loaded = false;

		protected void Load()
		{
			if (_loaded)
				throw new InvalidOperationException($"{GetType()} is already loaded!");

			_loaded = true;
		}
	}
}