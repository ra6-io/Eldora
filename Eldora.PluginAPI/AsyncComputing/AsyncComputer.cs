namespace Eldora.PluginAPI.AsyncComputing;

public sealed class AsyncComputer
{
	/// <summary>
	///     Starts a new asynchronous computation.
	/// </summary>
	/// <param name="computable"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static Task StartComputation(AbstractComputable computable, TaskCreationOptions options = TaskCreationOptions.LongRunning)
	{
		return Task.Factory.StartNew(computable.Compute, options);
	}
}