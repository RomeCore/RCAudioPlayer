using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.Core.Players
{
	public abstract class ElementPlayer : Player
	{
		public class UpdateArgs<TPlayable> where TPlayable : IPlayable
		{
			public TPlayable Playable { get; }

			public UpdateArgs(TPlayable playable)
			{
				Playable = playable;
			}
		}

		protected IPlayable? _current = null;
		protected WaveStream? _waveStream = null;

		public IPlayable? Current => _current;
		public AudioInput? InputStream { get; protected set; }
		public bool Loop { get; set; }

		public event EventHandler<UpdateArgs<IPlayable>> OnElementUpdate = (s, e) => { };

		public override void Previous()
		{
			_current?.SkipBackward();
		}

		public override void Next()
		{
			_current?.SkipForward();
		}

		public virtual async Task Play(IPlayable playable)
		{
			Master.Load(InputStream = await playable.GetInput());
			OnElementUpdate(this, new UpdateArgs<IPlayable>(playable));
		}

		public override async void Play()
		{
			if (_current != null)
				await Play(_current);
			else
				base.Play();
		}

		protected ElementPlayer(PlayerMaster master, string name) : base(master, name)
		{
			master.Output.StoppedNaturally = async () =>
			{
				if (Loop && _current != null)
					await Play(_current);
			};
		}
	}

	public abstract class ElementPlayer<TPlayable> : ElementPlayer
		where TPlayable : class, IPlayable
	{
		public new TPlayable? Current => _current as TPlayable;

		public new event EventHandler<UpdateArgs<TPlayable>> OnElementUpdate = (s, e) => { };

		public ElementPlayer(PlayerMaster master, string name) : base(master, name)
		{
		}

		public async Task Set(string str)
		{
			_current = await Create(str);
		}

		public override async Task Load(StreamReader reader)
		{
			var str = reader.ReadToEnd();
			if (str != null)
				await Set(str);
		}

		public override Task Save(StreamWriter writer)
		{
			if (_current != null)
				writer.Write(_current.Save());
			return Task.CompletedTask;
		}

		public abstract Task<IPlayable> Create(string str);
	}
}