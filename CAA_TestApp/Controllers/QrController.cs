using CAA_TestApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using QRCoder;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace CAA_TestApp.Controllers
{
    [Authorize]
    public class QrController : Controller
    {


        private readonly IConverter _converter;

        public QrController(IConverter converter)
        {
            _converter = converter;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewForPDF()
        {
            return View();
        }

       
        public IActionResult DownloadPDF()
        {
            string actualPage = HttpContext.Request.Path;
            string pageURL = HttpContext.Request.GetEncodedUrl();
            pageURL = pageURL.Replace(actualPage, "");
            pageURL = $"{pageURL}/Qr/ViewForPDF";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page = pageURL,
                    }
                }
            };
            var PDFFile = _converter.Convert(pdf);
            string PDFName = "Report_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
            return File(PDFFile, "application/pdf", PDFName);
        }

        public IActionResult GenerateQr() 
        {
            Random r = new Random();
            int isbn = r.Next(10, 50);

            QRCodeGenerator qrCodeGen = new QRCodeGenerator();
            QRCodeData qrData = qrCodeGen.CreateQrCode(isbn.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qr = new QRCode(qrData);

            using(MemoryStream ms = new MemoryStream())
            {
                using(Bitmap bitmap = qr.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64" + Convert.ToBase64String(ms.ToArray());
                }
            }

            return View();
        }


    }
}
