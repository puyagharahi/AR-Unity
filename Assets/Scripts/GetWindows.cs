using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

public class GetWindows : MonoBehaviour {
	[DllImport("user32.dll")] static extern int GetForegroundWindow();
	
	[DllImport("user32.dll", EntryPoint="MoveWindow")]  
	static extern int  MoveWindow (int hwnd, int x, int y,int nWidth,int nHeight,int bRepaint );
	
	[DllImport("user32.dll", EntryPoint="SetWindowLongA")]  
	static extern int  SetWindowLong (int hwnd, int nIndex,int dwNewLong);
	
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
	public 
	void Start ()
	{
		windowsText = GetComponent<Text>();
		windowsText.text = "";
		EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero); 
	}
	
	
	void Update ()
	{	windowsText.text = "";
		EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero); 

	}
	
	
	static void SetText(string n)
	{	
		//if(!n.Equals("Start") && !n.Equals("MonoDevelop") && !n.Equals("Program Manager"))
			windowsText.text += n+"\n";
	}
	
	protected static bool EnumTheWindows(IntPtr  hWnd, IntPtr lParam) 
	{ 
		int size = GetWindowTextLength(hWnd); 
		if (size++ > 0 && IsWindowVisible(hWnd)) 
		{ 
			StringBuilder sb = new StringBuilder(size); 
			GetWindowText(hWnd, sb, size); 
			SetText(sb.ToString());
			Bitmap temp = PrintWindow(hWnd);
			byte[] data = (byte[])convertdabytes.ConvertTo(temp,typeof(byte[]));
            //System.Drawing.Image img = (System.Drawing.Image)temp;
            Texture2D wintexture = new Texture2D(2, 2);
            wintexture.LoadImage(data);
            wintexture.Apply(); //wills use in list of texutres if string name not found make new and add to list

        } 
		return true; 
	} 


	[DllImport("user32.dll")]
	public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
	[DllImport("user32.dll")]
	public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
	
	public static Bitmap PrintWindow(IntPtr hwnd)    
	{       
		RECT rc;        
		GetWindowRect(hwnd, out rc);
		
		Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);        
		System.Drawing.Graphics gfxBmp = System.Drawing.Graphics.FromImage(bmp);        
		IntPtr hdcBitmap = gfxBmp.GetHdc();        
		
		PrintWindow(hwnd, hdcBitmap, 0);  
		
		gfxBmp.ReleaseHdc(hdcBitmap);               
		gfxBmp.Dispose(); 
		
		return bmp;   
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
