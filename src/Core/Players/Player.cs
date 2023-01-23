using System;
using System.IO;
using System.Threading.Tasks;

namespace RCAudioPlayer.Core.Players
{
	public abstract class Player
	{
		public PlayerMaster Master { get; }
		public virtual string Name { get; set; }

		public Player(PlayerMaster master, string name)
		{
			Master = master;
			Name = name;

			master.CurrentPlayer = this;
			master.Output.StoppedFunc = () => { };
			master.Output.StoppedNaturally = () => { };
		}

		public abstract Task Load(StreamReader reader);
		public abstract Task Save(StreamWriter writer);

		public virtual void Play() => Master.Output.Play();
		public virtual void Pause() => Master.Output.Pause();
		public virtual void Stop() => Master.Output.Stop();
		public virtual void PlayPause() => Master.Output.PlayPause();

		public abstract void Previous();
		public abstract void Next();
	}
}