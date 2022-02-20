using System.Runtime.InteropServices;

namespace System.SDKs.Dji
{
    static class TSDK
    {
        [DllImport("libdirp.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int dirp_create_from_rjpeg(byte[] data, int size, ref IntPtr ph);

        [DllImport("libdirp.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int dirp_get_rjpeg_resolution(IntPtr h, ref dirp_resolution_t resolution);

        [DllImport("libdirp.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int dirp_get_original_raw(IntPtr h, byte[] raw_image, int size);

        [DllImport("libdirp.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int dirp_measure(IntPtr h, byte[] temp_image, int size);

        [DllImport("libdirp.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int dirp_measure_ex(IntPtr h, byte[] temp_image, int size);
        
        [DllImport("libdirp.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int dirp_get_measurement_params(IntPtr h, ref bdirp_measurement_params_t measurement_params);

        [DllImport("libdirp.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int dirp_destroy(IntPtr h);
    }
    struct dirp_resolution_t
    {
        public int width;
        public int height;
    }
    struct bdirp_measurement_params_t
    {
        public float distance;
        public float humidity;
        public float emissivity;
        public float reflection;
    }
}