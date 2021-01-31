using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Contracts
{
    public enum FileTypes
    {
        None = 0x00,
        RDAT = 0x02, // room model in hal dat format (unknown if it uses a different file extension)
        DAT = 0x04,// character model in hal dat format
        CCD = 0x06,// collision file
        SAMP = 0x08, // shorter music files for fanfares etc.
        MSG = 0x0a, // string table
        FNT = 0x0c, // font
        SCD = 0x0e, // script data
        DATS = 0x10, // multiple .dat models in one archive
        GTX = 0x12, // texture
        GPT1 = 0x14, // particle data
        CAM = 0x18, // camera data
        REL = 0x1c, // relocation table
        PKX = 0x1e, // character battle model (same as dat with additional header information)
        WZX = 0x20, // move animation
        ISD = 0x28, // audio file header
        ISH = 0x2a, // audio file
        THH = 0x2c, // thp media header
        THD = 0x2e, // thp media data
        GSW = 0x30, // multi texture
        ATX = 0x32, // animated texture (official file extension is currently unknown)
        BIN = 0x34, // binary data


        // all arbitrary values
        FSYS = 0XF0, // don't know if it has it's own identifier
        THP = 0XF2,
        JSON = 0XF3,
        TXT = 0XF4,
        LZSS = 0XF5,
        TPL = 0XF6,

        BMP = 0XF7,
        JPEG = 0XF8,
        PNG = 0XF9,
        TEX0 = 0XFA,
        XDS = 0XFB,

        TOC = 0XFC,
        DOL = 0XFD,
        ISO = 0XFE,
        Unkown = 0XFF
    }

    public static class FileTypesExtension
    {
        public static bool IsTextureFormat(this FileTypes fileTypes)
        {
            return fileTypes == FileTypes.ATX || fileTypes == FileTypes.GTX;
        }
        public static string FileTypeName(this FileTypes fileType)
        {
            return fileType switch
            {
                FileTypes.None => "",
		        FileTypes.RDAT => ".rdat",
		        FileTypes.DAT => ".dat",
		        FileTypes.CCD => ".ccd",
		        FileTypes.SAMP => ".samp",
		        FileTypes.MSG => ".msg",
		        FileTypes.FNT => ".fnt",
		        FileTypes.SCD => ".scd",
		        FileTypes.DATS => ".dats",
		        FileTypes.GTX => ".gtx",
		        FileTypes.GPT1 => ".gpt1",
		        FileTypes.CAM => ".cam",
		        FileTypes.REL => ".rel",
		        FileTypes.PKX => ".pkx",
		        FileTypes.WZX => ".wzx",
		        FileTypes.ISD => ".isd",
		        FileTypes.ISH => ".ish",
		        FileTypes.THH => ".thh",
		        FileTypes.THD => ".thd",
		        FileTypes.GSW => ".gsw",
		        FileTypes.ATX => ".atx",
		        FileTypes.BIN => ".bin",
		        FileTypes.FSYS => ".fsys",
		        FileTypes.ISO => ".iso",
		        FileTypes.XDS => ".xds",
		        FileTypes.DOL => ".dol",
		        FileTypes.TOC => ".toc",
		        FileTypes.PNG => ".png",
		        FileTypes.BMP => ".bmp",
		        FileTypes.JPEG => ".jpg",
		        FileTypes.TEX0 => ".tex0",
		        FileTypes.TPL => ".tpl",
		        FileTypes.LZSS => ".lzss",
		        FileTypes.TXT => ".txt",
		        FileTypes.JSON => ".json",
		        FileTypes.THP => ".thp",
		        _ => ".bin",
            };
        }
    }
}
