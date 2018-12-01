[![Build status](https://ci.appveyor.com/api/projects/status/hwa0obfdvoxy9wkw?svg=true)](https://ci.appveyor.com/project/stesee/pdfavalidatorapi)

# PdfAValidatorApi
PdfAValidatorApi wraps calls to [VeraPdf](http://www.preforma-project.eu/pdfa-conformance-checker.html) in a .net Core Assembly and as WebApi. Access VeraPdf from your unit tests or integrate it into your micro architecture.

Install it using nuget package [PdfAValidatorApi](https://www.nuget.org/packages/PdfAValidator/):
```
Install-Package PdfAValidator
```

Sample - e.g. use it in your unit test to check compliance of some pdf: 
```C#
[Fact]
 public void ShouldDetectCompliantPdfA()
 {
     using (var pdfAValidator = new PdfAValidator.PdfAValidator())
     {
         var result = pdfAValidator.Validate(@"./TestPdfFiles/FromLibreOffice.pdf");
         Assert.True(result);
     }
 }
```

Sample - e.g. use it in your unit test to check the used sub standard of some pdf: 
```C#
[Fact]
 public void ShouldGetDetailedReportFromPdfA()
 {
     using (var pdfAValidator = new PdfAValidator.PdfAValidator())
     {
         var result = pdfAValidator.ValidateWithDetailedReport(@"./TestPdfFiles/FromLibreOffice.pdf");
         Assert.True(result.jobs.job.validationReport.isCompliant);
         Assert.True(result.jobs.job.validationReport.profileName == "PDF/A-1A validation profile");
     }
 }
```
