using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RCAudioPlayer.Core.Players
{
	public abstract class ListPlayer : ElementPlayer
	{
		public class ListUpdatedArgs<TPlayable> where TPlayable : IPlayable
		{
			public IReadOnlyList<TPlayable> List { get; }

			public ListUpdatedArgs(IReadOnlyList<TPlayable> list)
			{
				List = list;
			}
		}

		protected List<IPlayable> _playlist = new List<IPlayable>();
		protected List<int> _queue = new List<int>();
		protected bool _shuffle;

		public bool Shuffle { get => _shuffle; set { _shuffle = value; UpdateQueue(); } }

		public IReadOnlyList<IPlayable> Playlist => _playlist;
		public IReadOnlyList<int> Queue => _queue;

		public int ListPosition { get; protected set; }
		public int QueuePosition { get; protected set; }

		public new event EventHandler<UpdateArgs<IPlayable>> OnElementUpdate = (s, e) => { };
		public event EventHandler<ListUpdatedArgs<IPlayable>> OnListUpdated = (s, e) => { };

		protected int ClampIndex(int index)
		{
			if (_playlist.Count == 0)
				return -1;
			if (index < 0)
				return _playlist.Count - 1;
			if (index > _playlist.Count - 1)
				return 0;
			return index;
		}

		protected int GetIndex(int queueIndex)
		{
			if (queueIndex == -1 || _queue.Count == 0)
				return -1;
			return _queue[queueIndex];
		}

		protected void UpdatePos()
		{
			if (_playlist.Count > 0)
			{
				QueuePosition = ClampIndex(QueuePosition);
				ListPosition = GetIndex(QueuePosition);
				Update();
			}
		}

		protected void UpdateQueuePos()
		{
			if (_playlist.Count > 0 && Current != null)
				QueuePosition = _queue.IndexOf(_playlist.IndexOf(Current));
		}

		protected void UpdateQueue()
		{
			var queue = Enumerable.Range(0, _playlist.Count);
			if (Shuffle)
			{
				var random = new Random();
				queue = queue.OrderBy(k => random.Next());
			}
			_queue = queue.ToList();
			UpdateQueuePos();
		}

		protected void UpdateList()
		{
			UpdateQueue();
			OnListUpdated(this, new ListUpdatedArgs<IPlayable>(_playlist));
		}

		protected async Task Update(IPlayable playable)
		{
			_current = playable;
			await base.Play(playable);
		}

		protected void Update()
		{
			if (ListPosition > -1)
				Update(_playlist[ListPosition]).ConfigureAwait(true);
		}

		protected ListPlayer(PlayerMaster master, string name) : base(master, name)
		{
			base.OnElementUpdate += (s, e) => OnElementUpdate(s, e);

			master.Output.StoppedNaturally = async () =>
			{
				if (Loop && _current != null)
					await Play(_current);
				else
					Next();
			};
		}

		public virtual bool Insert(IPlayable playable, int position)
		{
			return Insert(new IPlayable[] {playable}, position);
		}
		
		public virtual bool Insert(IEnumerable<IPlayable> playables, int position)
		{
			if (position <= _playlist.Count)
			{
				foreach (var playable in playables)
					_playlist.Insert(position++, playable);
				UpdateList();
				return true;
			}
			return false;
		}
		
		public virtual void Add(IPlayable playable)
		{
			Insert(playable, _playlist.Count);
		}
		
		public virtual void AddFirst(IPlayable playable)
		{
			Insert(playable, 0);
		}
		
		public virtual bool Remove(int index)
		{
			if (index > -1 && index < _playlist.Count)
			{
				_playlist.RemoveAt(index);
				UpdateList();
				return true;
			}
			return false;
		}
		
		public virtual bool Remove(IPlayable playable)
		{
			if (_playlist.Remove(playable))
			{
				UpdateList();
				return true;
			}
			return false;
		}

		public virtual bool Move(IPlayable playable, int targetIndex)
		{
			if (targetIndex <= _playlist.Count && _playlist.Remove(playable))
			{
				_playlist.Insert(targetIndex, playable);
				UpdateList();
				return true;
			}
			return false;
		}
		
		public virtual bool Move(IPlayable source, IPlayable target)
		{
			if (_playlist.Contains(target) && _playlist.Remove(source))
			{
				_playlist.Insert(_playlist.IndexOf(target), source);
				UpdateList();
				return true;
			}
			return false;
		}
		
		public virtual bool Move(int sourceIndex, int targetIndex)
		{
			return Move(_playlist[sourceIndex], targetIndex);
		}

		public virtual int IndexOf(IPlayable playable)
		{
			return _playlist.IndexOf(playable);
		}

		public override void Previous()
		{
			if (_current?.SkipBackward() ?? true)
			{
				QueuePosition--;
				UpdatePos();
			}
		}

		public override void Next()
		{
			if (_current?.SkipForward() ?? true)
			{
				QueuePosition++;
				UpdatePos();
			}
		}

		public override async void Play()
		{
			if (_playlist.Count > 0 && Master.Output.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
				await Play(ListPosition);
			else
				base.Play();
		}

		public override async Task Play(IPlayable playable)
		{
			await Play(_playlist.IndexOf(playable));
		}

		public async Task Play(int index)
		{
			if (index > -1 && index < _playlist.Count)
			{
				await Update(_playlist[index]);
				UpdateQueuePos();
			}
		}

		public async void PlayQueue(int queueIndex)
		{
			if (queueIndex > -1 && queueIndex < _playlist.Count)
			{
				await Update(_playlist[GetIndex(queueIndex)]);
				QueuePosition = queueIndex;
			}
		}

		public virtual IAsyncEnumerable<IPlayable> Add(string str)
		{
            return Insert(new string[] { str }, _playlist.Count);
		}
		public virtual IAsyncEnumerable<IPlayable> Add(IEnumerable<string> strs)
		{
			return Insert(strs, _playlist.Count);
		}
		public virtual async IAsyncEnumerable<IPlayable> Insert(IEnumerable<Task<IPlayable>> asyncPlayables, int position)
		{
			foreach (var task in asyncPlayables)
			{
				var playable = await task;
				_playlist.Insert(position++, playable);
				yield return playable;
			}

			UpdateQueue();
		}
		public virtual IAsyncEnumerable<IPlayable> Insert(IEnumerable<string> strs, int position)
		{
			return Insert(strs.Select(s => Create(s)).ToList(), position);
		}
		public abstract Task<IPlayable> Create(string str);
	}

	public abstract class ListPlayer<TPlayable> : ListPlayer
		where TPlayable : class, IPlayable
	{
		public new event EventHandler<UpdateArgs<TPlayable>> OnElementUpdate = (s, e) => { };
		public new event EventHandler<ListUpdatedArgs<TPlayable>> OnListUpdated = (s, e) => { };

		public new IReadOnlyList<TPlayable> Playlist => base.Playlist.Cast<TPlayable>().ToList();
		public new TPlayable? Current => base.Current as TPlayable;

		public ListPlayer(PlayerMaster master, string name) : base(master, name)
		{
			base.OnElementUpdate += (s, e) =>
			{
				if (e.Playable is TPlayable playable)
					OnElementUpdate(s, new UpdateArgs<TPlayable>(playable));
			};
			base.OnListUpdated += (s, e) =>
			{
				if (e.List is IReadOnlyList<TPlayable> list)
					OnListUpdated(s, new ListUpdatedArgs<TPlayable>(list));
			};
		}

		public override async Task Load(StreamReader reader)
		{
			_playlist.Clear();

			var content = reader.ReadToEnd();
			var enumerator = Add(content.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
			await foreach (var _ in enumerator) ;

			ListPosition = 0;
			QueuePosition = 0;
		}

		public override Task Save(StreamWriter writer)
		{
			string result = string.Empty;
			foreach (var playable in _playlist)
				result += playable.Save() + '\n';

			writer.Write(result);
			return Task.CompletedTask;
		}

		public override bool Insert(IEnumerable<IPlayable> playables, int position)
		{
			try
			{
				return base.Insert(playables.Cast<TPlayable>(), position);
			}
			catch
			{
				return false;
			}
		}

		public async Task Play(TPlayable playable)
		{
			await Play((IPlayable)playable);
		}
	}
}