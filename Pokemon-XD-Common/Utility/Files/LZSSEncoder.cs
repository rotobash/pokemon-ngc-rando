using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace XDCommon.Utility
{
    public class LZSSEncoder
    {
		// ported directly from the original C implementation with minor adjustments
		// including the header here for reference
		// I wonder how many projects this has been used in...
		
		/**************************************************************
			LZSS.C -- A Data Compression Program
			(tab = 4 spaces)
		***************************************************************
			4/6/1989 Haruhiko Okumura
			Use, distribute, and modify this program freely.
			Please send me your improved versions.
				PC-VAN      SCIENCE
				NIFTY-Serve PAF01022
				CompuServe  74050,1022
		**************************************************************/

		const byte EI = 12; // offset bits
        const byte EJ = 4; // length bits
        const byte P = 2; // threshold
        const int UncodedF = 1 << EJ;
        const int EncodedF = 18;

        public const int N = 1 << EI; // window size
        public const uint LZSSbytes = 0x4C5A5353;
		const byte LZSSUncompressedSizeOffset = 0x04;
		const byte LZSSCompressedSizeOffset = 0x08;
		const byte LZSSUnkownOffset = 0x0C;// PBR only, unused in Colo/XD

		int[] lson;
        int[] rson;
        int[] dad;
        int matchLength, matchValue;

		void InitTree()  /* initialize trees */
		{
			int i;

			/* For i = 0 to N - 1, rson[i] and lson[i] will be the right and
			 left children of node i.  These nodes need not be initialized.
			 Also, dad[i] is the parent of node i.  These are initialized to
			 N (= N), which stands for 'not used.'
			 For i = 0 to 255, rson[N + i + 1] is the root of the tree
			 for strings that begin with character i.  These are initialized
			 to N.  Note there are 256 trees. */

			for (i = N + 1; i <= N + 256; i++) rson[i] = N;
			for (i = 0; i < N; i++) dad[i] = N;
		}

		void InsertNode(int r, byte[] slidingWindow)
		/* Inserts string of length F, text_buf[r..r+F-1], into one of the
		 trees (text_buf[r]'th tree) and returns the longest-match position
		 and length via the global variables matchValue and matchLength.
		 If matchLength = F, then removes the old node in favor of the new
		 one, because the old one will be deleted sooner.
		 Note r plays double role, as tree node and position in buffer. */
		{
			int i, p, cmp;
			int key;

			cmp = 1; key = r; p = N + 1 + slidingWindow[key];
			rson[r] = lson[r] = N; matchLength = 0;
			for (; ; )
			{
				if (cmp >= 0)
				{
					if (rson[p] != N) p = rson[p];
					else { rson[p] = r; dad[r] = p; return; }
				}
				else
				{
					if (lson[p] != N) p = lson[p];
					else { lson[p] = r; dad[r] = p; return; }
				}
				for (i = 1; i < EncodedF; i++)
					if ((cmp = slidingWindow[key + i] - slidingWindow[p + i]) != 0) break;
				if (i > matchLength)
				{
					matchValue = p;
					if ((matchLength = i) >= EncodedF) break;
				}
			}
			dad[r] = dad[p]; lson[r] = lson[p]; rson[r] = rson[p];
			dad[lson[p]] = r; dad[rson[p]] = r;
			if (rson[dad[p]] == p) rson[dad[p]] = r;
			else lson[dad[p]] = r;
			dad[p] = N;  /* remove p */
		}

		void DeleteNode(int p)  /* deletes node p from tree */
		{
			int q;

			if (dad[p] == N) return;  /* not in tree */
			if (rson[p] == N) q = lson[p];
			else if (lson[p] == N) q = rson[p];
			else
			{
				q = lson[p];
				if (rson[q] != N)
				{
					do { q = rson[q]; } while (rson[q] != N);
					rson[dad[q]] = lson[q]; dad[lson[q]] = dad[q];
					lson[q] = lson[p]; dad[lson[p]] = q;
				}
				rson[q] = rson[p]; dad[rson[p]] = q;
			}
			dad[q] = dad[p];
			if (rson[dad[p]] == p) rson[dad[p]] = q; else lson[dad[p]] = q;
			dad[p] = N;
		}

		public Stream Encode(Stream file)
		{
			// setup output stream
			string filename = string.Empty;
			string outputFilename = string.Empty;
			if (file is FileStream fs)
			{
				filename = fs.Name;
				outputFilename = $"{filename}.bak";
			}
			var outputStream = outputFilename.GetNewStream();
            outputStream.Write(LZSSbytes.GetBytes());
            outputStream.Write(((int)file.Length).GetBytes());
            outputStream.Write(0.GetBytes());
            outputStream.Write(0.GetBytes());

            int i, c, len, r, s, last_matchLength, code_buf_ptr, codesize;
			byte[] slidingWindow, code_buf;
			byte mask;

			code_buf = new byte[17];
			slidingWindow = new byte[N + EncodedF - 1];
			lson = new int[N + 1];
			rson = new int[N + 257];
			dad = new int[N + 1];

			InitTree();  /* initialize trees */
			code_buf[0] = 0;  /* code_buf[1..16] saves eight units of code, and
					   code_buf[0] works as eight flags, "1" representing that the unit
					   is an unencoded letter (1 byte), "0" a position-and-length pair
					   (2 bytes).  Thus, eight units require at most 16 bytes of code. */
			code_buf_ptr = mask = 1;
			codesize = 0;
			s = 0; r = N - EncodedF;
			for (i = s; i < r; i++) slidingWindow[i] = 0;  /* Clear the buffer with
													  any character that will appear often. */
			for (len = 0; len < EncodedF && (c = file.ReadByte()) >= 0; len++)
				slidingWindow[r + len] = (byte)c;  /* Read F bytes into the last F bytes of
							 the buffer */
			if (len == 0) return outputStream;  /* text of size zero */
			for (i = 1; i <= EncodedF; i++) InsertNode(r - i, slidingWindow);  /* Insert the F strings,
												  each of which begins with one or more 'space' characters.  Note
												  the order in which these strings are inserted.  This way,
												  degenerate trees will be less likely to occur. */
			InsertNode(r, slidingWindow);  /* Finally, insert the whole string just read.  The
					 global variables matchLength and matchValue are set. */
			do
			{
				if (matchLength > len) matchLength = len;  /* matchLength
													  may be spuriously long near the end of text. */
				if (matchLength <= P)
				{
					matchLength = 1;  /* Not long enough match.  Send one byte. */
					code_buf[0] |= mask;  /* 'send one byte' flag */
					code_buf[code_buf_ptr++] = slidingWindow[r];  /* Send uncoded. */
				}
				else
				{
					code_buf[code_buf_ptr++] = (byte) matchValue;
					code_buf[code_buf_ptr++] = (byte)
			(((matchValue >> 4) & 0xf0)
			 | (matchLength - (P + 1)));  /* Send position and
													length pair. Note matchLength > THRESHOLD. */
				}
				if ((mask <<= 1) == 0)
				{  /* Shift mask left one bit. */
					for (i = 0; i < code_buf_ptr ; i++)  /* Send at most 8 units of */
						outputStream.WriteByte(code_buf[i]);     /* code together */
					codesize += code_buf_ptr;

					code_buf = new byte[17]; code_buf_ptr = mask = 1;
				}
				last_matchLength = matchLength;
				for (i = 0; i < last_matchLength &&
					 (c = file.ReadByte()) >= 0; i++)
				{
					DeleteNode(s);      /* Delete old strings and */
					slidingWindow[s] = (byte)c;    /* read new bytes */
					if (s < EncodedF - 1) slidingWindow[s + N] = (byte)c;  /* If the position is
												  near the end of buffer, extend the buffer to make
												  string comparison easier. */
					s = (s + 1) & (N - 1); r = (r + 1) & (N - 1);
					/* Since this is a ring buffer, increment the position
					 modulo N. */
					InsertNode(r, slidingWindow);  /* Register the string in text_buf[r..r+F-1] */
				}
				while (i++ < last_matchLength)
				{   /* After the end of text, */
					DeleteNode(s);                  /* no need to read, but */
					s = (s + 1) & (N - 1); r = (r + 1) & (N - 1);
					if (--len != 0) InsertNode(r, slidingWindow);       /* buffer may not be empty. */
				}
			} while (len > 0);  /* until length of string to be processed is zero */

			if (code_buf_ptr > 1)
			{       /* Send remaining code. */
				for (i = 0; i < code_buf_ptr; i++) outputStream.WriteByte(code_buf[i]);
				codesize += code_buf_ptr;
			}

			// write the output size in the header
			outputStream.WriteBytesAtOffset(LZSSCompressedSizeOffset, (codesize + 0x10).GetBytes());

			if (file is FileStream)
			{
				outputStream.Dispose();
				File.Delete(filename);
				File.Move(outputFilename, filename);
				// bypass GetNewStream because it'll overwrite files
				return File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
			}

			return outputStream;
		}

		public static Stream Decode(Stream file)
        {
            uint flags = 0;
            int n, f, rless;
            n = N;
            f = UncodedF;
            rless = P;

            var slidingWindow = new byte[n];

            // setup output stream
            Stream outputStream;
            string filename = string.Empty;
            string outputFilename = string.Empty;
            if (file is FileStream fs)
            {
                filename = fs.Name;
                outputFilename = $"{filename}.bak";
            }
            outputStream = outputFilename.GetNewStream();

            int r = (n - f) - rless;
            n--;
            f--;

            while (true)
            {
                flags >>= 1;
                if ((flags & 0x100) == 0)
                {
                    var b = file.ReadByte();
                    if (b == -1) break;
                    flags = (uint)(b | 0xFF00);
                }

                if ((flags & 0x1) != 0)
                {
                    var b = file.ReadByte();
                    if (b == -1) break;
                    outputStream.WriteByte((byte)b);
                    slidingWindow[r++] = (byte)b;
                    r &= n;
                }
                else
                {
                    int i = file.ReadByte();
                    if (i == -1) break;
                    int j = file.ReadByte();
                    if (j == -1) break;

                    i |= (j >> EJ) << 8;
                    j = (j & f) + P;
                    for (int k = 0; k <= j; k++)
                    {
                        var b = slidingWindow[((i + k) & n)];
                        outputStream.WriteByte(b);
                        slidingWindow[r++] = b;
                        r &= n;
                    }
                }
            }

            outputStream.Flush();
            file.Dispose();

            if (file is FileStream)
            {
                outputStream.Dispose();
                File.Delete(filename);
                File.Move(outputFilename, filename);
                // bypass GetNewStream because it'll overwrite files
                return File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
            }
            return outputStream;
        }
    }
}
