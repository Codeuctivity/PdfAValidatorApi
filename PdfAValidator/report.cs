/// <summary>
/// Deserialized verapdf report
/// </summary>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public class report
{
    private reportReleaseDetails[] buildInformationField;

    private reportJobs jobsField;

    private reportBatchSummary batchSummaryField;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("releaseDetails", IsNullable = false)]
    public reportReleaseDetails[] buildInformation
    {
        get
        {
            return this.buildInformationField;
        }
        set
        {
            this.buildInformationField = value;
        }
    }

    /// <remarks/>
    public reportJobs jobs
    {
        get
        {
            return this.jobsField;
        }
        set
        {
            this.jobsField = value;
        }
    }

    /// <remarks/>
    public reportBatchSummary batchSummary
    {
        get
        {
            return this.batchSummaryField;
        }
        set
        {
            this.batchSummaryField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportReleaseDetails
{
    private string idField;

    private string versionField;

    private System.DateTime buildDateField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string version
    {
        get
        {
            return this.versionField;
        }
        set
        {
            this.versionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime buildDate
    {
        get
        {
            return this.buildDateField;
        }
        set
        {
            this.buildDateField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportJobs
{
    private reportJobsJob jobField;

    /// <remarks/>
    public reportJobsJob job
    {
        get
        {
            return this.jobField;
        }
        set
        {
            this.jobField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportJobsJob
{
    private reportJobsJobItem itemField;

    private reportJobsJobValidationReport validationReportField;

    private reportJobsJobDuration durationField;

    /// <remarks/>
    public reportJobsJobItem item
    {
        get
        {
            return this.itemField;
        }
        set
        {
            this.itemField = value;
        }
    }

    /// <remarks/>
    public reportJobsJobValidationReport validationReport
    {
        get
        {
            return this.validationReportField;
        }
        set
        {
            this.validationReportField = value;
        }
    }

    /// <remarks/>
    public reportJobsJobDuration duration
    {
        get
        {
            return this.durationField;
        }
        set
        {
            this.durationField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportJobsJobItem
{
    private string nameField;

    private string sizeField;

    /// <remarks/>
    public string name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string size
    {
        get
        {
            return this.sizeField;
        }
        set
        {
            this.sizeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportJobsJobValidationReport
{
    private reportJobsJobValidationReportDetails detailsField;

    private string profileNameField;

    private string statementField;

    private bool isCompliantField;

    /// <remarks/>
    public reportJobsJobValidationReportDetails details
    {
        get
        {
            return this.detailsField;
        }
        set
        {
            this.detailsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string profileName
    {
        get
        {
            return this.profileNameField;
        }
        set
        {
            this.profileNameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string statement
    {
        get
        {
            return this.statementField;
        }
        set
        {
            this.statementField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool isCompliant
    {
        get
        {
            return this.isCompliantField;
        }
        set
        {
            this.isCompliantField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportJobsJobValidationReportDetails
{
    private string passedRulesField;

    private string failedRulesField;

    private string passedChecksField;

    private string failedChecksField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string passedRules
    {
        get
        {
            return this.passedRulesField;
        }
        set
        {
            this.passedRulesField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string failedRules
    {
        get
        {
            return this.failedRulesField;
        }
        set
        {
            this.failedRulesField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string passedChecks
    {
        get
        {
            return this.passedChecksField;
        }
        set
        {
            this.passedChecksField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string failedChecks
    {
        get
        {
            return this.failedChecksField;
        }
        set
        {
            this.failedChecksField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportJobsJobDuration
{
    private ulong startField;

    private ulong finishField;

    private System.DateTime valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ulong start
    {
        get
        {
            return this.startField;
        }
        set
        {
            this.startField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ulong finish
    {
        get
        {
            return this.finishField;
        }
        set
        {
            this.finishField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute(DataType = "time")]
    public System.DateTime Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportBatchSummary
{
    private reportBatchSummaryValidationReports validationReportsField;

    private reportBatchSummaryFeatureReports featureReportsField;

    private reportBatchSummaryRepairReports repairReportsField;

    private reportBatchSummaryDuration durationField;

    private string totalJobsField;

    private string failedToParseField;

    private string encryptedField;

    /// <remarks/>
    public reportBatchSummaryValidationReports validationReports
    {
        get
        {
            return this.validationReportsField;
        }
        set
        {
            this.validationReportsField = value;
        }
    }

    /// <remarks/>
    public reportBatchSummaryFeatureReports featureReports
    {
        get
        {
            return this.featureReportsField;
        }
        set
        {
            this.featureReportsField = value;
        }
    }

    /// <remarks/>
    public reportBatchSummaryRepairReports repairReports
    {
        get
        {
            return this.repairReportsField;
        }
        set
        {
            this.repairReportsField = value;
        }
    }

    /// <remarks/>
    public reportBatchSummaryDuration duration
    {
        get
        {
            return this.durationField;
        }
        set
        {
            this.durationField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string totalJobs
    {
        get
        {
            return this.totalJobsField;
        }
        set
        {
            this.totalJobsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string failedToParse
    {
        get
        {
            return this.failedToParseField;
        }
        set
        {
            this.failedToParseField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string encrypted
    {
        get
        {
            return this.encryptedField;
        }
        set
        {
            this.encryptedField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportBatchSummaryValidationReports
{
    private string compliantField;

    private string nonCompliantField;

    private string failedJobsField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string compliant
    {
        get
        {
            return this.compliantField;
        }
        set
        {
            this.compliantField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nonCompliant
    {
        get
        {
            return this.nonCompliantField;
        }
        set
        {
            this.nonCompliantField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string failedJobs
    {
        get
        {
            return this.failedJobsField;
        }
        set
        {
            this.failedJobsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportBatchSummaryFeatureReports
{
    private string failedJobsField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string failedJobs
    {
        get
        {
            return this.failedJobsField;
        }
        set
        {
            this.failedJobsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportBatchSummaryRepairReports
{
    private string failedJobsField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string failedJobs
    {
        get
        {
            return this.failedJobsField;
        }
        set
        {
            this.failedJobsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public class reportBatchSummaryDuration
{
    private ulong startField;

    private ulong finishField;

    private System.DateTime valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ulong start
    {
        get
        {
            return this.startField;
        }
        set
        {
            this.startField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ulong finish
    {
        get
        {
            return this.finishField;
        }
        set
        {
            this.finishField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute(DataType = "time")]
    public System.DateTime Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}