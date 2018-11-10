using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace plugin_petercashel_ModdersGearbox.Support.Image
{
	// Work in progress BitMap replacement using Texture2D and TextureFormat.ARGB32
	// Author: Peter Cashel - github:pacas00
	// 

	public class BitMap : IDisposable
    {
        public Texture2D _Texture2D { get; private set; }
        public bool _Disposed { get; private set; }
        public int _Height { get; private set; }
        public int _Width { get; private set; }

        public BitMap(int width, int height)
        {
            _Width = width;
            _Height = height;
            _Texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        }

        public BitMap(Texture2D texture2D)
        {
            _Texture2D = texture2D;
            _Width = texture2D.width;
            _Height = texture2D.height;
        }

        public static implicit operator Texture2D(BitMap bmp)
        {
            return bmp._Texture2D;
        }

		//Load data with 
		//bool - _Texture2D.LoadImage(byte[] data) 
		//void - _Texture2D.LoadRawTextureData(byte[] data); 
		//void - _Texture2D.LoadRawTextureData(IntPtr data, int size);

		public void SetPixel(int x, int y, UnityEngine.Color colour)
        {
            _Texture2D.SetPixel(x, y, colour);
        }

        public void SetPixel(int x, int y, UnityEngine.Color32 colour)
        {
            _Texture2D.SetPixel(x, y, colour);
        }

        //Color implicitly converts to Color32, just cast.
        public UnityEngine.Color GetPixel(int x, int y)
        {
            return _Texture2D.GetPixel(x, y);
        }

        public void Resize(int width, int height, ResizeMode mode)
        {
            if (mode == ResizeMode.Bilinear)
            {
                TextureScale.Bilinear(_Texture2D, width, height);
			}

            if (mode == ResizeMode.Point)
            {
                TextureScale.Point(_Texture2D, width, height);
			}
        }

        public void Finalise(bool UpdateMipmaps = false, bool MakeNoLongerReadable = false)
        {
            _Texture2D.Apply(UpdateMipmaps, MakeNoLongerReadable);
        }

        public void Dispose()
        {
            if (_Disposed) return;
            _Disposed = true;
            _Texture2D = null;
            //GC.Collect(); //If you want to force it.
        }

		public void LoadPngOrJpg(string filePath)
        {
            if (File.Exists(filePath))
            {
                byte[] data = File.ReadAllBytes(filePath);
                _Texture2D.LoadImage(data);
            }
        }

		public void SaveAsPNG(string filePath)
        {
            _Texture2D.Apply();
            System.IO.File.WriteAllBytes(filePath, _Texture2D.EncodeToPNG());
        }

        public void SaveAsJPG(string filePath)
        {
            _Texture2D.Apply();
            System.IO.File.WriteAllBytes(filePath, _Texture2D.EncodeToJPG());
        }

        public enum ResizeMode
        {
            Point,
            Bilinear
        }
	}
}
