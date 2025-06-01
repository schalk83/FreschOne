// QRCodeUtils.cs
using QRCoder;

namespace FreschOne.Helpers
{
    //test
    public static class QRCodeUtils
    {
        public static string GenerateQrCode(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            try
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new BitmapByteQRCode(qrCodeData);
                var qrBytes = qrCode.GetGraphic(20);

                return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"QR code generation failed: {ex.Message}");
                return null;
            }
        }
    }
}
