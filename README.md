# PdfAValidatorApi

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/30d54e6caa344b12b27f0d725cac52d9)](https://app.codacy.com/app/stesee/PdfAValidatorApi?utm_source=github.com&utm_medium=referral&utm_content=Codeuctivity/PdfAValidatorApi&utm_campaign=Badge_Grade_Settings)
[![Build status](https://ci.appveyor.com/api/projects/status/hwa0obfdvoxy9wkw?svg=true)](https://ci.appveyor.com/project/stesee/pdfavalidatorapi) [![Nuget](https://img.shields.io/nuget/v/PdfaValidator.svg)](https://www.nuget.org/packages/PdfAValidator/)
[![Build status](https://codeuctivity.visualstudio.com/PdfAValidatorApi/_apis/build/status/PdfAValidator%20-%20CI)](https://codeuctivity.visualstudio.com/PdfAValidatorApi/_build/latest?definitionId=1)

PdfAValidatorApi wraps calls to [VeraPdf](http://www.preforma-project.eu/pdfa-conformance-checker.html) in a .net standard assembly and as WebApi. Access VeraPdf from your unit tests or integrate it into your micro architecture.

Install it using nuget package [PdfAValidatorApi](https://www.nuget.org/packages/PdfAValidator/):

```PowerShell
dotnet add package PdfAValidator
```

Sample - e.g. use it in your unit test to check compliance of some pdf:

```csharp
public static async Task ShouldDetectCompliantPdfA()
{
    using var pdfAValidator = new PdfAValidator();
    Assert.True(File.Exists("./TestPdfFiles/FromLibreOffice.pdf"));
    var result = await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOffice.pdf");
    Assert.True(result);
}
```

Sample - e.g. use it in your unit test to check the used sub standard of some pdf:

```csharp
public static async Task ShouldGetDetailedReportFromPdfA()
{
    using var pdfAValidator = new PdfAValidator();
    Assert.True(File.Exists("./TestPdfFiles/FromLibreOffice.pdf"));
    var result = await pdfAValidator.ValidateWithDetailedReportAsync("./TestPdfFiles/FromLibreOffice.pdf");
    Assert.True(result.Jobs.Job.ValidationReport.IsCompliant);
    Assert.True(result.Jobs.Job.ValidationReport.ProfileName == "PDF/A-1A validation profile");
}
```

## Demo OpenApi - PdfAValidatorWebApi

Give <https://pdfavalidator.azurewebsites.net> a try, but dont be disappointed if it is offline. The demo azure account is running on limited budget.

## Dependencies

### Windows

Everything comes with the nuget package

### Ubuntu 20.04

Current PdfAValidatorApi depends on opnejdf-8-jre.

```bash
sudo snap install dotnet-sdk
sudo snap alias dotnet-sdk.dotnet dotnet
sudo apt install openjdk-8-jre
sudo update-alternatives --config java
```
