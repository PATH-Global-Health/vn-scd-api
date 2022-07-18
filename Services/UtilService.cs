using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public interface IUtilService
    {
        Task<Image> ResizeImageAsync(IFormFile image);
        byte[] ImageToBytes(Image image);
        IFormFile BytesToImage(byte[] data, string imgName);
    }
    public class UtilService : IUtilService
    {
        public async Task<Image> ResizeImageAsync(IFormFile image)
        {
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await image.CopyToAsync(memoryStream);

                }
                catch (Exception)
                {
                    throw new Exception("Copy Faild");
                }

                using (var img = Image.FromStream(memoryStream))
                {

                    int x, y, w, h;

                    //HD Size
                    int desWidth = 1280;
                    int desHeight = 720;

                    // Vertical
                    if (img.Height > img.Width)
                    {
                        w = (img.Width * desHeight) / img.Height;
                        h = desHeight;
                        x = (desWidth - w) / 2;
                        y = 0;
                    }
                    else
                    {
                        //Horizontal
                        w = desWidth;
                        h = (img.Height * desWidth) / img.Width;
                        x = 0;
                        y = (desHeight - h) / 2;
                    }

                    var bmp = new Bitmap(desWidth, desHeight);
                    try
                    {

                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.DrawImage(img, x, y, w, h);
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("Bitmap faild");
                    }
                    return bmp;
                }
            }
        }
        public byte[] ImageToBytes(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Gif);
            //return Convert.ToBase64String(ms.ToArray(), 0, ms.ToArray().Length);
            return ms.ToArray();
        }

        public IFormFile BytesToImage(byte[] data, string imgName)
        {
            var stream = new MemoryStream(data);
            return new FormFile(stream, 0, data.Length, "name", imgName);
        }
    }
}
