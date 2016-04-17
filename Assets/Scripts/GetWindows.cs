using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Threading;

public class GetWindows : MonoBehaviour
{
    [DllImport("User32.dll")]
    public static extern int SelectObject(int hdc, int hgdiobj);
    [DllImport("User32.dll")]
    public static extern int GetDesktopWindow();
    [DllImport("User32.dll")]
    public static extern int GetWindowDC(int hWnd);
    [DllImport("User32.dll")]
    public static extern int ReleaseDC(int hWnd, int hDC);

    [DllImport("user32.dll")]
    static extern int GetForegroundWindow();

    [DllImport("user32.dll", EntryPoint = "MoveWindow")]
    static extern int MoveWindow(int hwnd, int x, int y, int nWidth, int nHeight, int bRepaint);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongA")]
    static extern int SetWindowLong(int hwnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern bool ShowWindowAsync(int hWnd, int nCmdShow);

    protected delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    protected static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    protected static extern int GetWindowTextLength(IntPtr hWnd);
    [DllImport("user32.dll")]
    protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
    [DllImport("user32.dll")]
    protected static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    const UInt32 SWP_NOSIZE = 0x0001;
    const UInt32 SWP_NOMOVE = 0x0002;
    const UInt32 SWP_SHOWWINDOW = 0x0040;

    public static Text windowsText;
    public Process[] processes;
    private static ImageConverter convertdabytes = new ImageConverter();
    public static SortedList<String, GameObject> ListWindow = new SortedList<string, GameObject>();

    static bool inited = false;

    void Start()
    {
        windowsText = GetComponent<Text>();
        windowsText.text = "";
        EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
        inited = true;
    }


    void Update()
    {
        windowsText.text = "";
        EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
    }

    
    public void OnPostRender()
    {
        foreach (var g in ListWindow)
        {
            var o = g.Value;
            if (o == null) continue;
            var comp = o.GetComponent<Renderer>();
            var material = comp.material;
            var temp = material.mainTexture;
            UnityEngine.Graphics.DrawTexture(
                new Rect(0, 0, temp.width, temp.height),
                temp, material);
        }
    }

    static void SetText(string n)
    {
        //if(!n.Equals("Start") && !n.Equals("MonoDevelop") && !n.Equals("Program Manager"))
        windowsText.text += n + "\n";
    }
    private static StringBuilder sb;
    private static Texture2D wintexture = new Texture2D(2, 2);
    private static int x = 0, y = 0, z = 0;
    protected static bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
    {
        int size = GetWindowTextLength(hWnd);
        if (size++ > 0 && IsWindowVisible(hWnd))
        {
            sb = new StringBuilder(size);
            GetWindowText(hWnd, sb, size);
            SetText(sb.ToString());
            
            if (sb.ToString().Equals("Windows Shell Experience Host") || sb.ToString().Equals("Program Manager")) return true;

            if (!ListWindow.ContainsKey(sb.ToString()))
            {
                Texture2D temp = PrintWindow(hWnd);
                if (temp == null)
                {
                    ListWindow.Add(sb.ToString(), null);
                    return true;
                }
                GameObject fuckYou = Instantiate(Resources.Load("Window")) as GameObject;
                fuckYou.GetComponent<Renderer>().material.mainTexture = temp;
                    Vector3 vec = new Vector3(x+=50, 0, z+=5);
                    fuckYou.transform.position = vec;
                Vector3 vv = fuckYou.transform.localScale;
                vv.x = temp.width / 200f;
                vv.z = temp.height / 200f;
                fuckYou.transform.localScale = vv;
                ListWindow.Add(sb.ToString(), fuckYou);
                //GUI.DrawTexture(new Rect(0, 0, temp.width, temp.height), temp, ScaleMode.ScaleToFit, true);
                //Destroy(temp);
            }
        }
        return true;
    }


    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll")]
    public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
    
    public static Texture2D PrintWindow(IntPtr hwnd)
    {
        RECT rc;
        GetWindowRect(hwnd, out rc);
        if (rc == null || rc.Width == 0) return null;
            MoveWindow(hwnd, 0, 0, rc.Width, rc.Height, true);
        SetForegroundWindow(hwnd);
        Thread.Sleep(100);
        Texture2D tex = new Texture2D(rc.Width, rc.Height, TextureFormat.RGB24, true);
        Bitmap target = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(target))
        {
            g.CopyFromScreen(new Point(rc.Left, rc.Top), Point.Empty, rc.Size);
        }
        MemoryStream ms = new MemoryStream();
        target.Save(ms, ImageFormat.Png);
        ms.Seek(0, SeekOrigin.Begin);

        tex.LoadImage(ms.ToArray());
        return tex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        private int _Left;
        private int _Top;
        private int _Right;
        private int _Bottom;

        public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
        {
        }
        public RECT(int Left, int Top, int Right, int Bottom)
        {
            _Left = Left;
            _Top = Top;
            _Right = Right;
            _Bottom = Bottom;
        }

        public int X
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Y
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Left
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Top
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Right
        {
            get { return _Right; }
            set { _Right = value; }
        }
        public int Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        public int Height
        {
            get { return _Bottom - _Top; }
            set { _Bottom = value + _Top; }
        }
        public int Width
        {
            get { return _Right - _Left; }
            set { _Right = value + _Left; }
        }
        public Point Location
        {
            get { return new Point(Left, Top); }
            set
            {
                _Left = value.X;
                _Top = value.Y;
            }
        }
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                _Right = value.Width + _Left;
                _Bottom = value.Height + _Top;
            }
        }

        public static implicit operator Rectangle(RECT Rectangle)
        {
            return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
        }
        public static implicit operator RECT(Rectangle Rectangle)
        {
            return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
        }
        public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
        {
            return Rectangle1.Equals(Rectangle2);
        }
        public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
        {
            return !Rectangle1.Equals(Rectangle2);
        }

        public override string ToString()
        {
            return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(RECT Rectangle)
        {
            return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
        }

        public override bool Equals(object Object)
        {
            if (Object is RECT)
            {
                return Equals((RECT)Object);
            }
            else if (Object is Rectangle)
            {
                return Equals(new RECT((Rectangle)Object));
            }

            return false;
        }
    }
}
