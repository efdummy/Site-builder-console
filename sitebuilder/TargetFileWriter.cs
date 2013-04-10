using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sitebuilder
{
    class TargetFileWriter
    {
        FileStream fsw;
        BinaryWriter writer;

        public TargetFileWriter(string resultFile, FileMode fm)
        {
            // Create the destination file and open it in write mode
            fsw = new FileStream(resultFile, fm);
            writer = new BinaryWriter(fsw);
        }
        ~TargetFileWriter()
        {
            // Close target file
            Close();
        }

        public void Close()
        {
            // Close target file
            if (writer != null) writer.Close();
            if (fsw.CanWrite) fsw.Close();
        }

        // Append a file
        public void Append(string fpath)
        {
            var fstream = new FileStream(fpath, FileMode.Open);
            var reader = new BinaryReader(fstream);
            writer.Write(reader.ReadBytes((int)fstream.Length));
            if (reader != null) reader.Close();
            if (fstream.CanRead) fstream.Close();
        }

        public void AppendWithoutBom(string fpath, int utfOption)
        {
            switch (utfOption)
            {
                case Params.UTF8_ON:
                    AppendWithoutUtf8Bom(fpath);
                    break;
                case Params.UTF16_ON:
                    AppendWithoutUtf8Bom(fpath);
                    break;
                case Params.UTF32_ON:
                    AppendWithoutUtf8Bom(fpath);
                    break;
                default:
                    // Normaly, we never reach this point !
                    throw new ArgumentException("Command line option unknown", utfOption.ToString());
                    break;
            }
        }



        // Append a file without UTF8 BOM
        public void AppendWithoutUtf8Bom(string fpath)
        {
            var fstream = new FileStream(fpath, FileMode.Open);
            var reader = new BinaryReader(fstream);
            try
            {
                long len = fstream.Length;
                switch (len)
                {
                    case 0:
                    case 1:
                    case 2:
                        writer.Write(reader.ReadBytes((int)fstream.Length));
                        break;
                    default:
                        byte[] buff = reader.ReadBytes(3);
                        if ((buff[0] == 239) && (buff[1] == 187) && (buff[2] == 191)) writer.Write(reader.ReadBytes((int)fstream.Length));
                        else { writer.Write(buff); writer.Write(reader.ReadBytes((int)fstream.Length)); }
                        break;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (fstream.CanRead) fstream.Close();
            }
        }

        // Append a file without UTF16 little endian BOM
        public void AppendWithoutUtf16Bom(string fpath)
        {
            var fstream = new FileStream(fpath, FileMode.Open);
            var reader = new BinaryReader(fstream);
            try
            {
                long len = fstream.Length;
                switch (len)
                {
                    case 0:
                    case 1:
                        writer.Write(reader.ReadBytes((int)fstream.Length));
                        break;
                    default:
                        byte[] buff = reader.ReadBytes(2);
                        if ((buff[0] == 255) && (buff[1] == 254)) writer.Write(reader.ReadBytes((int)fstream.Length));
                        else { writer.Write(buff); writer.Write(reader.ReadBytes((int)fstream.Length)); }
                        break;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (fstream.CanRead) fstream.Close();
            }
        }

        // Append a file without UTF32 little endian BOM
        public void AppendWithoutUtf32Bom(string fpath)
        {
            var fstream = new FileStream(fpath, FileMode.Open);
            var reader = new BinaryReader(fstream);
            try
            {
                long len = fstream.Length;
                switch (len)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        writer.Write(reader.ReadBytes((int)fstream.Length));
                        break;
                    default:
                        byte[] buff = reader.ReadBytes(4);
                        if ((buff[0] == 255) && (buff[1] == 254) && (buff[2] == 0) && (buff[3] == 0)) writer.Write(reader.ReadBytes((int)fstream.Length));
                        else { writer.Write(buff); writer.Write(reader.ReadBytes((int)fstream.Length)); }
                        break;
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (fstream.CanRead) fstream.Close();
            }
        }


    }
}
