# Codeuctivity.PdfAValidator

PdfAValidatorApi wraps calls to [VeraPdf](http://www.preforma-project.eu/pdfa-conformance-checker.html) in a .net standard assembly and as WebApi. Access VeraPdf from your unit tests. Works on Windows, Linux and MacOs.

Sample - e.g. use it in your unit test to check compliance of some pdf:

```csharp
public static async Task ShouldDetectCompliantPdfA()
{
    using var pdfAValidator = new PdfAValidator();
    var result = await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOffice.pdf");
    Assert.True(result);
}
```

Sample - e.g. use it in your unit test to check the used sub standard of some pdf:

```csharp
public static async Task ShouldGetDetailedReportFromPdfA()
{
    using var pdfAValidator = new PdfAValidator();
    var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOffice.pdf");
    Assert.True(result.Jobs.Job.ValidationReport.IsCompliant);
    Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1A validation profile");
}
```

Sample - e.g. use it in your unit test to check [PDF meta data](https://docs.verapdf.org/cli/feature-extraction/):

```csharp
 public static async Task ShouldGetFeaturesReportWhenAskingForIt()
 {
     using var pdfAValidator = new PdfAValidator();
     var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOffice.pdf", "--extract");
     var producerEntry = result.Jobs.Job.FeaturesReport.InformationDict.Entries.Single(e => e.Key == "Producer");
     Assert.Equal("LibreOffice 6.1", producerEntry.Value);
 }
```
