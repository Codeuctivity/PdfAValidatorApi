# PdfAValidatorApi

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/0f90c8f8a86943ccbd8da136f542104f)](https://www.codacy.com/gh/Codeuctivity/PdfAValidatorApi/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Codeuctivity/PdfAValidatorApi&amp;utm_campaign=Badge_Grade) [![Build status](https://ci.appveyor.com/api/projects/status/hwa0obfdvoxy9wkw?svg=true)](https://ci.appveyor.com/project/stesee/pdfavalidatorapi) [![Nuget](https://img.shields.io/nuget/v/PdfaValidator.svg)](https://www.nuget.org/packages/PdfAValidator/) [![Build status](https://codeuctivity.visualstudio.com/PdfAValidatorApi/_apis/build/status/PdfAValidator%20-%20CI)](https://codeuctivity.visualstudio.com/PdfAValidatorApi/_build/latest?definitionId=1)

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

Current PdfAValidatorApi depends on opnejdf-8-jre and .net core 3.1.

```bash
sudo apt install openjdk-8-jre
sudo update-alternatives --config java
```

#### Additional things for developing this package

#### Setup .net sdk 

Based on https://docs.microsoft.com/de-de/dotnet/core/install/linux-package-manager-ubuntu-2004

```bash
sudo snap remove dotnet-sdk
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-3.1
```