using System;

namespace Timestamps
{
    /// <summary>
    /// Object used to measure start, end, and total time associated with an operation.
    /// </summary>
    public class Timestamp
    {
        #region Public-Members

        /// <summary>
        /// The time at which the operation started.
        /// </summary>
        public DateTime Start
        {
            get
            {
                return _Start;
            }
            set
            {
                _Start = Convert.ToDateTime(value).ToUniversalTime();

                if (_End != null)
                {
                    if (_Start > _End.Value) throw new ArgumentException("Start time must be before end time.");
                }
            }
        }

        /// <summary>
        /// The time at which the operation ended.
        /// </summary>
        public DateTime? End
        {
            get
            {
                return _End;
            }
            set
            {
                if (value == null)
                {
                    _End = null;
                }
                else
                {
                    if (value < _Start) throw new ArgumentException("End time must be after start time.");
                    _End = Convert.ToDateTime(value).ToUniversalTime();
                }
            }
        }

        /// <summary>
        /// The total number of milliseconds that transpired between Start and End.
        /// </summary>
        public double? TotalMs
        {
            get
            {
                if (_End == null)
                {
                    return Math.Round(TotalMsBetween(_Start, DateTime.UtcNow), 2);
                }
                else
                {
                    return Math.Round(TotalMsBetween(_Start, _End.Value), 2);
                }
            }
        }

        #endregion

        #region Private-Members

        private DateTime _Start = DateTime.UtcNow;
        private DateTime? _End = null;

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