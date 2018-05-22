using System;
using System.Runtime.InteropServices;


namespace StbImage
{
    public static class StbImageDllImport
    {
        const string DllName = "StbImage";

        // flip the image vertically, so the first pixel in the output array is the bottom left
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void stbi_set_flip_vertically_on_load(int flag_true_if_should_flip);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr stbi_load_from_memory(Byte[] buffer, int len, out int x, out int y, out int comp, int req_comp);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void stbi_image_free(IntPtr retval_from_stbi_load);
    }

    public class ImageLoader : IDisposable
    {
        IntPtr m_p;
        readonly public int Width;
        readonly public int Height;
        ImageLoader(IntPtr p, int w, int h)
        {
            m_p = p;
            Width = w;
            Height = h;
        }

        public void CopyTo(Byte[] bytes)
        {
            var len = Width * Height * 4;
            if (bytes.Length != len)
            {
                throw new ArgumentException("bytes length is not valid");
            }
            Marshal.Copy(m_p, bytes, 0, len);
        }

        static int s_count;

        public static ImageLoader Create(Byte[] bytes)
        {
            if (s_count++ == 0)
            {
                StbImageDllImport.stbi_set_flip_vertically_on_load(1);
            }

            int w, h, ch;
            var p = StbImageDllImport.stbi_load_from_memory(bytes, bytes.Length, out w, out h, out ch, 4);
            if (p == default(IntPtr))
            {
                return null;
            }

            if (ch != 4)
            {
                StbImage.StbImageDllImport.stbi_image_free(p);
                return null;
            }

            return new ImageLoader(p, w, h);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                var p = m_p;
                m_p = default(IntPtr);
                if (p != default(IntPtr))
                {
                    StbImageDllImport.stbi_image_free(p);
                }

                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~ImageLoader() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
