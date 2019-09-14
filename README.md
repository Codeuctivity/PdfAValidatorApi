# PdfAValidatorApi

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/30d54e6caa344b12b27f0d725cac52d9)](https://app.codacy.com/app/stesee/PdfAValidatorApi?utm_source=github.com&utm_medium=referral&utm_content=Codeuctivity/PdfAValidatorApi&utm_campaign=Badge_Grade_Settings)
[![Build status](https://ci.appveyor.com/api/projects/status/hwa0obfdvoxy9wkw?svg=true)](https://ci.appveyor.com/project/stesee/pdfavalidatorapi) [![Nuget](https://img.shields.io/nuget/v/PdfaValidator.svg)](https://www.nuget.org/packages/PdfAValidator/)
[![Build status](https://codeuctivity.visualstudio.com/PdfAValidatorApi/_apis/build/status/PdfAValidator%20-%20CI)](https://codeuctivity.visualstudio.com/PdfAValidatorApi/_build/latest?definitionId=1)

PdfAValidatorApi wraps calls to [VeraPdf](http://www.preforma-project.eu/pdfa-conformance-checker.html) in a .net Core Assembly and as WebApi. Access VeraPdf from your unit tests or integrate it into your micro architecture.

Install it using nuget package [PdfAValidatorApi](https://www.nuget.org/packages/PdfAValidator/):

    Install-Package PdfAValidator

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
      using (var pdfAValidator = new  PdfAValidator.PdfAValidator())
      {
          var result =  pdfAValidator.ValidateWithDetailedRepor  (@"./TestPdfFileFromLibreOffice.pdf");
          Assert.Tru  (result.jobs.job.validationReport  isCompliant);
          Assert.Tru  (result.jobs.job.validationReport  profileName == "PDF/A-1A validation  profile");
      }
  }
  ```

[![NuGet Status](http://nugetstatus.com/PdfAValidator.png)](http://nugetstatus.com/packages/PdfAValidator)

## Demo OpenApi - PdfAValidatorWebApi

Give <https://pdfavalidator.azurewebsites.net> a try, but dont be disappointed if it is offline. The demo azure account is running on limited budget.
