﻿using System;
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

    [DllImport("user32.dll")] static extern int GetForegroundWindow();

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

    public static Text windowsText;
    public Process[] processes;
    private static ImageConverter convertdabytes = new ImageConverter();
    public static SortedList<String, GameObject> ListWindow = new SortedList<string, GameObject>();

    void Start()
    {
        windowsText = GetComponent<Text>();
        windowsText.text = "";
        EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
    }


    void Update()
    {
        windowsText.text = "";
        EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
    }


    static void SetText(string n)
    {
        //if(!n.Equals("Start") && !n.Equals("MonoDevelop") && !n.Equals("Program Manager"))
        windowsText.text += n + "\n";
    }
    private static Texture2D wintexture = new Texture2D(2, 2);
    protected static bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
    {
        int size = GetWindowTextLength(hWnd);
        if (size++ > 0 && IsWindowVisible(hWnd))
        {
            StringBuilder sb = new StringBuilder(size);
            GetWindowText(hWnd, sb, size);
            SetText(sb.ToString());

            if (!ListWindow.ContainsKey(sb.ToString()))
            {
                Texture2D temp = PrintWindow(hWnd);
                if (temp == null) return true;
                GameObject fuckYou = Instantiate(Resources.Load("Window")) as GameObject;
                fuckYou.GetComponent<Renderer>().sharedMaterial.mainTexture = temp;
                System.Random random = new System.Random();
                int randomNumber = random.Next(0, 100);
                Vector3 vec = new Vector3(randomNumber, 0, 0);
                fuckYou.transform.position += vec;
                ListWindow.Add(sb.ToString(), fuckYou);
                Destroy(temp);
            } else {
//                (ListWindow.Values[ListWindow.IndexOfKey(sb.ToString())]).GetComponent<Renderer>().sharedMaterial.mainTexture = temp;
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
        
        Texture2D tex = new Texture2D(rc.Width, rc.Height, TextureFormat.RGB24, false);
        UnityEngine.Debug.Log(rc);
        Bitmap target = new Bitmap(rc.Width, rc.Height);
        UnityEngine.Debug.Log('B');
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
		
		public int X {
			get { return _Left; }
			set { _Left = value; }
		}
		public int Y {
			get { return _Top; }
			set { _Top = value; }
		}
		public int Left {
			get { return _Left; }
			set { _Left = value; }
		}
		public int Top {
			get { return _Top; }
			set { _Top = value; }
		}
		public int Right {
			get { return _Right; }
			set { _Right = value; }
		}
		public int Bottom {
			get { return _Bottom; }
			set { _Bottom = value; }
		}
		public int Height {
			get { return _Bottom - _Top; }
			set { _Bottom = value + _Top; }
		}
		public int Width {
			get { return _Right - _Left; }
			set { _Right = value + _Left; }
		}
		public Point Location {
			get { return new Point(Left, Top); }
			set {
				_Left = value.X;
				_Top = value.Y;
			}
		}
		public Size Size {
			get { return new Size(Width, Height); }
			set {
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
			if (Object is RECT) {
				return Equals((RECT)Object);
			} else if (Object is Rectangle) {
				return Equals(new RECT((Rectangle)Object));
			}
			
			return false;
		}
	}
}
