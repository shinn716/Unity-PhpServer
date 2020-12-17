using UnityEngine;
using ZXing;
using ZXing.QrCode;

namespace Shinn
{
    public static class QRcodeHelper
    {
        public static Texture2D GetQRcodeTexture(string url)
        {
            Texture2D tex2D_result = new Texture2D(256, 256);                                   //設定QR Code圖片大小
            Color32[] color32 = ZXingEncode(url, tex2D_result.width, tex2D_result.height);      //儲存產生的QR Code
            tex2D_result.SetPixels32(color32);                                                  //設定要顯示的圖片像素
            tex2D_result.Apply();                                                               //申請顯示圖片
            return tex2D_result;
        }

        private static Color32[] ZXingEncode(string textForEncoding, int width = 256, int height = 256)
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,                                                 //設定格式為QR Code
                Options = new QrCodeEncodingOptions                                             //設定QR Code圖片寬度和高度
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);                                               //將字串寫入，同時回傳轉換後的QR Code
        }
    }
}
