using System;
using System.Runtime.InteropServices;
using static PortalMouse.Engine.Native.WinDef;
using static PortalMouse.Engine.Native.WinNt;

namespace PortalMouse.Engine.Native;

internal static class Gdi32 {
	private const string dllName = "gdi32.dll";

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-getdevicecaps"></see>
	/// </summary>
	[DllImport(dllName, CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern int GetDeviceCaps(IntPtr hdc, int index);

	public enum DeviceCap {
		/// <summary>
		/// Device driver version
		/// </summary>
		DRIVERVERSION = 0,
		/// <summary>
		/// Device classification
		/// </summary>
		TECHNOLOGY = 2,
		/// <summary>
		/// Horizontal size in millimeters
		/// </summary>
		HORZSIZE = 4,
		/// <summary>
		/// Vertical size in millimeters
		/// </summary>
		VERTSIZE = 6,
		/// <summary>
		/// Horizontal width in pixels
		/// </summary>
		HORZRES = 8,
		/// <summary>
		/// Vertical height in pixels
		/// </summary>
		VERTRES = 10,
		/// <summary>
		/// Number of bits per pixel
		/// </summary>
		BITSPIXEL = 12,
		/// <summary>
		/// Number of planes
		/// </summary>
		PLANES = 14,
		/// <summary>
		/// Number of brushes the device has
		/// </summary>
		NUMBRUSHES = 16,
		/// <summary>
		/// Number of pens the device has
		/// </summary>
		NUMPENS = 18,
		/// <summary>
		/// Number of markers the device has
		/// </summary>
		NUMMARKERS = 20,
		/// <summary>
		/// Number of fonts the device has
		/// </summary>
		NUMFONTS = 22,
		/// <summary>
		/// Number of colors the device supports
		/// </summary>
		NUMCOLORS = 24,
		/// <summary>
		/// Size required for device descriptor
		/// </summary>
		PDEVICESIZE = 26,
		/// <summary>
		/// Curve capabilities
		/// </summary>
		CURVECAPS = 28,
		/// <summary>
		/// Line capabilities
		/// </summary>
		LINECAPS = 30,
		/// <summary>
		/// Polygonal capabilities
		/// </summary>
		POLYGONALCAPS = 32,
		/// <summary>
		/// Text capabilities
		/// </summary>
		TEXTCAPS = 34,
		/// <summary>
		/// Clipping capabilities
		/// </summary>
		CLIPCAPS = 36,
		/// <summary>
		/// Bitblt capabilities
		/// </summary>
		RASTERCAPS = 38,
		/// <summary>
		/// Length of the X leg
		/// </summary>
		ASPECTX = 40,
		/// <summary>
		/// Length of the Y leg
		/// </summary>
		ASPECTY = 42,
		/// <summary>
		/// Length of the hypotenuse
		/// </summary>
		ASPECTXY = 44,
		/// <summary>
		/// Shading and Blending caps
		/// </summary>
		SHADEBLENDCAPS = 45,

		/// <summary>
		/// Logical pixels inch in X
		/// </summary>
		LOGPIXELSX = 88,
		/// <summary>
		/// Logical pixels inch in Y
		/// </summary>
		LOGPIXELSY = 90,

		/// <summary>
		/// Number of entries in physical palette
		/// </summary>
		SIZEPALETTE = 104,
		/// <summary>
		/// Number of reserved entries in palette
		/// </summary>
		NUMRESERVED = 106,
		/// <summary>
		/// Actual color resolution
		/// </summary>
		COLORRES = 108,

		// Printing related DeviceCaps. These replace the appropriate Escapes
		/// <summary>
		/// Physical Width in device units
		/// </summary>
		PHYSICALWIDTH = 110,
		/// <summary>
		/// Physical Height in device units
		/// </summary>
		PHYSICALHEIGHT = 111,
		/// <summary>
		/// Physical Printable Area x margin
		/// </summary>
		PHYSICALOFFSETX = 112,
		/// <summary>
		/// Physical Printable Area y margin
		/// </summary>
		PHYSICALOFFSETY = 113,
		/// <summary>
		/// Scaling factor x
		/// </summary>
		SCALINGFACTORX = 114,
		/// <summary>
		/// Scaling factor y
		/// </summary>
		SCALINGFACTORY = 115,
		/// <summary>
		/// Current vertical refresh rate of the display device (for displays only) in Hz
		/// </summary>
		VREFRESH = 116,
		/// <summary>
		/// Vertical height of entire desktop in pixels
		/// </summary>
		DESKTOPVERTRES = 117,
		/// <summary>
		/// Horizontal width of entire desktop in pixels
		/// </summary>
		DESKTOPHORZRES = 118,
		/// <summary>
		/// Preferred blt alignment
		/// </summary>
		BLTALIGNMENT = 119
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-display_devicew"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAY_DEVICEW {
		public uint cb = (uint)Marshal.SizeOf<DISPLAY_DEVICEW>();

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string DeviceName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string DeviceString;

		public uint StateFlags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string DeviceID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string DeviceKey;

		public DISPLAY_DEVICEW() {
			//Assign everything so the compiler stops complaining
			DeviceName = default!;
			DeviceString = default!;
			StateFlags = default;
			DeviceID = default!;
			DeviceKey = default!;
		}
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_path_target_info"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_PATH_SOURCE_INFO {
		public LUID adapterId;
		public uint id;
		public uint modeInfoIdx;
		public uint statusFlags;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_path_target_info"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_PATH_TARGET_INFO {
		public LUID adapterId;
		public uint id;
		public uint modeInfoIdx;
		private DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
		private DISPLAYCONFIG_ROTATION rotation;
		private DISPLAYCONFIG_SCALING scaling;
		private DISPLAYCONFIG_RATIONAL refreshRate;
		private DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
		public bool targetAvailable;
		public uint statusFlags;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_rational"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_RATIONAL {
		public uint Numerator;
		public uint Denominator;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_path_info"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_PATH_INFO {
		public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
		public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
		public uint flags;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_2dregion"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_2DREGION {
		public uint cx;
		public uint cy;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_video_signal_info"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO {
		public ulong pixelRate;
		public DISPLAYCONFIG_RATIONAL hSyncFreq;
		public DISPLAYCONFIG_RATIONAL vSyncFreq;
		public DISPLAYCONFIG_2DREGION activeSize;
		public DISPLAYCONFIG_2DREGION totalSize;
		public uint videoStandard;
		public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_target_mode"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_TARGET_MODE {
		public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_source_mode"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_SOURCE_MODE {
		public uint width;
		public uint height;
		public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
		public POINTL position;
	}

	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_MODE_INFO_UNION {
		[FieldOffset(0)]
		public DISPLAYCONFIG_TARGET_MODE targetMode;

		[FieldOffset(0)]
		public DISPLAYCONFIG_SOURCE_MODE sourceMode;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_mode_info"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_MODE_INFO {
		public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
		public uint id;
		public LUID adapterId;
		public DISPLAYCONFIG_MODE_INFO_UNION modeInfo;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_target_device_name_flags"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS {
		public uint value;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_device_info_header"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_DEVICE_INFO_HEADER {
		public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
		public uint size;
		public LUID adapterId;
		public uint id;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_target_device_name"></see>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_TARGET_DEVICE_NAME {
		public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
		public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
		public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
		public ushort edidManufactureId;
		public ushort edidProductCodeId;
		public uint connectorInstance;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string monitorFriendlyDeviceName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string monitorDevicePath;
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows-hardware/drivers/install/devpkey-device-configflags"></see>
	/// </summary>
	public enum QUERY_DEVICE_CONFIG_FLAGS : uint {
		QDC_ALL_PATHS = 0x00000001,
		QDC_ONLY_ACTIVE_PATHS = 0x00000002,
		QDC_DATABASE_CURRENT = 0x00000004,
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_video_output_technology"></see>
	/// </summary>
	public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint {
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = 0xFFFFFFFF,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = 0x80000000,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = 0xFFFFFFFF,
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_scanline_ordering"></see>
	/// </summary>
	public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint {
		DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
		DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
		DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
		DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
		DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
		DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = 0xFFFFFFFF,
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_rotation"></see>
	/// </summary>
	public enum DISPLAYCONFIG_ROTATION : uint {
		DISPLAYCONFIG_ROTATION_IDENTITY = 1,
		DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
		DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
		DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
		DISPLAYCONFIG_ROTATION_FORCE_UINT32 = 0xFFFFFFFF,
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_scaling"></see>
	/// </summary>
	public enum DISPLAYCONFIG_SCALING : uint {
		DISPLAYCONFIG_SCALING_IDENTITY = 1,
		DISPLAYCONFIG_SCALING_CENTERED = 2,
		DISPLAYCONFIG_SCALING_STRETCHED = 3,
		DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
		DISPLAYCONFIG_SCALING_CUSTOM = 5,
		DISPLAYCONFIG_SCALING_PREFERRED = 128,
		DISPLAYCONFIG_SCALING_FORCE_UINT32 = 0xFFFFFFFF,
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_pixelformat"></see>
	/// </summary>
	public enum DISPLAYCONFIG_PIXELFORMAT : uint {
		DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
		DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
		DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
		DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
		DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
		DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = 0xffffffff,
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_mode_info_type"></see>
	/// </summary>
	public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint {
		DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
		DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
		DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = 0xFFFFFFFF,
	}

	/// <summary>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_device_info_type"></see>
	/// </summary>
	public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint {
		DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
		DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
		DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
		DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
		DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
		DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
		DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = 0xFFFFFFFF,
	}
}
