using CoreLib;
using CoreLib.Extentions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AniFile3.Contents
{
    public class SearchResultContent : INotifyPropertyChanged
    {
        private string _imageHashPath;

        public string ImageUrl
        {
            get => _imageHashPath;
            set { _imageHashPath = value; NotifyPropertyChanged("ImageUrl"); }
        }
        public string Name { get; }
        public int Count { get; }

        public SearchResultContent(string imageUrl, string name, int count)
        {
            Name = name;
            Count = count;

            _imageHashPath = MakeHashPath(imageUrl);

            // 캐시 이미지가 있다면, 
            if (File.Exists(_imageHashPath))
            {
                // 바인딩되어 있는 프로퍼티에 넣고 끝
                ImageUrl = _imageHashPath;
            }
            else
            {
                CacheImage(imageUrl);
            }
        }

        private string MakeHashPath(string url)
        {
            string extension = Path.GetExtension(url);
            var cacheDirectory = AppDomain.CurrentDomain.BaseDirectory
                                + Preference.Instance.CacheDirectory
                                + "/image/";

            if (Directory.Exists(cacheDirectory) == false)
            {
                Directory.CreateDirectory(cacheDirectory);
            }

            return cacheDirectory + url.GetHashCode().ToString("X") + ".png";
        }

        private async void CacheImage(string url)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(url, UriKind.Absolute);
            }
            catch (UriFormatException e)
            {
                // URI 포맷 에러
                Log.Warning(e.Message);
                return ;
            }

            var web = new WebClient();
            byte[] data = await web.DownloadDataTaskAsync(uri).WithTimeout(5000);
            if(data == null)
            {
                Log.Warning("이미지 다운로드 실패," + url);
                return ;
            }

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(data);
            bitmap.EndInit();

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var fileStream = new FileStream(_imageHashPath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }

            // 바인딩되어 있는 프로퍼티에 넣고 끝
            ImageUrl = _imageHashPath;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
