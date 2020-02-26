using System;
using System.Xml.Serialization;
using System.Collections.Generic;

// example report file https://docs.verapdf.org/policy/info-dict/report.xml

namespace PdfAValidator
{
    /// <summary>
    /// ReleaseDetails shows details to the used veraPdf Verison
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
    /// Summarize the build verisons of each veraPdf component
    /// </summary>
    [XmlRoot(ElementName = "buildInformation")]
    public class BuildInformation
    {
        /// <summary>
        /// Collection of verison information of veraPdf components
        /// </summary>
        /// <value></value>
        [XmlElement(ElementName = "releaseDetails")]
        public List<ReleaseDetails> ReleaseDetails { get; set; } = new List<ReleaseDetails>();
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
        /// <value></value>
        [XmlElement(ElementName = "name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Pdf source size
        /// </summary>
        /// <value>e.g. 10130</value>
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; } = string.Empty;
    }
    [XmlRoot(ElementName = "check")]
    public class Check
    {
        [XmlElement(ElementName = "context")]
        public string Context { get; set; } = string.Empty;
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "rule")]
    public class Rule
    {
        [XmlElement(ElementName = "description")]
        public string Description { get; set; } = string.Empty;
        [XmlElement(ElementName = "object")]
        public string Object { get; set; } = string.Empty;
        [XmlElement(ElementName = "test")]
        public string Test { get; set; } = string.Empty;
        [XmlElement(ElementName = "check")]
        public List<Check> Check { get; set; } = new List<Check>();
        [XmlAttribute(AttributeName = "specification")]
        public string Specification { get; set; } = string.Empty;
        [XmlAttribute(AttributeName = "clause")]
        public string Clause { get; set; } = string.Empty;
        [XmlAttribute(AttributeName = "testNumber")]
        public string TestNumber { get; set; } = string.Empty;
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Number of passed checks
        /// </summary>
        [XmlAttribute(AttributeName = "passedChecks")]
        public string PassedChecks { get; set; } = string.Empty;
        [XmlAttribute(AttributeName = "failedChecks")]
        public string FailedChecks { get; set; } = string.Empty;
    }
    /// <summary>
    /// Detailed reports
    /// </summary>
    [XmlRoot(ElementName = "details")]
    public class Details
    {
        [XmlElement(ElementName = "rule")]
        public List<Rule> Rule { get; set; } = new List<Rule>();
        /// <summary>
        /// Nummber of passed rules
        /// </summary>
        [XmlAttribute(AttributeName = "passedRules")]
        public int PassedRules { get; set; }
        [XmlAttribute(AttributeName = "failedRules")]
        public int FailedRules { get; set; }
        [XmlAttribute(AttributeName = "passedChecks")]
        public int PassedChecks { get; set; }
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
        /// Is true, if the source pdf is complient
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
        /// <value></value>
        [XmlAttribute(AttributeName = "finish")]
        public string Finish { get; set; } = string.Empty;
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "job")]
    public class Job
    {
        [XmlElement(ElementName = "item")]
        public Item Item { get; set; } = new Item();
        [XmlElement(ElementName = "validationReport")]
        public ValidationReport ValidationReport { get; set; } = new ValidationReport();
        [XmlElement(ElementName = "duration")]
        public Duration Duration { get; set; } = new Duration();
    }

    [XmlRoot(ElementName = "jobs")]
    public class Jobs
    {
        [XmlElement(ElementName = "job")]
        public Job Job { get; set; } = new Job();
    }

    [XmlRoot(ElementName = "validationReports")]
    public class ValidationReports
    {
        [XmlAttribute(AttributeName = "compliant")]
        public string Compliant { get; set; } = string.Empty;
        [XmlAttribute(AttributeName = "nonCompliant")]
        public string NonCompliant { get; set; } = string.Empty;
        [XmlAttribute(AttributeName = "failedJobs")]
        public string FailedJobs { get; set; } = string.Empty;
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "featureReports")]
    public class FeatureReports
    {
        [XmlAttribute(AttributeName = "failedJobs")]
        public string FailedJobs { get; set; } = string.Empty;
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "repairReports")]
    public class RepairReports
    {
        [XmlAttribute(AttributeName = "failedJobs")]
        public string FailedJobs { get; set; } = string.Empty;
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "batchSummary")]
    public class BatchSummary
    {
        [XmlElement(ElementName = "validationReports")]
        public ValidationReports ValidationReports { get; set; } = new ValidationReports();
        [XmlElement(ElementName = "featureReports")]
        public FeatureReports FeatureReports { get; set; } = new FeatureReports();
        [XmlElement(ElementName = "repairReports")]
        public RepairReports RepairReports { get; set; } = new RepairReports();
        [XmlElement(ElementName = "duration")]
        public Duration Duration { get; set; } = new Duration();
        [XmlAttribute(AttributeName = "totalJobs")]
        public string TotalJobs { get; set; } = string.Empty;
        [XmlAttribute(AttributeName = "failedToParse")]
        public string FailedToParse { get; set; } = string.Empty;
        [XmlAttribute(AttributeName = "encrypted")]
        public string Encrypted { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "report")]
    public class Report
    {
        [XmlElement(ElementName = "buildInformation")]
        public BuildInformation BuildInformation { get; set; } = new BuildInformation();
        [XmlElement(ElementName = "jobs")]
        public Jobs Jobs { get; set; } = new Jobs();
        [XmlElement(ElementName = "batchSummary")]
        public BatchSummary BatchSummary { get; set; } = new BatchSummary();
    }

}
