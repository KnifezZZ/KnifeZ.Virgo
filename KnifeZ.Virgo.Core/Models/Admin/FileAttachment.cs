using KnifeZ.Extensions;
using KnifeZ.Virgo.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text.Json.Serialization;

namespace KnifeZ.Virgo.Core
{
    /// <summary>
    /// FileAttachment
    /// </summary>
    [Table("FileAttachments")]
    public class FileAttachment : BasePoco, IVirgoFile, IDisposable
    {
        [Display(Name = "FileName")]
        [Required(ErrorMessage = "{0}required")]
        public string FileName { get; set; }

        [Display(Name = "FileExt")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(10)]
        public string FileExt { get; set; }

        [Display(Name = "Path")]
        public string Path { get; set; }

        [Display(Name = "Length")]
        public long Length { get; set; }

        public DateTime UploadTime { get; set; }

        public string SaveMode { get; set; }

        public byte[] FileData { get; set; }

        public string ExtraInfo { get; set; }

        [NotMapped]
        public string Url { get; set; }

        [NotMapped]
        [JsonIgnore]
        public Stream DataStream { get; set; }

        public void Dispose ()
        {
            if (DataStream != null)
            {
                DataStream.Dispose();
            }
        }

        string IVirgoFile.GetID ()
        {
            return ID.ToString();
        }
    }
}
