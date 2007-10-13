namespace WatiN.Core
{
  using System;
  using System.Timers;

  /// <summary>
  /// This class provides a simple way to handle loops that have to time out after 
  /// a specified number of seconds.
  /// </summary>
  /// <example>
  /// This is an example how you could use this class in your code.
  /// <code>
  /// // timer should elapse after 30 seconds
  /// SimpleTimer timeoutTimer = new SimpleTimer(30);
  ///
  /// do
  /// {
  ///   // Your check logic goes here
  ///   
  ///   // wait 200 miliseconds
  ///   Thread.Sleep(200);
  /// } while (!timeoutTimer.Elapsed);
  /// </code>
  /// </example>
  public class SimpleTimer
  {
    private Timer clock = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleTimer"/> class.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    public SimpleTimer(int timeout)
    {
      if (timeout < 0)
      {
        throw new ArgumentOutOfRangeException("timeout", timeout, "Should be equal are greater then zero.");
      }

      if (timeout > 0)
      {
        clock = new Timer(timeout*1000);
        clock.AutoReset = false;
        clock.Elapsed += new ElapsedEventHandler(ElapsedEvent);
        clock.Start();
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="SimpleTimer"/> is elapsed.
    /// </summary>
    /// <value><c>true</c> if elapsed; otherwise, <c>false</c>.</value>
    public bool Elapsed
    {
      get { return (clock == null); }
    }

    private void ElapsedEvent(object source, ElapsedEventArgs e)
    {
      clock.Stop();
      clock.Close();
      clock = null;
    }
  }
}