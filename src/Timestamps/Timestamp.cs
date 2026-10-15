namespace Timestamps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Object used to measure start, end, and total time associated with an operation.
    /// </summary>
    public class Timestamp : IDisposable
    {
        #region Public-Members

        /// <summary>
        /// The time at which the operation started.  When instantiated, this is set to DateTime.UtcNow.
        /// </summary>
        public DateTime Start { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The time at which the operation ended.
        /// </summary>
        public DateTime? End { get; set; } = null;

        /// <summary>
        /// The total number of milliseconds that transpired between Start and End.
        /// </summary>
        public double? TotalMs
        {
            get
            {
                if (End == null)
                {
                    return Math.Round(TotalMsBetween(Start, DateTime.UtcNow), 2);
                }
                else
                {
                }
            }
        }




        /// <summary>
        /// The amount of time that transpired between Start and End.  If End is not set, the amount of time between Start and DateTime.UtcNow.
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (End == null)
                {
                    return DateTime.UtcNow - Start;
                }
                else
                {
                    return End.Value - Start;
                }
            }
        }
        /// <summary>
        /// Log messages attached to the object by the user.
        /// </summary>
        public Dictionary<DateTime, string> Messages
        {
            get
            {
                lock (_Lock)
                {
                    return new Dictionary<DateTime, string>(_Messages).OrderBy(d => d.Key).ToDictionary(d => d.Key, d => d.Value);
                }
            }
            set
            {
                if (value == null) value = new Dictionary<DateTime, string>();

                lock (_Lock)
                {
                    _Messages = value;
                }
            }
        }

        /// <summary>
        /// User-supplied metadata.
        /// </summary>
        public object Metadata
        {
            get
            {
                return _Metadata;
            }
            set
            {
                _Metadata = value;
            }
        }

        #endregion

        #region Private-Members

        private readonly object _Lock = new object();
        private Dictionary<DateTime, string> _Messages = new Dictionary<DateTime, string>();
        private object _Metadata = null;
        private bool _Disposed = false;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public Timestamp()
        {

        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Add a message.
        /// </summary>
        /// <param name="msg">Message.</param>
        public void AddMessage(string msg)
        {
            if (String.IsNullOrEmpty(msg)) throw new ArgumentNullException(nameof(msg));

            lock (_Lock)
            {
                DateTime key = DateTime.UtcNow;
                while (_Messages.ContainsKey(key))
                    key = key.AddTicks(1);
                _Messages.Add(key, msg);
            }
        }

        /// <summary>
        /// Stop the timestamp by setting End to DateTime.UtcNow.  If End is already set, this method has no effect, so the first end time is retained.
        /// </summary>
        public void Stop()
        {
            if (End == null) End = DateTime.UtcNow;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    
        // Removed incorrect Elapsed property from Dispose method


        
                }

                _Messages = null;
                _Metadata = null;
                _Disposed = true;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private-Methods

        private double TotalMsBetween(DateTime start, DateTime end)
        {
            try
            {
                start = start.ToUniversalTime();
                end = end.ToUniversalTime();
                TimeSpan total = end - start;
                return total.TotalMilliseconds;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion
    }
}