using RCAudioPlayer.Core.Playables;
using System.Threading.Tasks;

namespace RCAudioPlayer.Core.Players
{
	[Player("radio", true)]
	public class RadioPlayer : ElementPlayer<Radio>
	{
		public RadioPlayer(PlayerMaster master, string name) : base(master, name)
		{
		}

		public override Task<IPlayable> Create(string str)
		{
			return Task.Run(() => (IPlayable)new Radio(str));
		}
	}
}