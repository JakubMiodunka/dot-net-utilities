using System;
using System.Text;


namespace DotNetUtilities
{
    /// <summary>
    /// Progress bar utility, which visualize current state of tracked process and measures its performance.
    /// </summary>
    /// <remarks>
    /// General recommendations:
    ///     It is highly recommended to use progress tracker using 'using' statement.
    /// Interactions with std-out:
    ///     Progress bar prints its visual representation to std-out, so it is not recommended to perform
    ///     other operations during is runtime using std-out
    ///     directly - to print some values on std-out please use ProgressBar.WriteLine method.
    /// Comments:
    ///     Usage of Unicode characters U+2588, U+2589, U+258A, U+258B, U+258C, U+258D,
    ///     U+258E and U+258F (partially filled blocks) was taken into consideration to make
    ///     the progress bar more accurate and smooth, but it occurs that those symbols are
    ///     not supported by Windows CLI.Additionally after some trials I came to the conclusion,
    ///     that this solution would make the progress bar implementation
    ///     too complicated for what it is (over-engineered) and gave up further development.
    /// </remarks>
    /// <example>
    /// Simple example of tracker usage.
    /// <code>
    /// const int TotalSteps = 150;
    /// const int Delay = 200;
    /// 
    /// using (var progressBar = progressBar.Default(TotalSteps))
    /// {
    ///     for (int i = 0; i < TotalSteps; i++)
    ///     {
    ///         Thread.Sleep(Delay);
    ///         progressBar.WriteLine("Another step was performed");
    ///         progressBar.Update();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class ProgressBar : IDisposable
    {
        #region Constants
        private const char OpeningBarBracket = '|';
        private const char ClosingBarBracket = '|';
        private const char EmptyBarBlock = ' ';
        private const char FilledBarBlock = '█';

        private const char OpeningSectionBracket = '[';
        private const char ClosingSectionBracket = ']';
        private const char SectionFieldSeparator = '|';

        private const string UnknownTimeSpanIndicator = "--:--:--";
        private const string UnknownDateTimeIndicator = "--:--";
        #endregion

        #region Properties
        public int CurrentStep { get; private set; }
        public double PercentageProgress => 100.0 * CurrentStep / TotalSteps;
        public DateTime EstimatedRuntimeFinish { get; private set; }
        public TimeSpan EstimatedRemainingRuntime { get; private set; }
        public TimeSpan AverageTimePerStep { get; private set; }

        public readonly int TotalSteps;
        public readonly DateTime RuntimeBegin;

        private readonly string _label;
        private readonly int _size;
        #endregion

        #region Instantiation
        /// <summary>
        /// Creates new progress bar instance
        /// </summary>
        /// <param name="totalSteps">
        /// Total steps, required to complete the process.
        /// </param>
        /// <param name="size">
        /// Size of displayed progress bar, expressed in number of blocks 
        /// (characters) contained by the bar body.
        /// </param>
        /// <param name="label">
        /// String, which shall be used as progress bar label.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown, when value of at least one argument will be considered as invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        public ProgressBar(int totalSteps, int size = 25, string label = "Progress")
        {
            #region Arguments validation
            if (totalSteps < 0)
            {
                string argumentName = nameof(totalSteps);
                string errorMessage = $"Invalid value of total steps: {totalSteps}";
                throw new ArgumentOutOfRangeException(argumentName, totalSteps, errorMessage);
            }

            if (size < 1)
            {
                string argumentName = nameof(size);
                string errorMessage = $"Invalid size specified: {size}";
                throw new ArgumentOutOfRangeException(argumentName, size, errorMessage);
            }

            if (label is null)
            {
                string argumentName = nameof(label);
                const string ErrorMessage = "Provided label is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }
            #endregion

            CurrentStep = 0;
            TotalSteps = totalSteps;
            RuntimeBegin = DateTime.Now;
            EstimatedRuntimeFinish = DateTime.MinValue;
            EstimatedRemainingRuntime = TimeSpan.MinValue;
            AverageTimePerStep = TimeSpan.MinValue;

            _label = label;
            _size = size;

            Console.Write(this);
        }
        #endregion

        #region Formatting
        /// <summary>
        /// Generates string-based representation of provided TimeSpan value.
        /// </summary>
        /// <param name="timeSpan">
        /// TimeSpan value, which representation shall be generated.
        /// </param>
        /// <returns>
        /// String representation of given TimeSpan value.
        /// </returns>
        private string AsString(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.MinValue)
            {
                return UnknownTimeSpanIndicator;
            }

            return $"{timeSpan.TotalHours:00}:{timeSpan.ToString(@"mm\:ss")}";
        }

        /// <summary>
        /// Generates string-based representation of provided DateTime value.
        /// </summary>
        /// <param name="dateTime">
        /// DateTime value, which representation shall be generated.
        /// </param>
        /// <returns>
        /// String representation of given DateTime value.
        /// </returns>
        private string AsString(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return UnknownDateTimeIndicator;
            }

            return dateTime.ToString(@"HH\:mm");
        }

        /// <summary>
        /// Generates string-based bar filled proportionally
        /// to ratio between steps already completed by tracked process and all its steps.
        /// </summary>
        /// <returns>
        /// String-based bar filled proportionally
        /// to ratio between steps already completed by tracked process and all its steps.
        /// </returns>
        private string GetBar()
        {
            int filledBlocks = Convert.ToInt32(Math.Round(1.0 * CurrentStep / TotalSteps * _size));
            int emptyBlocks = _size - filledBlocks;

            var progressBar = new StringBuilder();

            progressBar.Append(OpeningBarBracket);
            progressBar.Append(FilledBarBlock, filledBlocks);
            progressBar.Append(EmptyBarBlock, emptyBlocks);
            progressBar.Append(ClosingBarBracket);

            return progressBar.ToString();
        }

        /// <summary>
        /// Generates string-based representation of ratio between
        /// steps already completed by tracked process and all its steps.
        /// </summary>
        /// <returns>
        /// String-based representation of ratio between
        /// steps already completed by tracked process and all its steps.
        /// </returns>
        private string GetStepsRatioSection()
        {
            var stepsRatioSection = new StringBuilder();

            stepsRatioSection.Append(OpeningSectionBracket);
            stepsRatioSection.Append(CurrentStep);
            stepsRatioSection.Append('/');
            stepsRatioSection.Append(TotalSteps);
            stepsRatioSection.Append(ClosingSectionBracket);

            return stepsRatioSection.ToString();
        }

        /// <summary>
        /// Generates string-based representation of time-related metrics
        /// of tracked process.
        /// </summary>
        /// <returns>
        /// String-based representation of time-related metrics of tracked process.
        /// </returns>
        private string GetTimeMetricksSection()
        {
            var timeMetricksSection = new StringBuilder();

            timeMetricksSection.Append(OpeningSectionBracket);
            timeMetricksSection.Append(AsString(RuntimeBegin));
            timeMetricksSection.Append(SectionFieldSeparator);
            timeMetricksSection.Append(AsString(EstimatedRuntimeFinish));
            timeMetricksSection.Append(SectionFieldSeparator);
            timeMetricksSection.Append(AsString(AverageTimePerStep));
            timeMetricksSection.Append(ClosingSectionBracket);

            return timeMetricksSection.ToString();
        }

        /// <summary>
        /// Generates string-based representation of current state of progress bar.
        /// </summary>
        /// <returns>
        /// String-based representation of current state of progress bar.
        /// </returns>
        public override string ToString()
        {
            var progressBar = new StringBuilder();

            progressBar.Append($"{_label}: ");
            progressBar.Append($"{Math.Round(PercentageProgress),3:F0}%");
            progressBar.Append(GetBar());
            progressBar.Append(GetStepsRatioSection());
            progressBar.Append(' ');
            progressBar.Append(GetTimeMetricksSection());

            return progressBar.ToString();
        }
        #endregion

        #region Interactions
        /// <summary>
        /// Updates progress bar with specified amount of steps newly completed by tracked process.
        /// </summary>
        /// <param name="steps">
        /// Number of steps, performed by the process since last update.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown, when value of at least one argument will be considered as invalid.
        /// </exception>
        private void UpdateCurrentStep(int steps)
        {
            #region Arguments validation
            if (steps < 0)
            {
                string argumentName = nameof(steps);
                string errorMessage = $"Invalid value of steps: {steps}";
                throw new ArgumentOutOfRangeException(argumentName, steps, errorMessage);
            }
            #endregion

            int newCurrentStep = CurrentStep + steps;

            if (TotalSteps < newCurrentStep)
            {
                CurrentStep = TotalSteps;
                return;
            }

            CurrentStep = newCurrentStep;
        }

        /// <summary>
        /// Triggers computation of time-related metrics of tracked process.
        /// </summary>
        private void UpdateTimeMetrics()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan runtimeDuration = currentTime - RuntimeBegin;

            AverageTimePerStep = new TimeSpan(runtimeDuration.Ticks / CurrentStep);
            EstimatedRemainingRuntime = new TimeSpan(AverageTimePerStep.Ticks * (TotalSteps - CurrentStep));
            EstimatedRuntimeFinish = currentTime + EstimatedRemainingRuntime;
        }

        /// <summary>
        /// Updates displayed progress bar with specified amount of steps newly completed by tracked process.
        /// </summary>
        /// <param name="steps">
        /// Number of steps, performed by the process since last update.
        /// </param>
        public void Update(int steps = 1)
        {
            UpdateCurrentStep(steps);
            UpdateTimeMetrics();

            Console.Write("\r");
            Console.Write(this);
        }

        /// <summary>
        /// Prints provided value to std-out above the progress bar.
        /// </summary>
        /// <param name="value">
        /// Value, required to be printed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when at least one reference-type argument is a null reference.
        /// </exception>
        public void WriteLine(object value)
        {
            #region Arguments validation
            if (value is null)
            {
                string argumentName = nameof(value);
                const string ErrorMessage = "Provided value is a null reference:";
                throw new ArgumentNullException(argumentName, ErrorMessage);
            }
            #endregion

            string progressBar = ToString();
            string blankLine = new string(' ', progressBar.Length);

            Console.Write("\r");
            Console.Write(blankLine);

            Console.Write("\r");
            Console.WriteLine(value);

            Console.Write(progressBar);
        }

        /// <summary>
        /// Prints a newline to leave the line, in which progress bar was being displayed.
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine();
        }
        #endregion
    }
}
