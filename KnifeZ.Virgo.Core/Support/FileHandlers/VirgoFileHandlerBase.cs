using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Models;
using KnifeZ.Extensions.DatabaseAccessor;

namespace KnifeZ.Virgo.Core.Support.FileHandlers
{
    public abstract class VirgoFileHandlerBase : IVirgoFileHandler
    {
        protected Configs _config;
        protected IDataContext _dc;

        public VirgoFileHandlerBase(Configs config, IDataContext dc)
        {
            _config = config;
            _dc = dc;
            if (_dc == null)
            {
                _dc = _config.CreateDC();
            }
        }


        public virtual void DeleteFile(IVirgoFile file)
        {

        }

        public virtual Stream GetFileData(IVirgoFile file)
        {
            return null;
        }

        public virtual IVirgoFile Upload(string fileName, long fileLength, Stream data, string group=null, string subdir = null, string extra = null,string fileContentType=null)
        {
            return null;
        }

    }
}
