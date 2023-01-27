using CAA_TestApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using QRCoder;

namespace CAA_TestApp.Controllers
{
    public class QrController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
