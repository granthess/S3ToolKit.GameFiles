/*
    Copyright 2012, Grant Hess

    This file is part of S3ToolKit.GameFiles.

    S3ToolKit.Utils is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with CC Magic.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S3ToolKit.GameFiles
{
    public class SubStream : Stream
    {
        #region Fields
        private long _Offset;
        private long _Length;
        private long _Position;
        #endregion

        #region properties
        public Stream BaseStream { get; private set; }


        #endregion

        #region Constructor
        public SubStream(Stream BaseStream, long Offset, long Length)
        {
            this.BaseStream = BaseStream;
            _Offset = Offset;
            _Length = Length;
            _Position = 0;
        }
        #endregion

        #region Helpers
        private void SetCurrentPosition(long Position)
        {
            if (BaseStream == null)
                throw new ObjectDisposedException("BaseStream");

            Seek(Position, SeekOrigin.Begin);
        }

        private bool GetCanRead()
        {
            if (BaseStream != null)
                return BaseStream.CanRead;
            else
                return false;
        }
        private bool GetCanSeek()
        {
            if (BaseStream != null)
                return BaseStream.CanSeek;
            else
                return false;
        }
        #endregion

        #region Stream Implementation
        public override bool CanRead { get { return GetCanRead(); } }
        public override bool CanSeek { get { return GetCanSeek(); } }
        public override bool CanWrite { get { return false; } }
        public override long Length { get { return _Length; } }
        public override long Position { get { return _Position; } set { SetCurrentPosition(value); } }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (BaseStream == null)
                throw new ObjectDisposedException("BaseStream");

            int Result = 0;

            lock (BaseStream)
            {
                // Keep BaseStream Consistent
                long OldPos = BaseStream.Position;
          

                // Validate Count
                if (count > buffer.Length)
                    throw new ArgumentException();

                if (count + _Offset > BaseStream.Length)
                {
                    count = (int)(BaseStream.Length - _Offset);
                }
                else if (count + _Position > _Length)
                {
                    count = (int)(_Length - _Position);
                }

                if (count <= 0)  // bypass if we are at or beyond end of stream
                    return 0;

                try
                {
                    BaseStream.Position = _Offset + _Position;
                    Result = BaseStream.Read(buffer, offset, count);
                }

                finally
                {
                    BaseStream.Position = OldPos;
                    _Position += Result;
                }
            }

            return Result;
        }

        
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (BaseStream == null)
                throw new ObjectDisposedException("BaseStream");

            long newoffset;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        newoffset = offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        newoffset = _Position + offset;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        newoffset = _Length - offset;
                        break;
                    }
                default:
                    {
                        newoffset = _Position;
                        break;
                    }
            }

            if (newoffset < 0)
                throw new ArgumentException();

            if (newoffset > _Length)
            {
                newoffset = _Length;
            }

            _Position = newoffset;
            return _Position;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            BaseStream = null;  // Don't CLOSE THE BASE STREAM !!!!!! !!!!!! VERY VERY VERY EXTREEMELY IMPORTANT TO LEAVE IT AS IS!!!
        }
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
