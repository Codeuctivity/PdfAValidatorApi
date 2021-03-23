using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

// example report file https://docs.verapdf.org/policy/info-dict/report.xml

namespace Codeuctivity
{
    /// <summary>
    /// ReleaseDetails shows details to the used veraPdf version
    /// </summary>
    [XmlRoot(ElementName = "releaseDetails")]
    public class ReleaseDetails
    {
        /// <summary>
        /// Component name
        /// </summary>
        /// <value>e.g. gui</value>
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Version of component
        /// </summary>
        /// <value>e.g. 1.0.6-PDFBOX</value>
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Tells the build date
        /// </summary>
        /// <value>e.g. 2017-01-13T11:30:00Z</value>
        [XmlAttribute(AttributeName = "buildDate")]
        public string BuildDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// Summarize the build versions of each veraPdf component
    /// </summary>
    [XmlRoot(ElementName = "buildInformation")]
    public class BuildInformation
    {
        /// <summary>
        /// Collection of version information of veraPdf components
        /// </summary>
        [XmlElement(ElementName = "releaseDetails")]
        public List<ReleaseDetails> ReleaseDetails { get; } = new List<ReleaseDetails>();
    }

    /// <summary>
    /// JobItem
    /// </summary>
    [XmlRoot(ElementName = "item")]
    public class Item
    {
        /// <summary>
        /// Pdf source path
        /// </summary>
        [XmlElement(ElementName = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Pdf source size
        /// </summary>
        /// <value>e.g. 10130</value>
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; } = string.Empty;
    }

    /// <summary>
    /// Check
    /// </summary>
    [XmlRoot(ElementName = "check")]
    public class Check
    {
        /// <summary>
        /// Context
        /// </summary>
        /// <value>e.g. root</value>
        [XmlElement(ElementName = "context")]
        public string Context { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        /// <value>e.g. failed</value>
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Rule
    /// </summary>
    [XmlRoot(ElementName = "rule")]
    public class Rule
    {
        /// <summary>
        /// Rule description
        /// </summary>
        [XmlElement(ElementName = "description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Pdf object
        /// </summary>
        /// <value>e.g. CosDocument</value>
        [XmlElement(ElementName = "object")]
        public string PdfObject { get; set; } = string.Empty;

        /// <summary>
        /// Test
        /// </summary>
        /// <value>e.g. doesInfoMatchXMP</value>
        [XmlElement(ElementName = "test")]
        public string Test { get; set; } = string.Empty;

        /// <summary>
        /// Collection of all checks
        /// </summary>
        [XmlElement(ElementName = "check")]
        public List<Check> Check { get; } = new List<Check>();

        /// <summary>
        /// Reference to iso standard paper
        /// </summary>
        /// <value>e.g. ISO 19005-1:2005</value>
        [XmlAttribute(AttributeName = "specification")]
        public string Specification { get; set; } = string.Empty;

        /// <summary>
        /// Reference to clause
        /// </summary>
        /// <value>e.g. 6.7.3</value>
        [XmlAttribute(AttributeName = "clause")]
        public string Clause { get; set; } = string.Empty;

        /// <summary>
        /// Test number, seems like some veraPdf internal thingy
        /// </summary>
        [XmlAttribute(AttributeName = "testNumber")]
        public string TestNumber { get; set; } = string.Empty;

        /// <summary>
        /// Outcome
        /// </summary>
        /// <value>e.g. failed</value>
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Number of passed checks
        /// </summary>
        [XmlAttribute(AttributeName = "passedChecks")]
        public string PassedChecks { get; set; } = string.Empty;

        /// <summary>
        /// FAiled checks, seems like some veraPdf internal thingy
        /// </summary>
        [XmlAttribute(AttributeName = "failedChecks")]
        public string FailedChecks { get; set; } = string.Empty;
    }

    /// <summary>
    /// Detailed reports
    /// </summary>
    [XmlRoot(ElementName = "details")]
    public class Details
    {
        /// <summary>
        /// Collection of rules
        /// </summary>
        [XmlElement(ElementName = "rule")]
        public List<Rule> Rule { get; } = new List<Rule>();

        /// <summary>
        /// Number of passed rules
        /// </summary>
        [XmlAttribute(AttributeName = "passedRules")]
        public int PassedRules { get; set; }

        /// <summary>
        /// Amount of failed rules
        /// </summary>
        [XmlAttribute(AttributeName = "failedRules")]
        public int FailedRules { get; set; }

        /// <summary>
        /// Amount of passed checks
        /// </summary>
        [XmlAttribute(AttributeName = "passedChecks")]
        public int PassedChecks { get; set; }

        /// <summary>
        /// Amount of failed checks
        /// </summary>
        [XmlAttribute(AttributeName = "failedChecks")]
        public int FailedChecks { get; set; }
    }

    /// <summary>
    /// Validation report
    /// </summary>
    [XmlRoot(ElementName = "validationReport")]
    public class ValidationReport
    {
        /// <summary>
        /// Validation Details like failed and passed rules
        /// </summary>
        [XmlElement(ElementName = "details")]
        public Details Details { get; set; } = new Details();

        /// <summary>
        /// Shows the detected used PdfA profile of the source
        /// </summary>
        /// <value>e.g. "PDF/A-1B validation profile"</value>
        [XmlAttribute(AttributeName = "profileName")]
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>
        /// Human readable validation outcome
        /// </summary>
        /// <value>e.g. "PDF file is compliant with Validation Profile requirements."</value>
        [XmlAttribute(AttributeName = "statement")]
        public string Statement { get; set; } = string.Empty;

        /// <summary>
        /// Is true, if the source pdf is compliant
        /// </summary>
        [XmlAttribute(AttributeName = "isCompliant")]
        public bool IsCompliant { get; set; }
    }

    /// <summary>
    /// Duration of validation
    /// </summary>
    [XmlRoot(ElementName = "duration")]
    public class Duration
    {
        /// <summary>
        /// Start of validation
        /// </summary>
        [XmlAttribute(AttributeName = "start")]
        public string Start { get; set; } = string.Empty;

        /// <summary>
        /// End of validation
        /// </summary>
        [XmlAttribute(AttributeName = "finish")]
        public string Finish { get; set; } = string.Empty;

        /// <summary>
        /// Duration
        /// </summary>
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Job details
    /// </summary>
    [XmlRoot(ElementName = "job")]
    public class Job
    {
        /// <summary>
        /// Meta information of validated file
        /// </summary>
        [XmlElement(ElementName = "item")]
        public Item Item { get; set; } = new Item();

        /// <summary>
        /// Report details
        /// </summary>
        [XmlElement(ElementName = "validationReport")]
        public ValidationReport ValidationReport { get; set; } = new ValidationReport();

        /// <summary>
        /// Task result
        /// </summary>
        [XmlElement(ElementName = "taskResult")]
        public TaskResult TaskResult { get; set; } = new TaskResult();

        /// <summary>
        /// Duration details
        /// </summary>
        [XmlElement(ElementName = "duration")]
        public Duration Duration { get; set; } = new Duration();
    }

    /// <summary>
    /// Contains all jobs, in case of PdfAValidator it is always one
    /// </summary>
    [XmlRoot(ElementName = "jobs")]
    public class Jobs
    {
        /// <summary>
        /// Contains the details to the validated pdfs
        /// </summary>
        [XmlElement(ElementName = "job")]
        public List<Job> AllJobs { get; set; } = new List<Job>();

        /// <summary>
        /// Convenience property for when there is only one Job
        /// </summary>
        public Job Job => AllJobs.FirstOrDefault();
    }

    /// <summary>
    /// Validation report details
    /// </summary>
    [XmlRoot(ElementName = "validationReports")]
    public class ValidationReports
    {
        /// <summary>
        /// Amount of compliant pdfs
        /// </summary>
        [XmlAttribute(AttributeName = "compliant")]
        public string Compliant { get; set; } = string.Empty;

        /// <summary>
        /// Amount of nonCompliant pdfs
        /// </summary>
        [XmlAttribute(AttributeName = "nonCompliant")]
        public string NonCompliant { get; set; } = string.Empty;

        /// <summary>
        /// Amount of pdfs failed to validate
        /// </summary>
        [XmlAttribute(AttributeName = "failedJobs")]
        public string FailedJobs { get; set; } = string.Empty;

        /// <summary>
        /// Seems like some veraPdf internal thingy
        /// </summary>
        /// <value>e.g. "1"</value>
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Task Result
    /// </summary>
    public class TaskResult
    {
        /// <summary>
        /// Type
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Is executed
        /// </summary>
        [XmlAttribute(AttributeName = "isExecuted")]
        public bool IsExecuted { get; set; }

        /// <summary>
        /// Is success
        /// </summary>
        [XmlAttribute(AttributeName = "isSuccess")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Duration
        /// </summary>
        [XmlElement(ElementName = "duration")]
        public Duration Duration { get; set; } = new Duration();

        /// <summary>
        /// Exception message
        /// </summary>
        [XmlElement(ElementName = "exceptionMessage")]
        public string ExceptionMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Not used with plain validation
    /// </summary>
    [XmlRoot(ElementName = "featureReports")]
    public class FeatureReports
    {
        /// <summary>
        /// Amount of  failed jobs
        /// </summary>
        [XmlAttribute(AttributeName = "failedJobs")]
        public string FailedJobs { get; set; } = string.Empty;

        /// <summary>
        /// Amount of succeeded jobs
        /// </summary>
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Not used in this integration
    /// </summary>
    [XmlRoot(ElementName = "repairReports")]
    public class RepairReports
    {
        /// <summary>
        /// Amount of failed jobs
        /// </summary>
        [XmlAttribute(AttributeName = "failedJobs")]
        public string FailedJobs { get; set; } = string.Empty;

        /// <summary>
        /// seems like some veraPdf internal thingy
        /// </summary>
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Summarize the validation job
    /// </summary>
    [XmlRoot(ElementName = "batchSummary")]
    public class BatchSummary
    {
        /// <summary>
        /// Summarized validation reports
        /// </summary>
        [XmlElement(ElementName = "validationReports")]
        public ValidationReports ValidationReports { get; set; } = new ValidationReports();

        /// <summary>
        /// Not used in this integration
        /// </summary>
        [XmlElement(ElementName = "featureReports")]
        public FeatureReports FeatureReports { get; set; } = new FeatureReports();

        /// <summary>
        /// Not used in this integration
        /// </summary>
        [XmlElement(ElementName = "repairReports")]
        public RepairReports RepairReports { get; set; } = new RepairReports();

        /// <summary>
        /// Summarized duration
        /// </summary>
        [XmlElement(ElementName = "duration")]
        public Duration Duration { get; set; } = new Duration();

        /// <summary>
        /// In this integration always one, if it does not fail
        /// </summary>
        [XmlAttribute(AttributeName = "totalJobs")]
        public string TotalJobs { get; set; } = string.Empty;

        /// <summary>
        /// Failed to parse
        /// </summary>
        [XmlAttribute(AttributeName = "failedToParse")]
        public string FailedToParse { get; set; } = string.Empty;

        /// <summary>
        /// Indicates pdf encryption
        /// </summary>
        [XmlAttribute(AttributeName = "encrypted")]
        public string Encrypted { get; set; } = string.Empty;
    }

    /// <summary>
    /// VeraPdf report
    /// </summary>
    [XmlRoot(ElementName = "report")]
    public class Report
    {
        /// <summary>
        /// VeraPdf bin details
        /// </summary>
        [XmlElement(ElementName = "buildInformation")]
        public BuildInformation BuildInformation { get; set; } = new BuildInformation();

        /// <summary>
        /// VeraPdf jobs - in this integration always contains just one job
        /// </summary>
        [XmlElement(ElementName = "jobs")]
        public Jobs Jobs { get; set; } = new Jobs();

        /// <summary>
        /// VeraPdf batch summary
        /// </summary>
        [XmlElement(ElementName = "batchSummary")]
        public BatchSummary BatchSummary { get; set; } = new BatchSummary();
    }
}