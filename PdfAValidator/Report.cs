using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace PdfAValidator
{
    [XmlRoot(ElementName = "releaseDetails")]
    public class ReleaseDetails
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "buildDate")]
        public string BuildDate { get; set; }
    }

    [XmlRoot(ElementName = "buildInformation")]
    public class BuildInformation
    {
        [XmlElement(ElementName = "releaseDetails")]
        public List<ReleaseDetails> ReleaseDetails { get; set; }
    }

    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }
    }

    [XmlRoot(ElementName = "check")]
    public class Check
    {
        [XmlElement(ElementName = "context")]
        public string Context { get; set; }
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
    }

    [XmlRoot(ElementName = "rule")]
    public class Rule
    {
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "object")]
        public string Object { get; set; }
        [XmlElement(ElementName = "test")]
        public string Test { get; set; }
        [XmlElement(ElementName = "check")]
        public List<Check> Check { get; set; }
        [XmlAttribute(AttributeName = "specification")]
        public string Specification { get; set; }
        [XmlAttribute(AttributeName = "clause")]
        public string Clause { get; set; }
        [XmlAttribute(AttributeName = "testNumber")]
        public string TestNumber { get; set; }
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "passedChecks")]
        public string PassedChecks { get; set; }
        [XmlAttribute(AttributeName = "failedChecks")]
        public string FailedChecks { get; set; }
    }

    [XmlRoot(ElementName = "details")]
    public class Details
    {
        [XmlElement(ElementName = "rule")]
        public List<Rule> Rule { get; set; }
        [XmlAttribute(AttributeName = "passedRules")]
        public int PassedRules { get; set; }
        [XmlAttribute(AttributeName = "failedRules")]
        public int FailedRules { get; set; }
        [XmlAttribute(AttributeName = "passedChecks")]
        public int PassedChecks { get; set; }
        [XmlAttribute(AttributeName = "failedChecks")]
        public int FailedChecks { get; set; }
    }

    [XmlRoot(ElementName = "validationReport")]
    public class ValidationReport
    {
        [XmlElement(ElementName = "details")]
        public Details Details { get; set; }
        [XmlAttribute(AttributeName = "profileName")]
        public string ProfileName { get; set; }
        [XmlAttribute(AttributeName = "statement")]
        public string Statement { get; set; }
        [XmlAttribute(AttributeName = "isCompliant")]
        public bool IsCompliant { get; set; }
    }

    [XmlRoot(ElementName = "duration")]
    public class Duration
    {
        [XmlAttribute(AttributeName = "start")]
        public string Start { get; set; }
        [XmlAttribute(AttributeName = "finish")]
        public string Finish { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "job")]
    public class Job
    {
        [XmlElement(ElementName = "item")]
        public Item Item { get; set; }
        [XmlElement(ElementName = "validationReport")]
        public ValidationReport ValidationReport { get; set; }
        [XmlElement(ElementName = "duration")]
        public Duration Duration { get; set; }
    }

    [XmlRoot(ElementName = "jobs")]
    public class Jobs
    {
        [XmlElement(ElementName = "job")]
        public Job Job { get; set; }
    }

    [XmlRoot(ElementName = "validationReports")]
    public class ValidationReports
    {
        [XmlAttribute(AttributeName = "compliant")]
        public string Compliant { get; set; }
        [XmlAttribute(AttributeName = "nonCompliant")]
        public string NonCompliant { get; set; }
        [XmlAttribute(AttributeName = "failedJobs")]
        public string FailedJobs { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "featureReports")]
    public class FeatureReports
    {
        [XmlAttribute(AttributeName = "failedJobs")]
        public string FailedJobs { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "repairReports")]
    public class RepairReports
    {
        [XmlAttribute(AttributeName = "failedJobs")]
        public string FailedJobs { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "batchSummary")]
    public class BatchSummary
    {
        [XmlElement(ElementName = "validationReports")]
        public ValidationReports ValidationReports { get; set; }
        [XmlElement(ElementName = "featureReports")]
        public FeatureReports FeatureReports { get; set; }
        [XmlElement(ElementName = "repairReports")]
        public RepairReports RepairReports { get; set; }
        [XmlElement(ElementName = "duration")]
        public Duration Duration { get; set; }
        [XmlAttribute(AttributeName = "totalJobs")]
        public string TotalJobs { get; set; }
        [XmlAttribute(AttributeName = "failedToParse")]
        public string FailedToParse { get; set; }
        [XmlAttribute(AttributeName = "encrypted")]
        public string Encrypted { get; set; }
    }

    [XmlRoot(ElementName = "report")]
    public class Report
    {
        [XmlElement(ElementName = "buildInformation")]
        public BuildInformation BuildInformation { get; set; }
        [XmlElement(ElementName = "jobs")]
        public Jobs Jobs { get; set; }
        [XmlElement(ElementName = "batchSummary")]
        public BatchSummary BatchSummary { get; set; }
    }

}
