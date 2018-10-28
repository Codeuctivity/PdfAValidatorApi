// HINWEIS: Für den generierten Code ist möglicherweise mindestens .NET Framework 4.5 oder .NET Core/Standard 2.0 erforderlich.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class report
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
public partial class reportReleaseDetails
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
public partial class reportJobs
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
public partial class reportJobsJob
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
public partial class reportJobsJobItem
{
    private string nameField;

    private ushort sizeField;

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
    public ushort size
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
public partial class reportJobsJobValidationReport
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
public partial class reportJobsJobValidationReportDetails
{
    private byte passedRulesField;

    private byte failedRulesField;

    private ushort passedChecksField;

    private byte failedChecksField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte passedRules
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
    public byte failedRules
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
    public ushort passedChecks
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
    public byte failedChecks
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
public partial class reportJobsJobDuration
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
public partial class reportBatchSummary
{
    private reportBatchSummaryValidationReports validationReportsField;

    private reportBatchSummaryFeatureReports featureReportsField;

    private reportBatchSummaryRepairReports repairReportsField;

    private reportBatchSummaryDuration durationField;

    private byte totalJobsField;

    private byte failedToParseField;

    private byte encryptedField;

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
    public byte totalJobs
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
    public byte failedToParse
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
    public byte encrypted
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
public partial class reportBatchSummaryValidationReports
{
    private byte compliantField;

    private byte nonCompliantField;

    private byte failedJobsField;

    private byte valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte compliant
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
    public byte nonCompliant
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
    public byte failedJobs
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
    [System.Xml.Serialization.XmlTextAttribute()]
    public byte Value
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
public partial class reportBatchSummaryFeatureReports
{
    private byte failedJobsField;

    private byte valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte failedJobs
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
    [System.Xml.Serialization.XmlTextAttribute()]
    public byte Value
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
public partial class reportBatchSummaryRepairReports
{
    private byte failedJobsField;

    private byte valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte failedJobs
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
    [System.Xml.Serialization.XmlTextAttribute()]
    public byte Value
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
public partial class reportBatchSummaryDuration
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