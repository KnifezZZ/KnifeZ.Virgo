using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Support.FileHandlers;
using KnifeZ.Virgo.Mvc;

namespace KnifeZ.Virgo.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/_file")]
    [Public]
    [ActionDescription("File")]
    public class FileApiController : BaseApiController
    {
        [HttpPost("[action]")]
        [ActionDescription("UploadFile")]
        public IActionResult Upload([FromServices] VirgoFileProvider fp, string sm = null, string groupName = null, string subdir=null,string csName= null)
        {
            var fh = fp.CreateFileHandler(sm, KnifeVirgo.ConfigInfo.CreateDC(csName));
            var FileData = Request.Form.Files[0];
            var file = fh.Upload(FileData.FileName, FileData.Length, FileData.OpenReadStream(),groupName,subdir,null, FileData.ContentType);
            return Ok(new { Id = file.GetID(), Name = file.FileName });
        }

        [HttpPost("[action]")]
        [ActionDescription("UploadPic")]
        public IActionResult UploadImage([FromServices] VirgoFileProvider fp,int? width = null, int? height = null, string sm = null, string groupName = null, string subdir = null, string csName = null)
        {
            if (width == null && height == null)
            {
                return Upload(fp,sm,groupName,csName);
            }
            var fh = fp.CreateFileHandler(sm, KnifeVirgo.ConfigInfo.CreateDC(csName));
            var FileData = Request.Form.Files[0];

            Image oimage = Image.FromStream(FileData.OpenReadStream());
            if (oimage == null)
            {
                return BadRequest(Localizer["UploadFailed"]);
            }
            if (width == null)
            {
                width = height * oimage.Width / oimage.Height;
            }
            if (height == null)
            {
                height = width * oimage.Height / oimage.Width;
            }
            MemoryStream ms = new MemoryStream();
            oimage.GetThumbnailImage(width.Value, height.Value, null, IntPtr.Zero).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            var file = fh.Upload(FileData.FileName, FileData.Length, ms, groupName,subdir);
            oimage.Dispose();
            ms.Dispose();

            if (file != null)
            {
                return Ok(new { Id = file.GetID(), Name = file.FileName });
            }
            return BadRequest(Localizer["UploadFailed"]);

        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("GetFileName")]
        public IActionResult GetFileName([FromServices] VirgoFileProvider fp, string id, string csName = null)
        {
            return Ok(fp.GetFileName(id, KnifeVirgo.ConfigInfo.CreateDC(csName)));
        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("GetFileModel")]
        public IActionResult GetFileModel ([FromServices] VirgoFileProvider fp, string id, string csName = null)
        {
            return Ok(fp.GetFileModel(id, KnifeVirgo.ConfigInfo.CreateDC(csName)));
        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("GetFile")]
        public IActionResult GetFile([FromServices] VirgoFileProvider fp, string id, string csName = null)
        {
            var file = fp.GetFile(id,true, KnifeVirgo.ConfigInfo.CreateDC(csName));


            if (file == null)
            {
                return BadRequest(Localizer["FileNotFound"]);
            }
            file.DataStream?.CopyToAsync(Response.Body);
            return new EmptyResult();
        }

        [HttpGet("[action]/{id}")]
        [ActionDescription("DownloadFile")]
        public IActionResult DownloadFile([FromServices] VirgoFileProvider fp, string id, string csName = null)
        {
            var file = fp.GetFile(id,true, KnifeVirgo.ConfigInfo.CreateDC(csName));
            if (file == null)
            {
                return BadRequest(Localizer["FileNotFound"]);
            }
            var ext = file.FileExt.ToLower();
            var contenttype = "application/octet-stream";
            if (ext == "pdf")
            {
                contenttype = "application/pdf";
            }
            if (ext == "png" || ext == "bmp" || ext == "gif" || ext == "tif" || ext == "jpg" || ext == "jpeg")
            {
                contenttype = $"image/{ext}";
            }
            return File(file.DataStream, contenttype, file.FileName ?? (Guid.NewGuid().ToString() + ext));
        }

        [HttpDelete("[action]/{id}")]
        [ActionDescription("DeleteFile")]
        public IActionResult DeletedFile([FromServices] VirgoFileProvider fp, string id, string csName = null)
        {
            fp.DeleteFile(id, KnifeVirgo.ConfigInfo.CreateDC(csName));
            return Ok();
        }
    }
}
