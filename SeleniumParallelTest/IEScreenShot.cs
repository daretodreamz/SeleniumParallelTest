using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SHDocVw;
using mshtml;


namespace SeleniumParallelTest
{
    public class IEScreenShot
    {

        //We need some system dll functions.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr parent /*HWND*/, IntPtr next /*HWND*/, string sClassName,
            IntPtr sWindowTitle);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.Dll")]
        public static extern void GetClassName(int h, StringBuilder s, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        public const int GW_CHILD = 5;
        public const int GW_HWNDNEXT = 2;

        public static bool IsDTDDocument(object document)
        {
            // XHtml declare flag string
            string DocTypeContent = @"-//W3C//DTD";
            mshtml.IHTMLDocument3 document3 = (mshtml.IHTMLDocument3)document;
            mshtml.IHTMLDOMChildrenCollection domChilds = (mshtml.IHTMLDOMChildrenCollection)document3.childNodes;
            mshtml.IHTMLDOMNode domNode = (mshtml.IHTMLDOMNode)domChilds.item(0);
            return domNode.nodeValue.ToString().Contains(DocTypeContent);
        }

        public static void TakeScreenShot()
        {

           // Thread.Sleep(20000);


            WebBrowser m_browser = null;

            ShellWindows shellWindows = new ShellWindowsClass();

            //Find first availble browser window.
            //Application can easily be modified to loop through and capture all open windows.
            string filename;
            foreach (SHDocVw.WebBrowser ie in shellWindows)
            {
                filename = Path.GetFileNameWithoutExtension(ie.FullName).ToLower();

                if (filename.Equals("iexplore"))
                {
                    m_browser = ie;
                    break;
                }
            }

            //Assign Browser Document
            mshtml.IHTMLDocument2 myDoc = (mshtml.IHTMLDocument2)m_browser.Document;
            mshtml.IHTMLDocument3 doc3 = (mshtml.IHTMLDocument3)myDoc;
            int heightsize = 0;
            int widthsize = 0;
            int screenHeight = 0;
            int screenWidth = 0;


            //if (!IsDTDDocument(myDoc))
            //{
            //    myDoc.body.setAttribute("scroll", "Yes", 0);

            //    //Get Browser Window Height
            //    heightsize = (int)myDoc.body.getAttribute("scrollHeight", 0);
            //    widthsize = (int)myDoc.body.getAttribute("scrollWidth", 0);

            //    //Get Screen Height
            //    screenHeight = (int)myDoc.body.getAttribute("clientHeight", 0);
            //    screenWidth = (int)myDoc.body.getAttribute("clientWidth", 0);
            //}
            //else
            {
                doc3.documentElement.setAttribute("scroll", "Yes", 0);

                //Get Browser Window Height
                heightsize = (int)doc3.documentElement.getAttribute("scrollHeight", 0);
                widthsize = (int)doc3.documentElement.getAttribute("scrollWidth", 0);

                //Get Screen Height
                screenHeight = (int)doc3.documentElement.getAttribute("clientHeight", 0);
                screenWidth = (int)doc3.documentElement.getAttribute("clientWidth", 0);
            }

            //Get bitmap to hold screen fragment.
            Bitmap bm = new Bitmap(screenWidth, screenHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

            //Create a target bitmap to draw into.
            Bitmap bm2 = new Bitmap(widthsize, heightsize,
                System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            Graphics g2 = Graphics.FromImage(bm2);

            Graphics g = null;
            IntPtr hdc;
            Image screenfrag = null;
            int brwTop = 0;
            int brwLeft = 0;
            int myPage = 0;
            IntPtr myIntptr = (IntPtr) m_browser.HWND;
            //Get inner browser window.
            int hwndInt = myIntptr.ToInt32();
            IntPtr hwnd = myIntptr;
            hwnd = GetWindow(hwnd, GW_CHILD);
            StringBuilder sbc = new StringBuilder(256);
            //Get Browser "Document" Handle
            while (hwndInt != 0)
            {
                
                hwndInt = hwnd.ToInt32();
                GetClassName(hwndInt, sbc, 256);

                if (sbc.ToString().IndexOf("Shell DocObject View", 0) > -1) // pre-IE7
                {
                    hwnd = FindWindowEx(hwnd, IntPtr.Zero, "Internet Explorer_Server", IntPtr.Zero);
                    break;
                }

                if (sbc.ToString().IndexOf("TabWindowClass", 0) > -1) // IE7
                {
                    hwnd = FindWindowEx(hwnd, IntPtr.Zero, "Shell DocObject View", IntPtr.Zero);
                    hwnd = FindWindowEx(hwnd, IntPtr.Zero, "Internet Explorer_Server", IntPtr.Zero);
                    break;
                }

                if (sbc.ToString().IndexOf("Frame Tab", 0) > -1) // IE8
                {
                    hwnd = FindWindowEx(hwnd, IntPtr.Zero, "TabWindowClass", IntPtr.Zero);
                    hwnd = FindWindowEx(hwnd, IntPtr.Zero, "Shell DocObject View", IntPtr.Zero);
                    hwnd = FindWindowEx(hwnd, IntPtr.Zero, "Internet Explorer_Server", IntPtr.Zero);
                    break;
                }
                hwnd = GetWindow(hwnd, GW_HWNDNEXT);

            }

            //Get Screen Height (for bottom up screen drawing)
            while ((myPage*screenHeight) < heightsize)
            {
                //if (!IsDTDDocument(myDoc))
                //    myDoc.body.setAttribute("scrollTop", (screenHeight - 5) * myPage, 0);
                //else
                    doc3.documentElement.setAttribute("scrollTop", (screenHeight - 5) * myPage, 0);
                ++myPage;
            }
            //Rollback the page count by one
            --myPage;

            int myPageWidth = 0;

            while ((myPageWidth*screenWidth) < widthsize)
            {
                //if (!IsDTDDocument(myDoc))
                //    myDoc.body.setAttribute("scrollLeft", (screenWidth - 5) * myPageWidth, 0);
                //else
                    doc3.documentElement.setAttribute("scrollLeft", (screenWidth - 5) * myPageWidth, 0);
                //if (!IsDTDDocument(myDoc))
                //    brwLeft = (int)myDoc.body.getAttribute("scrollLeft", 0);
                //else
                    brwLeft = (int)doc3.documentElement.getAttribute("scrollLeft", 0);
                for (int i = myPage; i >= 0; --i)
                {
                    //Shoot visible window
                    g = Graphics.FromImage(bm);
                    hdc = g.GetHdc();
                    //if (!IsDTDDocument(myDoc))
                    //    myDoc.body.setAttribute("scrollTop", (screenHeight - 5) * i, 0);
                    //else
                        doc3.documentElement.setAttribute("scrollTop", (screenHeight - 5) * i, 0);

                    //if (!IsDTDDocument(myDoc))
                    //    brwTop = (int)myDoc.body.getAttribute("scrollTop", 0);
                    //else
                        brwTop = (int)doc3.documentElement.getAttribute("scrollTop", 0);
                    PrintWindow(hwnd, hdc, 0);
                    g.ReleaseHdc(hdc);
                    g.Flush();
                    screenfrag = Image.FromHbitmap(bm.GetHbitmap());
                    g2.DrawImage(screenfrag, brwLeft, brwTop);
                }
                ++myPageWidth;
            }

            //Reduce Resolution Size
            double myResolution = Convert.ToDouble(100) * 0.01;
            int finalWidth = (int)((widthsize) * myResolution);
            int finalHeight = (int)((heightsize) * myResolution);
            Bitmap finalImage = new Bitmap(finalWidth, finalHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            Graphics gFinal = Graphics.FromImage((Image)finalImage);
            gFinal.DrawImage(bm2, 0, 0, finalWidth, finalHeight);

            //Get Time Stamp
            DateTime myTime = DateTime.Now;
            String format = "MM.dd.hh.mm.ss";

            //Create Directory to save image to.
            Directory.CreateDirectory("D:\\IECapture");

            //Write Image.
            EncoderParameters eps = new EncoderParameters(1);
            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Convert.ToInt64(100));
            ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
            finalImage.Save(@"D:\\IECapture\Captured_" + myTime.ToString(format) + ".jpg", ici, eps);

            //Clean Up.
            myDoc = null;
            g.Dispose();
            g2.Dispose();
            gFinal.Dispose();
            bm.Dispose();
            bm2.Dispose();
            finalImage.Dispose();
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        //[ComImport]
        //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        //[Guid("0000010d-0000-0000-C000-000000000046")]
        //private interface IViewObject
        //{
        //    [PreserveSig]
        //    int Draw([In] [MarshalAs(UnmanagedType.U4)] int dwDrawAspect, int lindex, IntPtr pvAspect,
        //        [In] /*tagDVTARGETDEVICE*/ IntPtr ptd, IntPtr hdcTargetDev, IntPtr hdcDraw,
        //        [In] /*COMRECT*/ Rectangle lprcBounds, [In] /*COMRECT*/ IntPtr lprcWBounds, IntPtr pfnContinue,
        //        [In] int dwContinue);

        //    [PreserveSig]
        //    int GetColorSet([In] [MarshalAs(UnmanagedType.U4)] int dwDrawAspect, int lindex, IntPtr pvAspect,
        //        [In] /*tagDVTARGETDEVICE*/ IntPtr ptd, IntPtr hicTargetDev, [Out] /*tagLOGPALETTE*/ IntPtr ppColorSet);

        //    [PreserveSig]
        //    int Freeze([In] [MarshalAs(UnmanagedType.U4)] int dwDrawAspect, int lindex, IntPtr pvAspect,
        //        [Out] IntPtr pdwFreeze);

        //    [PreserveSig]
        //    int Unfreeze([In] [MarshalAs(UnmanagedType.U4)] int dwFreeze);

        //    void SetAdvise([In] [MarshalAs(UnmanagedType.U4)] int aspects, [In] [MarshalAs(UnmanagedType.U4)] int advf,
        //        [In] [MarshalAs(UnmanagedType.Interface)] /*IAdviseSink*/ IntPtr pAdvSink);

        //    void GetAdvise([In] [Out] [MarshalAs(UnmanagedType.LPArray)] int[] paspects,
        //        [In] [Out] [MarshalAs(UnmanagedType.LPArray)] int[] advf,
        //        [In] [Out] [MarshalAs(UnmanagedType.LPArray)] /*IAdviseSink[]*/ IntPtr[] pAdvSink);
        //}

        //public static Bitmap Create(string url)
        //{
        //    using (var webBrowser = new WebBrowser())
        //    {
        //        webBrowser.ScrollBarsEnabled = false;
        //        webBrowser.ScriptErrorsSuppressed = true;
        //        webBrowser.Navigate(url);

        //        while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
        //        {
        //            Application.DoEvents();
        //        }

        //        webBrowser.Width = webBrowser.Document.Body.ScrollRectangle.Width;
        //        webBrowser.Height = webBrowser.Document.Body.ScrollRectangle.Height;

        //        var bitmap = new Bitmap(webBrowser.Width, webBrowser.Height);
        //        var graphics = Graphics.FromImage(bitmap);
        //        var hdc = graphics.GetHdc();

        //        var rect = new Rectangle(0, 0, webBrowser.Width, webBrowser.Height);

        //        var viewObject = (IViewObject) webBrowser.Document.DomDocument;
        //        viewObject.Draw(1, -1, (IntPtr) 0, (IntPtr) 0, (IntPtr) 0, hdc, rect, (IntPtr) 0, (IntPtr) 0, 0);

        //        graphics.ReleaseHdc(hdc);

        //        return bitmap;
        //    }
        //}

    }
}

