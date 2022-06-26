namespace DnDHelperV2.PluginAPI.AsyncComputing;

public abstract class AbstractComputable
{
	public event EventHandler<EventArgs>? OnComputingStarted;
	public event EventHandler<EventArgs>? OnComputingStepped;
	public event EventHandler<EventArgs>? OnComputingFinished;

	/// <summary>
	///     Starts the computation
	/// </summary>
	public abstract void Compute();

	/// <summary>
	///     Stops the computation
	/// </summary>
	public virtual void Cancel() {}


	/// <summary>
	///     Raises the ComputingStarted event
	/// </summary>
	/// <param name="args"></param>
	protected void RaiseComputingStarted(EventArgs args)
	{
		OnComputingStarted?.Invoke(this, args);
	}

	/// <summary>
	///     Raises the ComputingStepped event
	/// </summary>
	/// <param name="args"></param>
	protected void RaiseComputingStepped(EventArgs args)
	{
		OnComputingStepped?.Invoke(this, args);
	}

	/// <summary>
	///     Raises the ComputingFinished event
	/// </summary>
	/// <param name="args"></param>
	protected void RaiseComputingFinished(EventArgs args)
	{
		OnComputingFinished?.Invoke(this, args);
	}
}