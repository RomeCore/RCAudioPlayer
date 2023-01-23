using RCAudioPlayer.Core.Playables;
using System.Threading.Tasks;

namespace RCAudioPlayer.Core.Players
{
	[Player("radio_list")]
	public class RadioListPlayer : ListPlayer<Radio>
	{
		public RadioListPlayer(PlayerMaster master, string name) : base(master, name)
		{
		}

		public override Task<IPlayable> Create(string str)
		{
			return Task.Run(() => (IPlayable)new Radio(str));
		}
	}
}