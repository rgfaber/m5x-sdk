using System.IO;
using System.Text;
using M5x.Chassis.Mh.Stats;

namespace M5x.Chassis.Mh.Reporting
{
    /// <summary>
    ///     A file-based reporter that produces a timestamped-suffixed output file for each sample collection
    /// </summary>
    public class SampledFileReporter : ReporterBase
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly string _directory;
        private readonly Encoding _encoding;

        public SampledFileReporter(Metrics metrics, IDateTimeOffsetProvider dateTimeOffsetProvider = null) : this("",
            Encoding.UTF8, new HumanReadableReportFormatter(metrics), dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(Encoding encoding, Metrics metrics,
            IDateTimeOffsetProvider dateTimeOffsetProvider = null) : this("", encoding,
            new HumanReadableReportFormatter(metrics), dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(string directory, Metrics metrics,
            IDateTimeOffsetProvider dateTimeOffsetProvider = null) : this(directory, Encoding.UTF8,
            new HumanReadableReportFormatter(metrics), dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(string directory, Encoding encoding, Metrics metrics,
            IDateTimeOffsetProvider dateTimeOffsetProvider = null) : this(directory, encoding,
            new HumanReadableReportFormatter(metrics), dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(HealthChecks healthChecks, IDateTimeOffsetProvider dateTimeOffsetProvider = null) :
            this("", Encoding.UTF8, new HumanReadableReportFormatter(healthChecks), dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(Encoding encoding, HealthChecks healthChecks,
            IDateTimeOffsetProvider dateTimeOffsetProvider = null) : this("", encoding,
            new HumanReadableReportFormatter(healthChecks), dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(string directory, HealthChecks healthChecks,
            IDateTimeOffsetProvider dateTimeOffsetProvider = null) : this(directory, Encoding.UTF8,
            new HumanReadableReportFormatter(healthChecks), dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(string directory, Encoding encoding, HealthChecks healthChecks,
            IDateTimeOffsetProvider dateTimeOffsetProvider = null) : this(directory, encoding,
            new HumanReadableReportFormatter(healthChecks), dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(IReportFormatter formatter, IDateTimeOffsetProvider dateTimeOffsetProvider = null) :
            this("", Encoding.UTF8, formatter, dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(string directory, IReportFormatter formatter,
            IDateTimeOffsetProvider dateTimeOffsetProvider = null) : this(directory, Encoding.UTF8, formatter,
            dateTimeOffsetProvider)
        {
        }

        public SampledFileReporter(string directory, Encoding encoding, IReportFormatter formatter,
            IDateTimeOffsetProvider dateTimeOffsetProvider) : base(null, formatter)
        {
            _directory = directory;
            _encoding = encoding;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public override void Run()
        {
            using (var fs = new FileStream(GenerateFilePath(), FileMode.CreateNew))
            {
                using (Out = new StreamWriter(fs, _encoding))
                {
                    Out.Write(Formatter.GetSample());
                    Out.Flush();
                }
            }
        }

        private string GenerateFilePath()
        {
            return Path.Combine(_directory, $"{_dateTimeOffsetProvider.UtcNow.Ticks}.sample");
        }
    }
}