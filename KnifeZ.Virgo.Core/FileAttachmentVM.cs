using Microsoft.EntityFrameworkCore;
using KnifeZ.Virgo.Core.Extensions;
using System.IO;

namespace KnifeZ.Virgo.Core
{
    public class FileAttachmentVM : BaseCRUDVM<FileAttachment>
    {
        public override void DoDelete()
        {
            try
            {
                if (Entity.SaveFileMode == SaveFileModeEnum.Local && !string.IsNullOrEmpty(Entity.Path))
                {
                    File.Delete(Entity.Path);
                }
                FileAttachment del = new FileAttachment { ID = Entity.ID };
                DC.Set<FileAttachment>().Attach(del);
                DC.Set<FileAttachment>().Remove(del);
                DC.SaveChanges();
            }
            catch (DbUpdateException)
            {
                MSD.AddModelError("", Program._localizer["DataCannotDelete"]);
            }
        }
    }
}
