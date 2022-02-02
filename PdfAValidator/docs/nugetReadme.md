PdfAValidatorApi wraps calls to [VeraPdf](http://www.preforma-project.eu/pdfa-conformance-checker.html) in a .net standard assembly and as WebApi. Access VeraPdf from your unit tests. Works on Windows and Linux.

Sample - e.g. use it in your unit test to check compliance of some pdf:

```csharp
public static async Task ShouldDetectCompliantPdfA()
{
    using var pdfAValidator = new PdfAValidator();
    var result = await pdfAValidator.ValidateAsync("./TestPdfFiles/FromLibreOffice.pdf");
    Assert.True(result);
}
```
